using System;

namespace ShiJing.Utils.Log
{
    /// <summary>
    /// 控制台日志目标。按等级着色输出到标准错误/输出流，
    /// Warning 及以上写到 <see cref="Console.Error"/>，其余写到 <see cref="Console.Out"/>。
    /// </summary>
    public sealed class ConsoleLogTarget : LogTarget
    {
        public override void Write(LogEntry entry)
        {
            var line = entry.Format();

            // 着色：仅对交互式控制台生效，重定向到文件时保持纯文本。
            var color = GetColor(entry.Level);
            var prev = Console.ForegroundColor;
            try
            {
                var writer = entry.Level >= LogLevel.Warn ? Console.Error : Console.Out;
                if (color.HasValue)
                {
                    Console.ForegroundColor = color.Value;
                }
                writer.WriteLine(line);
            }
            finally
            {
                Console.ForegroundColor = prev;
            }
        }

        private static ConsoleColor? GetColor(LogLevel level) => level switch
        {
            LogLevel.Trace => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warn => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Fatal => ConsoleColor.Magenta,
            _ => null
        };
    }
}
