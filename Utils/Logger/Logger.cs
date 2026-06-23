using System;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// <see cref="ILogger"/> 的默认实现。持有来源标记，把日志委派给
    /// <see cref="LoggerService"/> 统一分发，自身保持轻量无状态。
    /// </summary>
    internal sealed class Logger : ILogger
    {
        private readonly LoggerService _service;
        public string Source { get; }

        public Logger(LoggerService service, string source)
        {
            _service = service;
            Source = source ?? string.Empty;
        }

        public void Trace(string message, Exception? exception = null)
            => _service.Dispatch(new LogEntry(DateTime.Now, LogLevel.Trace, Source, message, exception));

        public void Debug(string message, Exception? exception = null)
            => _service.Dispatch(new LogEntry(DateTime.Now, LogLevel.Debug, Source, message, exception));

        public void Info(string message, Exception? exception = null)
            => _service.Dispatch(new LogEntry(DateTime.Now, LogLevel.Info, Source, message, exception));

        public void Warn(string message, Exception? exception = null)
            => _service.Dispatch(new LogEntry(DateTime.Now, LogLevel.Warn, Source, message, exception));

        public void Error(string message, Exception? exception = null)
            => _service.Dispatch(new LogEntry(DateTime.Now, LogLevel.Error, Source, message, exception));

        public void Fatal(string message, Exception? exception = null)
            => _service.Dispatch(new LogEntry(DateTime.Now, LogLevel.Fatal, Source, message, exception));

        public ILogger ForSource(string source) => _service.GetLogger(source);
    }
}
