using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CO_IA.Common
{
    /// <summary>
    /// 下拉按钮控件,组合框派生类,下拉项使用DropdownButtonItem对象,支持点击事件,SelectedValue及SelectedItem、SelectedIndex等选中相关属性无效
    /// </summary>
    public class DropdownButton : ComboBox
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static DropdownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropdownButton), new FrameworkPropertyMetadata(typeof(DropdownButton)));
        }
        /// <summary>
        /// 按钮内容属性
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(object), typeof(DropdownButton), null);

        /// <summary>
        /// 获取或设置按钮内容
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DropdownButtonItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is DropdownButtonItem);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }
        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            this.SelectedIndex = -1;
        }
    }

    /// <summary>
    /// 下拉按钮控件内容项目
    /// </summary>
    public class DropdownButtonItem : ComboBoxItem
    {
        /// <summary>
        /// 点击事件定义
        /// </summary>
        public event RoutedEventHandler Click;

        /// <summary>
        /// 构造函数
        /// </summary>
        static DropdownButtonItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropdownButtonItem), new FrameworkPropertyMetadata(typeof(DropdownButtonItem)));
        }

        /// <summary>
        /// 鼠标左键抬起触发事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.Click != null)
            {
                this.Click(this, e);
            }
            this.IsSelected = false;
        }
    }
}
