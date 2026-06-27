using System;
using System.ComponentModel;
using ShiJing.Model.Settings;
using ShiJing.Utils.Log;

namespace ShiJing.ViewModels.Settings
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
            settingsModel = new SettingsModel();

            // 从全局 LoggerService 当前生效配置载入，使界面反映真实状态
            settingsModel.Logger.LoadFromService();

            // 日志等级变更 → 立即应用到全局 LoggerService（重建输出目标）
            // 在 LoadFromService 之后订阅，避免初始化时触发一次无意义的 Apply
            settingsModel.Logger.PropertyChanged += Logger_PropertyChanged;
        }

        public SettingsModel settingsModel { get; set; }

        /// <summary>所有可选日志等级，供 ComboBox 绑定。</summary>
        public LogLevel[] LogLevelValues { get; } = (LogLevel[])Enum.GetValues(typeof(LogLevel));

        private void Logger_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoggerSettingModel.MinLevel))
            {
                settingsModel.Logger.Apply();
            }
        }
    }
}
