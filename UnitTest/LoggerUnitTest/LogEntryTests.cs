using System;
using ShiJing.Utils.Log;

namespace ShiJing.LoggerUnitTest;

/// <summary>
/// <see cref="LogEntry.Format"/> 文本格式测试。
/// 保证写入控制台/文件的日志格式稳定可解析。
/// </summary>
public class LogEntryTests
{
    [Fact]
    public void Format_ContainsTimestampLevelSourceAndMessage()
    {
        var ts = new DateTime(2026, 6, 22, 10, 30, 45, 123);
        var entry = new LogEntry(ts, LogLevel.Info, "MyModule", "hello");

        var formatted = entry.Format();

        Assert.Contains("2026-06-22 10:30:45.123", formatted);
        Assert.Contains("[INFO]", formatted);
        Assert.Contains("[MyModule]", formatted);
        Assert.Contains("hello", formatted);
    }

    [Fact]
    public void Format_OmitsSourceSegmentWhenSourceEmpty()
    {
        var entry = new LogEntry(DateTime.Now, LogLevel.Warn, "", "no source");

        var formatted = entry.Format();

        Assert.DoesNotContain("[]", formatted);
        Assert.EndsWith("no source", formatted);
    }

    [Fact]
    public void Format_AppendsExceptionOnNewLineWhenProvided()
    {
        var ex = new InvalidOperationException("boom");
        var entry = new LogEntry(DateTime.Now, LogLevel.Error, "Svc", "failed", ex);

        var formatted = entry.Format();

        // Format() 使用 "\n" 作为异常的换行分隔符。
        Assert.Contains("\n", formatted);
        Assert.Contains("boom", formatted);
        Assert.Contains("System.InvalidOperationException", formatted);
    }

    [Fact]
    public void Format_DoesNotAppendExceptionWhenNull()
    {
        var entry = new LogEntry(DateTime.Now, LogLevel.Error, "Svc", "failed", null);

        var formatted = entry.Format();

        Assert.DoesNotContain("\n", formatted);
    }

    [Fact]
    public void Constructor_NullArgumentsAreNormalizedToEmpty()
    {
        var entry = new LogEntry(DateTime.Now, LogLevel.Info, null!, null!, null);

        Assert.Equal(string.Empty, entry.Source);
        Assert.Equal(string.Empty, entry.Message);
        Assert.Null(entry.Exception);
    }

    [Fact]
    public void Constructor_PreservesAllPropertyValues()
    {
        // 构造后属性应为不可变快照，原样回传。
        var ts = new DateTime(2026, 6, 22, 12, 0, 0, 456);
        var ex = new InvalidOperationException("boom");
        var entry = new LogEntry(ts, LogLevel.Warn, "Mod", "msg", ex);

        Assert.Equal(ts, entry.Timestamp);
        Assert.Equal(LogLevel.Warn, entry.Level);
        Assert.Equal("Mod", entry.Source);
        Assert.Equal("msg", entry.Message);
        Assert.Same(ex, entry.Exception);
    }

    [Fact]
    public void Constructor_EmptyStringSourceIsKeptAsEmpty()
    {
        // 区别于 null：空串应原样保留（而非替换为 "Application" 等占位）。
        var entry = new LogEntry(DateTime.Now, LogLevel.Info, "", "msg");

        Assert.Equal(string.Empty, entry.Source);
    }

    [Fact]
    public void Format_EmptyMessageStillProducesValidLine()
    {
        // 空消息不应破坏行结构：时间戳 + 等级 + 来源段 + 单空格后为空。
        var ts = new DateTime(2026, 6, 22, 8, 0, 0, 0);
        var entry = new LogEntry(ts, LogLevel.Info, "Mod", "");

        var formatted = entry.Format();

        Assert.Contains("2026-06-22 08:00:00.000", formatted);
        Assert.Contains("[INFO]", formatted);
        Assert.Contains("[Mod]", formatted);
    }

    [Fact]
    public void Format_LevelIsUpperInvariant()
    {
        // 枚举名应转为大写（ToUpperInvariant），与日志约定一致。
        var entry = new LogEntry(DateTime.Now, LogLevel.Trace, "S", "m");

        Assert.Contains("[TRACE]", entry.Format());
    }

    [Fact]
    public void Format_ExceptionStackTraceIsIncluded()
    {
        // 异常通过 ToString() 渲染，应包含类型名与消息，便于事后排查。
        var ex = new InvalidOperationException("boom");
        var entry = new LogEntry(DateTime.Now, LogLevel.Error, "Svc", "failed", ex);

        var formatted = entry.Format();

        Assert.Contains("System.InvalidOperationException", formatted);
        Assert.Contains("boom", formatted);
    }
}
