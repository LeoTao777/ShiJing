namespace ShiJing.Model.Settings
{
    /// <summary>
    /// 应用设置聚合模型，集中管理各功能模块的设置子项。
    /// </summary>
    public class SettingsModel
    {
        /// <summary>日志相关设置。</summary>
        public LoggerSettingModel Logger { get; set; } = new LoggerSettingModel();
    }
}