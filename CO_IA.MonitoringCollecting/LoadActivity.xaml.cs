
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
using CO_IA.UI.Collection;
using CO_IA.UI.Collection.DbEntity;
using PT.Profile.Interface;
using PT.Toolkit.Portable.Collections;
using AT_BC.Client.Extensions;
using AT_BC.Common;
namespace CO_IA.MonitoringCollecting
{
    /// <summary>
    /// LoadActivity.xaml 的交互逻辑
    /// </summary>
    public partial class LoadActivity : Window
    {
        public event Action<Activity> InvokeActivity;
        private bool _isLoadUserInfo = true;
        public LoadActivity(bool p_isLoadUserInfo)
        {
            InitializeComponent();
            this._isLoadUserInfo = p_isLoadUserInfo;
            this.textBlockDownload.Visibility = p_isLoadUserInfo ? Visibility.Visible : Visibility.Collapsed;
            SystemLoginService.IsLogin = p_isLoadUserInfo;
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
                if (e.Source is ListBoxImageItem)
                {
                    this.OpenActivity((e.Source as ListBoxImageItem).DataContext as Activity);
                }
            }
        }

        private void ListBoxRunnableActivityItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (e.Source is ListBoxImageItem)
                {
                    var activity=(e.Source as ListBoxImageItem).DataContext as Activity;
                    if (activity != null)
                    {
                        if (SQLiteDataService.IsExsitThisActivityFile(activity.Name))
                        {
                            if (MessageBox.Show(" 正在下载的'" + activity.Name + "'文件在系统中已存在，\r\n 若重新下载后，原文件中的数据将被清空。\r\n 是否重新下载?", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            {
                                SQLiteDataService.WriteActivityToLocal(activity.Guid);
                            }
                        }
                        else
                        {
                            SQLiteDataService.WriteActivityToLocal(activity.Guid);
                        }
                    }
                    this.OpenActivity(activity);
                    //this.OpenActivity((e.Source as ListBoxImageItem).DataContext as Activity);
                }
            }
        }
        
        private void OpenActivity(Activity activity)
        {
            if (this.InvokeActivity != null)
            {
                this.InvokeActivity(activity);
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
                #endregion
                invoker.Invoke();
            }
            #region 加载活动列表
            //List<Activity> activities2=null;
            //if (this._isLoadUserInfo)
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage>(channel =>
            //        {
            //            activities2 = channel.GetActivities().ToList();
            //        });
            //}
            //else
            //{
            List<Activity> activities2 = SQLiteDataService.QueryActivitiesFromLocal();
            //}
            if (gridLogin.Visibility == System.Windows.Visibility.Visible)
            {
                gridLogin.Visibility = System.Windows.Visibility.Collapsed;
            }
            this.dicLocalActivity.Clear();
            foreach (var activity in activities2)
            {
                this.dicLocalActivity[activity.Guid] = activity;
                ListBoxImageItem item = new ListBoxImageItem();
                item.DataContext = activity;
                item.ToolTip = activity.Name;

                if (activity.Icon!= null && activity.Icon.Length > 100)
                {
                    MemoryStream ms = new MemoryStream(activity.Icon);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();
                    item.ImageSource = bitmapImage;
                }
                else
                {
                    item.ImageSource = new BitmapImage(new Uri(@"/CO_IA.MonitoringCollecting;component/Images/defaultActivity.png", UriKind.RelativeOrAbsolute));
                }
                item.Header = activity.Name;
                //item.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                this.listBoxActivities.Items.Add(item);
            }

            #endregion
        }

        private Dictionary<string, Activity> dicLocalActivity = new Dictionary<string, Activity>();

        private void OpenActivity_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxActivities.SelectedItem != null && listBoxActivities.SelectedItem is ListBoxImageItem)
            {
                Activity activity = (listBoxActivities.SelectedItem as ListBoxImageItem).DataContext as Activity;
                this.OpenActivity(activity);
            }
        }

        private void OpenActivity_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void hyperlinkDownload_Click(object sender, RoutedEventArgs e)
        {
            this.gridDownloadAcitivities.Visibility = System.Windows.Visibility.Visible;
            if (this._isLoadUserInfo)
            {
                this.listBoxRunnableActivities.Items.Clear();
                var activities=PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage,ActivityExt[]>(channel =>
                    {
                        return channel.GetActivityWithPlaces(CO_IA.Types.ActivityStage.Prepare);
                    });

                foreach (var activity in activities)
                {
                    ListBoxImageItem item = new ListBoxImageItem();
                    item.DataContext = activity;
                    item.ToolTip = activity.Name;

                    if (activity.Icon != null && activity.Icon.Length > 100)
                    {
                        MemoryStream ms = new MemoryStream(activity.Icon);
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = ms;
                        bitmapImage.EndInit();
                        item.ImageSource = bitmapImage;
                    }
                    else
                    {
                        item.ImageSource = new BitmapImage(new Uri(@"/CO_IA.MonitoringCollecting;component/Images/defaultActivity.png", UriKind.RelativeOrAbsolute));
                    }
                    item.Header = activity.Name;
                    if (!this.dicLocalActivity.Keys.Contains(activity.Guid))
                    {
                        item.Opacity = 0.35;
                    }
                    this.listBoxRunnableActivities.Items.Add(item);
                }
            }
        }

        private void hyperlinkReturn_Click(object sender, RoutedEventArgs e)
        {
            this.gridDownloadAcitivities.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
