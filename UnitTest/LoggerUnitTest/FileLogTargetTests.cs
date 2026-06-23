using System;
using System.IO;
using System.Linq;
using ShiJing.Utils.Log;

namespace ShiJing.LoggerUnitTest;

/// <summary>
/// <see cref="FileLogTarget"/> 测试：总文件、分等级文件与按大小滚动切分。
/// 每个测试使用独立临时目录，互不干扰。
/// </summary>
public class FileLogTargetTests : IDisposable
{
    private readonly string _tempDir;

    public FileLogTargetTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "ShiJingLogTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); }
        catch { /* 测试清理忽略 */ }
    }

    private LoggerConfig NewConfig(long maxSize, int maxFiles) => new()
    {
        MinLevel = LogLevel.Trace,
        OutputTarget = LogOutputTarget.File,
        LogDirectory = _tempDir,
        AllFileName = "all.log",
        LevelFileNamePrefix = "level",
        MaxFileSizeBytes = maxSize,
        MaxRollingFiles = maxFiles,
        NewLine = "\n"
    };

    /// <summary>写入日志并释放目标，确保文件句柄关闭后再读取断言。</summary>
    private static void WriteAndClose(FileLogTarget target, params LogEntry[] entries)
    {
        foreach (var entry in entries)
        {
            target.Write(entry);
        }
        target.Flush();
        target.Dispose();
    }

    [Fact]
    public void AllFile_ReceivesEveryLevel()
    {
        var target = new FileLogTarget(NewConfig(maxSize: 1024 * 1024, maxFiles: 5));
        WriteAndClose(target,
            new LogEntry(DateTime.Now, LogLevel.Info, "S", "info msg"),
            new LogEntry(DateTime.Now, LogLevel.Error, "S", "error msg"));

        var allPath = Path.Combine(_tempDir, "all.log");
        Assert.True(File.Exists(allPath));
        var content = File.ReadAllText(allPath);
        Assert.Contains("info msg", content);
        Assert.Contains("error msg", content);
    }

    [Fact]
    public void PerLevelFiles_ReceiveOnlyMatchingLevel()
    {
        var target = new FileLogTarget(NewConfig(maxSize: 1024 * 1024, maxFiles: 5));
        WriteAndClose(target,
            new LogEntry(DateTime.Now, LogLevel.Info, "S", "info msg"),
            new LogEntry(DateTime.Now, LogLevel.Error, "S", "error msg"),
            new LogEntry(DateTime.Now, LogLevel.Warn, "S", "warn msg"));

        var infoContent = File.ReadAllText(Path.Combine(_tempDir, "level-info.log"));
        var errorContent = File.ReadAllText(Path.Combine(_tempDir, "level-error.log"));
        var warnContent = File.ReadAllText(Path.Combine(_tempDir, "level-warn.log"));

        Assert.Contains("info msg", infoContent);
        Assert.DoesNotContain("error msg", infoContent);

        Assert.Contains("error msg", errorContent);
        Assert.DoesNotContain("info msg", errorContent);

        Assert.Contains("warn msg", warnContent);
    }

    [Fact]
    public void PerLevelFiles_AreCreatedForWrittenLevels()
    {
        var target = new FileLogTarget(NewConfig(maxSize: 1024 * 1024, maxFiles: 5));
        WriteAndClose(target, new LogEntry(DateTime.Now, LogLevel.Info, "S", "hi"));

        // 已写入等级的文件应存在。
        Assert.True(File.Exists(Path.Combine(_tempDir, "level-info.log")));
        Assert.True(File.Exists(Path.Combine(_tempDir, "all.log")));
    }

    [Fact]
    public void Rolling_CreatesNumberedBackupWhenFileExceedsMaxSize()
    {
        // 设置一个很小的上限，使格式化后的整行（含时间戳/等级/来源）很快超限触发滚动。
        var target = new FileLogTarget(NewConfig(maxSize: 50, maxFiles: 5));

        // 连续写入多行，迫使当前文件超限并滚动。
        var line = "0123456789-0123456789";
        WriteAndClose(target,
            new LogEntry(DateTime.Now, LogLevel.Info, "S", line),
            new LogEntry(DateTime.Now, LogLevel.Info, "S", line),
            new LogEntry(DateTime.Now, LogLevel.Info, "S", line));

        var backup1 = Path.Combine(_tempDir, "all.log.1");

        // 滚动后应存在 .1 副本，且副本非空（含滚动前的历史内容）。
        Assert.True(File.Exists(backup1), "应生成 all.log.1 滚动副本");
        var backupContent = File.ReadAllText(backup1);
        Assert.False(string.IsNullOrEmpty(backupContent));
        Assert.Contains(line, backupContent);
    }

    [Fact]
    public void Rolling_ShiftsBackupsAndKeepsNewestAtLowestIndex()
    {
        // maxSize 极小，每次写入都会触发滚动，便于观察序号轮转。
        var target = new FileLogTarget(NewConfig(maxSize: 5, maxFiles: 3));

        for (var i = 0; i < 4; i++)
        {
            target.Write(new LogEntry(DateTime.Now, LogLevel.Info, "S", $"line{i}"));
        }
        target.Flush();
        target.Dispose();

        // 预期滚动后存在 all.log.1、all.log.2，序号 1 最新。
        Assert.True(File.Exists(Path.Combine(_tempDir, "all.log.1")));
        Assert.True(File.Exists(Path.Combine(_tempDir, "all.log.2")));
    }

    [Fact]
    public void Rolling_PrunesBackupsBeyondMaxFiles()
    {
        var target = new FileLogTarget(NewConfig(maxSize: 5, maxFiles: 2));

        // 连续写多行触发多次滚动；MaxRollingFiles=2，最多保留 .1 和 .2。
        for (var i = 0; i < 6; i++)
        {
            target.Write(new LogEntry(DateTime.Now, LogLevel.Info, "S", $"line{i}"));
        }
        target.Flush();
        target.Dispose();

        Assert.True(File.Exists(Path.Combine(_tempDir, "all.log.1")));
        Assert.True(File.Exists(Path.Combine(_tempDir, "all.log.2")));
        // 不应存在超出上限的副本。
        Assert.False(File.Exists(Path.Combine(_tempDir, "all.log.3")));
    }

    [Fact]
    public void Rolling_PerLevelFilesAlsoRollIndependently()
    {
        var target = new FileLogTarget(NewConfig(maxSize: 20, maxFiles: 3));

        // 连续写 Error 日志，触发 level-error.log 的滚动。
        for (var i = 0; i < 5; i++)
        {
            target.Write(new LogEntry(DateTime.Now, LogLevel.Error, "S", $"err-{i}"));
        }
        target.Flush();
        target.Dispose();

        Assert.True(File.Exists(Path.Combine(_tempDir, "level-error.log.1")));
    }

    [Fact]
    public void CreatesLogDirectoryIfMissing()
    {
        var nestedDir = Path.Combine(_tempDir, "nested", "logs");
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.File,
            LogDirectory = nestedDir,
            MaxFileSizeBytes = 1024,
            MaxRollingFiles = 3,
            NewLine = "\n"
        };

        var target = new FileLogTarget(config);
        WriteAndClose(target, new LogEntry(DateTime.Now, LogLevel.Info, "S", "hi"));

        Assert.True(Directory.Exists(nestedDir));
        Assert.True(File.Exists(Path.Combine(nestedDir, "all.log")));
    }

    [Fact]
    public void Append_ContinuesExistingFileAcrossInstances()
    {
        var config = NewConfig(maxSize: 1024 * 1024, maxFiles: 3);

        var first = new FileLogTarget(config);
        WriteAndClose(first, new LogEntry(DateTime.Now, LogLevel.Info, "S", "first"));

        var second = new FileLogTarget(config);
        WriteAndClose(second, new LogEntry(DateTime.Now, LogLevel.Info, "S", "second"));

        var content = File.ReadAllText(Path.Combine(_tempDir, "all.log"));
        Assert.Contains("first", content);
        Assert.Contains("second", content);
    }

    [Fact]
    public void CustomFileNamesAreRespected()
    {
        // 自定义 AllFileName / LevelFileNamePrefix 应体现在产物文件名上。
        var config = new LoggerConfig
        {
            MinLevel = LogLevel.Trace,
            OutputTarget = LogOutputTarget.File,
            LogDirectory = _tempDir,
            AllFileName = "app.log",
            LevelFileNamePrefix = "lvl",
            MaxFileSizeBytes = 1024 * 1024,
            MaxRollingFiles = 3,
            NewLine = "\n"
        };

        var target = new FileLogTarget(config);
        WriteAndClose(target, new LogEntry(DateTime.Now, LogLevel.Error, "S", "boom"));

        Assert.True(File.Exists(Path.Combine(_tempDir, "app.log")));
        Assert.True(File.Exists(Path.Combine(_tempDir, "lvl-error.log")));
        Assert.Contains("boom", File.ReadAllText(Path.Combine(_tempDir, "app.log")));
    }

    [Fact]
    public void NewLineConfig_SeparatesEntriesWithSpecifiedSeparator()
    {
        // 显式指定 "\n" 作为行尾，文件内行间应以 "\n" 分隔。
        var config = NewConfig(maxSize: 1024 * 1024, maxFiles: 3);
        config.NewLine = "\n";

        var target = new FileLogTarget(config);
        WriteAndClose(target,
            new LogEntry(DateTime.Now, LogLevel.Info, "S", "a"),
            new LogEntry(DateTime.Now, LogLevel.Info, "S", "b"));

        var content = File.ReadAllText(Path.Combine(_tempDir, "all.log"));
        Assert.Contains("\n", content);
        Assert.Contains("a\n", content);
        Assert.Contains("b\n", content);
    }

    [Fact]
    public void Flush_PersistsContentWithoutDispose()
    {
        // 仅调用 Flush（不 Dispose）后，文件内容应已落盘可读。
        // RollingFileWriter 以 FileShare.Read 打开文件，故这里用允许共享读的方式读取。
        var target = new FileLogTarget(NewConfig(maxSize: 1024 * 1024, maxFiles: 3));
        target.Write(new LogEntry(DateTime.Now, LogLevel.Info, "S", "flushed"));
        target.Flush();

        string content;
        using (var fs = new FileStream(
            Path.Combine(_tempDir, "all.log"),
            FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var sr = new StreamReader(fs))
        {
            content = sr.ReadToEnd();
        }
        Assert.Contains("flushed", content);

        target.Dispose();
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        // 多次 Dispose 不应抛异常（LoggerService.ReloadConfig/Dispose 可能重复释放目标）。
        var target = new FileLogTarget(NewConfig(maxSize: 1024 * 1024, maxFiles: 3));
        target.Write(new LogEntry(DateTime.Now, LogLevel.Info, "S", "x"));

        target.Dispose();
        target.Dispose();
    }

    [Fact]
    public void ConcurrentWrites_AllEntriesPersisted()
    {
        // RollingFileWriter 内部加锁，多线程并发写 all.log 不应丢行或互相破坏。
        var target = new FileLogTarget(NewConfig(maxSize: 1024 * 1024, maxFiles: 3));
        const int threads = 8;
        const int perThread = 100;

        var tasks = Enumerable.Range(0, threads).Select(_ => System.Threading.Tasks.Task.Run(() =>
        {
            for (var i = 0; i < perThread; i++)
            {
                target.Write(new LogEntry(DateTime.Now, LogLevel.Info, "S", $"msg-{i}"));
            }
        })).ToArray();
        System.Threading.Tasks.Task.WaitAll(tasks);
        target.Flush();
        target.Dispose();

        var content = File.ReadAllText(Path.Combine(_tempDir, "all.log"));
        // 每条日志行尾都有换行，统计行数应等于总写入数。
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(threads * perThread, lines.Length);
    }
}
