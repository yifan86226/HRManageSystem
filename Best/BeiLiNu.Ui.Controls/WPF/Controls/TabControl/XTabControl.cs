using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeiLiNu.Ui.Controls.WPF.Controls
{
    public class XTabControl : TabControl
    {
        static XTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XTabControl), new FrameworkPropertyMetadata(typeof(XTabControl)));
        }
        //创建或标识用于显示给定项的元素
        //protected override DependencyObject GetContainerForItemOverride()
        //{
        //    return new XTabItem();
        //}
        //确定指定项是否是（或可作为）其自己的 ItemContainer。
        //protected override bool IsItemItsOwnContainerOverride(object item)
        //{
        //    return (item is XTabItem);
        //}
    }
}
