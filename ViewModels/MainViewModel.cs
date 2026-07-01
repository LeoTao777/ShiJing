using iNKORE.UI.WPF.Modern.Common.IconKeys;
using ShiJing.Model;
using ShiJing.Views.SubPages.TopModules;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShiJing.ViewModels
{
    public class MainViewModel
    {
        /// <summary>主菜单项（绑定到 NavigationView.MenuItemsSource）</summary>
        public ObservableCollection<NavigationItem> MenuItems { get; set; } = new();

        /// <summary>页脚菜单项（绑定到 NavigationView.FooterMenuItemsSource）</summary>
        public ObservableCollection<NavigationItem> FooterItems { get; set; } = new();

        public MainViewModel()
        {
            var items = new[]
            {
                new NavigationItem("首页", NavigationSection.Main, SegoeFluentIcons.Home, new HomePage()),
                new NavigationItem("视频", NavigationSection.Main, FluentSystemIcons.ResizeVideo_24_Filled, new Video()),
                new NavigationItem("设置", NavigationSection.Footer, FluentSystemIcons.Settings_48_Regular, new ShiJing.Views.SubPages.TopModules.Settings()),
                new NavigationItem("关于", NavigationSection.Footer, SegoeFluentIcons.Info, new About()),
            };

            // 按 Section 拆分，供主菜单 / 页脚菜单分别绑定
            foreach (var item in items.Where(i => i.Section == NavigationSection.Main))
                MenuItems.Add(item);
            foreach (var item in items.Where(i => i.Section == NavigationSection.Footer))
                FooterItems.Add(item);
        }
    }
}
