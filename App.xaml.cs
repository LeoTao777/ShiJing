using System.Windows;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using Prism.DryIoc;
using Prism.Ioc;
using ShiJing.Model;
using ShiJing.Services.Navication;
using ShiJing.ViewModels;
using ShiJing.Views;
using ShiJing.Views.SubPages.TopModules;

namespace ShiJing
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 导航注册中心（单例），先于页面项注册注入
            containerRegistry.RegisterSingleton<INavigationRegistry, NavigationRegistry>();

            // 子页面注册为 Prism 导航视图，PageKey 与 NavigationItem.PageKey 对应
            containerRegistry.RegisterForNavigation<HomePage>(nameof(HomePage));
            containerRegistry.RegisterForNavigation<Video>(nameof(Video));
            containerRegistry.RegisterForNavigation<Settings>(nameof(Settings));
            containerRegistry.RegisterForNavigation<About>(nameof(About));

            // 根 ViewModel（依赖 IRegionManager + INavigationRegistry）
            containerRegistry.RegisterSingleton<NavigationRootViewModel>();

            // 把 4 个子页面登记为 NavigationItem（主菜单 / 页脚菜单）
            var registry = Container.Resolve<INavigationRegistry>();
            registry.Register(new NavigationItem
            {
                PageKey = nameof(HomePage),
                Title = "首页",
                Section = NavigationSection.Main,
                Icon = SegoeFluentIcons.Home
            });
            registry.Register(new NavigationItem
            {
                PageKey = nameof(Video),
                Title = "视频",
                Section = NavigationSection.Main,
                Icon = FluentSystemIcons.ResizeVideo_24_Filled
            });
            registry.Register(new NavigationItem
            {
                PageKey = nameof(Settings),
                Title = "设置",
                Section = NavigationSection.Footer,
                Icon = SegoeFluentIcons.Settings
            });
            registry.Register(new NavigationItem
            {
                PageKey = nameof(About),
                Title = "关于",
                Section = NavigationSection.Footer,
                Icon = SegoeFluentIcons.Info
            });
        }
    }
}
