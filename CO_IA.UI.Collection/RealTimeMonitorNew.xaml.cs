using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CO_IA.Data;
using CO_IA.Client;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.NavBar;
using CO_IA.UI.Collection.DataAnalysis;
using AgilentDll;
using GalaSoft.MvvmLight.Threading;
using CO_IA.UI.Collection.Chart;
using CO_IA.UI.Collection.ViewModel;
using CO_IA_Data;
using System.Collections.ObjectModel;
using CO_IA.Data.Portable.Collection;
using System.Windows.Threading;
using CO_IA.Data.MonitorPlan;
using System.Globalization;
using System.Threading;
using System.ComponentModel;

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// RealTimeMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeMonitorNew : UserControl, INotifyPropertyChanged
    {
        //public delegate void Action(List<RoundStationInfo> p_roundStatList, List<ActivityEquipmentInfo> p_equList);
        public event Action<List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>> MonitorData;
        private ViewModelLocator _viewmodel = null;
        private Activity _activity = null;
        private ActivityPlace _place = null;
        private DetailMonitorPlan _plan = null;
        //private List<FreqPlanActivity> _pfreqPlanList = null;
        private List<MonitorPlanInfo> _pfreqPlanList = null;
        private ChartView _cv = null;
        private System.Windows.Threading.DispatcherTimer _dTimer = new System.Windows.Threading.DispatcherTimer();
        private int _selectIndex = 0;
        private int _selectIndexBefore = 0;
        //public ObservableCollection<FreqRangeMonitorInfo> FreqRangeMonitorInfoList = new ObservableCollection<FreqRangeMonitorInfo>();
        
        public ObservableCollection<FreqRangeMonitorInfo> FreqRangeMonitorInfoList
        {
            get { return _freqRangeMonitorInfoList; }
            set
            {
                if (_freqRangeMonitorInfoList != value)
                {
                    _freqRangeMonitorInfoList = value;
                    NotifyPropertyChanged("FreqRangeMonitorInfoList");
                }
            }
        }
        private ObservableCollection<FreqRangeMonitorInfo> _freqRangeMonitorInfoList = new ObservableCollection<FreqRangeMonitorInfo>();


        #region 动态属性实现
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }
        #endregion

        /// <summary>
        /// 实时监测控件
        /// </summary>
        /// <param name="pFreqPlanList">匹配频率列表</param>
        /// <param name="pRoundStationList">周围台站列表</param>
        /// <param name="AfterMonitor">保存数据回调</param>
        /// <param name="activity">活动</param>
        /// <param name="place">地点</param>
        //add by michael
        public RealTimeMonitorNew(string p_monitorPlanID, List<MonitorPlanInfo> pFreqPlanList, List<ActivityEquipment> pEquipmentList,List<ActivitySurroundStation> pRoundStationList, Action<List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>> AfterMonitor = null, Activity activity = null, ActivityPlace place = null)
        {
            InitializeComponent();

            //_plan = p_monitorPlan;
            _viewmodel = Resources["Locator"] as ViewModelLocator;

            //_cv = new ChartView();
            //_cv.DataContext = _viewmodel.FreqDataCollect.LineChartViewModel;
            //nmb.Children.Add(_cv);

            _pfreqPlanList = pFreqPlanList;
            if (_viewmodel != null && _viewmodel.FreqDataCollectNew != null)
            {
                MonitorData = AfterMonitor;
                //realTimeMonitor_MonitorData
                _viewmodel.FreqDataCollectNew.AfterMonitor += realTimeMonitor_MonitorData;
                //_viewmodel.FreqDataCollectNew.AfterMonitor += AfterMonitor;
                _viewmodel.FreqDataCollectNew.RoundStationInfoList = pRoundStationList;
                _viewmodel.FreqDataCollectNew.EquipmentList = pEquipmentList;
                _viewmodel.FreqDataCollectNew.MonitorPlanID = p_monitorPlanID;//add by michael
                if (pFreqPlanList != null && pFreqPlanList.Count > 0) {
                    var fps = pFreqPlanList[0];
                    _viewmodel.FreqDataCollectNew.FPS = fps;
                    if (!(fps.kHzBand > 0))
                        fps.kHzBand = 12.5;
                    spin_start.Text = fps.MHzFreqFrom.ToString();
                    spin_stop.Text = fps.MHzFreqTo.ToString();
                    spin_bandWidth.Text = fps.kHzBand.ToString();
                }
                    

                if (activity != null)
                {
                    _viewmodel.FreqDataCollectNew.Activity = activity;
                }
                if (place != null)
                {
                    _viewmodel.FreqDataCollectNew.Place = place;
                }
            }
            foreach (var item in _pfreqPlanList)
            {
                var info = new FreqRangeMonitorInfo();
                info.FreqRangeName = item.MHzFreqFrom + "-" + item.MHzFreqTo;
                info.MHzFreqFrom = item.MHzFreqFrom;
                info.MHzFreqTo = item.MHzFreqTo;
                info.IllegalFreqInfoList = new ObservableCollection<IllegalFreqInfo>();
                info.LegalFreqInfoList = new ObservableCollection<LegalFreqInfo>();
                FreqRangeMonitorInfoList.Add(info);
            }
            tabControl.ItemsSource = FreqRangeMonitorInfoList;
            initListBoxData();
            
            
            DispatcherHelper.Initialize();
            img_burst_signal.Opacity = 0.5;
            //ctrlImageflick();
            //linkAgilentEquipment();
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<string>(this, "ShowMyMessage", OnShowMessage);//add by michael
        }
        //add end

        public void StopMono()
        {
            _dTimer.Stop();
            btnStop.Command.Execute(null);
            //AgilentDll.Sensor.Close();
            //_viewmodel = null;
            this.DataContext = null;
            //GC.Collect();
        }

        private void linkAgilentEquipment()
        {
            if (!AgilentDll.Sensor.Connect(spin_Ip.Text))
            {
                Sensor.IsUseSensor = false;
                MessageBox.Show("设备未连接！");
                return;
            }
            else
            {
                Sensor.IsUseSensor = true;
            }
        }

        private void initListBoxData()
        {
            listBoxEdit.ItemsSource = _pfreqPlanList;
            if (_pfreqPlanList.Count > 0)
                listBoxEdit.SelectedIndex = 0;
            //var _freqIdList = _plan.MAINGUID.Split(',').ToList();
            //listBoxEdit.ItemsSource = GetFreqPlanActivitys(_freqIdList);
        }


        //private List<FreqPlanActivity> GetFreqPlanActivitys(List<string> p_freqIdList)
        //{
        //    List<FreqPlanActivity> list = new List<FreqPlanActivity>();
        //    foreach (string freqId in p_freqIdList)
        //    {
        //        FreqPlanActivity actFreq = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, FreqPlanActivity>(channel =>
        //        {
        //            FreqPlanActivity freqPlanActivity = channel.GetSingFreqPlanActivity(freqId);
        //            return freqPlanActivity;
        //        });
        //        if (actFreq != null)
        //            list.Add(actFreq);
        //    }

        //    return list;
        //}

        //private void x_btnStartClick(object sender, RoutedEventArgs e)
        //{
        //    Button btn = (Button)sender;
        //    Grid grid = (Grid)btn.Parent;
        //    FreqNavBar fnr = (FreqNavBar)grid.Parent;
        //    fnr.PlaceGuid = LoginService.CurrentActivityPlace.Guid;
        //    //startMessure(fnr);
        //}
        private AgilentDll.Sensor.SAL_SEGMENT_CALLBACK _proc;
        //private void startMessure(FreqNavBar fnb)
        //{
        //    if (Sensor.IsUseSensor)
        //    {
        //        _proc = CallBack;
        //        AgilentDll.Sensor.SendWbqexCmd(Convert.ToDouble(fnb.FreqStart) * 1000000.0, Convert.ToDouble(fnb.FreqStop) * 1000000.0, Convert.ToDouble(fnb.BandWidth) * 1000.0, _proc);
        //        return;
        //    }
        //}

        //private int CallBack(ref AgilentDll.Sensor.SegmentData segData, IntPtr data)
        //{
        //    if (segData.errorNum != AgilentDll.Sensor.SalError.SAL_ERR_NONE)
        //        return -1;

        //    FreqDataTemplate FreqData = new FreqDataTemplate();
        //    FreqData.FreqCount = Convert.ToInt32(segData.numPoints);
        //    FreqData.MeasureTime = DateTime.Now;
        //    FreqData.StartFreq = segData.startFrequency;
        //    FreqData.StepValue = segData.frequencyStep;
        //    FreqData.segData = segData;
        //    FreqData.volList = new float[segData.numPoints];
        //    System.Runtime.InteropServices.Marshal.Copy(data, FreqData.volList, 0, Convert.ToInt32(segData.numPoints));

        //    //CurrentFreqFrameItem.FreqDataItemList.Add(FreqData);


        //    FreqLineDataItem FreqShowData = new FreqLineDataItem();
        //    FreqShowData.byteArray = new float[segData.numPoints];
        //    System.Runtime.InteropServices.Marshal.Copy(data, FreqShowData.byteArray, 0, Convert.ToInt32(segData.numPoints));
        //    FreqShowData.frequencyStep = segData.frequencyStep;
        //    FreqShowData.startFrequency = segData.startFrequency;

        //    //_LineChartViewModel.InsertShowData(FreqShowData);

        //    return 0;
        //}

        //private void ctrlImageflick()
        //{
        //    System.Windows.Threading.DispatcherTimer dTimer = new System.Windows.Threading.DispatcherTimer();
        //    dTimer.Tick += new EventHandler(dTimer_Tick);
        //    dTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
        //    dTimer.Start();

        //}

        //private void dTimer_Tick(object sender, EventArgs e)
        //{
        //    if (img_burst_signal.Opacity >= 1)
        //    {
        //        img_burst_signal.Opacity = 0.5;
        //    }
        //    else
        //    {
        //        img_burst_signal.Opacity += 0.1;
        //    }
        //}
        //private void slider_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        //{
        //    //if (signalCheckEdit.IsChecked == true)
        //    //{
        //    //    panel_burst_signal_list.Visibility = System.Windows.Visibility.Visible;
        //    //}
        //    //else
        //    //{
        //    //    panel_burst_signal_list.Visibility = System.Windows.Visibility.Collapsed;
        //    //}
        //}


        //private List<FreqPlanSegment> GetFreqPartPlan()
        //{
        //    try
        //    {
        //        return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanSegment>>(channel =>
        //        {
        //            return channel.GetFreqPlanInfo();
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.GetExceptionMessage());
        //        return null;
        //    }
        //}


        //private List<FreqPlanActivity> GetActivityFreqPlanInfoSource(string pPlaceId)
        //{
        //    try
        //    {
        //        //List<FreqPlanActivity> freqPlanActivitys =
        //        return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanActivity>>(channel =>
        //        {
        //            return channel.GetFreqPlanActivitys(pPlaceId);
        //        });
        //        //if (freqPlanActivitys != null)
        //        //    xfreqPartPlanList.FreqPlanInfoItemsSource = new System.Collections.ObjectModel.ObservableCollection<FreqPlanActivity>(freqPlanActivitys);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.GetExceptionMessage());
        //        return null;
        //    }
        //}

        private void listBoxEdit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /* 
             //remark by michael
            FreqPlanActivity fps = (FreqPlanActivity)listBoxEdit.SelectedItem;
            if (fps == null)
                return;

            AT_BC.Data.Range<double> freqValue = fps.FreqValue;
            spin_start.Text = ((int)(freqValue.Little / 1000000)).ToString();
            spin_stop.Text = ((int)(freqValue.Great / 1000000)).ToString();
            spin_bandWidth.Text = (Convert.ToDouble(fps.FreqBand.Split('/')[0]) / 1000).ToString();
            _viewmodel.FreqDataCollect.FPS = fps;

            List<Data.ActivityEquipmentInfo> equimentList = new List<ActivityEquipmentInfo>();
            equimentList.AddRange(fps.ApplyEquipments);
            _viewmodel.FreqDataCollect.EquipmentList = equimentList;

            ChangeTextBoxsEnable(false);
            */

            MonitorPlanInfo fps = (MonitorPlanInfo)listBoxEdit.SelectedItem;
            if (fps == null)
                return;

            //add michael for test
            if (!(fps.kHzBand > 0))
                fps.kHzBand = 12.5;
            //
            spin_start.Text = fps.MHzFreqFrom.ToString();
            spin_stop.Text = fps.MHzFreqTo.ToString();
            spin_bandWidth.Text = fps.kHzBand.ToString();
            _viewmodel.FreqDataCollectNew.FPS = fps;

            //List<Data.ActivityEquipmentInfo> equimentList = new List<ActivityEquipmentInfo>();
            //equimentList.AddRange(fps.ApplyEquipments);
           // _viewmodel.FreqDataCollect.EquipmentList = equimentList;

            //ChangeTextBoxsEnable(false);


            //if (pFreqPlanList != null && pFreqPlanList.Count > 0)
            //{
            //    List<Data.ActivityEquipmentInfo> equimentList = new List<ActivityEquipmentInfo>();
            //    foreach (FreqPlanActivity plan in pFreqPlanList)
            //    {
            //        equimentList.AddRange(plan.ApplyEquipments);
            //    }
            //    _viewmodel.FreqDataCollect.EquipmentList = equimentList;
            //}

            //btnStop.Command.Execute(null);
            //btnStart.Command.Execute(null);
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextBoxsEnable(true);
            //var list = (ObservableCollection<Data.Collection.AnalysisResult>)gridControl_freqInfo1.ItemsSource;
            if (_viewmodel != null && _viewmodel.FreqDataCollect != null)
            {

                //AfterMonitor(_viewmodel.FreqDataCollect.RoundStationInfoListMate, _viewmodel.FreqDataCollect.EquipmentListMate);
            }
            //chk_status.IsEnabled = true;
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            btn_monitor_start.IsEnabled = true;
            btn_monitor_stop.IsEnabled = false;

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextBoxsEnable(false);
            //chk_status.IsEnabled = false;
            btn_monitor_start.IsEnabled = false;
            btn_monitor_stop.IsEnabled = false;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
        }

        private void cbxSaveData_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {

        }

        private void ChangeTextBoxsEnable(bool enable)
        {
            spin_Ip.IsEnabled = enable;
            spin_Port.IsEnabled = enable;
            spin_start.IsEnabled = enable;
            spin_stop.IsEnabled = enable;
            spin_bandWidth.IsEnabled = enable;
            spin_signalLimit.IsEnabled = enable;
        }

        private void OnShowMessage(string str)
        {
            MessageBox.Show(str);
        }

        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{
        //    var tmpList = (List<MonitorPlanInfo>)listBoxEdit.ItemsSource;
        //    if (tmpList.Count > 0)
        //    {
        //        //if (chk_status.IsChecked == true)
        //        {
        //            btn_monitor_start.IsEnabled = true;
        //            btn_monitor_stop.IsEnabled = false;
        //        }
        //        //else
        //        {
        //            btn_monitor_start.IsEnabled = false;
        //            btn_monitor_stop.IsEnabled = false;
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("没有监测频段，不能进行循环监测！");
        //    }
        //}

        private void btn_monitor_start_Click(object sender, RoutedEventArgs e)
        {
            //chk_status.IsEnabled = false;
            btn_monitor_start.IsEnabled = false;
            btn_monitor_stop.IsEnabled = true;
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = false;

            int intInterval = Int32.Parse(text_interval.Text);
            if (listBoxEdit.SelectedIndex >= 0)
                _selectIndex = listBoxEdit.SelectedIndex;
            else
                _selectIndex = 0;
            _selectIndexBefore = _selectIndex;
            //var tmpList = (List<MonitorPlanInfo>)listBoxEdit.ItemsSource;
            //MonitorPlanInfo fps = tmpList[_selectIndex];
            //spin_start.Text = fps.MHzFreqFrom.ToString();
            //spin_stop.Text = fps.MHzFreqTo.ToString();
            //spin_bandWidth.Text = fps.kHzBand.ToString();
            //_viewmodel.FreqDataCollectNew.FPS = fps;
            //_dTimer = new System.Windows.Threading.DispatcherTimer();
            _dTimer.Tick += new EventHandler(_dTimer_Tick);

            _dTimer.Interval = new TimeSpan(0, 0, 0, intInterval, 0);
            _dTimer.Start();
        }

        private void btn_monitor_stop_Click(object sender, RoutedEventArgs e)
        {
            //chk_status.IsEnabled = true;
            btn_monitor_start.IsEnabled = true;
            btn_monitor_stop.IsEnabled = false;            
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            _dTimer.Stop();
            btnStop.Command.Execute(null);
        }

        private void _dTimer_Tick(object sender, EventArgs e)
        {
            var tmpList = (List<MonitorPlanInfo>)listBoxEdit.ItemsSource;
            if (tmpList.Count > 0)
            {
                if (_selectIndex == tmpList.Count)
                    _selectIndex = 0;
            }
            else
            {
                return;
            }
            //btnStop.Command.Execute(null);
            //Thread.Sleep(200);
            //btnStart.Command.Execute(null);
            MonitorPlanInfo fps = tmpList[_selectIndex];
            if (fps == null)
                return;

            if (!(fps.kHzBand > 0))
                fps.kHzBand = 12.5;

            spin_start.Text = fps.MHzFreqFrom.ToString();
            spin_stop.Text = fps.MHzFreqTo.ToString();
            spin_bandWidth.Text = fps.kHzBand.ToString();
            //_viewmodel.FreqDataCollectNew.FPS = fps;
            listBoxEdit.SelectedIndex = _selectIndex;

            btnStop.Command.Execute(null);
            Thread.Sleep(150);
            _viewmodel.FreqDataCollectNew.FPS = fps;
            btnStart.Command.Execute(null);
            _selectIndex++;            
        }

        private void realTimeMonitor_MonitorData(List<Data.Collection.AnalysisResult> matchSignals, List<Data.Collection.AnalysisResult> unMatchSignals, List<Data.Collection.AnalysisResult> unknowSignals,long times)
        {
            MonitorData(matchSignals, unMatchSignals, unknowSignals);
            //System.Console.WriteLine("index=" + (_selectIndex-1) + ",before=" + _selectIndexBefore);
            int i = 0;
            bool flag; 
            foreach (var item2 in _freqRangeMonitorInfoList)
            {
                flag = true;
                if (matchSignals != null && matchSignals.Count > 0)
                {
                    if (item2.MHzFreqFrom <= matchSignals[0].Frequency && matchSignals[0].Frequency <= item2.MHzFreqTo)
                    {
                        item2.Total+=times;
                        flag = false;
                    }
                }
                if (flag && unknowSignals != null && unknowSignals.Count > 0)
                {
                    if (item2.MHzFreqFrom <= unknowSignals[0].Frequency && unknowSignals[0].Frequency <= item2.MHzFreqTo)
                    {
                        item2.Total += times;
                        flag = true;
                    }
                }
            } 
          
            foreach (var item1 in matchSignals)
            {
                foreach (var item2 in _freqRangeMonitorInfoList)
                {
                    if (item2.MHzFreqFrom <= item1.Frequency && item1.Frequency <= item2.MHzFreqTo)
                    {
                        flag = false;
                        for (int j = 0; j < item2.LegalFreqInfoList.Count; j++)
                        {
                            if (item2.LegalFreqInfoList[j].Frequency == item1.Frequency)
                            {
                                i = j;
                                flag = true;
                            }
                            else
                            {
                                item2.LegalFreqInfoList[j].Occupy = Math.Round(item2.LegalFreqInfoList[j].Times * 100.0 / item2.Total,2);
                            }
                        }
                        if (flag)
                        {
                            item2.LegalFreqInfoList[i].StopTime = DateTime.Now;
                            item2.LegalFreqInfoList[i].Times++;
                            item2.LegalFreqInfoList[i].Occupy = Math.Round(item2.LegalFreqInfoList[i].Times * 100.0 / item2.Total,2);
                        }
                        else
                        {
                            var info = new LegalFreqInfo();
                            info.AmplitudeMaxValue = item1.AmplitudeMaxValue;
                            info.EquimentName = item1.EquimentName;
                            info.Frequency = item1.Frequency;                            
                            info.StationName = item1.StationName;
                            info.SendStatusPic = item1.SendStatusPic;
                            info.StartTime = DateTime.Now;
                            info.StopTime = DateTime.Now;
                            info.Times++;
                            info.Occupy = Math.Round(info.Times*100.0/item2.Total,2);

                            item2.LegalFreqInfoList.Add(info);
                        }
                    }
                }
            }

            foreach (var item1 in unknowSignals)
            {
                foreach (var item2 in _freqRangeMonitorInfoList)
                {
                    if (item2.MHzFreqFrom <= item1.Frequency && item1.Frequency <= item2.MHzFreqTo)
                    {
                        flag = false;
                        for (int j = 0; j < item2.IllegalFreqInfoList.Count; j++)
                        {
                            if (item2.IllegalFreqInfoList[j].Frequency == item1.Frequency)
                            {
                                flag = true;
                                i = j;
                            }
                            else
                            {
                                item2.IllegalFreqInfoList[j].Occupy = Math.Round(item2.IllegalFreqInfoList[j].Times * 100.0 / item2.Total,2);
                            }
                        }
                        if (flag)
                        {
                            item2.IllegalFreqInfoList[i].StopTime = DateTime.Now;
                            item2.IllegalFreqInfoList[i].Times++;
                            item2.IllegalFreqInfoList[i].Occupy = Math.Round(item2.IllegalFreqInfoList[i].Times * 100.0 / item2.Total,2);
                        }
                        else
                        {
                            var info = new IllegalFreqInfo();
                            info.AmplitudeMaxValue = item1.AmplitudeMaxValue;
                            info.Frequency = item1.Frequency;
                            info.SendStatusPic = item1.SendStatusPic;
                            info.StartTime = DateTime.Now;
                            info.StopTime = DateTime.Now;
                            info.Times++;
                            info.Occupy = Math.Round(info.Times * 100.0 / item2.Total,2);

                            item2.IllegalFreqInfoList.Add(info);
                        }
                    }
                }
            }
        }

    }

    public class FreqPlanNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var objFreq = (MonitorPlanInfo) value;
            return objFreq.MHzFreqFrom + "-" + objFreq.MHzFreqTo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class FreqRangeMonitorInfo : BaseClass
    {
        private string _freqRangeName;
        public string FreqRangeName
        {
            get { return _freqRangeName; }
            set
            {
                if (_freqRangeName != value)
                {
                    _freqRangeName = value;
                    NotifyPropertyChanged("FreqRangeName");
                }
            }
        }

        public double MHzFreqFrom
        {
            get;
            set;
        }

        public double MHzFreqTo
        {
            get;
            set;
        }

        public long Total
        {
            get;
            set;
        }
        private ObservableCollection<LegalFreqInfo> _legalFreqInfoList = new ObservableCollection<LegalFreqInfo>();
        public ObservableCollection<LegalFreqInfo> LegalFreqInfoList
        {
            get { return _legalFreqInfoList; }
            set
            {
                if (_legalFreqInfoList != value)
                {
                    _legalFreqInfoList = value;
                    NotifyPropertyChanged("LegalFreqInfoList");
                    //NotifyPropertyChanged("信号列表");
                }
            }
        }

        private ObservableCollection<IllegalFreqInfo> _illegalFreqInfoList = new ObservableCollection<IllegalFreqInfo>();
        public ObservableCollection<IllegalFreqInfo> IllegalFreqInfoList
        {
            get { return _illegalFreqInfoList; }
            set
            {
                if (_illegalFreqInfoList != value)
                {
                    _illegalFreqInfoList = value;
                    NotifyPropertyChanged("IllegalFreqInfoList");
                    //NotifyPropertyChanged("突发信号列表");
                }
            }
        }
        
    }
    
    public class LegalFreqInfo : BaseClass
    {
        private double _frequency;
        private int _amplitudeMaxValue;
        private double _occupy;
        private string _stationName;
        private string _sendStatusPic;
        private string _equimentName;
        private DateTime _startTime;
        private DateTime _stopTime;

        /// <summary>
        /// 信号频率
        /// </summary>
        public double Frequency
        {
            get { return _frequency; }
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    NotifyPropertyChanged("Frequency");
                    //NotifyPropertyChanged("信号频率(MHz)");
                }
            }
        }

        /// <summary>
        /// 发射状态图片
        /// </summary>
        public string SendStatusPic
        {
            get { return _sendStatusPic; }
            set
            {
                if (_sendStatusPic != value)
                {
                    _sendStatusPic = value;
                    NotifyPropertyChanged("SendStatusPic");
                    //NotifyPropertyChanged("发射情况");
                }
            }
        }

        /// <summary>
        /// 幅度最大值
        /// </summary>
        public int AmplitudeMaxValue
        {
            get { return _amplitudeMaxValue; }
            set
            {
                if (_amplitudeMaxValue != value)
                {
                    _amplitudeMaxValue = value;
                    NotifyPropertyChanged("AmplitudeMaxValue");
                    //NotifyPropertyChanged("发射强度(dBμV)");
                }
            }
        }
        /// <summary>
        /// 台站名称
        /// </summary>
        public string StationName
        {
            get { return _stationName; }
            set
            {
                if (_stationName != value)
                {
                    _stationName = value;
                    NotifyPropertyChanged("StationName");
                    //NotifyPropertyChanged("台站名称");
                }
            }
        }
        /// <summary>
        /// 占用度
        /// </summary>
        public double Occupy
        {
            get { return _occupy; }
            set
            {
                if (_occupy != value)
                {
                    _occupy = value;
                    NotifyPropertyChanged("Occupy");
                    //NotifyPropertyChanged("占用度(%)");
                }
            }
        }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquimentName
        {
            get { return _equimentName; }
            set
            {
                if (_equimentName != value)
                {
                    _equimentName = value;
                    NotifyPropertyChanged("EquimentName");
                    //NotifyPropertyChanged("设备名称");
                }
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }

        public DateTime StopTime
        {
            get { return _stopTime; }
            set
            {
                if (_stopTime != value)
                {
                    _stopTime = value;
                    NotifyPropertyChanged("StopTime");
                }
            }
        }

        public long Times
        {
            get;
            set;
        }
    }

    public class IllegalFreqInfo : BaseClass
    {
        private double _frequency;
        private int _amplitudeMaxValue;
        private string _sendStatusPic;
        private double _occupy;
        private DateTime _startTime;
        private DateTime _stopTime;

        /// <summary>
        /// 信号频率
        /// </summary>
        public double Frequency
        {
            get { return _frequency; }
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    NotifyPropertyChanged("Frequency");
                    //NotifyPropertyChanged("信号频率(MHz)");
                }
            }
        }

        /// <summary>
        /// 发射状态图片
        /// </summary>
        public string SendStatusPic
        {
            get { return _sendStatusPic; }
            set
            {
                if (_sendStatusPic != value)
                {
                    _sendStatusPic = value;
                    NotifyPropertyChanged("SendStatusPic");
                    //NotifyPropertyChanged("发射情况");
                }
            }
        }

        /// <summary>
        /// 幅度最大值
        /// </summary>
        public int AmplitudeMaxValue
        {
            get { return _amplitudeMaxValue; }
            set
            {
                if (_amplitudeMaxValue != value)
                {
                    _amplitudeMaxValue = value;
                    NotifyPropertyChanged("AmplitudeMaxValue");
                    //NotifyPropertyChanged("发射强度(dBμV)");
                }
            }
        }

        public double Occupy
        {
            get { return _occupy; }
            set
            {
                if (_occupy != value)
                {
                    _occupy = value;
                    NotifyPropertyChanged("Occupy");
                    //NotifyPropertyChanged("占用度(%)");
                }
            }
        }
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    NotifyPropertyChanged("StartTime");
                }
            }
        }

        public DateTime StopTime
        {
            get { return _stopTime; }
            set
            {
                if (_stopTime != value)
                {
                    _stopTime = value;
                    NotifyPropertyChanged("StopTime");
                }
            }
        }

        public long Times
        {
            get;
            set;
        }

    }

    public class BaseClass : INotifyPropertyChanged
    {

        #region 动态属性实现
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
            }
        }
        #endregion

    }
}
