using ShiJing.Model;
using ShiJing.Services.Navication;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShiJing.ViewModels
{
    public class NavigationRootViewModel : BindableBase
    {
        private readonly INavigationRegistry _registry;
        private readonly IRegionManager _regionManager;

        /// <summary>主菜单项（绑定到 NavigationView.MenuItemsSource）</summary>
        public ObservableCollection<NavigationItem> MenuItems { get; }

        /// <summary>页脚菜单项（绑定到 NavigationView.FooterMenuItemsSource）</summary>
        public ObservableCollection<NavigationItem> FooterItems { get; }

        private NavigationItem? _selectedItem;
        /// <summary>当前选中的导航项（TwoWay 绑定 NavigationView.SelectedItem）</summary>
        public NavigationItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value) && value != null)
                    OnItemSelected(value);
            }
        }

        private string _header = string.Empty;
        /// <summary>当前页面标题（绑定到 NavigationView.Header）</summary>
        public string Header
        {
            get => _header;
            private set => SetProperty(ref _header, value);
        }

        /// <summary>主区域名称，子页面通过 Prism 区域导航进入该区域</summary>
        public const string MainRegionName = "MainRegion";

        public NavigationRootViewModel(IRegionManager regionManager, INavigationRegistry registry)
        {
            _regionManager = regionManager;
            _registry = registry;

            // 按 Section 拆分注册项，供主菜单 / 页脚菜单分别绑定
            MenuItems = new ObservableCollection<NavigationItem>(
                _registry.Items.Where(i => i.Section == NavigationSection.Main));
            FooterItems = new ObservableCollection<NavigationItem>(
                _registry.Items.Where(i => i.Section == NavigationSection.Footer));
        }

        /// <summary>选中导航项 → 用 Prism 区域导航到对应页面</summary>
        private void OnItemSelected(NavigationItem item)
        {
            if (string.IsNullOrEmpty(item.PageKey))
                return;

            Header = item.Title;

            var parameters = new NavigationParameters
            {
                { "data", item.DataContext! }   // 传递 ControlInfoDataItem / Group
            };

            _regionManager.RequestNavigate(MainRegionName, item.PageKey, parameters);
        }
    }
}
