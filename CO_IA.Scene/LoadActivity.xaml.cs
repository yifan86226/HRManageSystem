
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AT_BC.Common.Controls;
using CO_IA.Client;
using CO_IA.Data;
using PT.Profile.Interface;
using PT.Toolkit.Portable.Collections;
using System.Windows.Controls;
using CO_IA.Data.Gps;
using I_CO_IA.PersonSchedule;
using AT_BC.Client.Extensions;
using AT_BC.Common;
using I_CO_IA.ActivityManage;
//using CO_IA.UI.Scene;

namespace CO_IA.Scene
{
    /// <summary>
    /// LoadActivity.xaml 的交互逻辑
    /// </summary>
    public partial class LoadActivity : Window
    {
        public event Action<Activity, ActivityPlace, PP_OrgInfo> InvokeActivityPlace;
        private bool _isLoadUserInfo = true;
        public LoadActivity(bool p_isLoadUserInfo)
        {
            InitializeComponent();
            this._isLoadUserInfo = p_isLoadUserInfo;
            //SystemLoginService.IsLogin = p_isLoadUserInfo;
        }
        private void rectangleContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确实要关闭系统吗", "关闭系统", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
            }
        }
        private void ListBoxItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem item = e.Source as ListBoxItem;
                if (item != null && item.DataContext is ActivityPlace)
                {
                    ActivityPlace place = item.DataContext as ActivityPlace;
                    var listBox = VisualTreeHelperExtension.GetParentObject<ListBox>(item);
                    if (listBox != null && place != null)
                    {
                        ActivityExt activity = listBox.DataContext as ActivityExt;
                        var userInfo = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PersonSchedule, PP_OrgInfo>(channel =>
                        {
                            return channel.GetPP_OrgInfoByPersonID(RiasPortal.Current.UserSetting.UserID, activity.Guid);
                        });
                        if (userInfo != null && !string.IsNullOrEmpty(userInfo.GUID))
                        {
                            //SystemLoginService.UserOrgInfo = userInfo;
                            //SystemLoginService.CurrentActivity = activity;
                            //SystemLoginService.CurrentActivityPlace = place;

                            this.OpenActivityPlace(activity, place, userInfo);
                        }
                        else
                        {
                            MessageBox.Show("无法打开活动,未能加载当前用户在该活动中的信息");
                        }
                    }

                }
            }
        }

        private void OpenActivityPlace(Activity activity, ActivityPlace place, PP_OrgInfo orgInfo)
        {
            if (this.InvokeActivityPlace != null)
            {
                this.InvokeActivityPlace(activity, place, orgInfo);
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();
        }
        private void Init()
        {
            if (_isLoadUserInfo)
            {
                AsyncBatchOperationInvoker invoker = new AsyncBatchOperationInvoker(result =>
                {
                    gridLogin.Visibility = System.Windows.Visibility.Collapsed;
                    if (!result.IsValid)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine("加载启动信息出错,程序将自动关闭");
                        builder.AppendLine("错误信息:");
                        builder.AppendLine(result.Exception.GetExceptionMessage());
                        MessageBox.Show(this, builder.ToString());
                        App.Current.Shutdown();
                    }
                },
                    hintInfo =>
                    {
                        this.textBlockHint.Text = hintInfo;
                    },
                        System.Threading.SynchronizationContext.Current);
                invoker.Add<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, DateTime>(channel =>
                {
                    return channel.GetServerTime();
                },
                result =>
                {
                    if (result.IsValid)
                    {
                        CO_IA.Client.Utility.RegisterLoginTime(result.Result);
                    }
                }, "正在读取服务器时间");
                #region 读取用户权限
                invoker.Add<ISubsystem, StringList>(channel =>
                {
                    return channel.GetUsableAuthoritiesBySubsystemID(RiasPortal.Current.UserSetting.UserID, CO_IA.Public.SubSystemIDs.Rias);
                },
                    result =>
                    {
                        if (result.IsValid)
                        {
                            var activities = result.Result;
                            RiasPortal.Current.UserSetting.UserRights = result.Result.ToArray();
                        }
                    }, "正在加载用户权限");
                #endregion

                #region 加载活动列表
                invoker.Add<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityExt[]>(channel =>
                {
                    return channel.GetActivityWithPlacesByUserID(CO_IA.Types.ActivityStage.Execute, RiasPortal.Current.UserSetting.UserID);
                },
                    result =>
                    {
                        if (result.IsValid)
                        {
                            var activities = result.Result.OrderBy(p => p.ActivityStage).ThenByDescending(p => p.CreateTime).ToArray();
                            this.listBoxActivities.ItemsSource = activities;
                        }
                    }, "正在加载活动");

                #endregion

                #region GPS配置信息
                invoker.Add<I_CO_IA.Setting.I_CO_IA_Setting, GpsDataAnalyseConfig>(channel =>
                {
                    return channel.GetGpsDataAnalyseConfig();
                },
                result =>
                {
                    if (result.IsValid)
                    {
                        RiasPortal.RegisterTypeSessionParam(result.Result);
                    }
                }, "读取GPS配置信息");
                #endregion

                #region 加载地图地址
                invoker.Add<I_CO_IA.Setting.I_CO_IA_Setting, MapConfig>(channel =>
                {
                    return channel.GetMapConig();
                },
                result =>
                {
                    if (result.IsValid)
                    {
                        RiasPortal.Current.MapConfig.ElectricUrl = result.Result.ElectricUrl;
                        RiasPortal.Current.MapConfig.SatelliteUrl = result.Result.SatelliteUrl;
                        RiasPortal.RegisterSessionParam(RiasPortal.MapDefaultArea, result.Result.DefaultGeoRange);
                    }
                }, "读取地图配置信息");
                invoker.Invoke();
                #endregion

                invoker.Invoke();
            }
        }
    }
}
