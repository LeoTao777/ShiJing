using Microsoft.Win32;
using ShiJing.Utils.Logger;
using ShiJing.Views.Controls.SegmentedControl;
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
        }

        private string _selectedFilter = "全部";

        /// <summary>
        /// 当前选中的筛选/排序项标题（与 SegmentedControl.SelectedValue 双向绑定）。
        /// </summary>
        public string SelectedFilter
        {
            get => _selectedFilter;
            set => SetProperty(ref _selectedFilter, value);
        }

        private SegmentedSortDirection _sortDirection = SegmentedSortDirection.None;

        /// <summary>
        /// 当前排序方向（与 SegmentedControl.SortDirection 双向绑定）。
        /// 选中排序项（按日期/按大小/按时长）时为正向或逆向，选中其他项时为 None。
        /// </summary>
        public SegmentedSortDirection SortDirection
        {
            get => _sortDirection;
            set => SetProperty(ref _sortDirection, value);
        }

        private int totalCount;
        public int TotalCount
        {
            get => totalCount;
            set => SetProperty(ref totalCount, value);
        }


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