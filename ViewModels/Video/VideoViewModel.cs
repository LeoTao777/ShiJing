namespace ShiJing.ViewModels.Video
{
    public class VideoViewModel : BindableBase
    {
        private int totalCount;
        public int TotalCount
        {
            get => totalCount;
            set => SetProperty(ref totalCount, value);
        }
    }
}
