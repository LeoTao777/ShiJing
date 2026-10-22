using System.Reflection.Metadata;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShiJing.Views.Controls.SegmentedControl
{
    /// <summary>
    /// SegmentedControl 的项容器，职责只有一个：
    /// 已选中的可排序项被再次触发时，切换其排序方向（正向 ⇄ 逆向）。
    /// 支持鼠标左键与键盘空格两种触发方式，未选中的项交给基类完成正常选中。
    /// </summary>
    public class SegmentedControlItem : ListBoxItem
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (TryToggleSortDirection())
            {
                // 吞掉这次点击，避免重复走一遍选择流程
                e.Handled = true;
                return;
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space && TryToggleSortDirection())
            {
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// 若本项"已选中且可排序"，切换其方向并返回 true；否则返回 false 让基类处理。
        /// </summary>
        private bool TryToggleSortDirection()
        {
            if (!IsSelected || Content is not SegmentedItem { IsSortable: true } item)
            {
                return false;
            }

            item.SortDirection = item.SortDirection == SegmentedSortDirection.Ascending
                ? SegmentedSortDirection.Descending
                : SegmentedSortDirection.Ascending;
            return true;
        }
    }
}
