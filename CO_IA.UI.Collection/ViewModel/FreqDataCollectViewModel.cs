using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
//using PWMIS.Core.Extensions;
//using PWMIS.DataProvider.Data;
//using PWMIS.DataProvider.Adapter;
//using PWMIS.Core;
//using PWMIS.DataMap.Entity;
using CO_IA.UI.Collection.helper;
using AgilentDll;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using CO_IA.UI.Collection.Chart;
using System.Threading;
using CO_IA.UI.Collection.Model;
using CO_IA.UI.Collection.DbEntity;
using System.Configuration;
using System.Collections.Concurrent;
using Best.VWPlatform.Common.Utility;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.NetworkInformation;
using CO_IA.Data.Collection;
using System.Collections.ObjectModel;
using CO_IA.Data.Portable.Collection;
using I_CO_IA.Collection;
using System.ComponentModel;

namespace CO_IA.UI.Collection.ViewModel
{
    public class FreqDataCollectViewModel : ViewModelBase, IDisposable
    {
        public event Action<List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>, List<Data.Collection.AnalysisResult>> AfterMonitor;

        FreqLineDataItem FreqShowDataGrid;
        private FreqTestStatus _TestStatus = FreqTestStatus.init;
        private Thread ThreadDbOperator;
        private DateTime collectTime;
        public List<Data.FreqPlanActivity> FreqPlanList;
        public Data.FreqPlanSegment FPS = null;
        /// <summary>
        /// 传递过来的装备集合
        /// </summary>
        public List<Data.ActivityEquipmentInfo> EquipmentList;
        /// <summary>
        /// 匹配到的装备集合
        /// </summary>
        public List<Data.ActivityEquipmentInfo> EquipmentListMate = new List<Data.ActivityEquipmentInfo>();
        /// <summary>
        /// 传过来的台站集合
        /// </summary>
        public List<CO_IA_Data.RoundStationInfo> RoundStationInfoList;
        /// <summary>
        /// 匹配到的台站集合
        /// </summary>
        public List<CO_IA_Data.RoundStationInfo> RoundStationInfoListMate = new List<CO_IA_Data.RoundStationInfo>();
        public Data.Activity Activity = null;
        public Data.ActivityPlace Place = null;
        public Data.MonitorPlan.DetailMonitorPlan MonitorPlan = null;

        public FreqDataCollectViewModel()
        {

            #region

            try
            {
                #endregion
                WinCloseCommand = new RelayCommand(() =>
                {
                    //if (_TestStatus != FreqTestStatus.pause && _TestStatus != FreqTestStatus.testing)
                    //    Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(this, "showmainwindow"));
                });

                #region 打开或关闭 接收机
                OpenReciverCommand = new RelayCommand(() =>
                {
                    if (pingEquipment(RoadTestInfor.ReciverIp))
                    {
                        if (!AgilentDll.Sensor.Connect(RoadTestInfor.ReciverIp))
                        {
                            Sensor.IsUseSensor = false;
                            Messenger.Default.Send<GenericMessage<string>>(new GenericMessage<string>(this, "ErrorConnectReciver"));
                            return;
                        }
                        else
                        {
                            Sensor.IsUseSensor = true;
                            ImgConectEable = System.Windows.Visibility.Visible;
                            ImgDisConectEable = System.Windows.Visibility.Collapsed;
                            TextColor = Brushes.Green;
                            TextStatus = "连接状态：设备准备就绪Sensor";
                        }
                    }
                }, () => { return !Sensor.IsUseSensor && _TestStatus != FreqTestStatus.testing && _TestStatus != FreqTestStatus.pause; });

                CloseReciverCommand = new RelayCommand(() =>
                {
                    ImgConectEable = System.Windows.Visibility.Collapsed;
                    ImgDisConectEable = System.Windows.Visibility.Visible;
                    TextColor = Brushes.Red;
                    TextStatus = "连接状态：设备未连接";
                    Sensor.Close();
                    Sensor.IsUseSensor = false;
                }, () => { return Sensor.IsUseSensor && _TestStatus != FreqTestStatus.testing && _TestStatus != FreqTestStatus.pause; ; });

                #endregion


                #region 频谱测量命令
                StopMeasurCommand = new RelayCommand(() =>
                {
                    BtnPauseText = "暂停";
                    Sensor.SendStopWbqexCmd();
                    _TestStatus = FreqTestStatus.stop;
                    _CollectionDataSave.closeSQLiteConnection();
                    _CollectionDataSave.Flag = 0;
                    UpdateTextBlockEnable();
                }, () =>
                {
                    return _TestStatus == FreqTestStatus.pause || _TestStatus == FreqTestStatus.testing;
                });

                StopRealTimeMonitorCommand = new RelayCommand(() =>
                {
                    BtnPauseText = "暂停";
                    Sensor.SendStopWbqexCmd();
                    _dTimer.Stop();
                    _dTimer = null;
                    _TestStatus = FreqTestStatus.stop;
                    UpdateTextBlockEnable();
                    dTimer.Stop();
                }, () =>
                {
                    return _TestStatus == FreqTestStatus.pause || _TestStatus == FreqTestStatus.testing;
                });

                PauseMeasurCommand = new RelayCommand(ExecutePauseMeasurCommand, CanExecutePauseMeasurCommand);
                StartMeasurCommand = new RelayCommand(ExecuteStartMeasurCommand, CanExecuteStartMeasurCommand);
                StartRealTimeMonitorCommand = new RelayCommand(ExecuteStartRealTimeMonitorCommand, CanExecuteStartRealTimeMonitorCommand);

                #endregion


                WinLoadedCommand = new RelayCommand(
                    () =>
                    {


                    }
                    );
                _LineChartViewModel = new ChartViewModel(this);
                _RtFreqDataModel = new RtFreqDataViewModel();
                _RtGpsDataModel = new RtGpsDataViewModel();
                _RtCdma2G_DataModel = new RtCdma2G_DataViewModel();
                _CarRunInfor = new CarRunInforViewModel();
                _RoadTestInfor = new RoadTestViewModel();
                _RoadTestInfor.AnalysisList = new System.Collections.ObjectModel.ObservableCollection<Data.Collection.AnalysisResult>();
                _CollectionDataSave = new Collection.CollectionDataSave();
                _CollectionDataSave.openSQLiteConnection();
                _CollectionDataSave.createTable();


                //ThreadDbOperator = new Thread(new ThreadStart(() =>
                //{

                //})) { IsBackground = true };
                //ThreadDbOperator.Start();
            }
            catch (Exception ex)
            {

                LogTool.Debug(ex.StackTrace);
                LogTool.Error("", ex);
            }
        }
        private string GetFreqTableName()
        {

#if !DEBUG
            //1.	频谱数据：Freq+车牌+日期+时间+开始频率+结束频率+分辨带宽
            return string.Format("Test_{1}_{0}_{2}Mhz_{3}Mhz_{4}", RoadTestInfor.CarPlate, DateTime.Now.ToString("yyyyMMddhhmmss"), RoadTestInfor.StartFreq, RoadTestInfor.EndFreq, RoadTestInfor.Bandwidth * 1000);
#else
            return "T_DataDebug";
#endif

        }
        #region 界面绑定实体对象
        private void UpdateTextBlockEnable()
        {
            if (_TestStatus != FreqTestStatus.init && _TestStatus != FreqTestStatus.stop)
            {
                TextBlockEable = true;

            }
            else
            {
                TextBlockEable = false;
            }
        }

