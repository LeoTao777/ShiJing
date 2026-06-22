using System.Collections.Generic;
using System.IO;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 文件日志目标。同时写入：
    /// <list type="bullet">
    ///   <item>总日志文件（记录所有等级），文件名由 <see cref="LoggerConfig.AllFileName"/> 指定；</item>
    ///   <item>分等级日志文件，文件名形如 <c>{LevelFileNamePrefix}-{level}.log</c>，
    ///         每个等级一个独立文件，便于按等级排查问题。</item>
    /// </list>
    /// 每个文件都按 <see cref="LoggerConfig.MaxFileSizeBytes"/> 滚动切分。
    /// </summary>
    public sealed class FileLogTarget : LogTarget
    {
        private readonly LoggerConfig _config;
        private readonly RollingFileWriter _allWriter;
        private readonly Dictionary<LogLevel, RollingFileWriter> _levelWriters;

        public FileLogTarget(LoggerConfig config)
        {
            _config = config;

            if (!Directory.Exists(config.LogDirectory))
            {
                Directory.CreateDirectory(config.LogDirectory);
            }

            var allPath = Path.Combine(config.LogDirectory, config.AllFileName);
            _allWriter = new RollingFileWriter(
                allPath, config.MaxFileSizeBytes, config.MaxRollingFiles, config.NewLine);

            _levelWriters = new Dictionary<LogLevel, RollingFileWriter>();
            foreach (var level in AllLevels())
            {
                var name = $"{config.LevelFileNamePrefix}-{level.ToString().ToLowerInvariant()}.log";
                var path = Path.Combine(config.LogDirectory, name);
                _levelWriters[level] = new RollingFileWriter(
                    path, config.MaxFileSizeBytes, config.MaxRollingFiles, config.NewLine);
            }
        }

        public override void Write(LogEntry entry)
        {
            var line = entry.Format();

            // 总文件：所有等级统一写入。
            _allWriter.Write(line);

            // 分等级文件：仅写入对应等级。
            if (_levelWriters.TryGetValue(entry.Level, out var writer))
            {
                writer.Write(line);
            }
        }

        public override void Flush()
        {
            _allWriter.Flush();
            foreach (var writer in _levelWriters.Values)
            {
                writer.Flush();
            }
        }

        public override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            _allWriter.Dispose();
            foreach (var writer in _levelWriters.Values)
            {
                writer.Dispose();
            }
            _levelWriters.Clear();
        }

        private static IEnumerable<LogLevel> AllLevels()
        {
            yield return LogLevel.Trace;
            yield return LogLevel.Debug;
            yield return LogLevel.Info;
            yield return LogLevel.Warn;
            yield return LogLevel.Error;
            yield return LogLevel.Fatal;
        }
    }
}
