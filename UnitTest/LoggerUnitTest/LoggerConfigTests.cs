using System;
using System.IO;
using ShiJing.Utils.Log;

namespace ShiJing.LoggerUnitTest;

/// <summary>
/// <see cref="LoggerConfig"/> 默认值与 <see cref="LogOutputTarget"/> 标志位组合测试。
/// </summary>
public class LoggerConfigTests
{
    [Fact]
    public void DefaultsAreSensible()
    {
        var config = new LoggerConfig();

        Assert.Equal(LogLevel.Info, config.MinLevel);
        Assert.Equal(LogOutputTarget.All, config.OutputTarget);
        Assert.Equal("all.log", config.AllFileName);
        Assert.Equal("level", config.LevelFileNamePrefix);
        Assert.Equal(10, config.MaxRollingFiles);
    }

    [Theory]
    [InlineData(LogOutputTarget.None, false, false)]
    [InlineData(LogOutputTarget.Console, true, false)]
    [InlineData(LogOutputTarget.File, false, true)]
    [InlineData(LogOutputTarget.All, true, true)]
    public void OutputTargetFlagsCompose(LogOutputTarget target, bool console, bool file)
    {
        Assert.Equal(console, target.HasFlag(LogOutputTarget.Console));
        Assert.Equal(file, target.HasFlag(LogOutputTarget.File));
    }

    [Fact]
    public void MaxFileSizeDefaultIsTenMegabytes()
    {
        var config = new LoggerConfig();

        Assert.Equal(10L * 1024 * 1024, config.MaxFileSizeBytes);
    }

    [Fact]
    public void LogDirectoryDefaultLiesUnderBaseDirectory()
    {
        var config = new LoggerConfig();

        // 默认目录应为 BaseDirectory 下的 logs 子目录。
        Assert.Equal(
            Path.Combine(AppContext.BaseDirectory, "logs"),
            config.LogDirectory);
    }

    [Fact]
    public void NewLineDefaultMatchesEnvironmentNewLine()
    {
        var config = new LoggerConfig();

        Assert.Equal(Environment.NewLine, config.NewLine);
    }

    [Fact]
    public void Properties_AreMutableAndPersistAssignedValues()
    {
        // LoggerConfig 为 POCO，setter 应持久化赋值，供 LoggerService 重建目标时读取。
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Fatal,
            OutputTarget = LogOutputTarget.Console,
            LogDirectory = "/tmp/custom",
            AllFileName = "app.log",
            LevelFileNamePrefix = "lvl",
            MaxFileSizeBytes = 2048,
            MaxRollingFiles = 3,
            NewLine = "\n"
        };

        Assert.Equal(LogLevel.Fatal, config.MinLevel);
        Assert.Equal(LogOutputTarget.Console, config.OutputTarget);
        Assert.Equal("/tmp/custom", config.LogDirectory);
        Assert.Equal("app.log", config.AllFileName);
        Assert.Equal("lvl", config.LevelFileNamePrefix);
        Assert.Equal(2048, config.MaxFileSizeBytes);
        Assert.Equal(3, config.MaxRollingFiles);
        Assert.Equal("\n", config.NewLine);
    }

    [Fact]
    public void OutputTarget_NoneHasNoFlags()
    {
        // None=0，既不包含 Console 也不包含 File，BuildTargets 据此构建空目标列表。
        Assert.False(LogOutputTarget.None.HasFlag(LogOutputTarget.Console));
        Assert.False(LogOutputTarget.None.HasFlag(LogOutputTarget.File));
    }
}
