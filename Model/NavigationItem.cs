using CommunityToolkit.Mvvm.ComponentModel;
using iNKORE.UI.WPF.Modern.Common.IconKeys;
using System.Windows;
namespace ShiJing.Model
{
    /// <summary>导航项归属区域：决定挂主菜单还是页脚菜单</summary>
    public enum NavigationSection
    {
        /// <summary>主菜单</summary>
        Main,

        /// <summary>页脚菜单</summary>
        Footer
    }

    public partial class NavigationItem(string name,  NavigationSection Section, FontIconData icon, FrameworkElement view) : ObservableObject
    {
        /// <summary>导航 Key，对应 Prism 注册的页面名（如 "HomePage"）</summary>
        [ObservableProperty]
        public partial string PageKey { get; set; } = name;

        /// <summary>显示标题</summary>
        [ObservableProperty]
        public partial string Title { get; set; } = name;

        /// <summary>归属区域，决定挂主菜单还是页脚菜单</summary>
        public NavigationSection Section { get; set; } = Section;

        /// <summary>菜单图标数据，绑定到 NavigationViewItem.Icon</summary>
        public FontIconData? Icon { get; set; } = icon;

        /// <summary>关联的数据上下文（ControlInfoDataItem / ControlInfoDataGroup），用于传参</summary>
        public FrameworkElement View { get; set; } = view;
    }
}
