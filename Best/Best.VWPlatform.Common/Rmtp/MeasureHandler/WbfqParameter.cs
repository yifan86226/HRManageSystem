using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 射频全景测量参数
    /// </summary>
    public sealed class WbfqParameter : INotifyPropertyChanged
    {
        private double _startFreq = 87;
        private double _stopFreq = 108;
        private double _rbw = 100000;
        private string _cbRbw = "25kHz";
        private string _polarization = "vertical";
        private string _smoothmode = "rms";
        private int _smoothparam = 128;
        private string _thresholdtype = "noise";
        private double _signalThreshold;
        private string _startFreqUnit = "MHz";
        private string _stopFreqUnit = "MHz";
        private string _rbwUnit = "Hz";
        private bool _hasThreshold = true;
        private int _learningTime = 10;
        private string _rbwWindow;
        private string _thresholddesc;
        private string _customthresholdvalues;
        /// <summary>
        /// 增益模式
        /// </summary>
        private string _gainmode = "agc";
        /// <summary>
        /// 人工增益参数，单位dBUv
        /// </summary>
        private int _gainparam = 10;
        public WbfqParameter()
        {
            ChannelizeBands = new List<string>();
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
        /// 测量分辨率视窗名称
        /// </summary>
        public string RbwWindow
        {
            get { return _rbwWindow; }
            set
            {
                _rbwWindow = value;
                OnPropertyChanged("RbwWindow");
            }
        }

        /// <summary>
        /// 测量分辨率
        /// </summary>
        public double Rbw
        {
            get
            {
                return _rbw;
            }
            set { _rbw = value; OnPropertyChanged("Rbw"); }
        }

        /// <summary>
        /// 测量分辨率
        /// </summary>
        public string CbRbw
        {
            get
            {
                var item = from i in RmtpDefaultCollection.Gsmr4MHzRbwSource where i.Key.Equals(_cbRbw) select i.Value;
                return (item.First() * 1000).ToString();
            }
            set { _cbRbw = value; OnPropertyChanged("CbRbw"); }
        }
        /// <summary>
        /// 测量分辨率单位
        /// </summary>
        public string RbwUnit
        {
            get
            {
                return _rbwUnit;
            }
            set { _rbwUnit = value; OnPropertyChanged("RbwUnit"); }
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
        /// 平滑方式
        /// </summary>
        public string SmoothMode
        {
            get
            {
                var s = Enum.Parse(typeof(SmoothWay), _smoothmode, true);
                var item = from i in RmtpDefaultCollection.SmoothWaySource where i.Value.Equals(s) select i.Key;
                return item.First();
            }
            set
            {
                _smoothmode = RmtpDefaultCollection.SmoothWaySource[value].ToString();
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
        /// 监测信号门限类型
        /// </summary>
        public string ThresholdType
        {
            get
            {
                if (!_hasThreshold) return "";
                if (_thresholdtype == "") _thresholdtype = "noise";
                var s = Enum.Parse(typeof(SignalShold), _thresholdtype, true);
                var item = from i in RmtpDefaultCollection.SignalSholdSource where i.Value.Equals(s) select i.Key;
                return item.First();
            }
            set
            {
                _thresholdtype = HasThreshold ? RmtpDefaultCollection.SignalSholdSource[value].ToString() : "";
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
        /// 信道化频段列表
        /// </summary>
        private List<string> ChannelizeBands { get; set; }

        public void SetChannelizeBands(IEnumerable<string> pValues)
        {
            if (pValues == null)
                return;
            ChannelizeBands.Clear();
            foreach (var v in pValues)
            {
                ChannelizeBands.Add(v);
            }
        }
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

        public List<Tuple<string, string>> ToList()
        {
            var paramList = new List<Tuple<string, string>>
                {
                    new Tuple<string,string>("startfreq",string.Format("{0}MHz",WMonitorUtile.ConvertFreqValue(StartFreqUnit,"mhz", StartFreq))),
                    new Tuple<string,string>("stopfreq",string.Format("{0}MHz",WMonitorUtile.ConvertFreqValue(StopFreqUnit,"mhz",StopFreq))),
                    new Tuple<string,string>("rbw",string.Format("{0}kHz",WMonitorUtile.ConvertFreqValue(RbwUnit,"khz",Rbw))),
                    new Tuple<string,string>("polarization", RmtpDefaultCollection.PolarizationSource[Polarization].ToString()),
                    new Tuple<string, string>("smoothmode", RmtpDefaultCollection.SmoothWaySource[SmoothMode].ToString()),
                    new Tuple<string, string>("storage",Storage?"on":"off"),
                    new Tuple<string,string>("gainmode", RmtpDefaultCollection.GainModeSource[GainMode].ToString()), 
                };
            if (RmtpDefaultCollection.SmoothWaySource[SmoothMode] != SmoothWay.none)
            {
                paramList.Add(new Tuple<string, string>("smoothparam", SmoothParam.ToString(CultureInfo.InvariantCulture)));
            }

            if (RmtpDefaultCollection.GainModeSource[GainMode] == Rmtp.GainMode.mgc)
            {
                paramList.Add(new Tuple<string, string>("gainparam", string.Format("{0}dBuV", GainParam)));
            } 

            if (!IsStudy)
            {
                if (HasThreshold)
                {
                    var tt = RmtpDefaultCollection.SignalSholdSource[ThresholdType].ToString();
                    string signalthreshold;
                    if(tt == "horizontal")
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
                        paramList.Add(new Tuple<string, string>("margin", Margin.ToString(CultureInfo.InvariantCulture)));
                    }
                    
                }
            }
            
            if (ChannelizeBands.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var cb in ChannelizeBands)
                {
                    sb.AppendFormat("{0};", cb);
                }
                if (sb.Length > 0)
                    sb = sb.Remove(sb.Length - 1, 1);
                paramList.Add(new Tuple<string, string>("channelizebands", sb.ToString()));
            }
            return paramList;
        }
    }
}
