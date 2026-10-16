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
            // 监听 SortOptions 变化
            SortOptions.CollectionChanged += (_, _) => UpdateSelectedValue();
            foreach (var option in SortOptions)
            {
                option.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(RadioOption.IsSelected) && s is RadioOption radio && radio.IsSelected)
                    {
                        UpdateSelectedValue();
                    }
                };
            }
        }
        private void UpdateSelectedValue()
        {
            SelectedValue = SortOptions.FirstOrDefault(x => x.IsSelected)?.Value ?? "all";
            AppLogger.Info($"SelectedValue updated to {SelectedValue}");
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

        private string selectedValue = "all";
        public string SelectedValue
        {
            get => selectedValue;
            set => SetProperty(ref selectedValue, value);
        }

        public DelegateCommand<string> SearchCommand { get; }
        public DelegateCommand OpenFolderCommand { get; }

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