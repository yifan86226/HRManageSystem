#region 文件描述
/**********************************************************************************
 * 创建人：wangxin
 * 摘  要：活动管理界面
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
using Microsoft.Win32;
using System.IO;
using CO_IA.UI.MAP;
using System.Drawing;
using I_GS_MapBase.Portal.Types;
using AT_BC.Client.Extensions;

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// ActivityManageModule.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityManageModule : EditableUserControl
    {
        /// <summary>
        /// 活动信息
        /// </summary>
        private Activity _activityInfo = new Activity();

        /// <summary>
        /// 地图类 xiaguohui
        /// </summary>
        private ActivityPlaceMap ActivityMap = new ActivityPlaceMap();

        /// <summary>
        /// 新建后未保存的活动地点
        /// </summary>
        private ActivityPlaceInfo newPlaceNotSave = null;

        public ActivityManageModule()
        {
            InitializeComponent();
            //初始化地图
            IniMap();
            //初始化界面信息
            InitPage();

        }
        //标记地图是否加载完成
        private bool flgMap = false;
        //记录上一次地图上绘制的点的id
        private List<string> point;
        #region 内部方法
        /// <summary>
        /// 初始化界面信息
        /// </summary>
        private void InitPage()
        {
            //初始化活动阶段下拉列表框
            var stages = Enum.GetValues(typeof(CO_IA.Types.ActivityStage)) as CO_IA.Types.ActivityStage[];
            foreach (var activityStage in stages)
            {
                if (activityStage != ActivityStage.None)
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.DataContext = activityStage;
                    item.Content = activityStage.GetDisplayNameFromEnumDisplayNameAttribute().ToString();
                    this.cbStage.Items.Add(item);
                }
            }

            //获取活动信息并绑定到页面
            _activityInfo = GetActivity();
            this.DataContext = _activityInfo;
            foreach (ComboBoxItem item in cbStage.Items)
            {
                if (item.Content.ToString() == _activityInfo.ActivityStage.GetDisplayNameFromEnumDisplayNameAttribute().ToString())
                {
                    cbStage.SelectedItem = item;
                }
            }
            this.txtType.Text = Utility.GetActivityTypeName(_activityInfo.ActivityType);

            if (_activityInfo.Icon != null)
            {
                MemoryStream stream = new MemoryStream(_activityInfo.Icon);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();//初始化
                bmp.StreamSource = stream;//设置源
                bmp.EndInit();//初始化结束
                img.Source = bmp;//设置图像Source
            }
            else
            {
                BitmapImage bmp = new BitmapImage(new Uri("Images/EventImg.png", UriKind.Relative));
                img.Source = bmp;//设置图像Source
            }

            //新建活动时不可以对地点进行操作，第一次保存后才可以
            if (_activityInfo.ActivityStage == ActivityStage.None)
            {
                //this.btnAddPlace.IsEnabled = false;
                //btnSelectPlace.IsEnabled = false;
                //btnDelPlace.IsEnabled = false;
                //btnPlaceSave.IsEnabled = false;

                spBtn.IsEnabled = false;

                _activityInfo.DateFrom = DateTime.Now;
                _activityInfo.DateTo = DateTime.Now;
                _activityInfo.Creator = RiasPortal.Current.UserSetting.UserName;
                _activityInfo.CreateTime = DateTime.Now;
                _activityInfo.ActivityStage = ActivityStage.Prepare;
                cbStage.SelectedIndex = 0;
            }

            //初始化地点列表
            InitPlaceList();

            ActivityMap.ShowMap.MapInitialized += MapInitialized;
            point = new List<string>();
        }

        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <returns>活动信息</returns>
        private static CO_IA.Data.Activity GetActivity()
        {
            Activity activity = new Activity();
            activity.Guid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            activity.Icon = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Icon;
            activity.Name = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Name;
            activity.ShortHand = CO_IA.Client.RiasPortal.ModuleContainer.Activity.ShortHand;
            activity.DateFrom = CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateFrom;
            activity.DateTo = CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateTo;
            activity.Organizer = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Organizer;
            activity.ActivityStage = CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityStage;
            activity.Description = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Description;
            activity.Creator = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Creator;
            activity.CreateTime = CO_IA.Client.RiasPortal.ModuleContainer.Activity.CreateTime;
            activity.ActivityType = CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityType;
            return activity;
        }

        /// <summary>
        /// 保存活动信息后更新CO_IA.Client.RiasPortal.ModuleContainer.Activity
        /// </summary>
        private void updateActivity()
        {
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid = _activityInfo.Guid;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.Icon = _activityInfo.Icon;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.Name = _activityInfo.Name;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.ShortHand = _activityInfo.ShortHand;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateFrom = _activityInfo.DateFrom;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateTo = _activityInfo.DateTo;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.Organizer = _activityInfo.Organizer;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityStage = _activityInfo.ActivityStage;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.Description = _activityInfo.Description;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.Creator = _activityInfo.Creator;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.CreateTime = _activityInfo.CreateTime;
            CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityType = _activityInfo.ActivityType;
        }

        /// <summary>
        /// 获取活动地点列表
        /// </summary>
        /// <param name="_activityGuid">活动guid</param>
        /// <returns></returns>
        private static CO_IA.Data.ActivityPlaceInfo[] GetPlacesByActivityId(string _activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo[]>(
                channel =>
                {
                    return channel.GetPlaceInfosByActivityId(_activityGuid);
                });
        }

        /// <summary>
        /// 初始化活动地点列表
        /// </summary>
        private void InitPlaceList()
        {
            //获取活动地点信息并绑定到页面
            ActivityPlaceInfo[] places = GetPlacesByActivityId(_activityInfo.Guid);
            listPlace.ItemsSource = places;
            if (places.Length > 0)
            {
                //将所有地点添加到地图中
                Dictionary<string, ActivityPlaceInfo> D = new Dictionary<string, ActivityPlaceInfo>();
                foreach (ActivityPlaceInfo place in places)
                {
                    D.Add(place.Guid, place);
                }
                //将地点的位置信息赋值 key为地点ID，value为地点信息
                //test
                ActivityMap.PlaceLocation = D;

                //选中第一个地点
                listPlace.SelectedIndex = 0;
                InitPlaceInfo();

                //将图片信息存储到图片上传页面
                //if (ActivityManageImage.listImage.Count() > 0)
                //{
                //    ActivityManageImage.listImage = new List<ListImage>();
                //}
                //for (int i = 0; i < places.Length; i++)
                //{
                //    if (places[i].Locations.Length > 0)
                //    {
                //        for (int j = 0; j < places[i].Locations.Length; j++)
                //        {
                //            ListImage temp = new ListImage();
                //            temp.locationGuid = places[i].Locations[j].GUID;
                //            //places[i].Locations[j].activityPlaceLocationImage = new List<ActivityPlaceLocationImage>(places[i].Locations[j].activityPlaceLocationImages);
                //            temp.listAPLImage = places[i].Locations[j].activityPlaceLocationImage;
                //            ActivityManageImage.listImage.Add(temp);
                //        }
                //    }

                //}

            }
            else
            {
                ActivityMap.PlaceLocation = new Dictionary<string, ActivityPlaceInfo>();
                grdPlace.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// 初始化右侧地点信息
        /// </summary>
        private void InitPlaceInfo()
        {
            ActivityPlaceInfo place = listPlace.SelectedItem as ActivityPlaceInfo;
            if (place != null)
            {
                ActivityPlaceInfo _placeInfo = new ActivityPlaceInfo();

                _placeInfo.Guid = place.Guid;
                _placeInfo.ActivityGuid = place.ActivityGuid;
                _placeInfo.Name = place.Name;
                _placeInfo.Image = place.Image;
                _placeInfo.Address = place.Address;
                _placeInfo.Contact = place.Contact;
                _placeInfo.Phone = place.Phone;
                _placeInfo.Graphics = place.Graphics;
                _placeInfo.Locations = (ActivityPlaceLocation[])place.Locations.Clone();

                grdPlace.DataContext = _placeInfo;
                if (grdPlace.Visibility == Visibility.Hidden)
                {
                    grdPlace.Visibility = Visibility.Visible;
                }
                if (_placeInfo.Image != null)
                {
                    MemoryStream stream = new MemoryStream(_placeInfo.Image);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();//初始化
                    bmp.StreamSource = stream;//设置源
                    bmp.EndInit();//初始化结束
                    //placeImg.Source = bmp;//设置图像Source
                }
                else
                {
                    BitmapImage bmp = new BitmapImage(new Uri("Images/PlaceImg.png", UriKind.Relative));
                    //placeImg.Source = bmp;//设置图像Source
                }

                //选中地点时选中图形 xiaguohui
                //test
                ActivityMap.CurrentPlaceId = _placeInfo.Guid;
            }
        }

        /// <summary>
        /// 列表中被选中的地点改变后重新绑定右侧显示的地点信息
        /// </summary>
        private void selectPlaceChanged()
        {
            //列表中选中的地点
            ActivityPlaceInfo _placeInfo = listPlace.SelectedItem as ActivityPlaceInfo;
            //右侧显示的地点
            ActivityPlaceInfo place = grdPlace.DataContext as ActivityPlaceInfo;
            //如果选中的项没有改变时，不刷新右侧显示的地点，只有改变时才刷新
            if (_placeInfo != null && place != null && _placeInfo.Guid != place.Guid)
            {
                //如果新建的地点信息没有保存，在选项改变后没有保存的项消失
                if (newPlaceNotSave != null)
                {
                    if (newPlaceNotSave.Guid != _placeInfo.Guid)
                    {
                        grdPlace.DataContext = null;
                        ActivityPlaceInfo[] _places = listPlace.ItemsSource as ActivityPlaceInfo[];
                        List<ActivityPlaceInfo> _placeList = _places.ToList();
                        _placeList.Remove(newPlaceNotSave);
                        listPlace.ItemsSource = _placeList.ToArray();
                        newPlaceNotSave = null;
                    }
                }
                //重新刷新右侧显示的地点信息
                InitPlaceInfo();
            }
        }

        /// <summary>
        /// 保存活动前验证活动信息
        /// </summary>
        /// <returns></returns>
        private bool Validated()
        {
            bool IsSuccess = true;
            StringBuilder strmsg = new StringBuilder();

            if (string.IsNullOrEmpty(_activityInfo.Name))
            {
                strmsg.Append("活动名称不能为空! \r");
                IsSuccess = false;
            }

            if (!string.IsNullOrEmpty(_activityInfo.Name) && _activityInfo.Name.Length > 50)
            {
                strmsg.Append("活动名称不能超过50个字! \r");
                IsSuccess = false;
            }

            if (string.IsNullOrEmpty(_activityInfo.ShortHand))
            {
                strmsg.Append("活动简称不能为空! \r");
                IsSuccess = false;
            }

            if (!string.IsNullOrEmpty(_activityInfo.ShortHand) && _activityInfo.ShortHand.Length > 10)
            {
                strmsg.Append("活动简称不能超过10个字! \r");
                IsSuccess = false;
            }
            if (this._activityInfo.DateFrom.Date > this._activityInfo.DateTo.Date)
            {
                strmsg.Append("活动开始日期不能大于活动结束日期");
                IsSuccess = false;
            }

            if (!string.IsNullOrEmpty(_activityInfo.Organizer) && _activityInfo.Organizer.Length > 50)
            {
                strmsg.Append("组织单位不能超过50个字! \r");
                IsSuccess = false;
            }

            if (!string.IsNullOrEmpty(_activityInfo.Description) && _activityInfo.Description.Length > 500)
            {
                strmsg.Append("活动简介不能超过500个字! \r");
                IsSuccess = false;
            }

            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return IsSuccess;
        }

        /// <summary>
        /// 保存地点前验证地点信息
        /// </summary>
        /// <param name="_placeInfo"></param>
        /// <returns></returns>
        private bool ValidatedPlace(ActivityPlaceInfo _placeInfo)
        {
            bool IsSuccess = true;
            StringBuilder strmsg = new StringBuilder();

            if (string.IsNullOrEmpty(_placeInfo.Name))
            {
                strmsg.Append("地点名称不能为空! \r");
                IsSuccess = false;
            }
            if (!string.IsNullOrEmpty(_placeInfo.Name) && _placeInfo.Name.Length > 100)
            {
                strmsg.Append("地点名称不能超过100个字! \r");
                IsSuccess = false;
            }
            if (!string.IsNullOrEmpty(_placeInfo.Address) && _placeInfo.Address.Length > 100)
            {
                strmsg.Append("地址不能超过100个字! \r");
                IsSuccess = false;
            }
            if (!string.IsNullOrEmpty(_placeInfo.Contact) && _placeInfo.Contact.Length > 10)
            {
                strmsg.Append("联系人不能超过10个字! \r");
                IsSuccess = false;
            }
            if (!string.IsNullOrEmpty(_placeInfo.Phone) && _placeInfo.Phone.Length > 20)
            {
                strmsg.Append("联系电话不能超过20个字! \r");
                IsSuccess = false;
            }
            //if (_placeInfo.Phone != null && !System.Text.RegularExpressions.Regex.IsMatch(_placeInfo.Phone, @"^[1]+\d{10}"))
            //{
            //    strmsg.Append("联系电话格式不正确! \r");
            //    IsSuccess = false;
            //}
            foreach (ActivityPlaceLocation location in _placeInfo.Locations)
            {
                if (string.IsNullOrEmpty(location.LocationName))
                {
                    strmsg.Append("位置名称不能为空! \r");
                    IsSuccess = false;
                }
                if (!string.IsNullOrEmpty(location.LocationName) && location.LocationName.Length > 50)
                {
                    strmsg.Append("位置名称不能超过50个字! \r");
                    IsSuccess = false;
                }
            }

            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return IsSuccess;
        }

        /// <summary>
        /// 初始化地图
        /// </summary>
        public void IniMap()
        {
            //给控件添加地图控件
            showMap.Children.Add(ActivityMap.ShowMap.MainMap);
        }

        #endregion

        #region 事件
        /// <summary>
        /// 添加地点按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddPlace_Click(object sender, RoutedEventArgs e)
        {
            //PlaceManageDialog placeManage = new PlaceManageDialog(_activityInfo.Guid);
            //placeManage.RefreshPlaceListEvent += () => { InitPlaceList(); };
            //placeManage.ShowDialog();
            if (newPlaceNotSave != null)
            {
                MessageBox.Show("新建地点未保存，请保存后再继续添加地点", "提示", MessageBoxButton.OK);
                return;
            }
            ActivityPlaceInfo[] places = listPlace.ItemsSource as ActivityPlaceInfo[];
            if (places != null && places.Length >= 10)
            {
                MessageBox.Show("活动区域最大数量为10。", "消息提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            List<ActivityPlaceInfo> placesList = places.ToList();
            ActivityPlaceInfo newPlace = new ActivityPlaceInfo();
            newPlace.ActivityGuid = _activityInfo.Guid;
            newPlace.Guid = Utility.NewGuid();
            newPlace.Name = "新建活动区域";
            newPlace.Locations = new ActivityPlaceLocation[0];
            placesList.Add(newPlace);
            listPlace.ItemsSource = placesList.ToArray();
            listPlace.SelectedItem = newPlace;
            newPlaceNotSave = newPlace;
            ActivityMap.PlaceLocation.Add(newPlace.Guid, newPlace);
            InitPlaceInfo();
        }

        /// <summary>
        /// 在地图上绘制按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewMap_Click(object sender, RoutedEventArgs e)
        {
            //在地图上绘制 xiaguohui
            ActivityMap.ShowEditMap();

        }

        /// <summary>
        /// 选择添加地点按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectPlace_Click(object sender, RoutedEventArgs e)
        {
            SelectPlaceAddDialog placeManage = new SelectPlaceAddDialog();
            placeManage.RefreshPlaceListEvent += () => { InitPlaceList(); };
            placeManage.ShowDialog(this);
        }


        //private void btnSelectType_Click(object sender, RoutedEventArgs e)
        //{
        //    SelectActivityTypeDialog typeDialog = new SelectActivityTypeDialog();
        //    typeDialog.ShowDialog();
        //}

        /// <summary>
        /// 保存活动信息按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveActivity_Click(object sender, RoutedEventArgs e)
        {
            var stages = Enum.GetValues(typeof(CO_IA.Types.ActivityStage)) as CO_IA.Types.ActivityStage[];
            foreach (var activityStage in stages)
            {
                if (activityStage.GetDisplayNameFromEnumDisplayNameAttribute().ToString() == ((ComboBoxItem)cbStage.SelectedItem).Content.ToString())
                {
                    _activityInfo.ActivityStage = activityStage;
                }
            }

            if (Validated())
            {

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(
                    channel =>
                    {
                        channel.SaveActivity(_activityInfo);
                        MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
                    });
            }
            //btnAddPlace.IsEnabled = true;
            //btnSelectPlace.IsEnabled = true;
            //btnDelPlace.IsEnabled = true;
            //btnPlaceSave.IsEnabled = true;

            spBtn.IsEnabled = true;
            updateActivity();
        }

        /// <summary>
        /// 地点列表点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listPlace_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            selectPlaceChanged();
        }

        /// <summary>
        /// 保存地点按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlaceSave_Click(object sender, RoutedEventArgs e)
        {
            if (listPlace.Items.Count <= 0)
            {
                return;
            }
            ActivityPlaceInfo _placeInfo = (ActivityPlaceInfo)grdPlace.DataContext;
            _placeInfo.Graphics = ActivityMap.PlaceLocation[_placeInfo.Guid].Graphics;
            if (ValidatedPlace(_placeInfo))
            {

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(
                    channel =>
                    {
                        if (channel.ValidationPlaceName(_placeInfo))
                        {
                            channel.SavePlaceInfo(_placeInfo);
                            MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
                            if (newPlaceNotSave != null)
                            {
                                newPlaceNotSave = null;
                            }
                            InitPlaceList();
                        }
                        else
                        {
                            MessageBox.Show("该活动区域名称已存在，请修改后重新保存。", "提示", MessageBoxButton.OK);
                        }
                    });
            }
        }

        /// <summary>
        /// 删除地点按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelPlace_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo place = listPlace.SelectedItem as ActivityPlaceInfo;
            if (place == null)
            {
                MessageBox.Show("请选中地点后再删除", "提示", MessageBoxButton.OK);
                return;
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(
                channel =>
                {
                    try
                    {
                        channel.DeletePlaceInfo(place.Guid);
                        MessageBox.Show("删除成功", "提示", MessageBoxButton.OK);
                        if (newPlaceNotSave != null)
                        {
                            newPlaceNotSave = null;
                        }
                        InitPlaceList();
                        listPlace.SelectedIndex = 0;
                        InitPlaceInfo();
                    }
                    catch 
                    {
                        MessageBox.Show("删除失败，此地点已有子记录，无法删除", "提示", MessageBoxButton.OK);
                    }
                });
        }

        /// <summary>
        ///  添加位置按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddLocation_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo _placeInfo = (ActivityPlaceInfo)grdPlace.DataContext;
            foreach (ActivityPlaceLocation location in _placeInfo.Locations)
            {
                if (string.IsNullOrEmpty(location.LocationName))
                {
                    MessageBox.Show("位置名称不能为空!", "消息提示", MessageBoxButton.OK);
                    return;
                }
                if (!string.IsNullOrEmpty(location.LocationName) && location.LocationName.Length > 50)
                {
                    MessageBox.Show("位置名称不能超过50个字!", "消息提示", MessageBoxButton.OK);
                    return;
                }
            }
            ActivityPlaceInfo place = grdPlace.DataContext as ActivityPlaceInfo;
            ActivityPlaceLocation[] locations = dg_LocationList.ItemsSource as ActivityPlaceLocation[];
            ActivityPlaceLocation newLocation = new ActivityPlaceLocation();
            newLocation.GUID = Utility.NewGuid();
            newLocation.ActivityPlaceGuid = place.Guid;
            newLocation.activityPlaceLocationImage = new List<ActivityPlaceLocationImage>();
            List<ActivityPlaceLocation> listLocation = locations.ToList();
            listLocation.Add(newLocation);
            locations = listLocation.ToArray();
            dg_LocationList.ItemsSource = locations;

        }

        /// <summary>
        /// 删除位置按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelLocation_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceLocation delLocation = dg_LocationList.SelectedItem as ActivityPlaceLocation;
            ActivityPlaceLocation[] locations = dg_LocationList.ItemsSource as ActivityPlaceLocation[];
            List<ActivityPlaceLocation> listLocation = locations.ToList();
            listLocation.Remove(delLocation);
            locations = listLocation.ToArray();
            dg_LocationList.ItemsSource = locations;
            if (delLocation != null)
            {
                //从地图上删除2017-05-25
                ActivityMap.ShowMap.RemoveSymbolElement("groups_" + delLocation.GUID);
            }
        }

        /// <summary>
        /// 上传活动图片事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
            ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp|gif|*.gif";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径
                byte[] tempByte = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中

                byte[] bytes=AT_BC.Common.ImageZipper.ZipAsJpg(tempByte, 300, 300);
                _activityInfo.Icon = bytes;
                MemoryStream stream = new MemoryStream(bytes);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();//初始化
                bmp.StreamSource = stream;//设置源
                bmp.EndInit();//初始化结束
                img.Source = bmp;//设置图像Source




                //Bitmap tempBm = ImageUtility.SmallPic(fileName, 300);
                //if (tempBm != null)
                //{
                //    MemoryStream ms = new MemoryStream();
                //    tempBm.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                //    byte[] bytes = ms.GetBuffer();
                //    _activityInfo.Icon = bytes;

                //}
                //else
                //{
                //    _activityInfo.Icon = tempByte;
                //}
                //_activityInfo.Icon = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                //img.Source = new BitmapImage(new Uri(fileName));//将图片显示到Image控件上
            }
        }
        //public System.Drawing.Bitmap SmallPic(string strOldPic, int intHeight)
        //{
        //    int intWidth = 0;
        //    System.Drawing.Bitmap objPic, objNewPic;
        //    try
        //    {
        //        objPic = new System.Drawing.Bitmap(strOldPic);
        //        intWidth = Convert.ToInt32(((double)intHeight / objPic.Size.Height) * objPic.Size.Width);
        //        if (objPic.Size.Width < intWidth || objPic.Size.Height < intHeight)
        //        {
        //            return null;
        //        }
        //        objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);

        //    }
        //    catch (Exception exp)
        //    { throw exp; }
        //    return objNewPic;
        //}

        /// <summary>
        /// 上传地点图片事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void placeImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ActivityPlaceInfo place=this.listPlace.SelectedValue as ActivityPlaceInfo;
            if (place == null)
            {
                return;
            }
            //ActivityPlaceInfo place = grdPlace.DataContext as ActivityPlaceInfo;
            OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
            ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp|gif|*.gif";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径
                place.Image = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                //System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;
                //Grid g = img.Parent as Grid;
                //foreach (var item in g.Children)
                //{
                //    if (item is System.Windows.Controls.Image)
                //    {
                //        System.Windows.Controls.Image image = item as System.Windows.Controls.Image;
                //        image.Source = new BitmapImage(new Uri(fileName));
                //        break;
                //    }
                //}
                //placeImg.Source = new BitmapImage(new Uri(fileName));//将图片显示到Image控件上
            }
        }

        /// <summary>
        /// 列表中的项被点击时的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //当列表中的项被点击时，设置列表选中这行数据
            //var item =e.Source as ListBoxItem;
            ActivityPlaceInfo _placeInfo = (e.Source as FrameworkElement).DataContext as ActivityPlaceInfo;
            listPlace.SelectedItem = _placeInfo;
            //重新绑定右侧显示的地点信息
            selectPlaceChanged();
        }
        /// <summary>
        /// 双击地点事件，地图显示当前选中地点范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo _placeInfo = (e.Source as FrameworkElement).DataContext as ActivityPlaceInfo;

            if (_placeInfo != null)
            {
                MapExtent extent = ActivityMap.ShowMap.GetGraphicExtentByID(new string[]{MapGroupTypes.AreaRange_.ToString()+_placeInfo.Guid});
                if (extent != null)
                    ActivityMap.ShowMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = extent.Xy2.X, Latitude = extent.Xy2.Y }, true);
                  
            }
            
        } 
        #endregion

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ActivityPlaceLocation locations = (e.Source as FrameworkElement).DataContext as ActivityPlaceLocation;
            if (locations.activityPlaceLocationImage == null)
                locations.activityPlaceLocationImage = new List<ActivityPlaceLocationImage>();

            ActivityManageDialog form = new ActivityManageDialog(locations.activityPlaceLocationImage, locations.GUID);
            form.ShowDialog(this);

            if (locations.activityPlaceLocationImage != null && locations.activityPlaceLocationImage.Count > 0)
            {
                MemoryStream stream = new MemoryStream(locations.activityPlaceLocationImage[0].Image);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();//初始化
                bmp.StreamSource = stream;//设置源
                bmp.EndInit();//初始化结束
                (e.Source as System.Windows.Controls.Image).Source = bmp;
            }
            //List<ListImage> listImageL = ActivityManageImage.listImage;
            //for (int i = 0; i < listImageL.Count; i++)
            //{
            //    if (listImageL[i].locationGuid == locations.GUID)
            //    {
            //        locations.activityPlaceLocationImage = listImageL[i].listAPLImage;
            //        //locations.activityPlaceLocationImages = locations.activityPlaceLocationImage.ToArray();
            //        if (listImageL[i].listAPLImage.Count() > 0)
            //        {
            //            MemoryStream stream = new MemoryStream(listImageL[i].listAPLImage[0].Image);
            //            BitmapImage bmp = new BitmapImage();
            //            bmp.BeginInit();//初始化
            //            bmp.StreamSource = stream;//设置源
            //            bmp.EndInit();//初始化结束
            //            (e.Source as System.Windows.Controls.Image).Source = bmp;

            //            break;
            //        }
            //    }
            //}
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceLocation delLocation = dg_LocationList.SelectedItem as ActivityPlaceLocation;
            if (delLocation == null)
            {
                MessageBox.Show("请先选择一条区域地点", "消息提示", MessageBoxButton.OK);
                return;
            }
            System.Windows.Point newPoint = new System.Windows.Point();

            WinMapCoordinate winMap = new WinMapCoordinate();
            winMap.mapcoor.OnLocationMove += (point) =>
                {
                    newPoint = point;
                };
            if (winMap.ShowDialog() == true)
            {
                delLocation.LocationLG = newPoint.X;
                delLocation.LocationLA = newPoint.Y;
            }
        }

        private void btnSelectExam_Click(object sender, RoutedEventArgs e)
        {
            if (listPlace == null || listPlace.SelectedItem == null)
            {
                MessageBox.Show("请先选择活动区域", "消息提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            ActivityPlaceInfo place = listPlace.SelectedItem as ActivityPlaceInfo;
            string selectPlaceGuid = place.Guid;
            SelectExamPlaceDialog form = new SelectExamPlaceDialog();
            form.OnGetDataEvent += (datas) =>
                {
                    ActivityPlaceLocation[] locations = dg_LocationList.ItemsSource as ActivityPlaceLocation[];

                    List<ActivityPlaceLocation> listLocation = locations.ToList();
                    for (int i = 0; i < datas.Count(); i++)
                    {
                        ActivityPlaceLocation newLocation = new ActivityPlaceLocation();
                        newLocation.GUID = datas[i].Guid;
                        newLocation.ActivityPlaceGuid = place.Guid;
                        newLocation.LocationName = datas[i].Name;
                        newLocation.LocationLA = Convert.ToDouble(datas[i].Location_la);
                        newLocation.LocationLG = Convert.ToDouble(datas[i].Location_lg);
                        newLocation.Remark = datas[i].Phone;
                        newLocation.activityPlaceLocationImage = datas[i].Images;

                        listLocation.Add(newLocation);

                        ListImage listImage = new ListImage();
                        listImage.locationGuid = datas[i].Guid;
                        listImage.listAPLImage = datas[i].Images;
                        //ActivityManageImage.listImage.Add(listImage);

                    }
                    locations = listLocation.ToArray();

                    this.dg_LocationList.ItemsSource = locations;
                };

            form.ShowDialog();
        }

        private void listPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectPlaceChanged();
            //if (flgMap)
            //{
            //    PaintMap();
            //}
        }
        private void MapInitialized(bool success)
        {

            if (success)
            {
                //flgMap = true;
                //PaintMap();
            }
        }
        //初始化地图显示2017-05-25
        private void PaintMap()
        {
            if (point.Count() > 0)
            {
                for (int i = 0; i < point.Count(); i++)
                {
                    ActivityMap.ShowMap.RemoveSymbolElement(point[i]);
                    ActivityMap.ShowMap.RemoveElement(point[i]);
                }
            }
            point.Clear();
            ActivityPlaceInfo place = listPlace.SelectedItem as ActivityPlaceInfo;
            if (place != null)
            {

                ActivityPlaceLocation[] locations = place.Locations;
                if (locations != null && locations.Length > 0)
                {
                    //用于地图显示点置中间位置
                    //AT_BC.Data.GeoPoint[] points = new AT_BC.Data.GeoPoint[locations.Length];

                    for (int i = 0; i < locations.Length; i++)
                    {
                        bool showImage = false;
                        string id = MapGroupTypes.Location_.ToString() + locations[i].GUID;
                        if (locations[i].activityPlaceLocationImage == null || locations[i].activityPlaceLocationImage.Count == 0)
                        {
                           
                        }
                        else
                        {
                            showImage = true;                                                  
                        }
                        LocationPoint locationpoint = new LocationPoint(showImage);
                        locationpoint.ElementId = id;
                        locationpoint.LocationInfo = locations[i];
                        locationpoint.ShowPlaceImageEvent += p_ShowPlaceImageEvent;
                        ActivityMap.ShowMap.AddElement(locationpoint, ActivityMap.ShowMap.GetMapPointEx(locations[i].LocationLG, locations[i].LocationLA));
                        point.Add(id);      
                    }
                }
            }
        }

        void p_ShowPlaceImageEvent(ActivityPlaceLocation obj)
        {
            ActivityManageDialog managerImage = new ActivityManageDialog(obj.activityPlaceLocationImage, "");
            managerImage.ShowDialog(this);
        }

        private void dg_LocationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ActivityPlaceLocation delLocation = dg_LocationList.SelectedItem as ActivityPlaceLocation;
            //if (delLocation != null)
            //{
            //    ActivityMap.ShowMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = delLocation.LocationLG, Latitude = delLocation.LocationLA });
            //}
        }
       
        private void LocationFix(object sender, RoutedEventArgs e)
        {
            ActivityPlaceLocation delLocation = dg_LocationList.SelectedItem as ActivityPlaceLocation;
            if (delLocation != null)
            {
                ActivityMap.ShowMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = delLocation.LocationLG, Latitude = delLocation.LocationLA });
            }
        }

        private void LocationChangeXY(object sender, RoutedEventArgs e)
        {
            ActivityPlaceLocation delLocation = dg_LocationList.SelectedItem as ActivityPlaceLocation;
            if (delLocation != null)
            {
                WinMapCoordinate coord = new WinMapCoordinate();
                coord.mapcoor.selectPoint = new System.Windows.Point() { X= delLocation.LocationLG,Y= delLocation .LocationLA};
                if (coord.ShowDialog() == true)
                {
                    delLocation.LocationLG = coord.mapcoor.selectPoint.X;
                    delLocation.LocationLA = coord.mapcoor.selectPoint.Y;

                    //重新绘制地图位置
                    //PaintMap();
                }
            }
        }

        //private void placeSelect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{

        //}

        private void buttonSettingPlaceImage_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo placeTemp = grdPlace.DataContext as ActivityPlaceInfo;
            ActivityPlaceInfo place = this.listPlace.SelectedValue as ActivityPlaceInfo;
            if (place == null)
            {
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
            ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp|gif|*.gif";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径
                place.Image = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                placeTemp.Image = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
            }
        }

        private void buttonSelectPlace_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("选择模版库或其他活动中的地点");
        }
        private void txtPhone_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }
        private void txtPhone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
        private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }
        //isDigit是否是数字
        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            }
            return true;
        }

        private void buttonVideoSetting_Click(object sender, RoutedEventArgs e)
        {
            ActivityVideoSettingDialog dialog = new ActivityVideoSettingDialog();
            dialog.ShowDialog(this);
        }
        
    }
}
