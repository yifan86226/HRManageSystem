using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.RIAS
{
    internal class NavigableModuleContainer : System.Windows.Controls.ContentControl
    {
        /// <summary>
        /// 定义依赖属性-按钮标题
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NavigableModuleContainer));

        /// <summary>
        /// 获取或设置显示标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
    }
}
