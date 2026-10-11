using Serilog;
using System.IO;

namespace ShiJing.Utils.Logger
{
    public static class LogConfig
    {
        public static void Initialize()
        {
            var logDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "logs");

            Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()

                // 日志公共属性
                .Enrich.FromLogContext()

                // 异步写入文件
                .WriteTo.Async(
                    sink => sink.File(
                        path: Path.Combine(logDirectory, "app-.log"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileTimeLimit: TimeSpan.FromDays(14),
                        retainedFileCountLimit: 30,
                        rollOnFileSizeLimit: true,
                        fileSizeLimitBytes: 50 * 1024 * 1024,
                        outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} " +
                            "[{Level:u3}] " +
                            "[{SourceFile}:{SourceLine}] " +
                            "{Message:lj}{NewLine}{Exception}"
                    ),
                    bufferSize: 10000
                )

                // 发生日志系统自身错误时输出诊断信息
                .CreateLogger();
        }
    }
}
