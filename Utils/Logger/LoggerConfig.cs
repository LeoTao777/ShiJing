using System.IO;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 输出目标，决定日志写入控制台、文件或两者。
    /// </summary>
    [Flags]
    public enum LogOutputTarget
    {
        /// <summary>不输出。</summary>
        None = 0,

        /// <summary>输出到控制台。</summary>
        Console = 1 << 0,

        /// <summary>输出到文件。</summary>
        File = 1 << 1,

        /// <summary>同时输出到控制台与文件。</summary>
        All = Console | File
    }

    /// <summary>
    /// 日志服务配置。所有字段在 <see cref="LoggerService"/> 启动时读取，
    /// 运行期修改需显式调用 <see cref="LoggerService.ReloadConfig"/>。
    /// </summary>
    public sealed class LoggerConfig
    {
        /// <summary>允许输出的最低日志等级，低于该等级的日志将被丢弃。</summary>
        public LogLevel MinLevel { get; set; } = LogLevel.Info;

        /// <summary>输出目标（控制台/文件/两者）。</summary>
        public LogOutputTarget OutputTarget { get; set; } = LogOutputTarget.All;

        /// <summary>日志文件根目录。文件目标启用时使用。</summary>
        public string LogDirectory { get; set; } =
            Path.Combine(AppContext.BaseDirectory, "logs");

        /// <summary>总日志文件名（记录所有等级），位于 <see cref="LogDirectory"/> 下。</summary>
        public string AllFileName { get; set; } = "all.log";

        /// <summary>分等级文件名前缀，最终文件形如 <c>level-error.log</c>。</summary>
        public string LevelFileNamePrefix { get; set; } = "level";

        /// <summary>单个日志文件最大字节数，超过则滚动切分。</summary>
        public long MaxFileSizeBytes { get; set; } = 10L * 1024 * 1024; // 10 MB

        /// <summary>每个日志文件保留的最大滚动副本数（超出后最旧的被删除）。</summary>
        public int MaxRollingFiles { get; set; } = 10;

        /// <summary>日志格式化行尾。</summary>
        public string NewLine { get; set; } = Environment.NewLine;
    }
}
