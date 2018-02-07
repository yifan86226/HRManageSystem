#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：图层管理中的项
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Map.Control
{
    /// <summary>
    /// LayerInfo.xaml 的交互逻辑
    /// </summary>
    public partial class LayerItem : UserControl
    {
        public object Target;
        public Action<object,bool> ChkChecked;
        public Action<object> ItemClose;
        public LayerItem()
        {
            InitializeComponent();
        }
        //
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            if (this.Parent != null)
            {
                StackPanel control = this.Parent as StackPanel;
                if (control != null)
                    control.Children.Remove(this);              

                if (ItemClose != null)
                {
                    ItemClose(this);
                }
            }
        }

        private void chk_Checked(object sender, RoutedEventArgs e)
        {
            setState();
        }

        private void chk_Unchecked(object sender, RoutedEventArgs e)
        {
            setState();
        }
        private void setState()
        {
            bool v = true;
            v = chk.IsChecked == true ? true : false;
            if (Target != null)
            {
                if (ChkChecked != null)
                {
                    ChkChecked(this, v);
                }
            }
        }
    }
}
