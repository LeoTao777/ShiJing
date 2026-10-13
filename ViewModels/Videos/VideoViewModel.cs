using Microsoft.Win32;
using ShiJing.Utils.Logger;
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
        }
    }
}
