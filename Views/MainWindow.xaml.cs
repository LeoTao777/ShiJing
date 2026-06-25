using iNKORE.UI.WPF.Modern.Controls;
using System.Windows;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;

namespace ShiJing.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public SubPages.TopModules.HomePage HomePage = new SubPages.TopModules.HomePage();
        public SubPages.TopModules.Video Video = new SubPages.TopModules.Video();
        public SubPages.TopModules.Settings Settings = new SubPages.TopModules.Settings();
        public SubPages.TopModules.About About = new SubPages.TopModules.About();

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = sender.SelectedItem;
            Page? page = null;

            if (item == NavigationViewItem_Home)
            {
                page = HomePage;
            }
            else if (item == NavigationViewItem_Video)
            {
                page = Video;
            }
            else if (item == NavigationViewItem_Settings)
            {
                page = Settings;
            }
            else if (item == NavigationViewItem_About)
            {
                page = About;
            }

            if (page != null)
            {
                NavigationView_Root.Header = page.Title;
                Frame_Main.Navigate(page);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationView_Root.SelectedItem = NavigationViewItem_Home;
        }



    }
}
