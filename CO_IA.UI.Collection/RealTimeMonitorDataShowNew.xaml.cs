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
using I_CO_IA.Collection;
using System.ComponentModel;
using CO_IA.Data.MonitorPlan;
using System.Globalization;

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// RealTimeMonitor.xaml 的交互逻辑
    /// </summary>
    public partial class RealTimeMonitorDataShowNew : UserControl
    {
        /// <summary>
        /// 主绘线程
        /// </summary>
        private DispatcherTimer _timer = null;
        /// <summary>
        /// 当前所选业务
        /// </summary>
        private FreqPlanActivity _currentPlan = null;
        /// <summary>
        /// 地点ID
        /// </summary>
        private string _placeId = string.Empty;
        //private DetailMonitorPlan _monitorPlan = null;
        //private List<MonitorPlanInfo> _monitorPlanList = null;
        private string _monitorPlanID;

        /// <summary>
        /// 大屏端实时显示控件
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="place"></param>
        //public RealTimeMonitorDataShowNew(DetailMonitorPlan p_monitorPlan)
        public RealTimeMonitorDataShowNew(List<MonitorPlanInfo> p_monitorPlanList)
        {
            InitializeComponent();
            //_monitorPlanList = p_monitorPlanList;
            initListBoxData(p_monitorPlanList);
            DispatcherHelper.Initialize();
            img_burst_signal.Opacity = 0.5;
            ctrlImageflick();
            this.Loaded += RealTimeMonitorDataShow_Loaded;
            this.Unloaded += RealTimeMonitorDataShow_Unloaded;
        }
        public void SetCondition(string p_monitorPlanID)
        {
            _monitorPlanID = p_monitorPlanID;
            var plan = listBoxEdit.SelectedItem as FreqPlanActivity;
            _currentPlan = plan;
            listBoxEdit_MouseLeftButtonUp(null, null);
            //ShowChartAndData();
        }

        private void ShowChartAndData()
        {
            if (_currentPlan == null || String.IsNullOrEmpty(_monitorPlanID))
                return;
            List<Data.Collection.AnalysisResult> legalSignals = null;
            List<Data.Collection.AnalysisResult> illegalitySignals = null;
            FreqLineDataItem spectrumData = null;

            var backbroudWorker = new BackgroundWorker();
            backbroudWorker.DoWork += ((s, p) =>
            {
                //合法信号
                legalSignals = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<Data.Collection.AnalysisResult>>(channel =>
                {
                    return channel.GetRealTimeMonitorAnalysis(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlanID, '0');
                });

                //冲突信号
                illegalitySignals = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<Data.Collection.AnalysisResult>>(channel =>
                {
                    return channel.GetRealTimeMonitorAnalysis(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlanID, '1');
                });

                //谱图数据
                spectrumData = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, FreqLineDataItem>(channel =>
                {
                    return channel.GetSpectrumData(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlanID);
                });
            });
            backbroudWorker.RunWorkerCompleted += ((s, p) =>
            {
                gridControl_freqInfo1.ItemsSource = legalSignals;
                gridControl_freqInfo.ItemsSource = illegalitySignals;

                if (spectrumData != null)
                {
                    double startFreq = spectrumData.TestFreqStart * 1000000;
                    double endFreq = spectrumData.TestFreqEnd * 1000000;
                    uint count = (uint)(1 + (endFreq - startFreq) / spectrumData.frequencyStep);
                    var values = FloatArrayToShortArray(spectrumData.byteArray);

                    {
                        x_widebandFreq.Clear();
                        x_widebandFreq.InitSpectrumProperty(null, null, startFreq, endFreq);
                        count = count > (uint)values.Count() ? (uint)values.Count() : count;
                        x_widebandFreq.Initializers(startFreq, spectrumData.frequencyStep, Convert.ToInt32(count));
                    }

                    x_widebandFreq.DrawLine(spectrumData.TestFreqStart * 1000000, spectrumData.frequencyStep, spectrumData.TestFreqStart * 1000000, spectrumData.frequencyStep, values, 0, Colors.Green, Best.VWPlatform.Controls.Freq.SpectrumLineType.Wave);

                }
                else
                {
                    x_widebandFreq.Clear();
                    x_widebandFreq.MeasureUnit = "dBμV";
                    x_widebandFreq.InitSpectrumProperty(null, null, 30000000, 30000000000);
                }
                x_widebandFreq.Update();
            });
            backbroudWorker.RunWorkerAsync();
        }

        private void RealTimeMonitorDataShow_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Tick -= Timer_Tick;
                _timer.Stop();
            }
        }

        private void RealTimeMonitorDataShow_Loaded(object sender, RoutedEventArgs e)
        {
            x_widebandFreq.Clear();
            x_widebandFreq.MeasureUnit = "dBμV";
            x_widebandFreq.InitSpectrumProperty(null, null, 30000000, 30000000000);
            x_widebandFreq.Update();

            //txtInfo.Text = string.Format("地点:{0}", _placeInfo.Name);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            bool isFirst = false;
            if (_timer.Interval.TotalMilliseconds != 5000)
            {
                isFirst = true;
                _timer.Interval = TimeSpan.FromMilliseconds(5000);
            }


            listBoxEdit.IsEnabled = false;

            if (_currentPlan == null || String.IsNullOrEmpty(_monitorPlanID))
                return;
            List<Data.Collection.AnalysisResult> legalSignals = null;
            List<Data.Collection.AnalysisResult> illegalitySignals = null;
            FreqLineDataItem spectrumData = null;

            var backbroudWorker = new BackgroundWorker();
            backbroudWorker.DoWork += ((s, p) =>
            {

                //合法信号
                legalSignals = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<Data.Collection.AnalysisResult>>(channel =>
                {
                    //return channel.GetRealTimeMonitorAnalysis(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlan.GUID, '0');
                    return channel.GetRealTimeMonitorAnalysis(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlanID, '0');
                });

                //冲突信号
                illegalitySignals = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<Data.Collection.AnalysisResult>>(channel =>
                {
                    //return channel.GetRealTimeMonitorAnalysis(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlan.GUID, '1');
                    return channel.GetRealTimeMonitorAnalysis(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlanID, '1');
                });

                //谱图数据
                spectrumData = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, FreqLineDataItem>(channel =>
                {
                    //return channel.GetSpectrumData(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlan.GUID);
                    return channel.GetSpectrumData(_currentPlan.ClassCode, _currentPlan.ActivityId, _monitorPlanID);
                });
            });
            backbroudWorker.RunWorkerCompleted += ((s, p) =>
            {
                gridControl_freqInfo1.ItemsSource = legalSignals;
                gridControl_freqInfo.ItemsSource = illegalitySignals;

                if (spectrumData != null)
                {
                    double startFreq = spectrumData.TestFreqStart * 1000000;
                    double endFreq = spectrumData.TestFreqEnd * 1000000;
                    uint count = (uint)(1 + (endFreq - startFreq) / spectrumData.frequencyStep);
                    var values = FloatArrayToShortArray(spectrumData.byteArray);

                    if (isFirst)
                    {
                        x_widebandFreq.Clear();
                        x_widebandFreq.InitSpectrumProperty(null, null, startFreq, endFreq);
                        count = count > (uint)values.Count() ? (uint)values.Count() : count;
                        x_widebandFreq.Initializers(startFreq, spectrumData.frequencyStep, Convert.ToInt32(count));
                    }

                    x_widebandFreq.DrawLine(spectrumData.TestFreqStart * 1000000, spectrumData.frequencyStep, spectrumData.TestFreqStart * 1000000, spectrumData.frequencyStep, values, 0, Colors.Green, Best.VWPlatform.Controls.Freq.SpectrumLineType.Wave);

                }
                x_widebandFreq.Update();
                listBoxEdit.IsEnabled = true;
            });
            backbroudWorker.RunWorkerAsync();
        }

        private short[] FloatArrayToShortArray(float[] array)
        {
            if (array == null || array.Length == 0)
                return null;

            List<short> list = new List<short>();
            array.All(obj => { list.Add((short)(obj + 107)); return true; });
            return list.ToArray();
        }

        private void initListBoxData(List<MonitorPlanInfo> p_monitorPlanList)
        {
            //listBoxEdit.ItemsSource = _pfreqPlanList;
            //var _freqIdList = _monitorPlan.MAINGUID.Split(',').ToList();
            listBoxEdit.ItemsSource = GetFreqPlanActivitysNew(p_monitorPlanList);
            if (p_monitorPlanList != null && p_monitorPlanList.Count > 0)
                listBoxEdit.SelectedIndex = 0;
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

        private List<FreqPlanActivity> GetFreqPlanActivitysNew(List<MonitorPlanInfo> p_List)
        {
            List<FreqPlanActivity> list = new List<FreqPlanActivity>();
            if (p_List != null)
            {
                foreach (var planInfo in p_List)
                {
                    FreqPlanActivity actFreq = new FreqPlanActivity();
                    actFreq.ActivityId = planInfo.ActivityGuid;
                    actFreq.ClassCode = planInfo.Key;
                    actFreq.FreqValue.Little = planInfo.MHzFreqFrom;
                    actFreq.FreqValue.Great = planInfo.MHzFreqTo;
                    actFreq.FreqBand = planInfo.kHzBand.ToString();
                    actFreq.FreqId = planInfo.Key;
                    if (actFreq != null)
                        list.Add(actFreq);
                }
            }
            return list;
        }

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
            FreqLineDataItem FreqShowData = new FreqLineDataItem();
            FreqShowData.byteArray = new float[segData.numPoints];
            System.Runtime.InteropServices.Marshal.Copy(data, FreqShowData.byteArray, 0, Convert.ToInt32(segData.numPoints));
            FreqShowData.frequencyStep = segData.frequencyStep;
            FreqShowData.startFrequency = segData.startFrequency;
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
        //        return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanActivity>>(channel =>
        //        {
        //            return channel.GetFreqPlanActivitys(pPlaceId);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.GetExceptionMessage());
        //        return null;
        //    }
        //}

        private void listBoxEdit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var plan = listBoxEdit.SelectedItem as FreqPlanActivity;
            _currentPlan = plan;

            if (_currentPlan == null)
                return;
            if (_currentPlan.FreqValue != null)
            {
                x_widebandFreq.Clear();
                x_widebandFreq.MeasureUnit = "dBμV";
                x_widebandFreq.InitSpectrumProperty(null, null, _currentPlan.FreqValue.Little, _currentPlan.FreqValue.Great);
                x_widebandFreq.Update();
            }

            if (_timer != null)
            {
                _timer.Tick -= Timer_Tick;
                _timer.Stop();
            }
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(1);
            _timer.Start();
        }
    }
    public class FreqPlanNameConverter1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var objFreq = (FreqPlanActivity)value;
            return objFreq.FreqValue.Little + "-" + objFreq.FreqValue.Great;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
