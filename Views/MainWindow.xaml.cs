using iNKORE.UI.WPF.Modern.Controls;
using ShiJing.Model;
using ShiJing.ViewModels;
using System.Collections.Generic;
using System.Windows;

namespace ShiJing.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 导航历史栈：记录「上一次」访问的导航项，用于返回按钮后退
        private readonly Stack<NavigationItem> _navHistory = new();

        // 当前是否正在执行后退导航（后退触发的 SelectionChanged 不再压栈，避免循环）
        private bool _isNavigatingBack;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 启动默认选中「首页」，触发 SelectionChanged → 同步内容区与 Header
            if (nvSample5.SelectedItem == null && nvSample5.MenuItems.Count > 0)
                nvSample5.SelectedItem = nvSample5.MenuItems[0];
        }

        private void Main_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // 通过模板生成的 NavigationViewItem，其 DataContext 就是绑定的 NavigationItem
            if (args.SelectedItemContainer is NavigationViewItem nvi &&
                nvi.DataContext is NavigationItem item)
            {
                // 后退时不压栈；正常导航时，若新项与栈顶不同则把「上一项」压栈
                // （args.SelectedItem 是新选中项，压入的是切换前的栈顶视角，这里用 item 与上次比较）
                if (!_isNavigatingBack && _currentItem != null && !ReferenceEquals(_currentItem, item))
                {
                    _navHistory.Push(_currentItem);
                }
                _currentItem = item;

                contentHost.Content = item.View;
                // 内容区顶部 Header 显示为当前导航项名称
                sender.Header = item.Title;
                // 有历史可回时启用返回按钮
                sender.IsBackEnabled = _navHistory.Count > 0;
            }
        }

        private NavigationItem? _currentItem;

        private void Main_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs e)
        {
            if (_navHistory.Count == 0)
                return;

            _isNavigatingBack = true;
            var prev = _navHistory.Pop();
            // 设置 SelectedItem 会触发 SelectionChanged，期间 _isNavigatingBack 阻止再次压栈
            nvSample5.SelectedItem = prev;
            _isNavigatingBack = false;
        }
    }
}
