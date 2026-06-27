using ShiJing.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ShiJing.Services.Navication
{

        /// <summary>
        /// 导航注册中心：负责把所有页面注册为 NavigationItem，
        /// 并暴露为 ObservableCollection 供视图绑定。
        /// </summary>
        public interface INavigationRegistry
        {
            /// <summary>所有导航项（顶层）</summary>
            ObservableCollection<NavigationItem> Items { get; }

            /// <summary>注册一个导航项</summary>
            void Register(NavigationItem item);

            /// <summary>根据 PageKey 查找</summary>
            NavigationItem? Find(string pageKey);
        }
    
}
