#region 文件描述
/**********************************************************************************
 * 创建人：wangxin
 * 摘  要：活动地点展示界面
 * 日  期：2016-08-12
 * ********************************************************************************/
#endregion
using CO_IA.Data;
using CO_IA.Types;
using CO_IA.Client;
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
using AT_BC.Types;
using System.IO;
using CO_IA.UI.MAP;

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// PlaceManageModule.xaml 的交互逻辑
    /// </summary>
    public partial class PlaceManageModule : UserControl
    {
        /// <summary>
        /// 地图类 xiaguohui
        /// </summary>
        private ActivityPlaceMap ActivityMap = new ActivityPlaceMap();

        public PlaceManageModule()
        {
            InitializeComponent();
            IniMap();
        }

        /// <summary>
        /// 初始化界面信息
        /// </summary>
        /// <param name="place"></param>
        public void InitPage(ActivityPlaceInfo place)
        {
            this.DataContext = place;
            if (place.Image != null)
            {
                MemoryStream stream = new MemoryStream(place.Image);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();//初始化
                bmp.StreamSource = stream;//设置源
                bmp.EndInit();//初始化结束
                placeImg.Source = bmp;//设置图像Source
            }
            
            ActivityMap.PlaceLocation = new Dictionary<string, ActivityPlaceInfo>() { { place.Guid, place } };
            ActivityMap.CurrentPlaceId = place.Guid;
            
        }

        /// <summary>
        /// 初始化地图
        /// </summary>
        public void IniMap()
        {
            //初始化地图
            showMap.Children.Add(ActivityMap.ShowMap.MainMap);
        }
    }
}
