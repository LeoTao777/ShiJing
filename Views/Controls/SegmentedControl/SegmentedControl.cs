using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace ShiJing.Views.Controls.SegmentedControl
{
    public class SegmentedControl : ListBox
    {
        /// <summary>
        /// 药丸样式的分段选择器：一组选项中互斥单选（如 全部/最近/按日期/按大小/按时长）。
        /// 继承 ListBox，利用其单选机制保证任意时刻至多一项被选中；
        /// 重写 OnSelectionChanged 阻止"点击已选中项导致没有选中项"的情况，保证始终有一项被选中。
        /// IsSortable 的选项被选中后参与排序：首次选中为正向（Ascending），
        /// 已选中状态下再次点击切换为逆向（Descending），之后交替切换。
        /// </summary>

        private SegmentedItem? _directionSource;

        static SegmentedControl()
        {
            // 让控件到 Themes/Generic.xaml 中查找默认样式（无外观控件的关键一步）
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SegmentedControl),
                new FrameworkPropertyMetadata(typeof(SegmentedControl)));
        }
        public SegmentedControl()
        {
            // 互斥的核心：强制单选
            SelectionMode = SelectionMode.Single;
        }

        /// <summary>
        /// 选项排列方向，侧边栏里用 Vertical，工具栏里用 Horizontal。
        /// </summary>
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(SegmentedControl),
                new FrameworkPropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// 药丸圆角半径。默认按高度一半取全圆角；侧边栏里可调小。
        /// </summary>
        public CornerRadius PillCornerRadius
        {
            get => (CornerRadius)GetValue(PillCornerRadiusProperty);
            set => SetValue(PillCornerRadiusProperty, value);
        }

        public static readonly DependencyProperty PillCornerRadiusProperty =
            DependencyProperty.Register(
                nameof(PillCornerRadius),
                typeof(CornerRadius),
                typeof(SegmentedControl),
                new FrameworkPropertyMetadata(new CornerRadius(16)));

        /// <summary>
        /// 当前选中排序项的方向；选中非排序项时为 None。
        /// 默认双向绑定：ViewModel 可读当前方向，也可写它以反向控制排序项。
        /// </summary>
        public SegmentedSortDirection SortDirection
        {
            get => (SegmentedSortDirection)GetValue(SortDirectionProperty);
            set => SetValue(SortDirectionProperty, value);
        }

        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register(
                nameof(SortDirection),
                typeof(SegmentedSortDirection),
                typeof(SegmentedControl),
                new FrameworkPropertyMetadata(
                    SegmentedSortDirection.None,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSortDirectionChanged));

        /// <summary>
        /// 使用自定义容器，接管"已选中项再次被点击"时的方向切换。
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride() => new SegmentedControlItem();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is SegmentedControlItem;

        /// <summary>
        /// 保证始终有一项被选中：如果本次变化后没有任何选中项，则恢复刚被移除的项。
        /// 同时维护排序方向：新选中的排序项重置为正向；把选中项的方向同步到控件的 SortDirection。
        /// </summary>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (SelectedItem is null && e.RemovedItems.Count > 0)
            {
                SetCurrentValue(SelectedItemProperty, e.RemovedItems[0]);
            }

            // 从别的项切换到可排序项：首次选中一律回到正向
            if (SelectedItem is SegmentedItem { IsSortable: true } sortable && e.AddedItems.Contains(sortable))
            {
                sortable.SortDirection = SegmentedSortDirection.Ascending;
            }

            // 跟踪新的选中项（旧的退订），其方向变化时同步到控件的 SortDirection
            if (_directionSource is not null)
            {
                _directionSource.PropertyChanged -= OnSelectedItemPropertyChanged;
                _directionSource = null;
            }
            if (SelectedItem is SegmentedItem selected)
            {
                _directionSource = selected;
                _directionSource.PropertyChanged += OnSelectedItemPropertyChanged;
            }

            SyncSortDirectionFromItem();
        }

        private void OnSelectedItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(SegmentedItem.SortDirection))
            {
                SyncSortDirectionFromItem();
            }
        }

        /// <summary>
        /// 项 → 控件：把选中项的方向写回控件的 SortDirection（SetCurrentValue 保留绑定）。
        /// </summary>
        private void SyncSortDirectionFromItem()
        {
            var direction = SelectedItem is SegmentedItem { IsSortable: true } sortable
                ? sortable.SortDirection
                : SegmentedSortDirection.None;
            if (SortDirection != direction)
            {
                SetCurrentValue(SortDirectionProperty, direction);
            }
        }

        /// <summary>
        /// 控件 → 项：ViewModel 经绑定写入 SortDirection 时，转写到当前选中的排序项。
        /// </summary>
        private static void OnSortDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentedControl { SelectedItem: SegmentedItem { IsSortable: true } item } &&
                item.SortDirection != (SegmentedSortDirection)e.NewValue)
            {
                item.SortDirection = (SegmentedSortDirection)e.NewValue;
            }
        }

    }
}
