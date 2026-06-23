using System;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 一条日志记录的不可变数据模型。
    /// </summary>
    public sealed class LogEntry
    {
        /// <summary>日志产生时间（本地时间）。</summary>
        public DateTime Timestamp { get; }

        /// <summary>日志等级。</summary>
        public LogLevel Level { get; }

        /// <summary>日志来源标记，通常是调用方类名或模块名。</summary>
        public string Source { get; }

        /// <summary>日志正文。</summary>
        public string Message { get; }

        /// <summary>与日志关联的异常（可选）。</summary>
        public Exception? Exception { get; }

        public LogEntry(DateTime timestamp, LogLevel level, string source, string message, Exception? exception = null)
        {
            Timestamp = timestamp;
            Level = level;
            Source = source ?? string.Empty;
            Message = message ?? string.Empty;
            Exception = exception;
        }

        /// <summary>
        /// 将日志格式化为单行文本，便于写入文件或控制台。
        /// </summary>
        public string Format()
        {
            var level = Level.ToString().ToUpperInvariant();
            var ts = Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var source = string.IsNullOrEmpty(Source) ? string.Empty : $" [{Source}]";
            var exception = Exception is null ? string.Empty : $"\n{Exception}";
            return $"{ts} [{level}]{source} {Message}{exception}";
        }
    }
}
