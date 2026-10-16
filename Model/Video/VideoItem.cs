using System.Windows.Media;

namespace ShiJing.Model.Video
{
    public class VideoItem
    {
        /// <summary>
        /// 视频文件的完整路径。
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 视频文件名，例如：旅行记录.mp4。
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 视频文件大小，例如：1.2 GB。
        /// </summary>
        /// todo:这里文件大小如果是mb就显示MB，超过1GB就显示GB，超过1TB就显示TB
        public string FileSize { get; set; } = string.Empty;

        /// <summary>
        /// 文件最后修改时间。
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 视频时长。未提取到时长时为空。
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// 视频缩略图。
        /// </summary>
        public ImageSource? Thumbnail { get; set; }

        /// <summary>
        /// 格式化后的视频时长。
        /// </summary>
        public string DurationText
        {
            get
            {
                if (Duration == null)
                    return "--:--";

                var duration = Duration.Value;

                return duration.TotalHours >= 1
                    ? duration.ToString(@"h\:mm\:ss")
                    : duration.ToString(@"mm\:ss");
            }
        }
    }
}
