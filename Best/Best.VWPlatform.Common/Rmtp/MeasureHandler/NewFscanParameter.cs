using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 频段扫描测量参数
    /// </summary>
    public sealed class NewFscanParameter : INotifyPropertyChanged
    {
        /// <summary>
        /// 起始频率
        /// </summary>
        private double _startFreq = 87;
        private string _startFreqUnit = "MHz";
        /// <summary>
        /// 终止频率
        /// </summary>
        private double _stopFreq = 108;
        private string _stopFreqUnit = "MHz";
        /// <summary>
        /// 步进
        /// </summary>
        private double _step = 100;
        private string _stepUnit = "kHz";
        /// <summary>
        /// 中频带宽
        /// </summary>
        private double _ifbw = 100000;
        private string _ifbwUnit = "Hz";
        /// <summary>
        /// 增益模式
        /// </summary>
        private string _gainmode = "agc";
        /// <summary>
        /// 人工增益参数，单位dB
        /// </summary>
        private int _gainparam = 10;
        /// <summary>
        /// 极化方式
        /// </summary>
        private string _polarization = "vertical";
        /// <summary>
        /// 检波方式
        /// </summary>
        private string _smoothmode = "rms";
        /// <summary>
        /// 检波方式参数
        /// </summary>
        private int _smoothparam = 128;
        /// <summary>
        /// 监测信号门限类型
        /// </summary>
        private string _thresholdtype = "horizontal";
        /// <summary>
        /// 监测信号门限
        /// </summary>
        private double _signalThreshold = 0;
        /// <summary>
        /// 启用信号门限
        /// </summary>
        private bool _hasThreshold = true;
        /// <summary>
        /// 预学习时长，用于电磁背景，不作为参数指令使用
        /// </summary>
        private int _learningTime = 10;
        /// <summary>
        /// 自定义门限，用于电磁背景
        /// </summary>
        private string _thresholddesc;
        /// <summary>
        /// 自定义门限值
        /// </summary>
        private string _customthresholdvalues;

        public NewFscanParameter()
        {
           
        }
        /// <summary>
        /// 测量起始频率
        /// </summary>
        public double StartFreq
        {
            get { return _startFreq; }
            set
            {
                _startFreq = value;
                OnPropertyChanged("StartFreq");
            }
        }
        /// <summary>
        /// 测量起始频率单位
        /// </summary>
        public string StartFreqUnit
        {
            get { return _startFreqUnit; }
            set { _startFreqUnit = value; OnPropertyChanged("StartFreqUnit"); }
        }
        /// <summary>
        /// 测量终止频率
        /// </summary>
        public double StopFreq
        {
            get { return _stopFreq; }
            set
            {
                _stopFreq = value;
                OnPropertyChanged("StopFreq");
            }
        }
        /// <summary>
        /// 测量终止频率单位
        /// </summary>
        public string StopFreqUnit
        {
            get { return _stopFreqUnit; }
            set { _stopFreqUnit = value; OnPropertyChanged("StopFreqUnit"); }
        }
        /// <summary>
        /// 步进 step Hz
        /// </summary>
        public double Step
        {
            get { return _step; }
            set
            {
                _step = value;
                OnPropertyChanged("Step");
            }
        }

        /// <summary>
        /// 步进单位
        /// </summary>
        public string StepUnit
        {
            get { return _stepUnit; }
            set
            {
                _stepUnit = value;
                OnPropertyChanged("StepUnit");
            }
        }
        /// <summary>
        /// 分析带宽
        /// </summary>
        public double Ifbw
        {
            get
            {
                return _ifbw;
            }
            set { _ifbw = value; OnPropertyChanged("Ifbw"); }
        }

        /// <summary>
        /// 测量分辨率单位
        /// </summary>
        public string IfbwUnit
        {
            get
            {
                return _ifbwUnit;
            }
            set { _ifbwUnit = value; OnPropertyChanged("IfbwUnit"); }
        }
        /// <summary>
        /// 增益模式
        /// </summary>
        public string GainMode
        {
            get
            {
                var s = Enum.Parse(typeof(GainMode), _gainmode, true);
                var item = from i in RmtpDefaultCollection.GainModeSource where i.Value.Equals(s) select i.Key;
                return item.First();
            }
            set
            {
                _gainmode = RmtpDefaultCollection.GainModeSource[value].ToString();
                OnPropertyChanged("GainMode");
            }
        }
        /// <summary>
        /// 人工增益参数
        /// </summary>
        public int GainParam
        {
            get
            {
                return _gainparam;
            }
            set
            {
                _gainparam = value;
                OnPropertyChanged("GainParam");
            }
        }
        /// <summary>
        /// 极化方式
        /// </summary>
        public string Polarization
        {
            get
            {
                var p = Enum.Parse(typeof(Polarization), _polarization, true);
                var item = from i in RmtpDefaultCollection.PolarizationSource where i.Value.Equals(p) select i.Key;
                return item.First();
            }
            set
            {
                _polarization = RmtpDefaultCollection.PolarizationSource[value].ToString();
                OnPropertyChanged("Polarization");
            }
        }
        /// <summary>
        /// 检波方式
        /// </summary>
        public string SmoothMode
        {
            get
            {
                var p = Enum.Parse(typeof(Detector), _smoothmode, true);
                var item = from i in RmtpDefaultCollection.DetectorSource where i.Value.Equals(p) select i.Key;
                return item.First();
            }
            set
            {
                _smoothmode = RmtpDefaultCollection.DetectorSource[value].ToString();
                OnPropertyChanged("SmoothMode");
            }
        }
        /// <summary>
        /// 平滑参数
        /// </summary>
        public int SmoothParam
        {
            get
            {
                return _smoothparam;
            }
            set
            {
                _smoothparam = value;
                OnPropertyChanged("SmoothParam");
            }
        }
        /// <summary>
        /// 启用信号门限
        /// </summary>
        public bool HasThreshold
        {
            get { return _hasThreshold; }
            set
            {
                _hasThreshold = value;
                if (!_hasThreshold)
                    ThresholdType = "";
                OnPropertyChanged("HasThreshold");
                OnPropertyChanged("ThresholdType");
            }
        }

        /// <summary>
        /// 信号门限类型
        /// </summary>
        public string ThresholdType
        {
            get
            {
                if (!_hasThreshold) return "";
                if (ConfigurationManager.AppSettings["FscanSignalShold"].Equals("1"))
                {
                    if (_thresholdtype == "") _thresholdtype = "noise";
                    var s = Enum.Parse(typeof(SignalShold), _thresholdtype, true);
                    var item = from i in RmtpDefaultCollection.SignalSholdSource where i.Value.Equals(s) select i.Key;
                    return item.First();
                }
                else
                {
                    if (_thresholdtype == "") _thresholdtype = "horizontal";
                    return _thresholdtype;
                }
            }
            set
            {
                if (ConfigurationManager.AppSettings["FscanSignalShold"].Equals("1"))  
                    _thresholdtype = HasThreshold ? RmtpDefaultCollection.SignalSholdSource[value].ToString() : "";
                else
                    _thresholdtype = HasThreshold ? "horizontal" : "";
                OnPropertyChanged("ThresholdType");
            }
        }

        /// <summary>
        /// 监测信号门限
        /// </summary>
        public double SignalThreshold
        {
            get { return _signalThreshold; }
            set { _signalThreshold = value; OnPropertyChanged("SignalThreshold"); }
        }
        /// <summary>
        /// 预学习时长
        /// </summary>
        public int LearningTime
        {
            get { return _learningTime; }
            set { _learningTime = value; OnPropertyChanged("LearningTime"); }
        }
        /// <summary>
        /// 门限裕度
        /// </summary>
        public int Margin { get; set; }

        /// <summary>
        /// 存储开关
        /// </summary>
        public bool Storage { get; set; }
        /// <summary>
        /// 是否是学习电磁背景
        /// </summary>
        public bool IsStudy { get; set; }

        /// <summary>
        /// 自定义门限描述
        /// </summary>
        public string ThresholdDesc
        {
            get { return _thresholddesc; }
            set { _thresholddesc = value; }
        }
        /// <summary>
        /// 自定义门限
        /// </summary>
        public string CustomThresholdValues
        {
            get { return _customthresholdvalues; }
            set { _customthresholdvalues = value; }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
        #endregion

        public List<Tuple<string, string>> ToTupleString()
        {
            var paramList = new List<Tuple<string, string>>();
            paramList.Add(new Tuple<string,string>("startfreq",string.Format("{0}MHz",WMonitorUtile.ConvertFreqValue(StartFreqUnit,"mhz", StartFreq))));
            paramList.Add(new Tuple<string,string>("stopfreq",string.Format("{0}MHz",WMonitorUtile.ConvertFreqValue(StopFreqUnit,"mhz",StopFreq))));
            paramList.Add(new Tuple<string, string>("step", string.Format("{0}kHz", WMonitorUtile.ConvertFreqValue(StepUnit, "khz", Step))));
            paramList.Add(new Tuple<string, string>("ifbw", string.Format("{0}kHz", WMonitorUtile.ConvertFreqValue(IfbwUnit, "khz", Ifbw))));
            paramList.Add(new Tuple<string,string>("polarization", RmtpDefaultCollection.PolarizationSource[Polarization].ToString()));
            paramList.Add(new Tuple<string, string>("detector", RmtpDefaultCollection.DetectorSource[SmoothMode].ToString()));
            //paramList.Add(new Tuple<string, string>("storage", Storage ? "on" : "off"));
            paramList.Add(new Tuple<string, string>("storage", "on"));

            if (RmtpDefaultCollection.DetectorSource[SmoothMode] != Detector.fast && RmtpDefaultCollection.DetectorSource[SmoothMode] != Detector.qbk && RmtpDefaultCollection.DetectorSource[SmoothMode] != Detector.avg)
            {
                paramList.Add(new Tuple<string, string>("smoothparam", SmoothParam.ToString(CultureInfo.InvariantCulture)));
            }

            paramList.Add(new Tuple<string, string>("ifbwmode", "xdb"));
            paramList.Add(new Tuple<string, string>("ifbwparam", "26"));
            paramList.Add(new Tuple<string, string>("statwindowtime", 5.ToString()));

            paramList.Add(new Tuple<string,string>("gainmode", RmtpDefaultCollection.GainModeSource[GainMode].ToString()));
            if (RmtpDefaultCollection.GainModeSource[GainMode] == Rmtp.GainMode.mgc)
            {
                paramList.Add(new Tuple<string, string>("gainparam", string.Format("{0}dBuV", GainParam)));
            }

            if (!IsStudy)
            {
                if (HasThreshold)
                {
                    if(ConfigurationManager.AppSettings["FscanSignalShold"].Equals("1"))
                    {
                        var tt = RmtpDefaultCollection.SignalSholdSource[ThresholdType].ToString();
                        string signalthreshold;
                        if (tt == "horizontal")
                        {
                            signalthreshold = string.Format("{0}dBμV", SignalThreshold);
                            paramList.Add(new Tuple<string, string>("thresholdtype", tt));
                            paramList.Add(new Tuple<string, string>("signalthreshold", signalthreshold));
                        }
                        else if (tt == "background")
                        {
                            //自定义门限
                            paramList.Add(new Tuple<string, string>("thresholdtype", "custom"));
                            paramList.Add(new Tuple<string, string>("thresholddesc", ThresholdDesc));
                            paramList.Add(new Tuple<string, string>("customthreshold", CustomThresholdValues));
                        }
                        else if (tt == "noise")
                        {
                            paramList.Add(new Tuple<string, string>("thresholdtype", tt));
                        }

                        if (tt != "horizontal")
                        {
                            //paramList.Add(new Tuple<string, string>("margin", Margin.ToString(CultureInfo.InvariantCulture)));
                            paramList.Add(new Tuple<string, string>("signalthreshold", Margin.ToString(CultureInfo.InvariantCulture)));
                        }
                    }
                    else
                    {
                        string signalthreshold = string.Format("{0}dBμV", SignalThreshold);
                        paramList.Add(new Tuple<string, string>("thresholdtype", ThresholdType));
                        paramList.Add(new Tuple<string, string>("signalthreshold", signalthreshold));
                    }
                }
            }

            return paramList;
        }
    }
}
