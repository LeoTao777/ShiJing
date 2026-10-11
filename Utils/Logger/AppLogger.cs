using Serilog;
using Serilog.Events;
using System.IO;
using System.Runtime.CompilerServices;
namespace ShiJing.Utils.Logger
{
    public static class AppLogger
    {
        public static void Debug(
       string message,
       [CallerFilePath] string filePath = "",
       [CallerLineNumber] int lineNumber = 0)
       => Write(LogEventLevel.Debug, message, null, filePath, lineNumber);

        public static void Info(
            string message,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
            => Write(LogEventLevel.Information, message, null, filePath, lineNumber);

        public static void Warning(
            string message,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
            => Write(LogEventLevel.Warning, message, null, filePath, lineNumber);

        public static void Error(
            string message,
            Exception? exception = null,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
            => Write(LogEventLevel.Error, message, exception, filePath, lineNumber);

        private static void Write(
            LogEventLevel level,
            string message,
            Exception? exception,
            string filePath,
            int lineNumber)
        {
            var logger = Log.ForContext(
                    "SourceFile", Path.GetFileName(filePath))
                .ForContext("SourceLine", lineNumber);

            logger.Write(level, exception, "{Message}", message);
        }
    }
    }
