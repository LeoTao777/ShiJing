using ShiJing.Model.Video;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShiJing.Views.Controls
{
    /// <summary>
    /// VideoCardControl.xaml 的交互逻辑
    /// </summary>
    public partial class VideoCardControl : UserControl
    {
        public VideoCardControl()
        {
            InitializeComponent();
        }

        // 当前卡片对应的视频
        public VideoItem? Video
        {
            get => (VideoItem?)GetValue(VideoProperty);
            set => SetValue(VideoProperty, value);
        }

        public static readonly DependencyProperty VideoProperty =
            DependencyProperty.Register(
                nameof(Video),
                typeof(VideoItem),
                typeof(VideoCardControl),
                new PropertyMetadata(null));

        // 打开文件所在目录的命令
        public ICommand? OpenFolderCommand
        {
            get => (ICommand?)GetValue(OpenFolderCommandProperty);
            set => SetValue(OpenFolderCommandProperty, value);
        }

        public static readonly DependencyProperty OpenFolderCommandProperty =
            DependencyProperty.Register(
                nameof(OpenFolderCommand),
                typeof(ICommand),
                typeof(VideoCardControl),
                new PropertyMetadata(null));

        // 从当前列表移除记录的命令
        public ICommand? RemoveCommand
        {
            get => (ICommand?)GetValue(RemoveCommandProperty);
            set => SetValue(RemoveCommandProperty, value);
        }

        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register(
                nameof(RemoveCommand),
                typeof(ICommand),
                typeof(VideoCardControl),
                new PropertyMetadata(null));
    }

}
