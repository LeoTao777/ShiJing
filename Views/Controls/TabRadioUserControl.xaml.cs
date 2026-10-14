using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ShiJing.Views.Controls
{
    /// <summary>
    /// TabRadioUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class TabRadioUserControl : UserControl
    {
        public TabRadioUserControl()
        {
            InitializeComponent();
        }

            public static readonly DependencyProperty OptionsProperty =
                DependencyProperty.Register(
                    nameof(Options),
                    typeof(ObservableCollection<RadioOption>),
                    typeof(TabRadioUserControl),
                    new PropertyMetadata(
                        new ObservableCollection<RadioOption>()));

            public ObservableCollection<RadioOption> Options
            {
                get => (ObservableCollection<RadioOption>)GetValue(OptionsProperty);
                set => SetValue(OptionsProperty, value);
            }

            public static readonly DependencyProperty GroupNameProperty =
                DependencyProperty.Register(
                    nameof(GroupName),
                    typeof(string),
                    typeof(TabRadioUserControl),
                    new PropertyMetadata("DefaultRadioGroup"));

            public string GroupName
            {
                get => (string)GetValue(GroupNameProperty);
                set => SetValue(GroupNameProperty, value);
            }

    }
}
