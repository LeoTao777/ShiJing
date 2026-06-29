using System.Windows;
using System.Windows.Controls;

namespace ShiJing.Views.Controls
{
    /// <summary>
    /// 文件夹选择控件：左侧文本框显示路径，右侧按钮打开文件资源管理器选择目标文件夹。
    /// 通过 <see cref="SelectedPath"/> 依赖属性双向绑定到宿主的数据源。
    /// </summary>
    public partial class FolderPicker : UserControl
    {
        /// <summary>当前选中的文件夹路径（双向绑定）。</summary>
        public string SelectedPath
        {
            get => (string)GetValue(SelectedPathProperty);
            set => SetValue(SelectedPathProperty, value);
        }

        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register(
                nameof(SelectedPath),
                typeof(string),
                typeof(FolderPicker),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        /// <summary>对话框标题，默认"选择文件夹"。</summary>
        public string DialogTitle
        {
            get => (string)GetValue(DialogTitleProperty);
            set => SetValue(DialogTitleProperty, value);
        }

        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register(
                nameof(DialogTitle),
                typeof(string),
                typeof(FolderPicker),
                new PropertyMetadata("选择文件夹"));

        /// <summary>路径文本是否可用；为 false 时文字置灰。</summary>
        public bool IsPathEnabled
        {
            get => (bool)GetValue(IsPathEnabledProperty);
            set => SetValue(IsPathEnabledProperty, value);
        }

        public static readonly DependencyProperty IsPathEnabledProperty =
            DependencyProperty.Register(
                nameof(IsPathEnabled),
                typeof(bool),
                typeof(FolderPicker),
                new FrameworkPropertyMetadata(true));

        public FolderPicker()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = string.IsNullOrEmpty(DialogTitle) ? "选择文件夹" : DialogTitle,
                ShowNewFolderButton = true,
            };

            // 若已有路径且存在，初始定位到该目录
            if (!string.IsNullOrEmpty(SelectedPath) && System.IO.Directory.Exists(SelectedPath))
            {
                dialog.SelectedPath = SelectedPath;
            }

            // 用当前窗口做 Owner，保证对话框置顶
            var owner = Window.GetWindow(this);
            var result = owner != null
                ? dialog.ShowDialog(new Win32Window(owner))
                : dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = dialog.SelectedPath;
            }
        }

        /// <summary>把 WPF Window 包装为 WinForms IWin32Window，作为对话框 Owner。</summary>
        private sealed class Win32Window : System.Windows.Forms.IWin32Window
        {
            private readonly IntPtr _handle;
            public Win32Window(Window window) => _handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            public IntPtr Handle => _handle;
        }
    }
}
