using CO_IA.Data;
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

namespace CO_IA.UI.Screen.MainPage
{
    /// <summary>
    /// MainPanel.xaml 的交互逻辑
    /// </summary>
    public partial class MainPanel : UserControl
    {
        ActivityPlaceInfo PlaceInfo=new ActivityPlaceInfo();
        //StatePrepare pre = null;
        //StateExecute exec = null;
        StateNotice notice = null;
        public MainPanel()
        {
            InitializeComponent();
            
        }
        public void LoadUI()
        {
            g_state.Children.Clear();
            notice = new StateNotice();
            g_state.Children.Add(notice);
            //if (Obj.Activity.ActivityStage == Types.ActivityStage.Prepare)
            //{
            //    if (pre == null)
            //        pre = new StatePrepare();
            //    else
            //        pre.Stop();

            //    g_state.Children.Add(pre);
            //}
            //else
            //{
            //    if (exec == null)
            //        exec = new StateExecute();
            //    else
            //        exec.Stop();

            //    g_state.Children.Add(exec);
            //}
        }
        //地点选择 通知事件 from 主界面
        public void AreaChange(ActivityPlaceInfo placeInfo)
        {
            if (placeInfo!=null)
            {
                PlaceInfo = placeInfo;
                stateInfo.PlaceInfo = placeInfo;
                this.DataContext = PlaceInfo;
                notice.Begin();
                //if (pre != null)
                //    pre.Begin();
                //if (exec != null)
                //{
                //    exec.Begin();
                //}
            }
        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img != null&&img.Tag!=null)
            {
                if (img.Tag.ToString() == "down")
                {
                    img.Tag = "up";
                    img.Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/MainPage/arrow_up.png", 0));
                    ginfo.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    img.Tag = "down";
                    img.Source = new BitmapImage(new Uri("/CO_IA.UI.Screen;component/Images/MainPage/arrow_down.png", 0));
                    ginfo.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void AreaName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //名称按钮事件
            //if (placeInfo.Name == "全部区域")
            {
                stateInfo.PlaceInfo = PlaceInfo;            
            }
        }

        public void ChangeGroupState(string groupId, bool online)
        {
            stateInfo.ChangeGroupState(groupId,online);
        }
    }
}