        private bool pingEquipment(string ipAddress)
        {
            bool status = false;
            Ping pingSender = new Ping();

            PingReply reply = pingSender.Send(ipAddress, 120);//第一个参数为ip地址，第二个参数为ping的时间

            if (reply.Status == IPStatus.Success)
            {
                status = true;
            }
            else
            {
                status = false;
                TextStatus = "IP地址不可用或网络连接有问题";
            }
            return status;
        }

        private bool _TextBlockEable = false;
        /// <summary>
        /// 文本框 的eable状态
        /// </summary>
        public bool TextBlockEable
        {
            get
            {

                return _TextBlockEable;
            }
            set
            {
                Set(() => TextBlockEable, ref _TextBlockEable, value);
            }
        }


        private System.Windows.Visibility _ImgConectEable = System.Windows.Visibility.Collapsed;
        /// <summary>
        /// 图片显示状态
        /// </summary>
        public System.Windows.Visibility ImgConectEable
        {
            get
            {

                return _ImgConectEable;
            }
            set
            {
                Set(() => ImgConectEable, ref _ImgConectEable, value);
            }
        }

        private System.Windows.Visibility _ImgDisConectEable = System.Windows.Visibility.Visible;
        /// <summary>
        /// 图片显示状态
        /// </summary>
        public System.Windows.Visibility ImgDisConectEable
        {
            get
            {

                return _ImgDisConectEable;
            }
            set
            {
                Set(() => ImgDisConectEable, ref _ImgDisConectEable, value);
            }
        }

        private Brush _TextColor = Brushes.Red;
        /// <summary>
        /// 文字颜色
        /// </summary>
        public Brush TextColor
        {
            get
            {

                return _TextColor;
            }
            set
            {
                Set(() => TextColor, ref _TextColor, value);
            }
        }

        private string _TextStatus = "连接状态：设备未连接";
        /// <summary>
        /// 文字颜色
        /// </summary>
        public string TextStatus
        {
            get
            {

                return _TextStatus;
            }
            set
            {
                Set(() => TextStatus, ref _TextStatus, value);
            }
        }
        private string _BtnPauseText = "暂停";

        /// <summary>
        /// 暂停按钮的Text
        /// </summary>
        public string BtnPauseText
        {
            get
            {
                return _BtnPauseText;
            }
            set
            {
                Set(() => BtnPauseText, ref _BtnPauseText, value);
            }
        }

        public bool _ButtonStatus;

        /// <summary>
        /// 结束按钮状态
        /// </summary>
        public bool ButtonStatus
        {
            get
            {
                return _ButtonStatus;
            }
            set
            {
                Set(() => ButtonStatus, ref _ButtonStatus, value);
            }
        }

        private RtGpsDataViewModel _RtGpsDataModel;

        /// <summary>
        /// 实时Gps数据 描述
        /// </summary>
        public RtGpsDataViewModel RtGpsDataModel
        {
            get
            {
                return _RtGpsDataModel;
            }
            set
            {
                Set(() => RtGpsDataModel, ref _RtGpsDataModel, value);
            }
        }




        private RtCdma2G_DataViewModel _RtCdma2G_DataModel;

        /// <summary>
        /// 实时Cdma2G数据 描述
        /// </summary>
        public RtCdma2G_DataViewModel RtCdma2G_DataModel
        {
            get
            {
                return _RtCdma2G_DataModel;
            }
            set
            {
                Set(() => RtCdma2G_DataModel, ref _RtCdma2G_DataModel, value);
            }
        }


        private RtFreqDataViewModel _RtFreqDataModel;

