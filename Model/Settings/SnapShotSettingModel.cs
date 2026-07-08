using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShiJing.Model.Settings
{
    public class SnapShotSettingModel : BindableBase
    {
        private string _snapShotDirectory = Path.Combine(AppContext.BaseDirectory, "snapshots");

        public string SnapShotDirectory
        {
            get => _snapShotDirectory;
            set => SetProperty(ref _snapShotDirectory, value);
        }
    }
}
