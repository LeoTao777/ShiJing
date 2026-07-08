using System;
using System.IO;
using System.Text.Json;
using ShiJing.Model.Settings;

namespace ShiJing.Utils.Settings
{
    /// <summary>
    /// 应用配置持久化服务：负责 <c>settings/settings.json</c> 的读写。
    /// 配置目录位于软件运行目录（<see cref="AppContext.BaseDirectory"/>）下的 <c>settings</c> 文件夹。
    /// 文件不存在时生成默认配置；读写失败时回退到默认配置且不抛出，保证启动不中断。
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>配置文件所在目录（运行目录下的 settings 文件夹）。</summary>
        public static string SettingsDirectory { get; } =
            Path.Combine(AppContext.BaseDirectory, "settings");

        /// <summary>配置文件完整路径。</summary>
        public static string SettingsFilePath { get; } =
            Path.Combine(SettingsDirectory, "settings.json");

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        /// <summary>
        /// 加载配置：文件存在则反序列化；不存在则写入默认配置并返回。
        /// 任何异常都不向上抛出，失败时返回默认 <see cref="SettingsModel"/>。
        /// </summary>
        public static SettingsModel LoadOrCreate()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var config = JsonSerializer.Deserialize<SettingsModel>(json, _jsonOptions);
                    return config ?? new SettingsModel();
                }

                // 文件不存在：生成默认配置文件
                var defaultConfig = new SettingsModel();
                Save(defaultConfig);
                return defaultConfig;
            }
            catch (Exception ex)
            {
                // 配置加载/解析失败不应阻断启动，回退到默认配置。
                try { Console.Error.WriteLine($"[SettingsManager] load failed: {ex.Message}"); }
                catch { /* 忽略 */ }
                return new SettingsModel();
            }
        }

        /// <summary>
        /// 将指定配置序列化写入 <c>settings.json</c>。目录不存在时自动创建。
        /// </summary>
        public static void Save(SettingsModel config)
        {
            if (config is null) throw new ArgumentNullException(nameof(config));

            try
            {
                Directory.CreateDirectory(SettingsDirectory);
                var json = JsonSerializer.Serialize(config, _jsonOptions);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                // 写入失败不向上抛出，仅打印，避免保存按钮导致界面崩溃。
                try { Console.Error.WriteLine($"[SettingsManager] save failed: {ex.Message}"); }
                catch { /* 忽略 */ }
            }
        }
    }
}
