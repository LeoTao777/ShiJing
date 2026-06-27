using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ShiJing.Utils.Log;
namespace ShiJing.Model.Settings
{
    /// <summary>
    /// 日志设置模型。承载并编辑 <see cref="LoggerConfig"/> 的各项配置，
    /// 在 <see cref="LoggerService"/> 与设置界面之间同步。实现属性变更通知，便于直接绑定。
    /// </summary>
    public class LoggerSettingModel : BindableBase
    {
        #region Constructor

        /// <summary>以 <see cref="LoggerConfig"/> 默认值初始化。</summary>
        public LoggerSettingModel()
        {
            Load(new LoggerConfig());
        }

        /// <summary>从指定配置载入初始化。</summary>
        public LoggerSettingModel(LoggerConfig config)
        {
            Load(config);
        }

        #endregion

        #region member

        private bool _isEnabled  = true;
        public bool IsEnabled {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        /// <summary>允许输出的最低日志等级，低于该等级的日志将被丢弃。</summary>
        private LogLevel _minLevel = LogLevel.Info;
        public LogLevel MinLevel {
            get => _minLevel;
            set => SetProperty(ref _minLevel, value);
        }

        private string _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        /// <summary>日志文件根目录。文件目标启用时使用。</summary>
        public string LogDirectory
        {
            get => _logDirectory;
            set => SetProperty(ref _logDirectory, value);
        }

        #endregion

        #region private method
        /// <summary>从指定 <see cref="LoggerConfig"/> 载入到当前模型。</summary>
        public void Load(LoggerConfig config)
        {
            if (config is null) throw new ArgumentNullException(nameof(config));

            MinLevel = config.MinLevel;
            LogDirectory = config.LogDirectory;
        }

        /// <summary>从全局 <see cref="LoggerService"/> 当前生效配置载入。</summary>
        public void LoadFromService() => Load(LoggerService.Instance.Config);

        /// <summary>依据当前属性构建一个新的 <see cref="LoggerConfig"/>。</summary>
        public LoggerConfig ToConfig() => new LoggerConfig
        {
            MinLevel = MinLevel,
            LogDirectory = LogDirectory,
        };

        /// <summary>将当前配置应用到全局 <see cref="LoggerService"/>（重建输出目标）。</summary>
        public void Apply() => LoggerService.Instance.ReloadConfig(ToConfig());
        #endregion

    }
}
