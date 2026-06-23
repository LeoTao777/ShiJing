using System;
using System.IO;
using ShiJing.Utils.Log;

namespace ShiJing.LoggerUnitTest;

/// <summary>
/// <see cref="ConsoleLogTarget"/> 测试：通过重定向 <see cref="Console.Out"/>/
/// <see cref="Console.Error"/> 捕获输出，验证 Warn 及以上写标准错误、其余写标准输出。
/// </summary>
public class ConsoleLogTargetTests
{
    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Info)]
    public void BelowWarn_WritesToConsoleOut(LogLevel level)
    {
        var target = new ConsoleLogTarget();
        var entry = new LogEntry(DateTime.Now, level, "S", "stdout message");

        using var sw = new StringWriter();
        var prevOut = Console.Out;
        try
        {
            Console.SetOut(sw);
            target.Write(entry);
        }
        finally
        {
            Console.SetOut(prevOut);
        }

        Assert.Contains("stdout message", sw.ToString());
    }

    [Theory]
    [InlineData(LogLevel.Warn)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Fatal)]
    public void WarnAndAbove_WritesToConsoleError(LogLevel level)
    {
        var target = new ConsoleLogTarget();
        var entry = new LogEntry(DateTime.Now, level, "S", "stderr message");

        using var sw = new StringWriter();
        var prevErr = Console.Error;
        try
        {
            Console.SetError(sw);
            target.Write(entry);
        }
        finally
        {
            Console.SetError(prevErr);
        }

        Assert.Contains("stderr message", sw.ToString());
    }

    [Fact]
    public void Write_OutputContainsFormattedTimestamp()
    {
        var target = new ConsoleLogTarget();
        var ts = new DateTime(2026, 6, 22, 8, 0, 0, 0);
        var entry = new LogEntry(ts, LogLevel.Info, "Mod", "hi");

        using var sw = new StringWriter();
        var prevOut = Console.Out;
        try
        {
            Console.SetOut(sw);
            target.Write(entry);
        }
        finally
        {
            Console.SetOut(prevOut);
        }

        var output = sw.ToString();
        Assert.Contains("2026-06-22 08:00:00.000", output);
        Assert.Contains("[INFO]", output);
        Assert.Contains("[Mod]", output);
    }

    [Fact]
    public void Write_TerminatesLineWithNewline()
    {
        // ConsoleLogTarget 使用 WriteLine，输出应以换行结尾，便于在重定向文件中分行。
        var target = new ConsoleLogTarget();
        var entry = new LogEntry(DateTime.Now, LogLevel.Info, "S", "line");

        using var sw = new StringWriter();
        var prevOut = Console.Out;
        try
        {
            Console.SetOut(sw);
            target.Write(entry);
        }
        finally
        {
            Console.SetOut(prevOut);
        }

        Assert.EndsWith(Environment.NewLine, sw.ToString());
    }

    [Fact]
    public void Write_RestoresConsoleForegroundColor()
    {
        // Write 在 finally 中恢复前景色；无论是否着色，调用后颜色应回到原值。
        var target = new ConsoleLogTarget();
        var entry = new LogEntry(DateTime.Now, LogLevel.Info, "S", "msg");

        var original = Console.ForegroundColor;
        using var sw = new StringWriter();
        var prevOut = Console.Out;
        try
        {
            Console.SetOut(sw);
            target.Write(entry);
        }
        finally
        {
            Console.SetOut(prevOut);
        }

        Assert.Equal(original, Console.ForegroundColor);
    }

    [Theory]
    [InlineData(LogLevel.Trace, "trace-msg")]
    [InlineData(LogLevel.Debug, "debug-msg")]
    [InlineData(LogLevel.Info, "info-msg")]
    [InlineData(LogLevel.Warn, "warn-msg")]
    [InlineData(LogLevel.Error, "error-msg")]
    [InlineData(LogLevel.Fatal, "fatal-msg")]
    public void Write_EveryLevelEmitsMessage(LogLevel level, string message)
    {
        // 全级别均应输出正文，确保没有等级被意外静默。
        var target = new ConsoleLogTarget();
        var entry = new LogEntry(DateTime.Now, level, "S", message);

        using var sw = new StringWriter();
        TextWriter prevWriter = level >= LogLevel.Warn ? Console.Error : Console.Out;
        try
        {
            if (level >= LogLevel.Warn)
            {
                Console.SetError(sw);
            }
            else
            {
                Console.SetOut(sw);
            }
            target.Write(entry);
        }
        finally
        {
            if (level >= LogLevel.Warn)
            {
                Console.SetError(prevWriter);
            }
            else
            {
                Console.SetOut(prevWriter);
            }
        }

        Assert.Contains(message, sw.ToString());
    }
}
