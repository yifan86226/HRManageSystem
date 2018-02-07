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
using CO_IA.Data;
using CO_IA.UI.MAP;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using CO_IA.Client;
using CO_IA.Types;

namespace CO_IA.UI.ActivityManage
{
    /// <summary>
    /// EmergencyManageModule.xaml 的交互逻辑
    /// </summary>
    public partial class EmergencyManageModule : UserControl
    {
        #region 变量
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
        //标记地图是否加载完成
        private bool flgMap = false;
        private const string cPlaceName = "新建应急区域";
        #endregion

        #region 构造函数
        public EmergencyManageModule()
        {
            InitializeComponent();
            IniMap();
            //初始化界面信息
            InitPage();
        }
        #endregion

        #region 事件
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
            btnAddPlace.IsEnabled = true;
            //btnUpdatePlace.IsEnabled = true;
            btnDelPlace.IsEnabled = true;
            updateActivity();
        }
        /// <summary>
        /// 添加地点按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddPlace_Click(object sender, RoutedEventArgs e)
        {
            if (newPlaceNotSave != null)
            {
                MessageBox.Show("新建应急区域未保存，请保存后再继续添加应急区域", "提示消息", MessageBoxButton.OK);
                return;
            }
            ActivityPlaceInfo[] places = listPlace.ItemsSource as ActivityPlaceInfo[];
            List<ActivityPlaceInfo> placesList = places.ToList();
            ActivityPlaceInfo newPlace = new ActivityPlaceInfo();
            newPlace.ActivityGuid = _activityInfo.Guid;
            newPlace.Guid = Utility.NewGuid();
            //newPlace.Name = this.txtName.Text;
            newPlace.Name = cPlaceName;//新建时，先添加默认名称
            newPlace.Locations = new ActivityPlaceLocation[0];
            placesList.Add(newPlace);
            listPlace.ItemsSource = placesList.ToArray();
            listPlace.SelectedItem = newPlace;
            newPlaceNotSave = newPlace;
            ActivityMap.PlaceLocation.Add(newPlace.Guid, newPlace);
            InitPlaceInfo();

            this.btnPlaceSave.IsEnabled = true;
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

                        if (listPlace.Items.Count <= 0)
                        {
                            this.txtName.Text = "";
                            this.txtAddress.Text = "";
                            this.txtContact.Text = "";
                            this.txtPhone.Text = "";
                            this.btnPlaceSave.IsEnabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("删除失败，此地点已有子记录，无法删除", "提示", MessageBoxButton.OK);
                    }
                });
        }
        private void listPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (newPlaceNotSave != null && listPlace.SelectedItem != null && newPlaceNotSave.Guid != ((ActivityPlaceInfo)listPlace.SelectedItem).Guid)
            {
                if (MessageBox.Show("新建考试区域未保存，当前切换区域将会丢失新建考区，是否切换？", "提示消息", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    selectPlaceChanged();
                    if (flgMap)
                    {
                        //PaintMap();
                    }
                }
                else
                {
                    listPlace.SelectedItem = newPlaceNotSave;
                }
            }
            else
            {
                selectPlaceChanged();
                if (flgMap)
                {
                    // PaintMap();
                }
            }

        }
        /// <summary>
        /// 保存地点按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlaceSave_Click(object sender, RoutedEventArgs e)
        {
            ActivityPlaceInfo _placeInfo = (ActivityPlaceInfo)grdPlace.DataContext;
            _placeInfo.Name = this.txtName.Text;
            _placeInfo.Address = this.txtAddress.Text;
            _placeInfo.Contact = this.txtContact.Text;
            _placeInfo.Phone = this.txtPhone.Text;
            //ActivityPlaceInfo _placeInfo = listPlace.SelectedItem as ActivityPlaceInfo;

            _placeInfo.Graphics = ActivityMap.PlaceLocation[_placeInfo.Guid].Graphics;
            if (ValidatedPlace(_placeInfo))
            {

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(
                    channel =>
                    {
                        channel.SavePlaceInfo(_placeInfo);
                        MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
                        if (newPlaceNotSave != null)
                        {
                            newPlaceNotSave = null;
                        }
                        InitPlaceList();
                    });
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
                _activityInfo.Icon = AT_BC.Common.ImageZipper.ZipAsJpg(tempByte,300,300);
                MemoryStream stream = new MemoryStream(_activityInfo.Icon);
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
                ////_activityInfo.Icon = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                //img.Source = new BitmapImage(new Uri(fileName));//将图片显示到Image控件上
            }
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
        #endregion

        #region 方法
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
        /// 列表中被选中的地点改变后重新绑定右侧显示的地点信息
        /// </summary>
        private void selectPlaceChanged()
        {
            if (listPlace.SelectedItem == null)
            {
                return;
            }
            //列表中选中的地点
            ActivityPlaceInfo _placeInfo = listPlace.SelectedItem as ActivityPlaceInfo;
            //右侧显示的地点
            //ActivityPlaceInfo place = grdPlace.DataContext as ActivityPlaceInfo;
            //如果选中的项没有改变时，不刷新右侧显示的地点，只有改变时才刷新
            //if (_placeInfo != null && place != null && _placeInfo.Guid != place.Guid)
            //{
            //如果新建的地点信息没有保存，在选项改变后没有保存的项消失
            if (newPlaceNotSave != null)
            {
                if (newPlaceNotSave.Guid != _placeInfo.Guid)
                {
                    //grdPlace.DataContext = null;
                    ActivityPlaceInfo[] _places = listPlace.ItemsSource as ActivityPlaceInfo[];
                    List<ActivityPlaceInfo> _placeList = _places.ToList();
                    _placeList.Remove(newPlaceNotSave);
                    listPlace.ItemsSource = _placeList.ToArray();
                    newPlaceNotSave = null;
                }
            }
            if (_placeInfo != null)
            {
                this.txtName.Text = _placeInfo.Name;
                this.txtAddress.Text = _placeInfo.Address;
                this.txtContact.Text = _placeInfo.Contact;
                this.txtPhone.Text = _placeInfo.Phone;
            }
            //重新刷新右侧显示的地点信息
            InitPlaceInfo();
            //}
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

                //选中地点时选中图形 xiaguohui
                //test
                ActivityMap.CurrentPlaceId = _placeInfo.Guid;
            }
            else
            {
                ActivityMap.CurrentPlaceId = "";
            }
        }
        /// <summary>
        /// 初始化地图
        /// </summary>
        public void IniMap()
        {
            //给控件添加地图控件
            showMap.Children.Add(ActivityMap.ShowMap.MainMap);
        }
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
                btnAddPlace.IsEnabled = false;
                //btnUpdatePlace.IsEnabled = false;
                btnDelPlace.IsEnabled = false;
                _activityInfo.DateFrom = DateTime.Now;
                _activityInfo.DateTo = DateTime.Now;
                _activityInfo.Creator = RiasPortal.Current.UserSetting.UserName;
                _activityInfo.CreateTime = DateTime.Now;
                _activityInfo.ActivityStage = ActivityStage.Prepare;
                cbStage.SelectedIndex = 0;
                this.btnPlaceSave.IsEnabled = false;
            }
            ////初始化地点列表
            InitPlaceList();
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
            }

            if (places.Length <= 0)
            {
                this.btnPlaceSave.IsEnabled = false;
            }
            else
            {
                this.btnPlaceSave.IsEnabled = true; ;
            }
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
        #endregion
    }
}
