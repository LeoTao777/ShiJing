using Microsoft.Win32;
using ShiJing.Utils.Logger;
using ShiJing.Views.Controls;
using System.Collections.ObjectModel;
using System.Windows;

namespace ShiJing.ViewModels.Videos
{
    public class VideoViewModel : BindableBase
    {
        public VideoViewModel()
        {
            SearchCommand = new DelegateCommand<string>(OnSearch);
            OpenFolderCommand = new DelegateCommand(OnOpenFolder);
        }

        private int totalCount;
        public int TotalCount
        {
            get => totalCount;
            set => SetProperty(ref totalCount, value);
        }

        public ObservableCollection<RadioOption> SortOptions { get; } = new()
    {
        new() { Text = "全部", Value = "all", IsSelected = true },
        new() { Text = "最近", Value = "recent" },
        new() { Text = "按日期", Value = "date" },
        new() { Text = "按大小", Value = "size" },
        new() { Text = "按时长", Value = "duration" }
    };

        public string SelectedValue =>
            SortOptions.FirstOrDefault(x => x.IsSelected)?.Value ?? "all";

        public DelegateCommand<string> SearchCommand { get; }
        public DelegateCommand OpenFolderCommand { get;}

        private void OnSearch(string query)
        {
            AppLogger.Info($"Search for {query}");
        }

        private void OnOpenFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "打开视频文件夹",
            };
            if (dialog.ShowDialog() != true) return;
            MessageBox.Show($"Opening folder: {dialog.FolderName}");
            AppLogger.Info($"Opening folder: {dialog.FolderName}");

            //todo:显示文件列表视图（列表或者卡片）
        }
    }
}
