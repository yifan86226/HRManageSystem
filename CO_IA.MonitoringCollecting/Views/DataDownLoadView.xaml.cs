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
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Data.Collection;
using CO_IA.UI.Collection.DbEntity;
using I_CO_IA.Collection;
using I_CO_IA.FreqStation;
using CO_IA_Data;
using I_CO_IA.Setting;

namespace CO_IA.MonitoringCollecting.Views
{
    /// <summary>
    /// DataDownLoadView.xaml 的交互逻辑
    /// </summary>
    public partial class DataDownLoadView : Window
    {
        //private string _currentAreaGuid = "CB161486E97E49E69E38DA7E8D065CD0";
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        public DataDownLoadView()
        {
            InitializeComponent();
        }
        private void BeginDownLoad()
        {
            _progressBar.Maximum = 4;
            _progressBar.Value = 0;
            OperateLog log = new OperateLog();
            log.Guid =CO_IA.Client.Utility.NewGuid();
            log.Operater = RiasPortal.Current.UserSetting.UserName;
            log.OperateDate = DateTime.Now;
            log.OperateType = OperateTypeEnum.DownLoad;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(_progressBar.SetValue);
            SQLiteDataService.Transaction = SQLiteConnect.GetSQLiteTransaction(SystemLoginService.CurrentActivity.Name);
            try
            {
                SQLiteDataService.DeleteStationBase(SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid);
                SQLiteDataService.DeleteFreqPlan(SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid);
                SQLiteDataService.DeletePlace(SystemLoginService.CurrentActivity.Guid);
                for (int i = 0; i < 4; i++)
                {
                    if (i == 0 && _statDownLoad.IsChecked == true)
                    {
                        _downLoadInfo.Text = "正在下载周围台站数据";
                        DownLoadStationBase();
                        log.OperateTables += "ACTIVITY_PLACE_STATION,";
                    }
                    //else if (i == 1 && _emitDownLoad.IsChecked == true)
                    //{
                    //    _downLoadInfo.Text = "正在下载设备信息数据";
                    //    DownLoadEquipInfo();
                    //    log.OperateTables += "ACTIVITY_STATION_EMIT,";
                    //}
                   
                    else if (i == 2 && _areaDownLoad.IsChecked == true)
                    {
                        _downLoadInfo.Text = "正在下载活动区域数据";
                        DownLoadActivityPlace();
                        log.OperateTables += "ACTIVITY_PLACE,ACTIVITY_PLACE_LOCATION,";

                    }
                    else if (i == 3 && _freqRangeDownLoad.IsChecked == true)
                    {
                        _downLoadInfo.Text = "正在下载频率预案数据";
                        DownLoadFreqPlan();
                        log.OperateTables += "activity_place_freq_plan, rias_equipment_freqplanning,";

                    }
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(i + 1) });
                }
                SQLiteDataService.SaveOperaterLog(log);
                
                SQLiteDataService.Transaction.Commit();
                _downLoadInfo.Text = "下载完成";

            }
            catch (Exception ex)
            {
                SQLiteDataService.Transaction.Rollback();
                Console.WriteLine(ex.Message);
                _downLoadInfo.Text = "下载失败";
                MessageBox.Show("失败原因：\r\n" + ex.Message);
            }
        }

        private void DownLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            BeginDownLoad();
        }

        /// <summary>
        /// 下载周围台站
        /// </summary>
        private void DownLoadStationBase()
        {
            List<ActivitySurroundStation> stationList = GetAroundStations();
            stationList.ForEach(p => p.EmitInfo.ForEach(q => q.FreqBand = q.FreqBand * 1000));
            SaveStationBaseToSqLite(stationList);
        }
        //private void DownLoadEquipInfo()
        //{
        //    List<ActivityEquipment> equipments = GetEquipments();
        //     SaveEquipmentToSqLite(equipments);
        //}

        /// <summary>
        /// 下载活动信息
        /// </summary>
        //private void DownLoadActivity()
        //{
        //    List<Activity> activityList = GetActivitys();
        //    SaveActivityToSqLite(activityList);
        //}
        /// <summary>
        /// 下载活动地点
        /// </summary>
        private void DownLoadActivityPlace()
        {
           
            List<ActivityPlaceInfo> placeList = GetActivityPlaces();
            //List<ActivityPlaceLocation> placeLocationList = GetActivityPlaceLocations();
            SaveActivityPlaceToSqLite(placeList);
            //SaveActivityPlaceLocationToSqLite(placeLocationList);
        }
        /// <summary>
        /// 下载频率预案
        /// </summary>
        private void DownLoadFreqPlan()
        {
            List<PlaceFreqPlan> freqList = GetPlaceFreqPlans();
            SavePlaceFreqPlan(freqList);
        }

        #region SQlite
        private void SaveStationBaseToSqLite(List<ActivitySurroundStation> p_statList)
        {
            foreach (ActivitySurroundStation stationBase in p_statList)
            {
                SQLiteDataService.DeleteEmitInfo(stationBase.STATGUID, "");
                SQLiteDataService.SaveStationBase(stationBase); 
            }
        }

        //private void SaveEmitInfoToSqLite(List<StationEmitInfo> p_emitList)
        //{
        //    foreach (StationEmitInfo emitInfo in p_emitList)
        //    {
        //        SQLiteDataService.SaveEmitInfo(emitInfo);
        //    }
        //}
        private void SaveActivityToSqLite(List<Activity> activityList)
        {
            foreach (Activity activity in activityList)
            {
                SQLiteDataService.SaveActivity(activity);
            }
        }

        private void SaveActivityPlaceToSqLite(List<ActivityPlaceInfo> placeList)
        {
            foreach (ActivityPlaceInfo place in placeList)
            {
                SQLiteDataService.DeletePlaceLocation(place.Guid);
                SQLiteDataService.SaveActivityPlace(place);
            }
        }

        private void SaveActivityPlaceLocationToSqLite(List<ActivityPlaceLocation> placeLocationList)
        {
            foreach (ActivityPlaceLocation location in placeLocationList)
            {
                SQLiteDataService.SaveActivityPlaceLocation(location);
            }
        }

        private void SavePlaceFreqPlan(List<PlaceFreqPlan> freqList)
        {
            foreach(PlaceFreqPlan freq in freqList)
            {
                SQLiteDataService.SaveFreqPlan(freq);
            }
        }
        #endregion

        #region Oracle
        private List<ActivitySurroundStation> GetAroundStations()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<ActivitySurroundStation>>(channel =>
            {
                return channel.GetActivitySurroundStations(SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid,null);
            });
        }

        private List<ActivityPlaceInfo> GetActivityPlaces()
        {
            return Utility.GetPlacesByActivityId(SystemLoginService.CurrentActivity.Guid).ToList();   
        }
        private List<PlaceFreqPlan> GetPlaceFreqPlans()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<PlaceFreqPlan>>(channel =>
            {
                return channel.GetPlaceFreqPlans(SystemLoginService.CurrentActivityPlace.Guid).ToList();
            });
        }
        private List<ActivityPlaceLocation> GetActivityPlaceLocations()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<ActivityPlaceLocation>>(channel =>
            {
                return channel.QueryPlaceLocation(SystemLoginService.CurrentActivity.Guid);
            });
        }
        #endregion



        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
