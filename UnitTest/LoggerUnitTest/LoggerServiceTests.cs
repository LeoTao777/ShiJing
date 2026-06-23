using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ShiJing.Utils.Log;

namespace ShiJing.LoggerUnitTest;

/// <summary>
/// 用于在测试中捕获分发日志的内存目标，避免依赖磁盘/控制台。
/// </summary>
internal sealed class RecordingLogTarget : LogTarget
{
    public List<LogEntry> Entries { get; } = new();
    public object Lock { get; } = new();

    public override void Write(LogEntry entry)
    {
        lock (Lock)
        {
            Entries.Add(entry);
        }
    }
}

/// <summary>
/// <see cref="LoggerService"/> 的核心行为测试：
/// 等级过滤、来源标记、分发、GetLogger 复用、配置热加载、线程安全。
/// </summary>
public class LoggerServiceTests
{
    private static LoggerService CreateWithRecording(
        LoggerConfig config, out RecordingLogTarget recorder)
    {
        var service = new LoggerService(config);
        recorder = new RecordingLogTarget();
        // 通过反射将 recorder 注入到 service 的 _targets 字段，
        // 替换默认按 OutputTarget 构建的目标，便于纯内存断言。
        var field = typeof(LoggerService).GetField(
            "_targets",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(service, new List<LogTarget> { recorder });
        return service;
    }

    [Fact]
    public void Dispatch_DropsEntriesBelowMinLevel()
    {
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Warn,
            OutputTarget = LogOutputTarget.None
        };
        var service = CreateWithRecording(config, out var recorder);

        var logger = service.GetLogger("Test");
        logger.Info("should be dropped");
        logger.Warn("kept");
        logger.Error("kept too");

        Assert.Equal(2, recorder.Entries.Count);
        Assert.Equal(LogLevel.Warn, recorder.Entries[0].Level);
        Assert.Equal(LogLevel.Error, recorder.Entries[1].Level);
    }

    [Fact]
    public void Dispatch_PassesThroughAtMinLevelBoundary()
    {
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Info,
            OutputTarget = LogOutputTarget.None
        };
        var service = CreateWithRecording(config, out var recorder);

        var logger = service.GetLogger("Test");
        logger.Trace("drop"); // < Info
        logger.Debug("drop"); // < Info
        logger.Info("keep");  // == Info, 通过

