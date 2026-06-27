using System.Windows;
using ShiJing.ViewModels;

namespace ShiJing.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NavigationRootViewModel _viewModel;

        public MainWindow(NavigationRootViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 启动默认选中「首页」，触发 Prism 区域导航
            if (_viewModel.SelectedItem == null && _viewModel.MenuItems.Count > 0)
                _viewModel.SelectedItem = _viewModel.MenuItems[0];
        }
    }
}
