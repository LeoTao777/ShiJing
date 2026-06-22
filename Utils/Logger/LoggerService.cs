using System;
using System.Collections.Generic;
using System.Threading;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 日志服务：单例，统一管理日志配置、输出目标与日志分发。
    /// 对外通过 <see cref="GetLogger"/> 提供 <see cref="ILogger"/>，
    /// 内部按配置分发到控制台/文件目标。线程安全。
    /// </summary>
    public sealed class LoggerService : IDisposable
    {
        private static readonly Lazy<LoggerService> _instance = new(
            () => new LoggerService(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>全局单例。首次访问时按默认配置初始化。</summary>
        public static LoggerService Instance => _instance.Value;

        private readonly object _lock = new();
        private readonly Dictionary<string, ILogger> _loggers = new(StringComparer.Ordinal);

        private LoggerConfig _config;
        private List<LogTarget> _targets = new();
        private bool _disposed;

        /// <summary>用默认配置初始化（首次访问单例时调用）。</summary>
        public LoggerService() : this(new LoggerConfig())
        {
        }

        /// <summary>用指定配置初始化，便于测试或自定义启动参数。</summary>
        public LoggerService(LoggerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            BuildTargets();
        }

        /// <summary>当前生效的配置（只读视图）。</summary>
        public LoggerConfig Config
        {
            get
            {
                lock (_lock)
                {
                    return _config;
                }
            }
        }

        /// <summary>
        /// 获取（或复用）绑定到指定来源标记的 logger。
        /// 相同来源返回同一实例，避免重复创建。
        /// </summary>
        public ILogger GetLogger(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                source = "Application";
            }

            lock (_lock)
            {
                if (_loggers.TryGetValue(source, out var existing))
                {
                    return existing;
                }

                var logger = new Logger(this, source);
                _loggers[source] = logger;
                return logger;
            }
        }

        /// <summary>获取绑定到某类型全名的 logger。</summary>
        public ILogger GetLogger(Type type) => GetLogger(type?.FullName ?? "Application");

        /// <summary>
        /// 分发一条日志到所有启用的目标。低于 <see cref="LoggerConfig.MinLevel"/> 的日志被丢弃。
        /// </summary>
        internal void Dispatch(LogEntry entry)
        {
            // 先做等级过滤，避免无谓的锁与 IO。
            if (entry.Level < Config.MinLevel)
            {
                return;
            }

            List<LogTarget> snapshot;
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }
                snapshot = _targets;
            }

            // 每个目标内部自行保证线程安全，这里无需持有全局锁，
            // 避免多线程写日志时相互阻塞。
            foreach (var target in snapshot)
            {
                try
                {
                    target.Write(entry);
                }
                catch
                {
                    // 单个目标写入失败不应影响其他目标或调用方。
                    // 控制台兜底打印，便于发现配置/磁盘问题。
                    try { Console.Error.WriteLine($"[LoggerService] target write failed: {entry.Format()}"); }
                    catch { /* 忽略 */ }
                }
            }
        }

        /// <summary>
        /// 应用新的配置并重建输出目标。原有目标被释放。
        /// 已创建的 logger 实例继续有效，会自动使用新配置。
        /// </summary>
        public void ReloadConfig(LoggerConfig config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            List<LogTarget> oldTargets;
            lock (_lock)
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(nameof(LoggerService));
                }

                oldTargets = _targets;
                _config = config;
                BuildTargets();
            }

            // 在锁外释放旧目标，避免与正在写入的线程争用。
            // 此时 _targets 已指向新目标，新日志会写入新文件；
            // 旧目标可能仍被某个 Dispatch 线程短暂使用，其内部已做异常兜底。
            foreach (var target in oldTargets)
            {
                try { target.Dispose(); }
                catch { /* 忽略释放异常 */ }
            }
        }

        /// <summary>立即刷新所有目标的缓冲（如文件流）。</summary>
        public void Flush()
        {
            List<LogTarget> snapshot;
            lock (_lock)
            {
                snapshot = _targets;
            }

            foreach (var target in snapshot)
            {
                try { target.Flush(); }
                catch { /* 忽略 */ }
            }
        }

        private void BuildTargets()
        {
            var targets = new List<LogTarget>();
            if (_config.OutputTarget.HasFlag(LogOutputTarget.Console))
            {
                targets.Add(new ConsoleLogTarget());
            }
            if (_config.OutputTarget.HasFlag(LogOutputTarget.File))
            {
                targets.Add(new FileLogTarget(_config));
            }

            _targets = targets;
        }

        public void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            List<LogTarget> snapshot;
            lock (_lock)
            {
                if (_disposed)
                {
                    return;
                }
                _disposed = true;
                snapshot = _targets;
                _targets = new List<LogTarget>();
            }

            foreach (var target in snapshot)
            {
                try { target.Dispose(); }
                catch { /* 忽略 */ }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
