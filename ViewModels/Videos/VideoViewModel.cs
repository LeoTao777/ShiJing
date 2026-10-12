using ShiJing.Utils.Logger;
using System.Windows;

namespace ShiJing.ViewModels.Videos
{
    public class VideoViewModel : BindableBase
    {
        public VideoViewModel()
        {
            SearchCommand = new DelegateCommand<string>(OnSearch);
        }

        private int totalCount;
        public int TotalCount
        {
            get => totalCount;
            set => SetProperty(ref totalCount, value);
        }

        public DelegateCommand<string> SearchCommand { get; set; }
        private void OnSearch(string query)
        {
            AppLogger.Info($"Search for {query}");
        }
    }
}