        /// <summary>
        /// 实时频率数据 描述
        /// </summary>
        public RtFreqDataViewModel RtFreqDataModel
        {
            get
            {
                return _RtFreqDataModel;
            }
            set
            {
                Set(() => RtFreqDataModel, ref _RtFreqDataModel, value);
            }
        }



        private ChartViewModel _LineChartViewModel;
        /// <summary>
        /// 图表控件实体
        /// </summary>
        public ChartViewModel LineChartViewModel
        {
            get
            {
                return _LineChartViewModel;
            }
            set
            {
                Set(() => LineChartViewModel, ref _LineChartViewModel, value);
            }
        }

        private CollectionDataSave _CollectionDataSave;
        /// <summary>
        /// 采集数据存储SQLite
        /// </summary>
        public CollectionDataSave CollectionDataSave
        {
            get
            {
                return _CollectionDataSave;
            }
            set
            {
                Set(() => CollectionDataSave, ref _CollectionDataSave, value);
            }
        }

        private RoadTestViewModel _RoadTestInfor;
        /// <summary>
        /// 采集任务信息
        /// </summary>
        public RoadTestViewModel RoadTestInfor
        {
            get
            {
                return _RoadTestInfor;
            }
            set
            {
                Set(() => RoadTestInfor, ref _RoadTestInfor, value);
            }
        }



        private CarRunInforViewModel _CarRunInfor;
        /// <summary>
        /// 车辆运动信息
        /// </summary>
        public CarRunInforViewModel CarRunInfor
        {
            get
            {
                return _CarRunInfor;
            }
            set
            {
                Set(() => CarRunInfor, ref _CarRunInfor, value);
            }
        }


        private string _GpsSerPort = "COM2";

        /// <summary>
        /// Gps对应的串口号
        /// </summary>
        public string GpsSerPort
        {
            get
            {
                return _GpsSerPort;
            }
            set
            {
                Set(() => GpsSerPort, ref _GpsSerPort, value);
            }
        }

        private System.Windows.Threading.DispatcherTimer _dTimer;
        #endregion


        #region COMMAND

        public RelayCommand WinCloseCommand { get; private set; }


        public RelayCommand OpenGpsCommand { get; private set; }
        public RelayCommand CLoseGpsCommand { get; private set; }
        public RelayCommand OpenReciverCommand { get; private set; }
        public RelayCommand CloseReciverCommand { get; private set; }



        public RelayCommand StopMeasurCommand { get; private set; }
        public RelayCommand StartMeasurCommand { get; private set; }

        public RelayCommand StopRealTimeMonitorCommand { get; private set; }
        public RelayCommand StartRealTimeMonitorCommand { get; private set; }

        public RelayCommand PauseMeasurCommand { get; private set; }
        public RelayCommand WinLoadedCommand { get; private set; }

        private string CurrentTestTableName;
        void ExecuteStartMeasurCommand()
        {
            if (!_CollectionDataSave.IsConnectionOpen)
            {
                _CollectionDataSave.openSQLiteConnection();
            }
            CurrentTestTableName = GetFreqTableName();
            if (DbHelper.CreateFreqTable(CurrentTestTableName))
            {
                RoadTestInfor.FreqDataTable = CurrentTestTableName;
            }
            DbHelper.CreateCdma1xTable(CurrentTestTableName);

            if (Sensor.IsUseSensor)
            {
                //存储任务信息
                RoadTest rt = new RoadTest();
                rt.Bandwidth = Convert.ToInt32(RoadTestInfor.Bandwidth * 1000);
                rt.CarPlate = RoadTestInfor.CarPlate;
                //rt.EndFreq = RoadTestInfor.EndFreq;
                rt.ReciverIp = RoadTestInfor.ReciverIp;
                rt.RoadTestName = RoadTestInfor.RoadTestName;
                //rt.StartFreq = RoadTestInfor.StartFreq;
                rt.TestDateTime = DateTime.Now;
                rt.TestSample = RoadTestInfor.TestSample;
                rt.TestStaffName = RoadTestInfor.TestStaffName;
                rt.Bz = "";
                rt.ReciverPort = 0;
                rt.FreqDataTable = RoadTestInfor.FreqDataTable;

                //if (EntityQuery<RoadTest>.Instance.Insert(rt) > 0)
                {
                    //Clear()
                    _LineChartViewModel.Clear();
                    FreqMeasureId = 0;
                    FreqMeasurePakageId = 0;
                    FreqDataIndex = 0;
                    _TestStatus = FreqTestStatus.testing;
                    _CollectionDataSave.MeasureID = "Measure" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    IsRecordFreqCount = true;
                    endFreq = Convert.ToDouble(RoadTestInfor.EndFreq) * 1000 * 1000.0;
                    _proc = CallBack;
                    AgilentDll.Sensor.SendWbqexCmd(RoadTestInfor.StartFreq * 1000000.0, RoadTestInfor.EndFreq * 1000000.0, RoadTestInfor.Bandwidth * 1000.0, _proc);
                }
                collectTime = DateTime.Now;
                UpdateTextBlockEnable();
                return;
            }
        }


        bool CanExecuteStartMeasurCommand()
        {
            if (!Sensor.IsUseSensor)
            {
                return false;
            }

            if (string.IsNullOrEmpty(RoadTestInfor.CarPlate))
                return false;
            if (_TestStatus == FreqTestStatus.init || _TestStatus == FreqTestStatus.stop)
                return RoadTestInfor.StartFreq > 0 && RoadTestInfor.EndFreq > 0 && RoadTestInfor.Bandwidth > 0;
            else
                return false;
        }

