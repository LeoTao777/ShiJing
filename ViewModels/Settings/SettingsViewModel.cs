using ShiJing.Model.Settings;
using ShiJing.Utils.Settings;

namespace ShiJing.ViewModels.Settings
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel()
        {
            settingsModel = new SettingsModel();

            // 从全局 LoggerService 当前生效配置载入，使界面反映真实状态
            // （启动时已由 App.OnStartup 从 settings.json 加载到 LoggerService）
            //settingsModel.Logger.LoadFromService();

            SaveCommand = new DelegateCommand(Save);
        }

        public SettingsModel settingsModel { get; set; }

        /// <summary>所有可选日志等级，供 ComboBox 绑定。</summary>
        //public LogLevel[] LogLevelValues { get; } = (LogLevel[])Enum.GetValues(typeof(LogLevel));

        /// <summary>保存按钮命令：写入 settings.json 并应用到全局 LoggerService。</summary>
        public DelegateCommand SaveCommand { get; }

        private void Save()
        {
            // 1. 持久化到 settings/settings.json
            SettingsManager.Save(settingsModel);

            // 2. 应用到全局 LoggerService（重建输出目标）
            //settingsModel.Logger.Apply();
        }
    }
}
