using System.IO;
namespace ShiJing.Model.Settings
{
    public class VideoSettingModel : BindableBase
    {
        private string _deafultVideoDirectory = Path.Combine(AppContext.BaseDirectory, "Video");

        public string DefaultVideoDirectory
        {
            get => _deafultVideoDirectory;
            set => SetProperty(ref _deafultVideoDirectory, value);
        }
    }
}