        System.Windows.Threading.DispatcherTimer dTimer = new System.Windows.Threading.DispatcherTimer();
        void ExecuteStartRealTimeMonitorCommand()
        {
            //CurrentTestTableName = GetFreqTableName();
            //if (DbHelper.CreateFreqTable(CurrentTestTableName))
            //{
            //    RoadTestInfor.FreqDataTable = CurrentTestTableName;
            //}
            //DbHelper.CreateCdma1xTable(CurrentTestTableName);

            if (Sensor.IsUseSensor)
            {
                //存储任务信息
                RoadTest rt = new RoadTest();
                rt.Bandwidth = Convert.ToInt32(RoadTestInfor.Bandwidth * 1000);
                rt.CarPlate = RoadTestInfor.CarPlate;
                //rt.EndFreq = RoadTestInfor.EndFreq;
                rt.ReciverIp = RoadTestInfor.ReciverIp;
                rt.RoadTestName = RoadTestInfor.RoadTestName;
                //rt.StartFreq = RoadTestInfor.StartFreq;
                rt.TestDateTime = DateTime.Now;
                rt.TestSample = RoadTestInfor.TestSample;
                rt.TestStaffName = RoadTestInfor.TestStaffName;
                rt.Bz = "";
                rt.ReciverPort = 0;
                rt.FreqDataTable = RoadTestInfor.FreqDataTable;

                //if (EntityQuery<RoadTest>.Instance.Insert(rt) > 0)
                {
                    //Clear()
                    _LineChartViewModel.Clear();
                    FreqMeasureId = 0;
                    FreqMeasurePakageId = 0;
                    FreqDataIndex = 0;
                    _TestStatus = FreqTestStatus.testing;
                    IsRecordFreqCount = true;
                    endFreq = Convert.ToDouble(RoadTestInfor.EndFreq) * 1000 * 1000.0;
                    _proc = RealTimeMonitorCallBack;
                    AgilentDll.Sensor.SendWbqexCmd(RoadTestInfor.StartFreq * 1000000.0, RoadTestInfor.EndFreq * 1000000.0, RoadTestInfor.Bandwidth * 1000.0, _proc);
                    _dTimer = new System.Windows.Threading.DispatcherTimer();
                    _dTimer.Tick += new EventHandler(dTimer_Tick);
                    _dTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                    _dTimer.Start();
                }
                collectTime = DateTime.Now;
                UpdateTextBlockEnable();
                return;
            }
        }


        bool CanExecuteStartRealTimeMonitorCommand()
        {
            if (!Sensor.IsUseSensor)
            {
                return false;
            }

            if (string.IsNullOrEmpty(RoadTestInfor.CarPlate))
                return false;
            if (_TestStatus == FreqTestStatus.init || _TestStatus == FreqTestStatus.stop)
                return RoadTestInfor.StartFreq > 0 && RoadTestInfor.EndFreq > 0 && RoadTestInfor.Bandwidth > 0 && RoadTestInfor.SignalLimit > 0;
            else
                return false;
        }

        private bool CanExecutePauseMeasurCommand()
        {
            switch (_TestStatus)
            {
                case FreqTestStatus.init:
                case FreqTestStatus.stop:

                    return false;
                    break;
                case FreqTestStatus.testing:
                case FreqTestStatus.pause:
                    return true;
                    break;
            }

            return false;
        }

