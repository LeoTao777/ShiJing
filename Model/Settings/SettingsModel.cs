using System.Text.Json.Serialization;

namespace ShiJing.Model.Settings
{
    /// <summary>
    /// 应用设置聚合模型，集中管理各功能模块的设置子项。
    /// </summary>
    public class SettingsModel
    {
        /// <summary>日志相关设置。</summary>
        [JsonPropertyName("logger")]
        public LoggerSettingModel Logger { get; set; } = new LoggerSettingModel();

        /// <summary>截图相关设置。</summary>
        [JsonPropertyName("snapshot")]
        public SnapShotSettingModel SnapShot { get; set; } = new SnapShotSettingModel();

        /// <summary>视频相关设置。</summary>
        [JsonPropertyName("Video")]
        public VideoSettingModel Video { get; set; } = new VideoSettingModel();

    }
}