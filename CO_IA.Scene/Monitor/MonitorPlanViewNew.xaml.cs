using AT_BC.Common;
using CO_IA.Data;
using CO_IA_Data;
using CO_IA_Data.StationManage;
using I_CO_IA.FreqStation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.Scene.Monitor
{
    /// <summary>
    /// MonitorPlanViewNew.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorPlanViewNew : UserControl
    {
        private List<ActivitySurroundStation> _surroundStations;
        private CheckBox checkBoxAll;

        private MonitorExcuteParam _monitorParam = new MonitorExcuteParam();
        /// <summary>
        /// 提供给监测端的监测参数
        /// </summary>
        public MonitorExcuteParam MonitorParam
        {
            get
            {
                _monitorParam.MonitorPlanInfos = (dataGridMonitorPlan.ItemsSource as ObservableCollection<MonitorPlanInfo>).Where(p => p.IsChecked).ToList();
                foreach (MonitorPlanInfo info in _monitorParam.MonitorPlanInfos)
                {
                    _monitorParam.ActivitySurroundStations.AddRange(GetAroundStationsByFreqRange(info.MHzFreqFrom, info.MHzFreqTo));
                    _monitorParam.ActivityEquipments.AddRange(GetActivityEquipmentByFreqRange(info.MHzFreqFrom, info.MHzFreqTo));
                }
                return _monitorParam;
            }
        }
       
        public MonitorPlanViewNew()
        {
            InitializeComponent();
            _surroundStations = DataOperator.GetAroundStations();
            LoadMonitorFreqInfos();
        }


        private void LoadMonitorFreqInfos()
        {
            MonitorPlanInfo[] monitorPlans = DataOperator.GetMonitorFreqInfos(SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid);
            if (monitorPlans == null || monitorPlans.Length == 0)
            {
                this.dataGridMonitorPlan.ItemsSource = new ObservableCollection<MonitorPlanInfo>();
            }
            else
            {
                this.dataGridMonitorPlan.ItemsSource = new ObservableCollection<MonitorPlanInfo>(monitorPlans);
            }
        }

        private void dataGridMonitorPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MonitorPlanInfo monitorPlanInfo = (sender as DataGrid).SelectedItem as MonitorPlanInfo;
            stationdatagrid.ItemsSource = GetAroundStationsByFreqRange(monitorPlanInfo.MHzFreqFrom, monitorPlanInfo.MHzFreqTo);

            equdatagrid.ItemsSource = GetActivityEquipmentByFreqRange(monitorPlanInfo.MHzFreqFrom, monitorPlanInfo.MHzFreqTo);
        }
        private ActivityEquipment[] GetActivityEquipmentByFreqRange(double p_freqStart, double p_freqEnd)
        {
            EquInspectionQueryCondition condition = new EquInspectionQueryCondition();
            condition.CheckState = (new CheckStateEnum[] { CheckStateEnum.Qualified }).ToList();
            condition.ActivityGuid  = SystemLoginService.CurrentActivity.Guid;
            condition.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid;
            condition.SendFreqLittle = p_freqStart;
            condition.SendFreqGreat = p_freqEnd;
            return DataOperator.GetEquipmentInspections(condition).Select(p => p.ActivityEquipment).ToArray();


            //EquipmentLoadStrategy strategy = GetEquipmentLoadStrategy(p_freqStart, p_freqEnd);
            //return DataOperator.GetEquipments(strategy);
        }
        EquipmentLoadStrategy GetEquipmentLoadStrategy(double p_freqStart, double p_freqEnd)
        {
            return new EquipmentLoadStrategy()
            {
                ActivityGuid = SystemLoginService.CurrentActivity.Guid,
                PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid,
                FreqRange = new AT_BC.Data.Range<double?>() 
                { 
                    Great = p_freqEnd, 
                    Little = p_freqStart 
                }
            };
        }

        private void checkBoxAll_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxAll = sender as CheckBox;
        }
        private List<ActivitySurroundStation> GetAroundStationsByFreqRange(double p_freqStart,double p_freqEnd)
        {
            List<ActivitySurroundStation> stationList = new List<ActivitySurroundStation>();
            foreach(ActivitySurroundStation station in _surroundStations)
            {
               foreach(StationEmitInfo emit in station.EmitInfo)
               {
                   if (emit.FreqEC != null && emit.FreqEC >= p_freqStart && emit.FreqEC <= p_freqEnd)
                   {
                       stationList.Add(station);
                   }
                   else if (emit.FreqEFB>= p_freqStart && emit.FreqEFE <= p_freqEnd && emit.FreqEFB <= emit.FreqEFE)
                   {
                       stationList.Add(station);
                   }
               }
            }
            return stationList;    
        }

        
        public static Organization[] Orgs
        {
            get
            {
                return DataOperator.GetORGSource(new OrgQueryCondition() { ActivityGuid = SystemLoginService.CurrentActivity.Guid });
            }
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExcuteMonitorBtn_Click(object sender, RoutedEventArgs e)
        {
            _toolBar.Visibility = System.Windows.Visibility.Collapsed;
            MonitorTaskRunControlNew taskRunControl = new MonitorTaskRunControlNew(MonitorParam);
            this.gridMonitorRunningContainer.Children.Clear();
            this.gridMonitorRunningContainer.Children.Add(taskRunControl);
            taskRunControl.StopMonitored += bFinished =>
            {
                _toolBar.Visibility = System.Windows.Visibility.Visible;
                if (bFinished)
                {
                    taskRunControl.StopMono();
                    taskRunControl = null;
                    this.gridMonitorRunningContainer.Children.Clear();
                }
                this.gridMonitorRunningContainer.Visibility = System.Windows.Visibility.Collapsed;
            };
            this.gridMonitorRunningContainer.Visibility = System.Windows.Visibility.Visible;
        }

    }
}
