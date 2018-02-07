#region 文件描述
/**********************************************************************************
 * 创建人：wangxin
 * 摘  要：活动地点选择添加界面
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
using System.Windows.Shapes;
using AT_BC.Types;

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// SelectPlaceAddDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPlaceAddDialog : Window
    {
        public event Action RefreshPlaceListEvent;
        public SelectPlaceAddDialog()
        {
            InitializeComponent();
            InitPage();
        }

        #region 内部方法
        /// <summary>
        /// 初始化界面信息
        /// </summary>
        private void InitPage()
        {
            Activity[] activitys = getAllActivitys();
            Activity activity = new Activity();
            activity.Guid = "";
            activity.Name = "全部";
            Activity[] activityItems = new Activity[activitys.Length + 1];
            activityItems[0] = activity;
            activitys.ToList().CopyTo(activityItems, 1);
            cbActivity.ItemsSource = activityItems;
        }

        /// <summary>
        /// 获取全部活动
        /// </summary>
        /// <returns></returns>
        private Activity[] getAllActivitys()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.Activity[]>(
                channel =>
                {
                    return channel.GetActivities();
                });
        }

        /// <summary>
        /// 查询活动地点
        /// </summary>
        /// <param name="activityGuid"></param>
        /// <param name="placeName"></param>
        /// <returns></returns>
        private ActivityPlaceQueryResult[] getPlaces(string activityGuid, string placeName)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceQueryResult[]>(
                channel =>
                {
                    return channel.GetPlaceInfos(activityGuid, placeName);
                });
        }
        #endregion

        #region 事件
        /// <summary>
        /// 查询按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            string activityGuid = "";
            Activity activity = cbActivity.SelectedItem as Activity;
            if (activity != null && activity.Guid != "")
            {
                activityGuid = activity.Guid;
            }
            string placeName = txtQueryName.Text;
            ActivityPlaceQueryResult[] placeList = getPlaces(activityGuid, placeName);
            dg_PlaceList.ItemsSource = placeList;
        }

        /// <summary>
        /// 地点列表点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dg_PlaceList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ActivityPlaceQueryResult place = dg_PlaceList.SelectedItem as ActivityPlaceQueryResult;
            if (place != null)
            {
                grid_PlaceInfo.Visibility = Visibility.Visible;
                placeModule.InitPage(place.ActivityPlaceInfo);
            }
        }

        /// <summary>
        /// 选择添加按钮时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAdd_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceQueryResult place = dg_PlaceList.SelectedItem as ActivityPlaceQueryResult;
            if (place != null)
            {
                ActivityPlaceInfo newPlace = new ActivityPlaceInfo();
                newPlace.Guid = Utility.NewGuid();
                newPlace.ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
                newPlace.Name = place.ActivityPlaceInfo.Name;
                newPlace.Address = place.ActivityPlaceInfo.Address;
                newPlace.Contact = place.ActivityPlaceInfo.Contact;
                newPlace.Phone = place.ActivityPlaceInfo.Phone;
                newPlace.Image = place.ActivityPlaceInfo.Image;
                newPlace.Graphics = place.ActivityPlaceInfo.Graphics;
                ActivityPlaceLocation[] newLocations = new ActivityPlaceLocation[place.ActivityPlaceInfo.Locations.Length];
                for (int i = 0; i < place.ActivityPlaceInfo.Locations.Length; i++)
                {
                    string locationGuid = Utility.NewGuid();
                    newLocations[i] = new ActivityPlaceLocation();
                    newLocations[i].GUID = locationGuid;
                    newLocations[i].ActivityPlaceGuid = newPlace.Guid;
                    newLocations[i].LocationName = place.ActivityPlaceInfo.Locations[i].LocationName;
                    newLocations[i].LocationLG = place.ActivityPlaceInfo.Locations[i].LocationLG;
                    newLocations[i].LocationLA = place.ActivityPlaceInfo.Locations[i].LocationLA;

                    for (int j = 0; j < place.ActivityPlaceInfo.Locations[i].activityPlaceLocationImage.Count(); j++)
                    {
                        place.ActivityPlaceInfo.Locations[i].activityPlaceLocationImage[j].GUID = Utility.NewGuid();
                        place.ActivityPlaceInfo.Locations[i].activityPlaceLocationImage[j].ACTIVITY_PLACE_LOCATION_GUID = locationGuid;
                    }
                    newLocations[i].activityPlaceLocationImage = place.ActivityPlaceInfo.Locations[i].activityPlaceLocationImage;
                }
                newPlace.Locations = newLocations;
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(
                    channel =>
                    {
                        channel.SavePlaceInfo(newPlace);
                        MessageBox.Show("添加地点成功", "提示", MessageBoxButton.OK);
                        if (RefreshPlaceListEvent != null)
                        {
                            RefreshPlaceListEvent();
                        }
                        this.Close();
                    });
            }
            else
            {
                MessageBox.Show("请选中一个地点", "提示", MessageBoxButton.OK);
            }
        }
        #endregion
    }
}
