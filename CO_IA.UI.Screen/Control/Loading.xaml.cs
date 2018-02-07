#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：“加载数据”等待 显示界面
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

namespace CO_IA.UI.Screen.Control
{
    /// <summary>
    /// Loading.xaml 的交互逻辑
    /// </summary>
    public partial class Loading : UserControl
    {
        string defaultMessage = "正在加载数据...";
        public string Message
        {
            set
            {
                textBlockHint.Text = value;
            }
        }
        public Loading()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Visibility v = this.Visibility;
            if (v != System.Windows.Visibility.Visible)
                Message = defaultMessage;
        }     
   
    }
}
