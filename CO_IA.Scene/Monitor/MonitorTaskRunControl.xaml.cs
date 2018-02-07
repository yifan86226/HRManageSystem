using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CO_IA.Data;
using CO_IA.Data.Collection;
using CO_IA.Data.MonitorPlan;
using CO_IA.InterferenceAnalysis;
using CO_IA.UI.Collection;
using CO_IA.UI.StationPlan;
using CO_IA_Data;



namespace CO_IA.Scene.Monitor
{
    /// <summary>
    /// MonitorTaskRunControl.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorTaskRunControl : UserControl
    {
        public event StopMonitor StopMonitored;
        //干扰集合
        Dictionary<ActivityEquipmentInfo, List<InterfereResult>> dicInterfereResult = new Dictionary<ActivityEquipmentInfo, List<InterfereResult>>();
        //互调干扰集合
        Dictionary<ActivityEquipmentInfo, List<IMInterfereResult>> dicIMInterfereResult = new Dictionary<ActivityEquipmentInfo, List<IMInterfereResult>>();

        private List<FreqPlanActivity> _freqPlanActivitys; //原来的设备 对应现在的List<ActivityEquipment>
        private List<RoundStationInfo> _roundStationInfos; //原来的周围台站，对应现在的List<ActivitySurroundStation>
        private DetailMonitorPlan _detailMonitorPlan;      //原来的监测预案，对象现在的List<MonitorPlanInfo>
        private CO_IA.UI.Collection.RealTimeMonitor realTimeMonitor;
        public MonitorTaskRunControl(DetailMonitorPlan p_monitorPlan, List<FreqPlanActivity> p_freqPlanActivitys, List<RoundStationInfo> p_roundStationInfos)
        {
            InitializeComponent();
            this._detailMonitorPlan = p_monitorPlan;
            this._freqPlanActivitys = p_freqPlanActivitys;
            this._roundStationInfos = p_roundStationInfos;
            
            initGrid();
        }

        private void initGrid()
        {
            grid_monitor.Children.Clear();
            #region 构造数据，将来删除
            //if (_freqPlanActivitys.Count > 0)
            //{
            //    foreach (FreqPlanActivity freqPlan in _freqPlanActivitys)
            //    {
            //        if (freqPlan.ApplyEquipments.Count == 0)
            //        {
            //            freqPlan.ApplyEquipments.AddRange(GetActivityEquipmentInfo1(freqPlan));
            //        }
            //    }
            //}
            //if (_roundStationInfos.Count == 0)
            //{
            //    _roundStationInfos.AddRange(GetRoundStationInfo());
            //}
            #endregion
            
            var acticity = SystemLoginService.CurrentActivity;
            var place = SystemLoginService.CurrentActivityPlace;
            realTimeMonitor = new CO_IA.UI.Collection.RealTimeMonitor(_detailMonitorPlan, _freqPlanActivitys, _roundStationInfos,
                realTimeMonitor_AfterMonitor,
                acticity,
                place);
            grid_monitor.Children.Add(realTimeMonitor);
        }

        public void StopMono()
        {
            if (realTimeMonitor != null)
            {
                realTimeMonitor.StopMono();
                realTimeMonitor = null;
            }
        }

        //public delegate void DeleFunc();
        //public void Func()
        //{
        //    CO_IA.UI.Collection.RealTimeMonitor realTimeMonitor = new CO_IA.UI.Collection.RealTimeMonitor(_freqPlanActivitys, _roundStationInfos);
        //    realTimeMonitor.AfterMonitor += realTimeMonitor_AfterMonitor;
        //    grid_monitor.Children.Add(realTimeMonitor);
        //}
        //public void TestMethod(object data)
        //{
        //    System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
        //    new DeleFunc(Func));
        //}

        /// <summary>
        /// 监测扫描后，执行计算干扰方法
        /// </summary>
        /// <param name="matchSignals">发送中的匹配信号</param>
        /// <param name="unMatchSignals">未发送的匹配信号</param>
        /// <param name="unknowSignals">未匹配上的信号</param>
        void realTimeMonitor_AfterMonitor(List<AnalysisResult> matchSignals, List<AnalysisResult> unMatchSignals, List<AnalysisResult> unknowSignals)
        {
            if (_isPause == false)
            {
                //_freqPlanActivitys
                //_roundStationInfos
                List<ActivityEquipmentInfo> equipList = new List<ActivityEquipmentInfo>();
                List<RoundStationInfo> statList = new List<RoundStationInfo>();
                //扫描每隔5秒返回一次
                //匹配的
                foreach (AnalysisResult analysisResult in matchSignals)
                {
                    ActivityEquipmentInfo equip = GetEquipment(analysisResult.FreqGuid);
                    if (equip != null)
                        equipList.Add(equip);
                    RoundStationInfo stat = GetRoundStation(analysisResult.FreqGuid);
                    if (stat != null)
                        statList.Add(stat);
                }
                //未匹配的计算
                foreach (AnalysisResult analysisResult in unMatchSignals)
                {
                    ActivityEquipmentInfo equip = GetEquipment(analysisResult.FreqGuid);
                    if (equip != null)
                        equipList.Add(equip);
                    RoundStationInfo stat = GetRoundStation(analysisResult.FreqGuid);
                    if (stat != null)
                        statList.Add(stat);
                }
                //未知信号
                //foreach (AnalysisResult analysisResult in unknowSignals)
                //{
                    
                //}

                CalculateRealize(statList, equipList, unknowSignals);
            }
            
        }
        private ActivityEquipmentInfo GetEquipment(string p_freqGuid)
        {
            foreach (FreqPlanActivity freqPlan in _freqPlanActivitys)
            {
                foreach (ActivityEquipmentInfo equip in freqPlan.ApplyEquipments)
                {
                    if (equip.GUID == p_freqGuid)
                    {
                        return equip;
                    }
                }
            }
            return null;
        }
        private RoundStationInfo GetRoundStation(string p_freqGuid)
        {
            foreach (RoundStationInfo stat in _roundStationInfos)
            {
                foreach (FreqEmitInfo emit in stat.EmitInfos)
                {
                    if (emit.Guid == p_freqGuid)
                    {
                        return stat;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 调用干扰计算方法
        /// </summary>
        /// <param name="p_roundStatList"></param>
        /// <param name="p_equList"></param>
        private void CalculateRealize(List<RoundStationInfo> p_roundStatList, List<ActivityEquipmentInfo> p_equList, List<AnalysisResult> p_IllegalSignal)
        {
            InterferedCalculateManage interferedCalculateManage = new InterferedCalculateManage(p_equList, p_roundStatList, AnalysisType.SameFreq | AnalysisType.ADJFreq | AnalysisType.IM, p_IllegalSignal);
            InterferedResult.ItemsSource = interferedCalculateManage.GetInterferedAllEquipments();
            dicInterfereResult = interferedCalculateManage.DicInterfereResult;
            dicIMInterfereResult = interferedCalculateManage.DicIMInterfereResult;
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
            if (StopMonitored != null)
            {
                StopMonitored(false);
            }
        }

        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            if (StopMonitored != null)
            {
                //StopMono();
                StopMonitored(true);
            }
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }

        private T FindFirstVisualChild<T>(DependencyObject obj, string childName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && child.GetValue(NameProperty).ToString() == childName)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindFirstVisualChild<T>(child, childName);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        #region 假数据源
        /// <summary>
        /// 计算干扰的测试方法，最后需要去掉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculatorTest_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            List<RoundStationInfo> roundStatList = new List<RoundStationInfo>();

            for (int i = 0; i < _roundStationInfos.Count; i++)
            {
                roundStatList.Add(_roundStationInfos[i]);
            }
            if (roundStatList.Count == 2 && roundStatList[0].EmitInfos.Count > 0 && roundStatList[1].EmitInfos.Count > 2)
            {
                roundStatList[0].EmitInfos[0].FreqEC = 15; //1   MHz   
                roundStatList[0].EmitInfos[0].FreqBand = 2000;   //kHz

                roundStatList[1].EmitInfos[0].FreqEC = 15; //2
                roundStatList[1].EmitInfos[0].FreqBand = 200;

                roundStatList[1].EmitInfos[1].FreqEC = 13;  //3
                roundStatList[1].EmitInfos[1].FreqBand = 1000;

                roundStatList[1].EmitInfos[2].FreqEC = 2;  //4
                roundStatList[1].EmitInfos[2].FreqBand = 100;
                //1和2同频干扰 ，1和3邻频干扰，3+4=1 二阶互调干扰。3+4=2 二阶互调干扰
            }

            List<ActivityEquipmentInfo> equList = new List<ActivityEquipmentInfo>();
            foreach (var activity in _freqPlanActivitys)
            {
                equList.AddRange(activity.ApplyEquipments);
            }
            if (equList.Count > 3)
            {
                equList[0].AssignFreq = 67; //1
                equList[0].Band = 250;

                equList[1].AssignFreq = 67; //2
                equList[1].Band = 500;

                equList[2].AssignFreq = 66.5; //3
                equList[2].Band = 250;

                equList[3].AssignFreq = 68;  //4
                equList[3].Band = 250;
                // 1和2同频干扰  2和3邻频干扰  1和3和4是五阶互调干扰 2和3和4是五阶互调干扰
            }
            //CalculateRealize(roundStatList, equList);
        }

        private List<ActivityEquipmentInfo> GetActivityEquipmentInfo1(FreqPlanActivity p_freqPlan)
        {
            EquipmentQueryCondition equipCondition = new EquipmentQueryCondition();
            equipCondition.ActivityGuid = "344BF3F8C4DF4CCCB7DCC341DC8EEBB0";//p_freqPlan.ActivityId;
            equipCondition.IsMobile = true;
            //equipCondition.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid;
            return DataOperator.GetTaskListInfosByParam(equipCondition);
        }

        private List<RoundStationInfo> GetRoundStationInfo()
        {
            List<RoundStationInfo> statList = new List<RoundStationInfo>();
            RoundStationInfo stat1 = new RoundStationInfo();
            //stat1.FreqActivity = new FreqPlanActivity();
            stat1.EmitInfos = new List<FreqEmitInfo>();
            List<EmitInfo> list1 = DataOperator.GetEmitInfo("CB161486E97E49E69E38DA7E8D065CD0");
            stat1.EmitInfos = GetFreqEmitInfoList(list1);
            RoundStationInfo stat2 = new RoundStationInfo();
            //stat2.FreqActivity = new FreqPlanActivity();
            stat2.EmitInfos = new List<FreqEmitInfo>();
            List<EmitInfo> list2 = DataOperator.GetEmitInfo("0aed5de4ebf8434e8bba78fede459c33");
            stat2.EmitInfos = GetFreqEmitInfoList(list2);
            statList.Add(stat1);
            statList.Add(stat2);
            return statList;
        }
        private List<FreqEmitInfo> GetFreqEmitInfoList(List<EmitInfo> list)
        {
            List<FreqEmitInfo> freqList = new List<FreqEmitInfo>();
            foreach (EmitInfo emit in list)
            {
                FreqEmitInfo freq = new FreqEmitInfo();
                freq.AntEgain = emit.AntEgain;
                freq.AntHight = emit.AntHeight;
                freq.AntPole = emit.AntPole.ToString();
                freq.EquPow = emit.EquPower;
                freq.FeedLose = emit.FeedLose;
                freq.FreqBand = emit.FreqBand;
                freq.FreqEC = emit.FreqEc;
                freq.FreqMod = emit.FreqMod;
                freq.FreqRC = emit.FreqRc;
                freq.Guid = emit.Guid;
                freq.PlaceGuid = emit.PlaceGuid;
                freq.RCVAntEgain = emit.RcvAntEgain;
                freq.RCVAntHight = emit.RcvAntHeight;
                freq.RCVFeedLose = emit.RcvFeedLose;
                freq.StatAT = emit.StatAt;
                freq.StationGuid = emit.StationGuid;
                freqList.Add(freq);

            }
            return freqList;
        }
        #endregion

        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
            if (dgr != null)
            {
                ActivityEquipmentInfo equip = dgr.DataContext as ActivityEquipmentInfo;
                InterferedDetailInfoDialog dialog = new InterferedDetailInfoDialog(equip, dicInterfereResult, dicIMInterfereResult);
                dialog.Show();
            }
        }
        /// <summary>
        /// 暂停
        /// </summary>
        private bool _isPause = false;
        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt.Content != null && bt.Content.ToString() == "暂停")
            {
                bt.Content = "继续";
                _isPause = true;
            }
            else
            {
                bt.Content = "暂停";
                _isPause = false;
            }
        }

    }

    public delegate void StopMonitor(bool finished);

    public class InterfereResultEquipment
    {
        private InterfereTypeEnum _interfereType = InterfereTypeEnum.无干扰;
        private string _stationOrg;
        private string _equipmentName;
        private int _equipmentCount;
        private double _sendPower;
        private double _assignFreq;

        public InterfereTypeEnum InterfereType
        {
            get { return _interfereType; }
            set { _interfereType = value; }
        }

        public string StationOrg
        {
            get { return _stationOrg; }
            set { _stationOrg = value; }
        }

        public string EquipmentName
        {
            get { return _equipmentName; }
            set { _equipmentName = value; }
        }

        public int EquipmentCount
        {
            get { return _equipmentCount; }
            set { _equipmentCount = value; }
        }

        public double SendPower
        {
            get { return _sendPower; }
            set { _sendPower = value; }
        }

        public double AssignFreq
        {
            get { return _assignFreq; }
            set { _assignFreq = value; }
        }

    }

}
