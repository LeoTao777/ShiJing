using System.Windows;
using ShiJing.Utils.Log;
using ShiJing.Utils.Settings;

namespace ShiJing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // 启动时加载持久化配置（文件不存在则生成默认 settings.json），
            // 并应用到全局 LoggerService，覆盖单例的默认配置。
            var settings = SettingsManager.LoadOrCreate();
            LoggerService.Instance.ReloadConfig(settings.Logger.ToConfig());

            base.OnStartup(e);
        }
    }
}
