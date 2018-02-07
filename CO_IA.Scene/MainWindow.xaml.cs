using System.Collections.Generic;
using System.Windows;
using CO_IA.Client;
using CO_IA.Data;
//using CO_IA.UI.Scene;
using DevExpress.Xpf.Editors;
using I_CO_IA.PersonSchedule;
using CO_IA.Data.Gps;
using System;
using System.Threading;
using CO_IA.UI.Collection.DbEntity;
using System.Windows.Input;
using AT_BC.Data.Helpers;
using System.Linq;
using CO_IA.UI.RemoteScreenCapturer;
using System.Windows.Media.Imaging;
namespace CO_IA.Scene
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow : Window, Client.IRiasModuleContainer, Client.IMessageReceiver, IGpsOrbitReceiver, AT_BC.Data.IAsyncExceptionReceiver
    {
        private PP_VehicleInfo vehicleInfo;
        private SerialPortSetting serialPortSetting;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            CO_IA.Client.RiasPortal.SetupModuleContainer(this);
            //SystemLoginService.SynTypes.Add(typeof(CO_IA.UI.Collection.LoginService));
            this.Loaded += MainWindow_Loaded;

            showConnect = System.Windows.Visibility.Collapsed;
        }

        public void LoadActivityPlace(CO_IA.Data.Activity activity, ActivityPlace place, PP_OrgInfo orgInfo)
        {

         
            //RiasPortal.UpdateUserRights(orgInfo);
            //RiasPortal.DutyList.Clear();
            //if (orgInfo != null && !string.IsNullOrWhiteSpace(orgInfo.DUTY))
            //{
            //    RiasPortal.DutyList.AddRange(orgInfo.DUTY.Split(','));
            //}

            this.executorLognInfo.LoginOrg = orgInfo;
            //this.executorLognInfo.LoginPlace = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, ActivityPlaceInfo>(channel =>
            //    {
            //        return channel.GetPlaceInfo(place.Guid);
            //    });
            this.Activity = activity;
            //SystemLoginService.CurrentActivity = activity;
            //SystemLoginService.CurrentActivityPlace = place;
            _activityName.Text = activity.Name;
            this.textBlockPlace.Text = place.Name;
            textBlockPersonGroup.Text = orgInfo.NAME;
            //LoadUserGroupInfo();
            //SetMenuDisplayState();
            //try
            //{
            //    int servicePort = CO_IA.Client.Utility.StartMessageService(port =>
            //        {
            //            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MessageCenter.I_CO_IA_MessageCenter>
            //            (channel =>
            //            {
            //                channel.RegisterSceneClient(port, this.Activity.Guid, this.executorLognInfo.LoginOrg.GUID, this.executorLognInfo.LoginPlace.Guid);
            //            });
            //        });
            //    CO_IA.Client.Utility.RegisterMessageReceiver(this);
            //    //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MessageCenter.I_CO_IA_MessageCenter>
            //    //(channel =>
            //    //{
            //    //    channel.RegisterSceneClient(servicePort, this.Activity.Guid, this.executorLognInfo.LoginOrg.GUID, this.executorLognInfo.LoginPlace.Guid);
            //    //});
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(this, ex.Message, "启动消息接收服务错误");
            //}
            //try
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PersonSchedule>(
            //        channel =>
            //        {
            //            var vehicle = channel.GetPP_VehicleInfo(this.executorLognInfo.LoginOrg.GUID);
            //            if (vehicle != null && !string.IsNullOrWhiteSpace(vehicle.GUID))
            //            {
            //                this.vehicleInfo = vehicle;
            //                this.checkBoxGps.Visibility = System.Windows.Visibility.Visible;
            //            }
            //        });
            //}
            //catch (System.Exception e)
            //{
            //    MessageBox.Show(this, e.Message, "读取车辆信息失败");
            //}
            //xFreqAppButton.IsChecked = true;
            this.xTaskAppButton.IsChecked = true;
        }

        private void SetMenuDisplayState()
        {
            var dutyCodeQuerier = Utility.GetUIFactory().DutyCodeQuerier;
            bool support=Utility.IsSupported(new Types.ActivityStep[] { Types.ActivityStep.FreqPlanning, Types.ActivityStep.StationPlanning}, new string[] {
            dutyCodeQuerier.FreqMonitor,dutyCodeQuerier.FreqStation,dutyCodeQuerier.EquipmentInspection});
            if (support)
            {
                xFreqAppButton.Visibility = System.Windows.Visibility.Visible;
                xStatAppButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                xFreqAppButton.Visibility = System.Windows.Visibility.Collapsed;
                xStatAppButton.Visibility = System.Windows.Visibility.Collapsed;
            }
            bool hasMonitorDuty = Utility.HasDuty(dutyCodeQuerier.FreqMonitor);
            if (hasMonitorDuty)
            {
                xMonitorAppButton.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                xMonitorAppButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            //, Types.ActivityStep.MonitorPlanning 

            //var uiFactory = Utility.GetUIFactory();
            
            //bool hasFreqPlan = uiFactory.GetUIBuilder(this.Activity.ActivityType).CanBuildStep(Types.ActivityStep.FreqPlanning);
            //bool hasMonitorDuty = Utility.HasDuty(uiFactory.DutyCodeQuerier.FreqMonitor);
            //if (!hasFreqPlan)
            //{
            //    xFreqAppButton.Visibility = System.Windows.Visibility.Collapsed;
            //    xStatAppButton.Visibility = System.Windows.Visibility.Collapsed;
            //}
            //else if (hasMonitorDuty)
            //{
            //    xFreqAppButton.Visibility = System.Windows.Visibility.Collapsed;
            //    xStatAppButton.Visibility = System.Windows.Visibility.Collapsed;
            //}
            //if (hasMonitorDuty)
            //{
            //    xMonitorAppButton.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    ////非监测组
            //    //xFreqAppButton.Visibility = System.Windows.Visibility.Collapsed;
            //    //xStatAppButton.Visibility = System.Windows.Visibility.Collapsed;
            //    xMonitorAppButton.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        private List<FrameworkElement> moduleList = new List<FrameworkElement>();

        private void SetupModule<T>() where T : FrameworkElement, new()
        {
            this.xGridContianer.Children.Clear();
            foreach (FrameworkElement element in moduleList)
            {
                if (element is T)
                {
                    this.xGridContianer.Children.Add(element);
                    return;
                }
            }
            var module = new T();
            moduleList.Add(module);
            this.xGridContianer.Children.Add(module);
        }

        private void LoadUserGroupInfo()
        {
            //SystemLoginService.UserOrgInfo = GetCurrentUserOrgInfo();
            textBlockPersonGroup.Text = this.executorLognInfo.LoginOrg.NAME;
        }
        //private PP_OrgInfo GetCurrentUserOrgInfo()
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PersonSchedule, PP_OrgInfo>(channel =>
        //    {
        //        return channel.GetPP_OrgInfoByPersonID(RiasPortal.Current.UserSetting.UserID, SystemLoginService.CurrentActivity.Guid);
        //    });
        //}
        //public static CO_IA.Data.ActivityPlaceInfo[] GetPlacesByActivityId(string p_activityGuid)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo[]>(
        //        channel =>
        //        {
        //            return channel.GetPlaceInfosByActivityId(p_activityGuid);
        //        });
        //}
        #region override

        public override void EndInit()
        {
            base.EndInit();
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rect = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rect.Width;
            this.Height = rect.Height;
        }

        #endregion

        #region 内部事件

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string loginArea = CO_IA.Client.RiasPortal.Current.SystemConfig.LoginArea;
            if (!string.IsNullOrWhiteSpace(loginArea) && loginArea.Length >= 2)
            {
                this.textBlockArea.Text = CO_IA.Client.Utility.GetAreaName(CO_IA.Client.RiasPortal.Current.SystemConfig.LoginArea.Substring(0, 2));
            }
            //taskView = new CO_IA.Scene.Task.TemporaryTaskView(); //任务列表有问题626
            //this.moduleList.Add(taskView);
            this.textBlockLoginUser.Text = CO_IA.Client.RiasPortal.Current.UserSetting.UserName;
        }

        //private CO_IA.Scene.Task.TemporaryTaskView taskView;

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确实要关闭系统吗", "关闭系统", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
                //System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
        protected override void OnClosed(System.EventArgs e)
        {
            CO_IA.Client.Utility.StopMessageService();
            base.OnClosed(e);
        }
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        #endregion

        #region 菜单事件

        private void xRbTask_Checked(object sender, RoutedEventArgs e)
        {
            this.SetupModule<CO_IA.UI.MonitorTask.ExecutorTaskManageModule>();
        }

        private void xRbTask_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void xRbFreqPlan_Checked(object sender, RoutedEventArgs e)
        {
            this.SetupModule<CO_IA.Scene.FreqPlan.FreqPlanViewNew>();
        }

        private void xRbMonitorPlan_Checked(object sender, RoutedEventArgs e)
        {
            //SystemLoginService.CopyLoginInfomation(typeof(CO_IA.UI.Collection.LoginService));
            this.SetupModule<CO_IA.Scene.Monitor.MonitorPlanViewNew>();
        }

        private void xStatPlan_Checked(object sender, RoutedEventArgs e)
        {
            this.SetupModule<CO_IA.Scene.StationPlan.StationPlanViewNew>();
        }


        private void checkBoxWorkLog_Checked(object sender, RoutedEventArgs e)
        {
            this.SetupModule<CO_IA.UI.MonitorTask.WorkLogModule>();
        }

        #endregion

        /// <summary>
        /// 消息接收函数入口,该接口负责接收消息并将其处理,该方法将在SyncContext指定上下文上执行
        /// 程序实现者需要改写该方法
        /// </summary>
        /// <param name="message">收到的消息</param>
        public void Receive(Data.ActivityMessage message)
        {
            //if (message.MessageType == "Task")
            //{
            //    var taskListInfo = DataContractSerializeHelper.DeSerialize<CO_IA.Data.TaskManage.TaskListInfo>(message.Content);
            //    if (taskView == null)
            //    {
            //        taskView = new CO_IA.Scene.Task.TemporaryTaskView();
            //    }
            //    this.taskView.AddTask(taskListInfo);
            //}
        }

        private System.Threading.SynchronizationContext syncContext = System.Threading.SynchronizationContext.Current;

        public System.Threading.SynchronizationContext SyncContext
        {
            get
            {
                return this.syncContext;
            }
        }

        //private void AreaComBoxEdit_SelectedIndexChanged(object sender, RoutedEventArgs e)
        //{
        //    ComboBoxEdit CboxEdit = sender as ComboBoxEdit;
        //    ComboBoxEditItem item = CboxEdit.SelectedItem as ComboBoxEditItem;
        //    SystemLoginService.CurrentActivityPlace = item.DataContext as ActivityPlace;
        //}

        public CO_IA.Data.Activity Activity
        {
            get;
            private set;
        }

        public GS_MapBase.MapControl CreateMap()
        {
            throw new System.NotImplementedException();
        }

        //private void xRbMonitorPlanOld_Checked(object sender, RoutedEventArgs e)
        //{
        //    this.SetupModule<CO_IA.Scene.Monitor.MonitorPlanView>();
        //}
        private Visibility _showconnect;
        private Visibility showConnect
        {
            get { return _showconnect; }
            set {
                if (_showconnect != value)
                {
                    _showconnect = value;
                    this.Dispatcher.BeginInvoke(new Action(() => { LayConnect.Visibility = _showconnect; }));
                }                
            }
        }
        private void checkBoxGps_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as System.Windows.Controls.CheckBox;
            try
            {
                if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
                {
                    if (this.serialPortSetting == null)
                    {
                        this.SettingSerialPort();
                    }
                    if (this.serialPortSetting != null)
                    {
                        var gpsDisposor = GpsOrbitDisposor.CreateDisposor(this.vehicleInfo.VEHICLE_NUMB, SystemLoginService.CurrentActivity.Guid);
                        gpsDisposor.RegisterReceiver(this);
                        gpsDisposor.ExceptionReceiver = this;
                        GpsLib.GpsSensor.Connect(this.serialPortSetting.IP, this.serialPortSetting.Port, gpsDisposor);
                        showConnect = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        checkBox.IsChecked = false;
                    }
                }
                else
                {
                    showConnect = System.Windows.Visibility.Collapsed;
                    this.gpsMonitoringTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    this.gpsMonitoringTimer = null;
                    GpsLib.GpsSensor.CloseGps();
                }
            }
            catch (System.Exception ex)
            {
                showConnect = System.Windows.Visibility.Collapsed;
                (sender as System.Windows.Controls.CheckBox).IsChecked = false;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (checkBox.IsChecked == true)
                {
                    
                    this.imageWarning.ToolTip = null;
                    this.stopwatch.Restart();
                   
                    this.gpsMonitoringTimer = new System.Threading.Timer(obj =>
                    {
                        if (GpsLib.GpsSensor.IsOpenGps)
                        {
                            showConnect = System.Windows.Visibility.Collapsed;
                            //
                            
                            if (this.stopwatch.Elapsed.Seconds >= 60)
                            {
                                this.gpsMonitoringTimer.Change(Timeout.Infinite, Timeout.Infinite);
                                this.gpsMonitoringTimer = null;
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        this.checkBoxGps.IsChecked = false;
                                        GpsLib.GpsSensor.CloseGps();
                                        MessageBox.Show("GPS设备超过60秒钟没有传回数据,将被关闭,请检查设备");
                                    }));
                            }
                        }
                        else
                        {
                            if (this.stopwatch.Elapsed.Seconds >= 30)
                            {
                                this.gpsMonitoringTimer.Change(Timeout.Infinite, Timeout.Infinite);
                                this.gpsMonitoringTimer = null;
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    showConnect = System.Windows.Visibility.Collapsed;
                                    GpsLib.GpsSensor.CloseGps();
                                    MessageBox.Show("连接GPS设备超时");
                                }));
                                
                                
                            }
                        }
                    }, null, 1000, 1000);
                }
                else
                {
                    if (this.gpsMonitoringTimer != null)
                    {
                        this.gpsMonitoringTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        this.gpsMonitoringTimer = null;
                        GpsLib.GpsSensor.CloseGps();
                    }
                }
            }
        }

        private System.Threading.Timer gpsMonitoringTimer;
        private void SettingSerialPort()
        {
            SerialPortSetting portSetting = this.serialPortSetting;
            if (portSetting == null)
            {
                //portSetting = new SerialPortSetting { BaudRate = 4800, DataBits = 8, Parity = System.IO.Ports.Parity.None, StopBits = System.IO.Ports.StopBits.None, PortName = "" };
                portSetting = new SerialPortSetting { IP="192.168.3.239", Port=7788 };
            }
            SerialPortSettingDialog dialog = new SerialPortSettingDialog();
            dialog.Closing += dialog_Closing;
            dialog.SetSetting(portSetting);
            dialog.ShowDialog(this);
        }

        private void checkBoxGps_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var checkBox = sender as System.Windows.Controls.CheckBox;
            if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
            {
                return;
            }
            this.SettingSerialPort();
        }

        void dialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialog = sender as SerialPortSettingDialog;
            if (dialog.DialogResult == true)
            {
                try
                {
                    this.serialPortSetting = dialog.GetSetting();
                }
                catch (System.Exception ex)
                {
                    e.Cancel = true;
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void Receive(GpsOrbit gpsOrbit)
        {
            this.stopwatch.Restart();
        }

        public void Receive(Exception ex)
        {
            this.imageWarning.ToolTip = string.Format("{0:T} GPS数据处理异常\r\n\t{1}", DateTime.Now, ex.GetExceptionMessage());
        }

        private void rectangleContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AT_BC.Common.CheckableCheckBoxHelper.CheckAll(sender, e);
        }


        public ExecutorLoginInfo GetExecutorLoginInfo()
        {
            return this.executorLognInfo;
        }

        private ExecutorLoginInfo executorLognInfo = new ExecutorLoginInfo();

        private void RemoteWatch_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScreenCapturer.Instance.ShowToolWindow(((result) =>
                {
                    switch (result)
                    { 
                        case 0:
                            RemoteWatch.Source = new BitmapImage(new Uri("/CO_IA.Scene;component/Images/ScreenON.png", 0));
                            break;
                        case -1:
                        case -2:
                            RemoteWatch.Source = new BitmapImage(new Uri("/CO_IA.Scene;component/Images/ScreenOFF.png", 0));
                            break; 
                    }
                }));
        }

    }
}
