#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：主界面标题控件，显示活动图标、名称等
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
    /// ActivityTitle.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityTitle : UserControl
    {
        public string TitleText
        {
            get
            {
                return txtTitle.Text;
            }
            set
            {
                txtTitle.Text = value;
                txtTitle.ToolTip = value;
            }
        }
        public string InfoText
        {
            get
            {
                return "";
            }
            set
            {
                
            }
        }
        public ImageSource ActiveImage
        {
            get { return img_logo.Source; }
            set
            {
                img_logo.Source = value;
            }
        }
       
        public ActivityTitle()
        {
            InitializeComponent();
        }

       
        private void txtInfo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

       

        public void AddMenu(FrameworkElement img)
        {
            menu.Children.Add(img);
        }
        public void RemoveMenu(string name)
        {
            if (menu.Children.Count == 0)
                return;
            foreach (var item in menu.Children)
            {
                FrameworkElement img = item as FrameworkElement;
                if (img != null && img.Tag != null && img.Tag.ToString() != name)
                {
                    menu.Children.Remove(img);
                    break;
                }
            }
        }
        public void ClearMenu(string Flag=null)
        {
            if (Flag == null)
                menu.Children.Clear();
            else
            {
                for (int i=0;i<menu.Children.Count;i++)
                { 
                    FrameworkElement  element = menu.Children[i] as FrameworkElement;
                    if (element != null && element.Tag != null && element.Tag.ToString() == Flag)
                    {
                        menu.Children.RemoveAt(i);
                        i--;
                    }

                }
            }
        }
        public void SetMenuVisible(string name,bool Show)
        {
            if (menu.Children.Count == 0)
                return;
            foreach (var item in menu.Children)
            {
                FrameworkElement img = item as FrameworkElement;
                if (img != null && img.Tag != null && img.Tag.ToString() != name)
                {
                    if (Show)
                        img.Visibility = System.Windows.Visibility.Visible;
                    else
                        img.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                }
            }
        }
    }
}
