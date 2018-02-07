#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：绘制元素开关面板
 * 日 期 ：2017-06-30
 ***************************************************************#@#***************************************************************/
#endregion
using AT_BC.Common;
using AT_BC.Data;
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
    /// SelectPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPanel : UserControl
    {
        /// <summary>
        /// 已设台
        /// </summary>
        Action<bool> HaveStationEvent=null;
        
        LoadStation loadStation = null;
        LoadMonitorStation loadMonitorStation = null;
        LoadOrgEqu loadOrgEqu = null;
        LoadAreaTips loadAreaTips = null;
        LoadMonitorGroup loadMonitorGroup = null;

        public SelectPanel()
        {
            InitializeComponent();
        }

       
        private void Checked(object sender, RoutedEventArgs e)
        {
             CheckBox chk = sender as CheckBox;
             CheckChange(chk);
        }

        private void Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            CheckChange(chk);
        }

        private void CheckChange(CheckBox chk)
        {
            switch (chk.Name)
            {
                case "btnHaveStation":
                    if (loadStation == null)
                        loadStation = new LoadStation(this);
                    loadStation.SetDoIt(chk.IsChecked.Value);
                    if (!chk.IsChecked.Value)
                    {
                        loadStation.unLoad();
                        loadStation = null;
                    }
                    if (HaveStationEvent != null)
                        HaveStationEvent(chk.IsChecked.Value);
                    break;
                case "btnMonitor":
                    if (loadMonitorStation == null)
                    {
                        loadMonitorStation = new LoadMonitorStation(this);
                        loadMonitorStation.DrawIt();
                    }
                    loadMonitorStation.Show(chk.IsChecked.Value);

                    if (chk.IsChecked.Value)
                    {
                        imgMonitor.Opacity = 1;
                        imgMonitor.ToolTip = "打开列表";
                    }
                    else
                    {
                        imgMonitor.Opacity = 0.4;
                        imgMonitor.ToolTip = "";
                    }
                    //if (!chk.IsChecked.Value)
                    //{
                    //    loadMonitorStation = null;
                    //}
                    
                    break;
                case "btnCompany":
                    if (loadOrgEqu == null)
                    {
                        loadOrgEqu = new LoadOrgEqu();
                        loadOrgEqu.DrawIt();
                    }
                    loadOrgEqu.Show(chk.IsChecked.Value);
                    if (chk.IsChecked.Value)
                        imgOrg.Opacity = 1;
                    else
                        imgOrg.Opacity = 0.4;

                    //if (!chk.IsChecked.Value)
                    //{
                    //    loadOrgEqu = null;
                    //}

                   
                    break;
                case "btnAreaInfo":
                    if (loadAreaTips == null)
                    {
                        loadAreaTips = new LoadAreaTips();
                        loadAreaTips.DrawIt();
                    }
                    loadAreaTips.Show(chk.IsChecked.Value);
                    //if (!chk.IsChecked.Value)
                    //{
                    //    loadAreaTips = null;
                    //}

                   
                    break;
                case "btnMonitorGroup":
                    if (loadMonitorGroup == null)
                    {
                        loadMonitorGroup = new LoadMonitorGroup(this);
                        loadMonitorGroup.DrawIt();
                    }
                    loadMonitorGroup.Show(chk.IsChecked.Value);
                    //if (!chk.IsChecked.Value)
                    //{
                    //    loadMonitorGroup = null;
                    //}

                    
                    break;
            }
        }

        //private void Image_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    Image img = sender as Image;
        //    img.Opacity = 1;
        //}

        //private void Image_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    Image img = sender as Image;
        //    img.Opacity = 0.4;
        //}

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img.Opacity != 1)
                return;
            if (img.Tag != null)
            {
                if (img.Tag.ToString() == "参保单位")
                { 
                
                }
                if (img.Tag.ToString() == "监测站")
                {
                    Dialog.FixedStationDetailDialog detail = new Dialog.FixedStationDetailDialog(null);
                    detail.Owner = VisualTreeHelperExtension.GetParentObject<System.Windows.Window>(this); 
                    detail.Show();
                }
            }
        }

       
    }
   
}
