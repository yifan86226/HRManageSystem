using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CO_IA.Data;
using CO_IA.MonitoringCollecting.Views;
using CO_IA.UI.Collection;
using CO_IA.UI.Collection.DbEntity;
using CO_IA.UI.StationManage;
using CO_IA_Data;
using DevExpress.Xpf.Editors;
using System.Windows.Input;
using System.Windows.Controls;

namespace CO_IA.MonitoringCollecting
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        CO_IA.UI.Collection.DataAanalysis dataAanalysis;
        private Action<Activity, ActivityPlace> AreaChanged;
        private ActivityPlace _currentPlace;

        public MainWindow(ActivityPlace p_activityPlace, Action<Activity, ActivityPlace> p_areaChanged)
        {
            InitializeComponent();
            this._currentPlace = p_activityPlace;
            this.AreaChanged = p_areaChanged;
            SystemLoginService.SynTypes.Add(typeof(CO_IA.UI.Collection.LoginService));
            this.Loaded += MainWindow_Loaded;
        }
      
        private void rectangleContainer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// 加载活动
        /// </summary>
        /// <param name="activity"></param>
        public void LoadActivity(CO_IA.Data.Activity activity)
        {
            SystemLoginService.CurrentActivity = activity;
            SetNetLinkState(SystemLoginService.IsLogin);
            _activityName.Text = activity.Name;
            List<ActivityPlace> placeList = SQLiteDataService.QueryPlaceByActivityGuid(activity.Name,activity.Guid);
            if (placeList.Count == 0)
            {
                _layerMask.Visibility = System.Windows.Visibility.Visible;
                xCollect.Visibility = System.Windows.Visibility.Collapsed;
                xAppButtonAnalysis.Visibility = System.Windows.Visibility.Collapsed;
                xDataUpload.Visibility = System.Windows.Visibility.Collapsed;
                _noPlaceTooltip.Text = "没有找到活动区域，请在保持联网的状态下，下载最新活动区域数据后，再重新启动系统";
                return;
            }
            else
            {
                _layerMask.Visibility = System.Windows.Visibility.Collapsed;
                xCollect.Visibility = System.Windows.Visibility.Visible;
                xAppButtonAnalysis.Visibility = System.Windows.Visibility.Visible;
                if (SystemLoginService.IsLogin == true)
                {
                    xDataUpload.Visibility = System.Windows.Visibility.Visible;
                }
               _noPlaceTooltip.Text = "";
                
            }
            
            LoadAreaCbox(placeList);
            
        }
        /// <summary>
        /// 加载活动区域的Combox
        /// </summary>
        /// <param name="p_placeList"></param>
        private void LoadAreaCbox(List<ActivityPlace> p_placeList)
        {
            LoadAreaCboxItems(p_placeList);
            SetAreaCboxSelectedItem(); 
        }

        private void LoadAreaCboxItems(List<ActivityPlace> p_placeList)
        {
            for (int i = 0; i < p_placeList.Count; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.FontSize = 12;
                item.FontWeight = FontWeights.Normal;
                item.DataContext = p_placeList[i];
                item.Content = p_placeList[i].Name;
                item.Tag = p_placeList[i].Guid;
                _areaCBox.Items.Add(item);
            }
        }

        /// <summary>
        /// 设置不同网络连接状态的显示内容
        /// </summary>
        /// <param name="p_isLink"></param>
        private void SetNetLinkState(bool p_isLink)
        {
            if (p_isLink == false)
            {
                xDataUpload.Visibility = System.Windows.Visibility.Collapsed;
                xDataDownload.Visibility = System.Windows.Visibility.Collapsed;
                _errorHint.Text = "网络未连接";
            }
            else
            {
                xDataUpload.Visibility = System.Windows.Visibility.Visible;
                xDataDownload.Visibility = System.Windows.Visibility.Visible;
                _errorHint.Text = "";
            }
        }

        private void SetAreaCboxSelectedItem()
        {
            if (_currentPlace != null && !string.IsNullOrEmpty(_currentPlace.Guid))
            {
                for (int i = 0; i < _areaCBox.Items.Count; i++)
                {
                    if ((_areaCBox.Items[i] as ComboBoxItem).Tag.ToString() == _currentPlace.Guid)
                    {
                        _areaCBox.UpdateLayout();
                        _areaCBox.SelectedIndex = i;
                    }
                }
            }
            else
            {
                if (_areaCBox.Items.Count > 0)
                {
                    (_areaCBox.Items[0] as ComboBoxItem).IsSelected = true;
                    SystemLoginService.CurrentActivityPlace = (_areaCBox.Items[0] as ComboBoxItem).DataContext as ActivityPlace;
                    _currentPlace = SystemLoginService.CurrentActivityPlace;
                }
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            xCollect.IsChecked = true;
            textBlockArea.Text = CO_IA.Client.Utility.GetAreaName(CO_IA.Client.RiasPortal.Current.SystemConfig.LoginArea.Substring(0, 2)) + CO_IA.Client.Utility.RiasSystemName + "-环境分析";
           
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("确实要关闭系统吗", "关闭系统", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
        /// <summary>
        /// 界面最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        private void xCollect_Checked(object sender, RoutedEventArgs e)
        {
            xGridContianer.Children.Clear();
            //CO_IA.UI.Collection.FreqCollection freqCollection = new CO_IA.UI.Collection.FreqCollection();

            CO_IA.UI.Collection.FreqCollectionNew freqCollection = new CO_IA.UI.Collection.FreqCollectionNew();//add by michael
            xGridContianer.Children.Add(freqCollection);
        }

       

        private void xAppButtonAnalysis_Checked(object sender, RoutedEventArgs e)
        {
            if (dataAanalysis == null)
            {
                dataAanalysis = new UI.Collection.DataAanalysis();
                dataAanalysis.initDataSource(null);
                dataAanalysis.StationHyperlinkClick += dataAanalysis_StationHyperlinkClick;
            }
            xGridContianer.Children.Clear();
            xGridContianer.Children.Add(dataAanalysis);

           //测试代码
           //string freqGruidTest = "35619909-AA2B-4DF9-B03E-2C63FCA50813";
           //dataAanalysis_StationHyperlinkClick(freqGruidTest);
        }

        void dataAanalysis_StationHyperlinkClick(string p_stationGuid)
        {
            if (string.IsNullOrEmpty(p_stationGuid))
                return;
            if (!SystemLoginService.IsLogin)//脱机
            {
                List<StationEmitInfo> listEmitInfo = SQLiteDataService.QueryEmitInfoByStatID(p_stationGuid);
                List<ActivitySurroundStation> listStation = SQLiteDataService.QueryStationBaseInfo(p_stationGuid);
                StationDetailDialog dialog1 = new StationDetailDialog(listStation, listEmitInfo);
                dialog1.ShowDialog(this);
                return;
            }
            StationDetailDialog dialog = new StationDetailDialog(p_stationGuid);
            dialog.ShowDialog(this);
            //List<RoundStationInfo> list = GetRoundStationInfos(p_freqGuid);
            //if (list.Count > 0)
            //{
            //    //AroundStationQueryDialog dialog = new AroundStationQueryDialog(list);
            //    //dialog.ShowDialog(this);
            //}
            //else
            //{
            //    MessageBox.Show("数据库中没有找到频率ID为'" + p_freqGuid + "'的台站信息");
            //}
        }
        //private List<RoundStationInfo> GetRoundStationInfos(string p_freqGuid)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation, List<RoundStationInfo>>(channel =>
        //    {
        //        return channel.QueryRoundStationsByFreq(p_freqGuid);
        //    });
        //}

        private void xDataUpload_Click(object sender, RoutedEventArgs e)
        {
            CO_IA.MonitoringCollecting.Views.DataUpLoadView upLoadView = new CO_IA.MonitoringCollecting.Views.DataUpLoadView();
            upLoadView.Show();
        }

        private void xDataDownload_Click(object sender, RoutedEventArgs e)
        {
            CO_IA.MonitoringCollecting.Views.DataDownLoadView downLoadView = new CO_IA.MonitoringCollecting.Views.DataDownLoadView();
            downLoadView.Show();
        }

        private void AreaCBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox CboxEdit = sender as ComboBox;
            ComboBoxItem item = CboxEdit.SelectedItem as ComboBoxItem;
            if (item == null) return;
            ActivityPlace place = item.DataContext as ActivityPlace;
            if (AreaChanged != null && !string.IsNullOrEmpty(SystemLoginService.CurrentActivityPlace.Guid) && place.Guid != SystemLoginService.CurrentActivityPlace.Guid)
            {
                if (MessageBox.Show("如果更改当前监测区域，当前界面数据将会被丢弃，\r\n 系统将重新加载新页面，是否继续更改当前区域", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SystemLoginService.CurrentActivityPlace = item.DataContext as ActivityPlace;
                    AreaChanged(SystemLoginService.CurrentActivity, SystemLoginService.CurrentActivityPlace);
                }
                else
                {
                    SetAreaCboxSelectedItem();
                    return;
                }
            }
        }

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

    }
}
