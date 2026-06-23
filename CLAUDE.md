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
- **单元测试框架**:`xUnit + Moq`
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
├── ShiJing.slnx          # 解决方案（新版 slnx 格式，仅包含单个项目）
├── ShiJing.csproj        # 项目文件（SDK 风格，WPF 启用）
├── App.xaml / App.xaml.cs# 应用入口与资源
├── MainWindow.xaml       # 主窗口 XAML
├── MainWindow.xaml.cs    # 主窗口代码后置
├── AssemblyInfo.cs       # 程序集信息（ThemeInfo 配置）
├── .gitignore
├── README.md
└── LICENSE
```

当前处于初始脚手架阶段：`App` 与 `MainWindow` 均为模板默认实现，业务功能尚未开发。

## 开发约定

- **命名空间**：统一使用 `ShiJing`。
- **XAML 风格**：保持与模板一致，使用 `mc:Ignorable="d"` 以支持设计时数据。
- **可空性**：项目启用 `Nullable`，新增代码需注意可空引用类型标注。
- **隐式 using**：已启用 `ImplicitUsings`，常见命名空间无需手动 `using`，但 WPF 相关命名空间（如 `System.Windows.*`）仍需显式引入。
- **解决方案**：使用 `.slnx` 而非传统 `.sln`，新增项目时通过 `dotnet sln ShiJing.slnx add <项目>` 管理。

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
