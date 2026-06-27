using iNKORE.UI.WPF.Modern.Common.IconKeys;

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

    public class NavigationItem
    {
        /// <summary>导航 Key，对应 Prism 注册的页面名（如 "HomePage"）</summary>
        public string PageKey { get; set; } = string.Empty;

        /// <summary>显示标题</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>归属区域，决定挂主菜单还是页脚菜单</summary>
        public NavigationSection Section { get; set; } = NavigationSection.Main;

        /// <summary>菜单图标数据，绑定到 NavigationViewItem.Icon</summary>
        public FontIconData? Icon { get; set; }

        /// <summary>关联的数据上下文（ControlInfoDataItem / ControlInfoDataGroup），用于传参</summary>
        public object? DataContext { get; set; }
    }
}
