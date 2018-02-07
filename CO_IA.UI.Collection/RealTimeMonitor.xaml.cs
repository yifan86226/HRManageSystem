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

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// RealTimeMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeMonitor : UserControl
    {
        //public delegate void Action(List<RoundStationInfo> p_roundStatList, List<ActivityEquipmentInfo> p_equList);
        private ViewModelLocator _viewmodel = null;
        private Activity _activity = null;
        private ActivityPlace _place = null;
        private DetailMonitorPlan _plan = null;
        private List<FreqPlanActivity> _pfreqPlanList = null;
        private ChartView _cv = null;

        /// <summary>
        /// 实时监测控件
        /// </summary>
        /// <param name="pFreqPlanList">匹配频率列表</param>
        /// <param name="pRoundStationList">周围台站列表</param>
        /// <param name="AfterMonitor">保存数据回调</param>
        /// <param name="activity">活动</param>
        /// <param name="place">地点</param>
        public RealTimeMonitor(DetailMonitorPlan p_monitorPlan, List<FreqPlanActivity> pFreqPlanList, List<RoundStationInfo> pRoundStationList, Action<List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>> AfterMonitor = null, Activity activity = null, ActivityPlace place = null)
        {
            InitializeComponent();

            _plan = p_monitorPlan;
            _viewmodel = Resources["Locator"] as ViewModelLocator;

            //_cv = new ChartView();
            //_cv.DataContext = _viewmodel.FreqDataCollect.LineChartViewModel;
            //nmb.Children.Add(_cv);

            _pfreqPlanList = pFreqPlanList;
            if (_viewmodel != null && _viewmodel.FreqDataCollect != null)
            {
                _viewmodel.FreqDataCollect.AfterMonitor += AfterMonitor;
                _viewmodel.FreqDataCollect.RoundStationInfoList = pRoundStationList;
                _viewmodel.FreqDataCollect.MonitorPlan = _plan;

                if (activity != null)
                {
                    _viewmodel.FreqDataCollect.Activity = activity;
                }
                if (place != null)
                {
                    _viewmodel.FreqDataCollect.Place = place;
                }
            }

            initListBoxData();
            DispatcherHelper.Initialize();
            img_burst_signal.Opacity = 0.5;
            ctrlImageflick();
            linkAgilentEquipment();
        }

        public void StopMono()
        {
            btnStop.Command.Execute(null);
            AgilentDll.Sensor.Close();
            _viewmodel = null;
            this.DataContext = null;
            GC.Collect();
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

        private int CallBack(ref AgilentDll.Sensor.SegmentData segData, IntPtr data)
        {
            if (segData.errorNum != AgilentDll.Sensor.SalError.SAL_ERR_NONE)
                return -1;

            FreqDataTemplate FreqData = new FreqDataTemplate();
            FreqData.FreqCount = Convert.ToInt32(segData.numPoints);
            FreqData.MeasureTime = DateTime.Now;
            FreqData.StartFreq = segData.startFrequency;
            FreqData.StepValue = segData.frequencyStep;
            FreqData.segData = segData;
            FreqData.volList = new float[segData.numPoints];
            System.Runtime.InteropServices.Marshal.Copy(data, FreqData.volList, 0, Convert.ToInt32(segData.numPoints));

            //CurrentFreqFrameItem.FreqDataItemList.Add(FreqData);


            FreqLineDataItem FreqShowData = new FreqLineDataItem();
            FreqShowData.byteArray = new float[segData.numPoints];
            System.Runtime.InteropServices.Marshal.Copy(data, FreqShowData.byteArray, 0, Convert.ToInt32(segData.numPoints));
            FreqShowData.frequencyStep = segData.frequencyStep;
            FreqShowData.startFrequency = segData.startFrequency;

            //_LineChartViewModel.InsertShowData(FreqShowData);

            return 0;
        }

        private void ctrlImageflick()
        {
            System.Windows.Threading.DispatcherTimer dTimer = new System.Windows.Threading.DispatcherTimer();
            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
            dTimer.Start();

        }

        private void dTimer_Tick(object sender, EventArgs e)
        {
            if (img_burst_signal.Opacity >= 1)
            {
                img_burst_signal.Opacity = 0.5;
            }
            else
            {
                img_burst_signal.Opacity += 0.1;
            }
        }
        private void slider_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (signalCheckEdit.IsChecked == true)
            {
                panel_burst_signal_list.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                panel_burst_signal_list.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

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
            //if (pFreqPlanList != null && pFreqPlanList.Count > 0)
            //{
            //    List<Data.ActivityEquipmentInfo> equimentList = new List<ActivityEquipmentInfo>();
            //    foreach (FreqPlanActivity plan in pFreqPlanList)
            //    {
            //        equimentList.AddRange(plan.ApplyEquipments);
            //    }
            //    _viewmodel.FreqDataCollect.EquipmentList = equimentList;
            //}

            btnStop.Command.Execute(null);
            btnStart.Command.Execute(null);
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextBoxsEnable(true);
            //var list = (ObservableCollection<Data.Collection.AnalysisResult>)gridControl_freqInfo1.ItemsSource;
            if (_viewmodel != null && _viewmodel.FreqDataCollect != null)
            {

                //AfterMonitor(_viewmodel.FreqDataCollect.RoundStationInfoListMate, _viewmodel.FreqDataCollect.EquipmentListMate);
            }

        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextBoxsEnable(false);
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
    }
}
