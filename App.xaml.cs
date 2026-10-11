using ShiJing.Utils.Logger;
using ShiJing.Utils.Settings;
using System.Windows;
using Serilog;
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

            LogConfig.Initialize();

            AppLogger.Info("应用启动");

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppLogger.Info("应用退出");

            // 刷新并关闭异步日志管道
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}
