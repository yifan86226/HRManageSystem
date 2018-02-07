using CO_IA.Data;
using CO_IA.Data.Collection;
using CO_IA.InterferenceAnalysis;
using CO_IA_Data;
using EMCS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
using System.Windows.Threading;

namespace CO_IA.Scene.Monitor
{
    /// <summary>
    /// MonitorTaskRunControlNew.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorTaskRunControlNew : UserControl
    {
        private CO_IA_Data.StationManage.MonitorExcuteParam MonitorParam;
        public event StopMonitor StopMonitored;
       
        //private CO_IA.UI.Collection.RealTimeMonitor realTimeMonitor;
        private CO_IA.UI.Collection.RealTimeMonitorNew realTimeMonitor;

        /// <summary>
        /// 用于计算的所有频率集合
        /// </summary>
        private List<ComparableFreq> _calculateAllFreqs = new List<ComparableFreq>();
        private List<ComparableFreq> _calculateAllRCFreqs = new List<ComparableFreq>();
        private List<ComparableFreq> _unKnowSignalFreqs = new List<ComparableFreq>();
        public MonitorTaskRunControlNew()
        {
            InitializeComponent();
        }

        public MonitorTaskRunControlNew(CO_IA_Data.StationManage.MonitorExcuteParam MonitorParam)
            :this()
        {
            // TODO: Complete member initialization
            this.MonitorParam = MonitorParam;
            InitGrid();
        }

        private void InitGrid()
        {
            grid_monitor.Children.Clear();
            realTimeMonitor = new CO_IA.UI.Collection.RealTimeMonitorNew(SystemLoginService.UserOrgInfo.GUID, MonitorParam.MonitorPlanInfos, MonitorParam.ActivityEquipments, MonitorParam.ActivitySurroundStations,
              realTimeMonitor_AfterMonitor,
              SystemLoginService.CurrentActivity,
              SystemLoginService.CurrentActivityPlace);
            LoadCalculateFreqs();
            //end
           
            grid_monitor.Children.Add(realTimeMonitor);
        }

        private void LoadCalculateFreqs()
        {
            foreach(var equip in MonitorParam.ActivityEquipments)
            {
                _calculateAllFreqs.Add(new ComparableFreq(equip.AssignSendFreq == null ? 0 : (double)equip.AssignSendFreq, equip.AssignSpareFreq == null ? 0 : (double)equip.AssignSpareFreq, equip.Band_kHz == null ? 0 : (double)equip.Band_kHz, equip.Key.ToString()));
                _calculateAllRCFreqs.Add(new ComparableFreq(equip.ReceiveFreq == null ? 0 : (double)equip.ReceiveFreq, equip.AssignSpareFreq == null ? 0 : (double)equip.AssignSpareFreq, equip.Band_kHz == null ? 0 : (double)equip.Band_kHz, equip.Key.ToString()));
            }
            foreach(var station in MonitorParam.ActivitySurroundStations)
            {
                foreach(var emit in station.EmitInfo)
                {
                    _calculateAllFreqs.Add(new ComparableFreq(emit.FreqEC == null ? 0 : (double)emit.FreqEC, 0, emit.FreqBand, emit.Guid));
                    _calculateAllRCFreqs.Add(new ComparableFreq(emit.FreqRC == null ? 0 : (double)emit.FreqRC, 0, emit.FreqBand, emit.Guid));
                }
            }
        }
       
         /// <summary>
        /// 暂停
        /// </summary>
        private bool _isPause = false;
        void realTimeMonitor_AfterMonitor(List<AnalysisResult> matchSignals, List<AnalysisResult> unMatchSignals, List<AnalysisResult> unknowSignals)
        {
            BdWidthMHzTokHzConvert(matchSignals);
            BdWidthMHzTokHzConvert(unMatchSignals);
            BdWidthMHzTokHzConvert(unknowSignals);
            //异步调用计算方法，否则界面卡顿现象严重
            Action<List<AnalysisResult>, List<AnalysisResult>, List<AnalysisResult>,bool> asyncAction = LoadCalculateResult;
            Action<IAsyncResult> resultHandler = delegate(IAsyncResult asyncResult)
            {
                asyncAction.EndInvoke(asyncResult);
            };
            AsyncCallback asyncActionCallback = delegate(IAsyncResult asyncResult)
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, resultHandler, asyncResult);
            };
            asyncAction.BeginInvoke(matchSignals, unMatchSignals, unknowSignals, (bool)xUsedDataCheck.IsChecked, asyncActionCallback, null);

        }
        /// <summary>
        /// 将带宽转为MHz，用于计算。
        /// </summary>
        /// <param name="p_analysisResults"></param>
        void BdWidthMHzTokHzConvert(List<AnalysisResult> p_analysisResults)
        {
            p_analysisResults.ForEach(p => p.BandWidth = p.BandWidth / 1000);          
        }
        /// <summary>
        /// 计算的实现
        /// </summary>
        /// <param name="p_calculateAllFreqs"></param>
        /// <param name="p_calculateAllRCFreqs"></param>
        /// <param name="p_unKnowSignalFreqs"></param>
        private void CalculateRealize(List<ComparableFreq> p_calculateAllFreqs, List<ComparableFreq> p_calculateAllRCFreqs, List<ComparableFreq> p_unKnowSignalFreqs)
        {
            #region
            //if(p_unKnowSignalFreqs.Count > 15)
            //{ 
            //    //发 106.5 ，96.8 收 106 ， 96.8
            //    p_unKnowSignalFreqs[0] = new ComparableFreq(106.4, 0, 0, 0.5, ""); //同频
            //    p_unKnowSignalFreqs[1] = new ComparableFreq(96.8, 0, 0, 0.200, "");  //同频

            //    p_unKnowSignalFreqs[2] = new ComparableFreq(105.5, 0, 0, 0.400, ""); //邻频
            //    p_unKnowSignalFreqs[3] = new ComparableFreq(107.5, 0, 0, 0.400, ""); //邻频
            //    p_unKnowSignalFreqs[4] = new ComparableFreq(95.8, 0, 0, 0.400, "");  //邻频
            //    p_unKnowSignalFreqs[5] = new ComparableFreq(97.8, 0, 0, 0.400, "");  //邻频

            //    p_unKnowSignalFreqs[6] = new ComparableFreq(206, 0, 0, 0.400, "");  //二阶互调
            //    p_unKnowSignalFreqs[7] = new ComparableFreq(100, 0, 0, 0.400, "");  //二阶互调
            //    p_unKnowSignalFreqs[8] = new ComparableFreq(50, 0, 0, 0.400, "");   //二阶互调
            //    p_unKnowSignalFreqs[9] = new ComparableFreq(56, 0, 0, 0.400, "");   //二阶互调

            //    p_unKnowSignalFreqs[10] = new ComparableFreq(90, 0, 0, 0.400, "");     //三阶互调
            //    p_unKnowSignalFreqs[11] = new ComparableFreq(106.8, 0, 0, 0.400, "");  //三阶互调
            //    p_unKnowSignalFreqs[12] = new ComparableFreq(100, 0, 0, 0.400, "");    //三阶互调

            //    p_unKnowSignalFreqs[13] = new ComparableFreq(95, 0, 0, 0.400, "");     //五阶互调
            //    p_unKnowSignalFreqs[14] = new ComparableFreq(94.1, 0, 0, 0.400, "");   //五阶互调
            //}
            #endregion



            CompareResultToBindingData bindingData = new CompareResultToBindingData(GetEquipment, GetSurroundStation);
            //同频干扰
            List<InterferedBindingData> gridSource = new List<InterferedBindingData>();
            SameFreqCalculator samefreqCalc = new SameFreqCalculator(p_calculateAllFreqs.ToArray(), p_unKnowSignalFreqs.ToArray());
            SameFreqCompareResult[] sameResults = samefreqCalc.CalcSameFreqInterference();
            if (sameResults != null && sameResults.Length >0)
            {
                foreach(var sameResult in sameResults)
                {
                    List<InterferedBindingData> sameFreqlist = bindingData.GetBindingData(sameResult);
                    gridSource.AddRange(sameFreqlist);
                }
            }
           
            //邻频干扰
            AdjFreqCalculator adjfreqCalc = new AdjFreqCalculator(p_calculateAllFreqs.ToArray(), p_unKnowSignalFreqs.ToArray());
            AdjFreqCompareResult[] adjInterfResult = adjfreqCalc.CalcAdjFreqInterference();
            if (adjInterfResult != null && adjInterfResult.Length >0)
            {
                List<InterferedBindingData> adjFreqlist = bindingData.GetBindingData(adjInterfResult);
                gridSource.AddRange(adjFreqlist);
            }
            //互调干扰
            IMCalculator calculator = new IMCalculator(GetTransmitter(p_unKnowSignalFreqs), new Receiver[0], GetTransmitter(p_calculateAllFreqs), GetReceivers(p_calculateAllRCFreqs) );
            IMOrder order = IMOrder.Second | IMOrder.Third | IMOrder.Fifth;
            IMCompareResult imInterfResult = calculator.CalcReceiverIMInterference(order);
            if(imInterfResult!=null)
            {
                List<InterferedBindingData> imFreqlist = bindingData.GetBindingData(imInterfResult);
                gridSource.AddRange(imFreqlist);
            }
            ItemsSourceBinding(gridSource);
            
        }

        private void ItemsSourceBinding(List<InterferedBindingData> gridSource)
        {
            //显示控件跨线程
            Action action = () =>
            {
                InterferedResult.ItemsSource = null;
                InterferedResult.ItemsSource = gridSource;
            };
            if (System.Threading.Thread.CurrentThread != InterferedResult.Dispatcher.Thread)
            {
                InterferedResult.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, action);
            }
            else
            {
                action();
            }
        }
        /// <summary>
        /// 处理结算结果
        /// </summary>
        /// <param name="matchSignals">匹配到的数据</param>
        /// <param name="unMatchSignals">没匹配到的数据</param>
        /// <param name="unknowSignals">未知信号</param>
        /// <param name="isUsedData">是否使用全部数据进行计算，因为跨线程问题，所以使用参数传递</param>
        void LoadCalculateResult(List<AnalysisResult> matchSignals, List<AnalysisResult> unMatchSignals, List<AnalysisResult> unknowSignals, bool isUsedAllData)
        {
            if (_isPause == false)
            {
                List<ComparableFreq> matchFreqList = new List<ComparableFreq>();
                List<ComparableFreq> matchRCFreqList = new List<ComparableFreq>();
                List<ComparableFreq> unMatchFreqList = new List<ComparableFreq>();
                List<ComparableFreq> unMatchRCFreqList = new List<ComparableFreq>();
                //扫描每隔5秒返回一次
                //匹配的
                foreach (AnalysisResult analysisResult in matchSignals)
                {
                    LoadEquipFreq(matchFreqList, matchRCFreqList, analysisResult.FreqGuid);

                    ActivitySurroundStation stat = GetSurroundStation(analysisResult.FreqGuid);
                    if (stat != null)
                    {
                        LoadStatFreq(matchFreqList, matchRCFreqList, stat.STATGUID);
                    }
                }
                //未匹配的计算
                foreach (AnalysisResult analysisResult in unMatchSignals)
                {
                    LoadEquipFreq(unMatchFreqList, unMatchRCFreqList, analysisResult.FreqGuid);

                    ActivitySurroundStation stat = GetSurroundStation(analysisResult.FreqGuid);
                    if (stat != null)
                    {
                        LoadStatFreq(unMatchFreqList, unMatchRCFreqList, stat.STATGUID);
                    }
                }
                //未知信号
                LoadUnKnowSignalFreqs(unknowSignals);

                if (isUsedAllData == false)
                {
                    CalculateRealize(matchFreqList, matchRCFreqList, _unKnowSignalFreqs);
                }
                else
                {
                    CalculateRealize(_calculateAllFreqs, _calculateAllRCFreqs, _unKnowSignalFreqs);
                }

            }
        }

        Receiver[] GetReceivers(List<ComparableFreq> p_comparableFreqs)
        {
            Receiver[] receivers = new Receiver[p_comparableFreqs.Count];
            for (int i = 0; i < p_comparableFreqs.Count; i++)
            {
                if(i<receivers.Length)//by xgh                
                    receivers[i] = new Receiver(p_comparableFreqs[i].FreqID, p_comparableFreqs[i], new
                    ReceiverParams() { IFBand = new EMCFreqValue(EMCFreqUnitEnum.KHz, p_comparableFreqs[i].Band) }, null);
            }
            return receivers;
        }

        Transmitter[] GetTransmitter(List<ComparableFreq> p_comparableFreqs)
        {
            Transmitter[] transmitters = new Transmitter[p_comparableFreqs.Count];
            for (int i = 0; i < p_comparableFreqs.Count; i++)
            {
                if(i<transmitters.Length)// by xgh
                transmitters[i] = new Transmitter(p_comparableFreqs[i].FreqID, p_comparableFreqs[i], new TransmitterParams() { Band = new EMCFreqValue(EMCFreqUnitEnum.KHz, p_comparableFreqs[i].Band) }, null);
            }
            return transmitters;
        }

       

       

        private void LoadUnKnowSignalFreqs(List<AnalysisResult> unknowSignals)
        {
            foreach(AnalysisResult rst in unknowSignals)
            {
                if(string.IsNullOrEmpty(rst.Id))
                {
                    rst.Id = Guid.NewGuid().ToString();
                }
                _unKnowSignalFreqs.Add(new ComparableFreq(rst.Frequency,0,rst.BandWidth,rst.Id));
            }
        }

        void LoadEquipFreq(List<ComparableFreq> p_freqList, List<ComparableFreq> p_rcfreqList, string p_freqID)
        {
            ActivityEquipment equip = GetEquipment(p_freqID);
            if (equip != null)
            {
                 p_freqList.Add(new ComparableFreq(equip.AssignSendFreq == null ? 0 : (double)equip.AssignSendFreq, equip.AssignSpareFreq == null ? 0 : (double)equip.AssignSpareFreq, equip.Band_kHz == null ? 0 : (double)equip.Band_kHz, equip.Key.ToString()));
                 p_rcfreqList.Add(new ComparableFreq(equip.ReceiveFreq == null ? 0 : (double)equip.ReceiveFreq, 0, equip.Band_kHz == null ? 0 : (double)equip.Band_kHz, equip.Key.ToString()));
            }
        }

        void LoadStatFreq(List<ComparableFreq> p_freqList, List<ComparableFreq> p_rcfreqList, string p_freqID)
        {
            ActivitySurroundStation stat = GetSurroundStation(p_freqID);
            if (stat != null)
            {
                foreach(StationEmitInfo freqInfo in stat.EmitInfo)
                {
                    p_freqList.Add(new ComparableFreq(freqInfo.FreqEC == null ? 0 : (double)freqInfo.FreqEC, 0, freqInfo.FreqBand, freqInfo.Guid));
                    p_rcfreqList.Add(new ComparableFreq(freqInfo.FreqRC == null ? 0 : (double)freqInfo.FreqRC, 0, freqInfo.FreqBand, freqInfo.Guid));
                }
            }
        }
       
        private ActivityEquipment GetEquipment(string p_equipID)
        {
           return  MonitorParam.ActivityEquipments.Where(p => p.Key == p_equipID).FirstOrDefault();
        }

        private ActivitySurroundStation GetSurroundStation(string p_emitID)
        {
            foreach(ActivitySurroundStation station in MonitorParam.ActivitySurroundStations)
            {
                var list = station.EmitInfo.Where(p => p.Guid == p_emitID).ToList();
                if (list.Count > 0)
                    return station;
            }
            return null;
        }

        private void buttonReturn_Click(object sender, RoutedEventArgs e)
        {
         
        }

        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            if (StopMonitored != null)
            {
                StopMonitored(true);
            }
        }

        private void CalculatorTest_Click(object sender, MouseButtonEventArgs e)
        {

        }
        private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
            //if (dgr != null)
            //{
            //    ActivityEquipmentInfo equip = dgr.DataContext as ActivityEquipmentInfo;
            //    InterferedDetailInfoDialog dialog = new InterferedDetailInfoDialog(equip, dicInterfereResult, dicIMInterfereResult);
            //    dialog.Show();
            //}
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            _isPause = !_isPause;
            (sender as Button).Content = _isPause == true ? "继续" : "暂停";
        }
        public void StopMono()
        {
            if (realTimeMonitor != null)
            {
                realTimeMonitor.StopMono();
                realTimeMonitor = null;
            }
        }
    }

    public delegate void StopMonitor(bool finished);
    
}