        Assert.Single(recorder.Entries);
        Assert.Equal(LogLevel.Info, recorder.Entries[0].Level);
    }

    [Fact]
    public void GetLogger_SameSourceReturnsSameInstance()
    {
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });

        var a = service.GetLogger("Foo");
        var b = service.GetLogger("Foo");

        Assert.Same(a, b);
    }

    [Fact]
    public void GetLogger_DifferentSourceReturnsDifferentInstance()
    {
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });

        var a = service.GetLogger("Foo");
        var b = service.GetLogger("Bar");

        Assert.NotSame(a, b);
    }

    [Fact]
    public void GetLogger_EmptySourceFallsBackToApplication()
    {
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });

        var logger = service.GetLogger("");

        Assert.Equal("Application", logger.Source);
    }

    [Fact]
    public void GetLogger_ByTypeUsesTypeFullName()
    {
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });

        var logger = service.GetLogger(typeof(LoggerServiceTests));

        Assert.Equal(typeof(LoggerServiceTests).FullName, logger.Source);
    }

    [Fact]
    public void Logger_PreservesSourceAndMessageInDispatchedEntry()
    {
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.None
        };
        var service = CreateWithRecording(config, out var recorder);

        var logger = service.GetLogger("Billing");
        logger.Warn("low balance");

        Assert.Single(recorder.Entries);
        Assert.Equal("Billing", recorder.Entries[0].Source);
        Assert.Equal("low balance", recorder.Entries[0].Message);
        Assert.Equal(LogLevel.Warn, recorder.Entries[0].Level);
    }

    [Fact]
    public void Logger_ExceptionIsCarriedThrough()
    {
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.None
        };
        var service = CreateWithRecording(config, out var recorder);

        var logger = service.GetLogger("Test");
        var ex = new ArgumentException("bad arg");
        logger.Error("failed", ex);

        Assert.Single(recorder.Entries);
        Assert.Same(ex, recorder.Entries[0].Exception);
    }

    [Fact]
    public void ForSource_ReturnsLoggerBoundToGivenSource()
    {
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });
        var parent = service.GetLogger("Parent");

        var child = parent.ForSource("Child");

        Assert.Equal("Child", child.Source);
    }

    [Fact]
    public void ReloadConfig_ThrowsOnNull()
    {
        var service = new LoggerService(new LoggerConfig());

        Assert.Throws<ArgumentNullException>(() => service.ReloadConfig(null!));
    }

    [Fact]
    public void ReloadConfig_RebuildsTargetsToUseNewMinLevel()
    {
        var service = new LoggerService(new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.None
        });

        // 用 recording 目标替换默认目标。
        var recorder = new RecordingLogTarget();
        var field = typeof(LoggerService).GetField(
            "_targets",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(service, new List<LogTarget> { recorder });

        var logger = service.GetLogger("Test");
        logger.Debug("before reload: kept");

        // 热加载：把最低等级提升到 Error。
        service.ReloadConfig(new LoggerConfig
        {
            MinLevel = LogLevel.Error,
            OutputTarget = LogOutputTarget.None
        });

        // reload 会重建目标列表（变为空），故 recorder 不应再收到新日志。
        logger.Warn("after reload: dropped");

        Assert.Single(recorder.Entries);
        Assert.Equal("before reload: kept", recorder.Entries[0].Message);
    }

    [Fact]
    public void Dispatch_IsThreadSafeUnderConcurrency()
    {
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.None
        };
        var service = CreateWithRecording(config, out var recorder);

        var logger = service.GetLogger("Concurrent");
        const int threads = 8;
        const int perThread = 200;

        var tasks = Enumerable.Range(0, threads).Select(_ => System.Threading.Tasks.Task.Run(() =>
        {
            for (var i = 0; i < perThread; i++)
            {
                logger.Info($"msg-{i}");
            }
        })).ToArray();

        System.Threading.Tasks.Task.WaitAll(tasks);

        Assert.Equal(threads * perThread, recorder.Entries.Count);
    }

    [Fact]
    public void Dispatch_AfterDisposeDropsEntriesSilently()
    {
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.None
        };
        var service = CreateWithRecording(config, out var recorder);
        var logger = service.GetLogger("Test");

        service.Dispose();
        logger.Info("after dispose");

        Assert.Empty(recorder.Entries);
    }

    [Fact]
    public void Constructor_NullConfig_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LoggerService(null!));
    }

    [Fact]
    public void Config_ReturnsInitiallyProvidedConfig()
    {
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Warn,
            OutputTarget = LogOutputTarget.None
        };
        var service = new LoggerService(config);

        Assert.Same(config, service.Config);
    }

    [Fact]
    public void GetLogger_NullSourceFallsBackToApplication()
    {
        // GetLogger(string) 对 null 做空判断兜底，与空串一致返回 "Application"。
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });

        var logger = service.GetLogger((string)null!);

        Assert.Equal("Application", logger.Source);
    }

    [Fact]
    public void GetLogger_ByNullTypeFallsBackToApplication()
    {
        // GetLogger(Type) 在 type 为 null 时使用 "Application"。
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });

        var logger = service.GetLogger((Type)null!);

        Assert.Equal("Application", logger.Source);
    }

    [Fact]
    public void GetLogger_CachesApplicationLoggerForEmptyAndNull()
    {
        // 空串与 null 均归一化为 "Application"，应复用同一实例。
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });

        var a = service.GetLogger("");
        var b = service.GetLogger((string)null!);

        Assert.Same(a, b);
    }

    [Fact]
    public void ReloadConfig_ThrowsAfterDispose()
    {
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });
        service.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            service.ReloadConfig(new LoggerConfig { OutputTarget = LogOutputTarget.None }));
    }

    [Fact]
    public void ReloadConfig_PropagatesNewConfigViaConfigGetter()
    {
        var service = new LoggerService(new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.None
        });

        var newConfig = new LoggerConfig
        {
            MinLevel = LogLevel.Fatal,
            OutputTarget = LogOutputTarget.None
        };
        service.ReloadConfig(newConfig);

        Assert.Same(newConfig, service.Config);
    }

    [Fact]
    public void Flush_DoesNotThrowAfterDispose()
    {
        // Dispose 后 Flush 应静默返回，便于在关闭流程中无序调用。
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });
        service.Dispose();

        service.Flush();
    }

    [Fact]
    public void Dispatch_WritesThroughToConfiguredFileTarget()
    {
        // 端到端：默认按 OutputTarget.File 构建的真实文件目标，验证 Dispatch 真的落盘。
        var tempDir = Path.Combine(Path.GetTempPath(), "ShiJingLogTests_e2e_" + Guid.NewGuid().ToString("N"));
        try
        {
            var service = new LoggerService(new LoggerConfig
            {
                MinLevel = LogLevel.Trace,
                OutputTarget = LogOutputTarget.File,
                LogDirectory = tempDir,
                MaxFileSizeBytes = 1024 * 1024,
                MaxRollingFiles = 3,
                NewLine = "\n"
            });

            var logger = service.GetLogger("E2E");
            logger.Info("end-to-end message");
            service.Flush();
            service.Dispose();

            var content = File.ReadAllText(Path.Combine(tempDir, "all.log"));
            Assert.Contains("end-to-end message", content);
            Assert.Contains("[E2E]", content);
        }
        finally
        {
            try { Directory.Delete(tempDir, recursive: true); }
            catch { /* 清理忽略 */ }
        }
    }

    [Fact]
    public void ForSource_ReturnsCachedLoggerForSameSource()
    {
        // ForSource 委托回 GetLogger，相同来源应复用实例。
        var service = new LoggerService(new LoggerConfig
        {
            OutputTarget = LogOutputTarget.None
        });
        var parent = service.GetLogger("Parent");

        var a = parent.ForSource("Child");
        var b = parent.ForSource("Child");

        Assert.Same(a, b);
    }
}
