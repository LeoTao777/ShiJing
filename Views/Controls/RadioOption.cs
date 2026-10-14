using System;
using System.Collections.Generic;
using System.Text;

namespace ShiJing.Views.Controls
{
    public class RadioOption : BindableBase
    {
        private bool isSelected;

        public string Text { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
    }
}