        private void ExecutePauseMeasurCommand()
        {
            try
            {
                if (_TestStatus == FreqTestStatus.testing)
                {
                    BtnPauseText = "继续";
                    _TestStatus = FreqTestStatus.pause;
                    Sensor.SendStopWbqexCmd();

                }
                else
                {
                    BtnPauseText = "暂停";
                    _TestStatus = FreqTestStatus.testing;
                    endFreq = Convert.ToDouble(RoadTestInfor.EndFreq) * 1000 * 1000.0;
                    _proc = CallBack;
                    AgilentDll.Sensor.SendWbqexCmd(RoadTestInfor.StartFreq * 1000000.0, RoadTestInfor.EndFreq * 1000000.0, RoadTestInfor.Bandwidth * 1000.0, _proc);

                }

                UpdateTextBlockEnable();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        #region 接受机数据处理
        private int _FreqDataIndex = 0;
        private int FreqDataIndex
        {
            get
            {
                if (_FreqDataIndex + 1 == int.MaxValue)
                    _FreqDataIndex = 0;
                return _FreqDataIndex;
            }
            set
            {

                if (value + 1 == int.MaxValue)
                    _FreqDataIndex = 0;
                else
                {
                    _FreqDataIndex = value;
                }
            }
        }


        private int _FreqMeasurePakageId = 0;

        /// <summary>
        /// 某小段的 序号 （对应一个全频段中某个段频谱数据）
        /// </summary>
        private int FreqMeasurePakageId
        {
            get
            {
                if (_FreqMeasurePakageId == int.MaxValue)
                    _FreqMeasurePakageId = 0;

                return _FreqMeasurePakageId;
            }
            set
            {
                _FreqMeasurePakageId = value;
            }
        }



        private int _FreqMeasureId = 0;

        /// <summary>
        /// 全频段扫描中的 序号 （对应多段频谱数据）
        /// </summary>
        private int FreqMeasureId
        {
            get
            {
                if (_FreqMeasureId == int.MaxValue)
                    _FreqMeasureId = 0;

                return _FreqMeasureId;
            }
            set
            {
                _FreqMeasureId = value;
            }
        }

        //private GpsLib.Gps.Gps_CALLBACK _Gps_Proc;
        private AgilentDll.Sensor.SAL_SEGMENT_CALLBACK _proc;
        //private AgilentDll.Decoder_CDMA2G.CDMA2G_CALLBACK _CDMA2G_CallBack;


        ///注释2016.7.25
        //public bool CDMA2G_CALLBACK(ref AgilentDll.Decoder_CDMA2G.Decoder_CDMA2G_Data cdmaData)
        //{
        //    if (cdmaData.success == 0)
        //        return false;

        //    return true;
        //}

        //public int My_Gps_CALLBACK(ref GpsLib.Gps.Decoder_Gps_Data GpsDataHeader)
        //{
        //    if (GpsDataHeader.LocationState == 'V')
        //        return -1;

        //    CarRunInfor.AltitudeValue = GpsDataHeader.Height;
        //    CarRunInfor.CarSpeed = GpsDataHeader.Speed;
        //    CarRunInfor.LongitudeValue = GpsDataHeader.Longitude;
        //    CarRunInfor.LatitudeValue = GpsDataHeader.Latitude;
        //    CarRunInfor.SatelliteCount = GpsDataHeader.UsingStartNum;


        //    RtGpsDataModel.Altitude = GpsDataHeader.Height;
        //    RtGpsDataModel.Speed = GpsDataHeader.Speed;
        //    RtGpsDataModel.Longitude = GpsDataHeader.Longitude;
        //    RtGpsDataModel.Latitude = GpsDataHeader.Latitude;
        //    RtGpsDataModel.SatelliteCount = GpsDataHeader.UsingStartNum;
        //    return 0;
        //}

        ConcurrentQueue<FreqFrameItem> FreqFrameItemQueue = new ConcurrentQueue<FreqFrameItem>();
        FreqFrameItem CurrentFreqFrameItem;
        DateTime beforDT = System.DateTime.Now;
        private double endFreq;
        int FreqCount = 0;
        bool IsRecordFreqCount = true;
        private int CallBack(ref AgilentDll.Sensor.SegmentData segData, IntPtr data)
        {
            if (segData.errorNum != AgilentDll.Sensor.SalError.SAL_ERR_NONE)
                return -1;
            if (segData.startFrequency <= RoadTestInfor.StartFreq * 1000000.0)
            {
                FreqMeasureId++;
                FreqMeasurePakageId = 0;
            }
            FreqMeasurePakageId++;
            FreqDataIndex++;

            if (FreqMeasurePakageId == 1)
            {
                if (CurrentFreqFrameItem != null)
                {
                    lock (FreqFrameItemQueue)
                    {
                        FreqFrameItemQueue.Enqueue(CurrentFreqFrameItem);
                        Monitor.Pulse(FreqFrameItemQueue);
                    }
                }
                TimeSpan ts = System.DateTime.Now.Subtract(beforDT);
                CurrentFreqFrameItem = new FreqFrameItem(FreqMeasureId, ts);
            }

            FreqDataTemplate FreqData = new FreqDataTemplate();
            //FreqData.MapNewTableName(RoadTestInfor.FreqDataTable);
            FreqData.FreqCount = Convert.ToInt32(segData.numPoints);
            FreqData.AltitudeValue = CarRunInfor.AltitudeValue;
            FreqData.LatitudeValue = CarRunInfor.LatitudeValue;
            FreqData.LongitudeValue = CarRunInfor.LongitudeValue;
            FreqData.SatelliteCount = CarRunInfor.SatelliteCount;
            FreqData.CarSpeed = CarRunInfor.CarSpeed;
            FreqData.DataIndex = FreqDataIndex;
            FreqData.FreqMeasurePakageId = FreqMeasurePakageId;
            FreqData.MeasureId = FreqMeasureId;
            FreqData.MeasureTime = DateTime.Now;
            FreqData.StartFreq = segData.startFrequency;
            FreqData.StepValue = segData.frequencyStep;
            FreqData.segData = segData;
            FreqData.volList = new float[segData.numPoints];
            System.Runtime.InteropServices.Marshal.Copy(data, FreqData.volList, 0, Convert.ToInt32(segData.numPoints));

            CurrentFreqFrameItem.FreqDataItemList.Add(FreqData);


            FreqLineDataItem FreqShowData = new FreqLineDataItem();
            FreqShowData.byteArray = new float[segData.numPoints];
            System.Runtime.InteropServices.Marshal.Copy(data, FreqShowData.byteArray, 0, Convert.ToInt32(segData.numPoints));
            FreqShowData.FreqMeasureId = FreqMeasureId;
            FreqShowData.FreqMeasurePakageId = FreqMeasurePakageId;
            FreqShowData.frequencyStep = segData.frequencyStep;
            FreqShowData.startFrequency = segData.startFrequency;
            FreqShowData.TestFreqStart = RoadTestInfor.StartFreq;
            FreqShowData.TestFreqEnd = RoadTestInfor.EndFreq;
            FreqShowData.DataIndex = FreqDataIndex;


            _LineChartViewModel.InsertShowData(FreqShowData);

            DateTime now = DateTime.Now;
            if (collectTime.AddMilliseconds(1000 * 60 * 15).Hour == now.Hour && collectTime.AddMilliseconds(1000 * 60 * 15).Minute == now.Minute && collectTime.AddMilliseconds(1000 * 60 * 15).Second == now.Second)
            {
                _CollectionDataSave.saveSqliteIndex(FreqShowData);
                collectTime = now;
            }
            if (IsRecordFreqCount)
            {
                if (FreqCount != 0 && RoadTestInfor.StartFreq * 1000000 == FreqShowData.startFrequency)
                {
                    _CollectionDataSave.saveFreqCount(FreqCount);
                    IsRecordFreqCount = false;
                }
                else
                {
                    FreqCount += FreqShowData.byteArray.Length;
                }
            }
            _CollectionDataSave.saveFreqData(FreqShowData.byteArray, FreqShowData);

            //UpdateRtFreqDataModel(segData);


            return 0;
        }

        Dictionary<int, float[]> segDataDic = new Dictionary<int, float[]>();
        /// <summary>
        /// 实时监测回调
        /// </summary>
        /// <param name="segData"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private int RealTimeMonitorCallBack(ref AgilentDll.Sensor.SegmentData segData, IntPtr data)
        {
            if (segData.errorNum != AgilentDll.Sensor.SalError.SAL_ERR_NONE)
                return -1;
            if (segData.startFrequency <= RoadTestInfor.StartFreq * 1000000.0)
            {
                FreqMeasureId++;
                FreqMeasurePakageId = 0;
            }
            FreqMeasurePakageId++;
            FreqDataIndex++;

            if (FreqMeasurePakageId == 1)
            {
                if (CurrentFreqFrameItem != null)
                {
                    lock (FreqFrameItemQueue)
                    {
                        FreqFrameItemQueue.Enqueue(CurrentFreqFrameItem);
                        Monitor.Pulse(FreqFrameItemQueue);
                    }
                }
                TimeSpan ts = System.DateTime.Now.Subtract(beforDT);
                CurrentFreqFrameItem = new FreqFrameItem(FreqMeasureId, ts);
            }

            FreqDataTemplate FreqData = new FreqDataTemplate();
            //FreqData.MapNewTableName(RoadTestInfor.FreqDataTable);
            FreqData.FreqCount = Convert.ToInt32(segData.numPoints);
            FreqData.AltitudeValue = CarRunInfor.AltitudeValue;
            FreqData.LatitudeValue = CarRunInfor.LatitudeValue;
            FreqData.LongitudeValue = CarRunInfor.LongitudeValue;
            FreqData.SatelliteCount = CarRunInfor.SatelliteCount;
            FreqData.CarSpeed = CarRunInfor.CarSpeed;
            FreqData.DataIndex = FreqDataIndex;
            FreqData.FreqMeasurePakageId = FreqMeasurePakageId;
            FreqData.MeasureId = FreqMeasureId;
            FreqData.MeasureTime = DateTime.Now;
            FreqData.StartFreq = segData.startFrequency;
            FreqData.StepValue = segData.frequencyStep;
            FreqData.segData = segData;
            FreqData.volList = new float[segData.numPoints];
            System.Runtime.InteropServices.Marshal.Copy(data, FreqData.volList, 0, Convert.ToInt32(segData.numPoints));

            CurrentFreqFrameItem.FreqDataItemList.Add(FreqData);


            FreqLineDataItem FreqShowData = new FreqLineDataItem();
            FreqShowData.byteArray = new float[segData.numPoints];
            System.Runtime.InteropServices.Marshal.Copy(data, FreqShowData.byteArray, 0, Convert.ToInt32(segData.numPoints));
            FreqShowData.FreqMeasureId = FreqMeasureId;
            FreqShowData.FreqMeasurePakageId = FreqMeasurePakageId;
            FreqShowData.frequencyStep = segData.frequencyStep;
            FreqShowData.startFrequency = segData.startFrequency;
            FreqShowData.TestFreqStart = RoadTestInfor.StartFreq;
            FreqShowData.TestFreqEnd = RoadTestInfor.EndFreq;
            FreqShowData.DataIndex = FreqDataIndex;

            _LineChartViewModel.InsertShowData(FreqShowData);

            if (!segDataDic.ContainsKey(FreqMeasurePakageId))
            {
                segDataDic.Add(FreqMeasurePakageId, FreqShowData.byteArray);
            }
            if (FreqShowDataGrid == null || FreqShowDataGrid.byteArray.Length <= FreqShowData.byteArray.Length)
                FreqShowDataGrid = FreqShowData;


            //ObservableCollection<AnalysisResult> tempList = new ObservableCollection<AnalysisResult>();
            //double startFreq = FreqShowData.startFrequency;
            //for (int i = 0; i < FreqShowData.byteArray.Length; i++)
            //{
            //    if (FreqShowData.byteArray[i] + 107 >= Convert.ToDouble(RoadTestInfor.SignalLimit))
            //    {
            //        AnalysisResult analysisResult = new AnalysisResult();

            //        analysisResult.Frequency = startFreq;
            //        analysisResult.BandWidth = RoadTestInfor.Bandwidth;
            //        analysisResult.AmplitudeMaxValue = Convert.ToInt32(FreqShowData.byteArray[i])+107;
            //        tempList.Add(analysisResult);
            //    }
            //    else {
            //        startFreq += Convert.ToDouble(RoadTestInfor.Bandwidth);
            //    }
            //}
            //RoadTestInfor.AnalysisList = tempList;
            //DateTime now = DateTime.Now;
            //if (collectTime.AddMilliseconds(1000 * 60 * 15).Hour == now.Hour && collectTime.AddMilliseconds(1000 * 60 * 15).Minute == now.Minute && collectTime.AddMilliseconds(1000 * 60 * 15).Second == now.Second)
            //{
            //    _CollectionDataSave.saveSqliteIndex(FreqShowData);
            //    collectTime = now;
            //}
            //if (IsRecordFreqCount)
            //{
            //    if (FreqCount != 0 && RoadTestInfor.StartFreq * 1000000 == FreqShowData.startFrequency)
            //    {
            //        _CollectionDataSave.saveFreqCount(FreqCount);
            //        IsRecordFreqCount = false;
            //    }
            //    else
            //    {
            //        FreqCount += FreqShowData.byteArray.Length;
            //    }
            //}
            //_CollectionDataSave.saveFreqData(FreqShowData.byteArray, FreqShowData);

            //UpdateRtFreqDataModel(segData);


            return 0;
        }
#if __NOTUSE
        private void UpdateRtFreqDataModel(AgilentDll.Sensor.SegmentData segData)
        {
            RtFreqDataModel.FreqMeasureNO = string.Format("{0}-{1}-{2}", FreqMeasureId, FreqMeasurePakageId, FreqDataIndex);
            RtFreqDataModel.FreqDataUpdateTime = DateTime.Now.ToLongTimeString();
            RtFreqDataModel.frequencyStep = segData.frequencyStep;
            RtFreqDataModel.startFrequency = segData.startFrequency / 1000000.0;
            RtFreqDataModel.FreqDataCount = segData.numPoints;
        }

#endif

        public void UpdateRtFreqDataModel(FreqLineDataItem segData)
        {
            RtFreqDataModel.FreqMeasureNO = string.Format("{0}-{1}-{2}", FreqMeasureId, FreqMeasurePakageId, FreqDataIndex);
            RtFreqDataModel.FreqDataUpdateTime = DateTime.Now.ToLongTimeString();
            RtFreqDataModel.frequencyStep = segData.frequencyStep;
            RtFreqDataModel.startFrequency = segData.startFrequency / 1000000.0;
            RtFreqDataModel.FreqDataCount = (uint)segData.byteArray.Length;
        }


        private void dTimer_Tick(object sender, EventArgs e)
        {
            if (_dTimer.Interval.TotalMilliseconds != 5000)
            {
                _dTimer.Interval = new TimeSpan(0, 0, 0, 0, 5000);
            }
            if (FreqShowDataGrid == null)
                return;
            EquipmentListMate.Clear();
            RoundStationInfoListMate.Clear();
            ObservableCollection<AnalysisResult> tempList = new ObservableCollection<AnalysisResult>();
            double startFreq = FreqShowDataGrid.TestFreqStart * 1000;
            ObservableCollection<AnalysisResult> analysisList = new ObservableCollection<AnalysisResult>();
            ObservableCollection<AnalysisResult> illegalAnalysisList = new ObservableCollection<AnalysisResult>();

            #region 废弃
            //foreach (var tmp in EquipmentList)
            //{
            //    AnalysisResult analysisResult = new AnalysisResult();
            //    analysisResult.Frequency = tmp.SendFreq != null ? tmp.SendFreq.Value : 0;// startFreq / 1000;
            //    analysisResult.BandWidth = RoadTestInfor.Bandwidth;

            //    //segDataDic.Where((obj)=> {return obj.Value })
            //    int i = (int)(tmp.SendFreq.Value / Convert.ToDouble(RoadTestInfor.Bandwidth));

            //    analysisResult.AmplitudeMaxValue = Convert.ToInt32(kvp.Value[i]) + 107;
            //    if (kvp.Value[i] + 107 >= Convert.ToDouble(RoadTestInfor.SignalLimit))
            //    {
            //        analysisResult.SendStatusPic = @"/CO_IA.UI.Collection;component/Images/right_24x24.png";
            //    }
            //    else
            //    {
            //        analysisResult.SendStatusPic = @"/CO_IA.UI.Collection;component/Images/right_24x24.png";
            //    }
            //    tempList.Add(analysisResult);
            //} 
            #endregion

            //逻辑 根据当前业务注册的设备列表的发射频率区段判断
            //1、根据测量的频率生成测量的结果列表
            foreach (KeyValuePair<int, float[]> kvp in segDataDic)
            {
                for (int i = 0; i < kvp.Value.Length; i++)
                {
                    //if (string.IsNullOrEmpty(RoadTestInfor.SignalLimit))
                    //{
                    //    RoadTestInfor.SignalLimit = "20";
                    //}                   
                    if (kvp.Value[i] + 107 >= RoadTestInfor.SignalLimit)
                    {
                        AnalysisResult analysisResult = new AnalysisResult();
                        analysisResult.Frequency = startFreq / 1000;
                        analysisResult.BandWidth = RoadTestInfor.Bandwidth;
                        analysisResult.AmplitudeMaxValue = Convert.ToInt32(kvp.Value[i]) + 107;
                        analysisResult.SendStatusPic = @"/CO_IA.UI.Collection;component/Images/right_24x24.png";
                        analysisResult.IsSend = true;
                        tempList.Add(analysisResult);
                        startFreq += Convert.ToDouble(RoadTestInfor.Bandwidth);
                    }
                    else
                    {
                        //    AnalysisResult analysisResult = new AnalysisResult();
                        //    analysisResult.Frequency = startFreq / 1000;
                        //    analysisResult.BandWidth = RoadTestInfor.Bandwidth;
                        //    analysisResult.AmplitudeMaxValue = Convert.ToInt32(kvp.Value[i]) + 107;
                        //    analysisResult.IsSend = false;
                        //    analysisResult.SendStatusPic = @"/CO_IA.UI.Collection;component/Images/wrong_24x24.png";
                        //    tempList.Add(analysisResult);
                        startFreq += Convert.ToDouble(RoadTestInfor.Bandwidth);
                    }
                }
            }
            //2、传递过来的设备列表与测量到的频率列表进行比对
            if (EquipmentList != null)
            {
                //循环测量列表，找到与之相符合的传递过来的设备列表中的项
                foreach (var tmp in tempList)
                {
                    //看该信号有没有设备发送
                    var equiment = EquipmentList.Find(obj => obj.AssignFreq == tmp.Frequency);

                    //如果信号有发送设备，那么写入信号列表
                    if (equiment != null)
                    {
                        tmp.EquimentName = equiment.Name;
                        tmp.StationName = equiment.StationName;
                        tmp.FreqGuid = equiment.GUID;
                        analysisList.Add(tmp);
                        if (!EquipmentListMate.Contains(equiment))
                            EquipmentListMate.Add(equiment);
                    }
                    //如果没有发送设备，那么就是冲突信号了
                    else
                    {
                        tmp.SendStatusPic = @"/CO_IA.UI.Collection;component/Images/right_24x24.png";
                        illegalAnalysisList.Add(tmp);
                    }

                    //找到了台站
                    var roundStation = RoundStationInfoList.Find(obj => obj.FreqPart.FreqValue.Little <= tmp.Frequency && obj.FreqPart.FreqValue.Great >= tmp.Frequency);
                    //如果能找到匹配的台站，那么在台站匹配列表中加入项
                    if (roundStation != null)
                    {
                        if (!RoundStationInfoListMate.Contains(roundStation))
                            RoundStationInfoListMate.Add(roundStation);
                    }
                }

                //循环传递过来的设备列表
                foreach (var equment in EquipmentList)
                {
                    //找到传递的设备列表中未能匹配到且频率值在所设置范围内的加入到结果列表中，并将其发射状态设置为未发射
                    if (!EquipmentListMate.Contains(equment)) // && equment.SendFreq.Value >= FreqShowDataGrid.TestFreqStart && equment.SendFreq.Value <= FreqShowDataGrid.TestFreqEnd)
                    {
                        AnalysisResult analysisResult = new AnalysisResult();
                        analysisResult.FreqGuid = equment.GUID;
                        analysisResult.EquimentName = equment.Name;
                        analysisResult.StationName = equment.StationName;
                        analysisResult.Frequency = equment.AssignFreq != null ? equment.AssignFreq.Value : 0;
                        analysisResult.BandWidth = RoadTestInfor.Bandwidth;
                        analysisResult.IsSend = false;
                        //analysisResult.AmplitudeMaxValue = Convert.ToInt32(kvp.Value[i]) + 107;
                        analysisResult.SendStatusPic = @"/CO_IA.UI.Collection;component/Images/wrong_24x24.png";
                        if (analysisList.Where(obj => { return obj.Frequency == analysisResult.Frequency; }).Count() <= 0)
                        {
                            analysisList.Add(analysisResult);
                        }
                    }
                }
            }
            RoadTestInfor.AnalysisList = new ObservableCollection<AnalysisResult>(analysisList.OrderBy(obj => obj.Frequency));
            RoadTestInfor.IllegalAnalysisList = illegalAnalysisList;
            segDataDic.Clear();

            //3、上传分析结果
            if (FPS == null)
                return;
            if (!RoadTestInfor.SaveData)
                return;

            var backbroudWorker = new BackgroundWorker();
            backbroudWorker.DoWork += ((s, p) =>
            {

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, bool>(channel =>
                {
                    bool result0 = true;
                    bool result1 = true;
                    bool result2 = true;

                    channel.RemoveRealTimeMonitor(FPS.ClassCode, Activity.Guid, MonitorPlan.GUID);

                    result0 = analysisList == null || analysisList.Count == 0 ? false : channel.UpLoadRealTimeMonitor(FPS.ClassCode, Activity.Guid, MonitorPlan.GUID, analysisList.ToList(), '0');
                    result1 = illegalAnalysisList == null || illegalAnalysisList.Count == 0 ? false : channel.UpLoadRealTimeMonitor(FPS.ClassCode, Activity.Guid, MonitorPlan.GUID, illegalAnalysisList.ToList(), '1');
                    result1 = FreqShowDataGrid == null ? false : channel.UpLoadSpectrumData(FPS.ClassCode, Activity.Guid, MonitorPlan.GUID, FreqShowDataGrid);
                    return result0 && result1 && result2;
                });
            });
            backbroudWorker.RunWorkerCompleted += ((s, p) =>
            {
                if (AfterMonitor != null)
                    AfterMonitor(analysisList.Where((obj) => { return obj.IsSend; }).ToList(), analysisList.Where((obj) => { return !obj.IsSend; }).ToList(), illegalAnalysisList.ToList());
            });
            backbroudWorker.RunWorkerAsync();
        }

        private bool SavaFreqDataToDataBase(AgilentDll.Sensor.SegmentData segData, IntPtr data)
        {
            return true;
        }

        //private Queue<FreqDataTemplate> dBFreqLineDataItemBuffer = new Queue<FreqDataTemplate>();

        //public void InsertShowData(FreqDataTemplate data)
        //{
        //    lock (dBFreqLineDataItemBuffer)
        //    {
        //        dBFreqLineDataItemBuffer.Enqueue(data);
        //        Monitor.Pulse(dBFreqLineDataItemBuffer);
        //    }
        //}


        private bool ShowFreqData(AgilentDll.Sensor.SegmentData segData, IntPtr data)
        {
            return false;
        }



        #endregion
        #endregion


        public void Dispose()
        {
            if (ThreadDbOperator != null)
                ThreadDbOperator.Abort();
        }
        public override void Cleanup()
        {
            // Clean up if needed
            this.Dispose();
            base.Cleanup();
        }
    }
}
