using ShiJing.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace ShiJing.Services.Navication
{
    /// <summary>
    /// 导航注册中心：负责把所有页面注册为 NavigationItem，
    /// 并暴露为 ObservableCollection 供视图绑定。
    /// </summary>
    public class NavigationRegistry : INavigationRegistry
    {
        public ObservableCollection<NavigationItem> Items { get; } = new();

        /// <summary>注册一个导航项</summary>
        public void Register(NavigationItem item)
        {
            if (item != null && Find(item.PageKey) == null)
                Items.Add(item);
        }

        /// <summary>根据 PageKey 查找</summary>
        public NavigationItem? Find(string pageKey)
        {
            if (string.IsNullOrEmpty(pageKey))
                return null;

            return Items.FirstOrDefault(i => i.PageKey == pageKey);
        }
    }
}
