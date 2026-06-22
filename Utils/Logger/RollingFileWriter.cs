using System.IO;
using System.Text;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 单个滚动文件写入器：负责单个逻辑日志文件（如 <c>all.log</c> 或
    /// <c>level-error.log</c>）的写入与按大小滚动切分。线程安全。
    /// </summary>
    /// <remarks>
    /// 滚动策略：当前文件写入后若超出 <c>maxSize</c>，则关闭当前流，
    /// 删除序号最大的副本，将 <c>.i</c> 依次上移为 <c>.i+1</c>，
    /// 把当前文件改名为 <c>.1</c>，再新建同名文件继续写入。
    /// 副本命名形如 <c>all.log.1</c>、<c>all.log.2</c>，序号越小越新。
    /// </remarks>
    internal sealed class RollingFileWriter : IDisposable
    {
        private readonly string _basePath;       // 例如 .../logs/all.log
        private readonly long _maxSize;          // 单文件最大字节数
        private readonly int _maxFiles;          // 保留的最大滚动副本数
        private readonly string _newLine;
        private readonly Encoding _encoding = new UTF8Encoding(false); // 无 BOM，避免滚动后重复

        private readonly object _lock = new();
        private StreamWriter? _writer;
        private long _currentSize;

        public RollingFileWriter(string basePath, long maxSize, int maxFiles, string newLine)
        {
            _basePath = basePath;
            _maxSize = maxSize;
            _maxFiles = maxFiles;
            _newLine = newLine;
        }

        public void Write(string line)
        {
            var data = line + _newLine;
            var byteCount = _encoding.GetByteCount(data);

            lock (_lock)
            {
                EnsureOpen();

                // 当前文件已有内容且写入后会超限：先滚动再写入。
                if (_currentSize > 0 && _currentSize + byteCount > _maxSize)
                {
                    Roll();
                    EnsureOpen();
                }

                _writer!.Write(data);
                _writer.Flush();
                _currentSize += byteCount;
            }
        }

        public void Flush()
        {
            lock (_lock)
            {
                _writer?.Flush();
            }
        }

        private void EnsureOpen()
        {
            if (_writer is not null)
            {
                return;
            }

            var dir = Path.GetDirectoryName(_basePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // 追加模式打开：应用重启后继续在同一文件续写。
            var stream = new FileStream(
                _basePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read,
                bufferSize: 4096,
                FileOptions.SequentialScan);

            _currentSize = stream.Length;
            _writer = new StreamWriter(stream, _encoding)
            {
                AutoFlush = false
            };
        }

        /// <summary>执行一次滚动切分：关闭当前文件并按序号轮转。</summary>
        private void Roll()
        {
            CloseWriter();

            // 删除序号最大的副本（如 all.log.10）。
            var oldest = $"{_basePath}.{_maxFiles}";
            if (File.Exists(oldest))
            {
                File.Delete(oldest);
            }

            // 从大到小依次上移：all.log.(i) -> all.log.(i+1)。
            for (var i = _maxFiles - 1; i >= 1; i--)
            {
                var src = $"{_basePath}.{i}";
                if (File.Exists(src))
                {
                    File.Move(src, $"{_basePath}.{i + 1}");
                }
            }

            // 当前文件变为 .1。
            if (File.Exists(_basePath))
            {
                File.Move(_basePath, $"{_basePath}.1");
            }

            _currentSize = 0;
        }

        private void CloseWriter()
        {
            if (_writer is null)
            {
                return;
            }

            _writer.Flush();
            _writer.Dispose();
            _writer = null;
        }

        public void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            lock (_lock)
            {
                CloseWriter();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
