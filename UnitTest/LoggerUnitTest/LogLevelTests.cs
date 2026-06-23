using ShiJing.Utils.Log;

namespace ShiJing.LoggerUnitTest;

/// <summary>
/// <see cref="LogLevel"/> 枚举顺序与比较行为测试。
/// 等级数值必须按严重程度递增，<see cref="LoggerService"/> 的 MinLevel 过滤依赖此约定。
/// </summary>
public class LogLevelTests
{
    [Fact]
    public void LevelsAreOrderedBySeverity()
    {
        Assert.True(LogLevel.Trace < LogLevel.Debug);
        Assert.True(LogLevel.Debug < LogLevel.Info);
        Assert.True(LogLevel.Info < LogLevel.Warn);
        Assert.True(LogLevel.Warn < LogLevel.Error);
        Assert.True(LogLevel.Error < LogLevel.Fatal);
    }

    [Theory]
    [InlineData(LogLevel.Trace, 0)]
    [InlineData(LogLevel.Debug, 1)]
    [InlineData(LogLevel.Info, 2)]
    [InlineData(LogLevel.Warn, 3)]
    [InlineData(LogLevel.Error, 4)]
    [InlineData(LogLevel.Fatal, 5)]
    public void LevelsHaveExpectedNumericValues(LogLevel level, int expected)
    {
        Assert.Equal(expected, (int)level);
    }
}
