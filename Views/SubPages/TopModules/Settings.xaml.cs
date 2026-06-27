using System.Windows.Controls;
using ShiJing.ViewModels.Settings;

namespace ShiJing.Views.SubPages.TopModules
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        private readonly SettingsViewModel _viewModel = new SettingsViewModel();

        public Settings()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }
    }
}
