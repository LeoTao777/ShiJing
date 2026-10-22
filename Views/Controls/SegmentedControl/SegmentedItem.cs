using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShiJing.Views.Controls.SegmentedControl
{
    /// <summary>
    /// SegmentedControl 的选项模型。
    /// IsSortable = true 的选项参与排序：被选中时方向由控件置为 Ascending（正向），
    /// 在已选中状态下再次点击则在 Ascending / Descending 之间切换。
    /// 实现 INotifyPropertyChanged 是因为箭头方向需要随 SortDirection 实时翻转。
    /// </summary>
    public class SegmentedItem : INotifyPropertyChanged
    {
        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        private bool _isSortable;

        /// <summary>
        /// 是否为排序选项（决定标题右侧是否显示方向箭头，以及重复点击是否切换方向）。
        /// </summary>
        public bool IsSortable
        {
            get => _isSortable;
            set => SetField(ref _isSortable, value);
        }

        private SegmentedSortDirection _sortDirection = SegmentedSortDirection.None;

        /// <summary>
        /// 当前排序方向。仅 IsSortable 项有意义；未选中时保留原值，被选中时由控件重置为 Ascending。
        /// </summary>
        public SegmentedSortDirection SortDirection
        {
            get => _sortDirection;
            set => SetField(ref _sortDirection, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
