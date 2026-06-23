# CLAUDE.md

本文件为 Claude Code（及其他 AI 编程助手）在本仓库中工作时的指引文档。

## 项目概述

**ShiJing（拾境）** 是一款极简、轻量级的本地视频管理桌面应用。核心功能：

- 一键视频帧截图（视频画面快照）
- 本地截图保存
- 截图自动与其来源视频关联

目标平台：Windows 桌面。

## 技术栈

- **运行时 / 语言**：.NET 10 (`net10.0-windows`)，C#（启用 `Nullable` 与 `ImplicitUsings`）
- **UI 框架**：WPF（`UseWPF=true`），XAML + 代码后置（code-behind）
- **单元测试框架**：`xUnit`（测试通过反射注入内存目标实现纯内存断言，未引用 Mock 框架）
- **解决方案格式**：`.slnx`（新版 XML 解决方案格式）
- **输出类型**：`WinExe`（Windows 可执行程序）

## 构建与运行

```bash
# 还原依赖
dotnet restore

# 构建解决方案
dotnet build ShiJing.slnx

# 运行（WPF 应用，需在 Windows 上执行）
dotnet run --project ShiJing.csproj
```

> 说明：项目面向 `net10.0-windows`，需安装 .NET 10 SDK，且只能在 Windows 上构建/运行 WPF。

## 项目结构

```
ShiJing/
├── ShiJing.slnx                # 解决方案（新版 slnx 格式，包含主项目与测试项目）
├── ShiJing.csproj              # 主项目文件（SDK 风格，WPF 启用，排除 UnitTest/** 编译项）
├── Directory.Build.props       # 目录级 MSBuild 属性
├── Directory.Build.targets     # 目录级 MSBuild 目标
├── App.xaml / App.xaml.cs      # 应用入口与资源
├── MainWindow.xaml             # 主窗口 XAML
├── MainWindow.xaml.cs          # 主窗口代码后置
├── AssemblyInfo.cs             # 程序集信息（ThemeInfo 配置）
├── CLAUDE.md                   # AI 编程助手指引
├── doc/                        # 开发文档
│   └── 01-日志系统开发文档.md   # 日志模块设计与开发说明
├── Utils/
│   └── Logger/                 # 日志模块（命名空间 ShiJing.Utils.Log）
│       ├── LogLevel.cs         # 日志等级枚举（Trace~Fatal）
│       ├── ILogger.cs          # 对外日志接口
│       ├── Logger.cs           # ILogger 默认实现（internal）
│       ├── LoggerService.cs    # 单例日志服务，管理配置/目标/分发
│       ├── LoggerConfig.cs     # 日志配置 POCO + LogOutputTarget 枚举
│       ├── LogEntry.cs         # 不可变日志记录模型 + Format()
│       ├── LogTarget.cs        # 输出目标抽象基类
│       ├── ConsoleLogTarget.cs # 控制台目标（按等级着色）
│       ├── FileLogTarget.cs    # 文件目标（总文件 + 分等级文件）
│       └── RollingFileWriter.cs# 单文件滚动写入器（internal）
├── UnitTest/
│   └── LoggerUnitTest/         # 日志模块单元测试（xUnit）
├── .gitignore
├── README.md
└── LICENSE
```

当前进度：脚手架已完成，**日志模块已落地**（核心实现 + 单元测试齐全），其余业务功能（视频导入、播放、截图等）尚未开发。

## 开发约定

- **命名空间**：统一使用 `ShiJing`。
- **XAML 风格**：保持与模板一致，使用 `mc:Ignorable="d"` 以支持设计时数据。
- **可空性**：项目启用 `Nullable`，新增代码需注意可空引用类型标注。
- **隐式 using**：已启用 `ImplicitUsings`，常见命名空间无需手动 `using`，但 WPF 相关命名空间（如 `System.Windows.*`）仍需显式引入。
- **解决方案**：使用 `.slnx` 而非传统 `.sln`，新增项目时通过 `dotnet sln ShiJing.slnx add <项目>` 管理。

## 日志模块

日志系统位于 `Utils/Logger/`（命名空间 `ShiJing.Utils.Log`），为自研轻量组件，详见 [`doc/01-日志系统开发文档.md`](doc/01-日志系统开发文档.md)。

要点：

- **入口**：`LoggerService.Instance`（单例），通过 `GetLogger(string)` / `GetLogger(Type)` 获取 `ILogger`；相同来源复用同一实例。
- **分层**：`ILogger`(public) → `Logger`(internal) → `LoggerService`(public 单例) → `LogTarget` → `RollingFileWriter`(internal)。
- **配置**：`LoggerConfig`（最低等级、输出目标、文件目录、滚动策略、行尾），支持 `ReloadConfig` 运行期热加载。
- **输出目标**：控制台（`ConsoleLogTarget`，按等级着色）+ 文件（`FileLogTarget`，总文件 `all.log` 与分等级文件 `level-<level>.log`，按大小滚动）。
- **线程安全**：`Dispatch` 仅在锁内取目标快照后释放锁再写入，各目标内部自保证线程安全，单目标失败不影响其他目标。
- **测试**：`UnitTest/LoggerUnitTest/`，通过反射注入内存 `RecordingLogTarget` 做纯内存断言。

调用示例：

```csharp
using ShiJing.Utils.Log;

private static readonly ILogger _log = LoggerService.Instance.GetLogger(typeof(MainWindow));
_log.Info("应用启动");
_log.Error("处理失败", ex);
```

## 待开发功能（基于项目定位）

- [ ] 视频导入与本地库管理
- [ ] 视频播放与逐帧浏览
- [ ] 一键帧截图（快照）
- [ ] 截图本地保存与目录组织
- [ ] 截图与来源视频的关联记录

## Git 协作

- 主分支：`main`
- 开发分支命名示例：`feature/<作者>/<功能>`（如 `feature/liutao/log`）
- 提交信息约定：`Feat:` / `Fix:` / `Docs:` / `Refactor:` 等前缀（参考已有提交 `Feat:Init project`）

## 备注

- `.gitignore` 中已忽略 `bin/`、`obj/`、`.vs/`、`*.user` 等构建与 IDE 产物。
- 不要将 `.artifact/`、`.vs/` 等目录提交到仓库。
