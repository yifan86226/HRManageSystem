#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：地图Window窗口
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using I_GS_MapBase.Portal;
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
using System.Windows.Shapes;

namespace CO_IA.UI.MAP
{
    /// <summary>
    /// Map.xaml 的交互逻辑
    /// </summary>
    public partial class UMap : Window
    {
        /// <summary>
        /// 地图类
        /// </summary>
        public MapGIS mapGis;
        public bool Move = true;
        ActivityPlaceMap apMap = null;
        public UMap(ActivityPlaceMap apmap)
        {
            InitializeComponent();
            mapGis = new MapGIS();
            frame_window.Children.Insert(0,mapGis.MainMap);
            apMap = apmap;

            mapGis.BarGraphicChange += BarGraphicChange;
            
        }
        #region 窗口控制
        public string titleName
        {
            get { return title_name.Content.ToString(); }
            set
            {
                title_name.Content = value;
                Title = value;
            }
        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!mapGis.Modify)
            {
                this.Close();
                return;
            }
            if (MessageBox.Show("是否不保存并关闭窗口？", "关闭", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }

        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed && Move)
            {
                this.DragMove();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void lock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Activate();
        }

        private void max_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion

        public void BarGraphicChange(bool b)
        {
            if (mapGis.DrawList.Count > 0)
                dwBtn.Visibility = System.Windows.Visibility.Visible;
            else
            {
                dwBtn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!mapGis.Modify)
            {
                this.Close();
                return;
            }
            if (MessageBox.Show("是否不保存并关闭窗口？", "关闭", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //保存修改
            apMap.UpdateLocation(mapGis.GetGraphicString());
            this.Close();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //定位
            if (mapGis.DrawList.Count > 0)
            {
                ReturnDrawGraphicInfo R = mapGis.DrawList[0].Value as ReturnDrawGraphicInfo;
                if(R!=null)
                {
                    mapGis.setExtent(new AT_BC.Data.GeoPoint() { Longitude = R.GraphicExtent.Xy1.X, Latitude = R.GraphicExtent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = R.GraphicExtent.Xy2.X, Latitude = R.GraphicExtent.Xy2.Y });
                }
            }
        }

        private void dwBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            dwBtn.Opacity = 1;
        }

        private void dwBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            dwBtn.Opacity = 0.5;
        }
    }
}
