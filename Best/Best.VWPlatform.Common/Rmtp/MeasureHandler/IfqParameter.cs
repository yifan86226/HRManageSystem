using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Best.VWPlatform.Common.Utility;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 中频全景测量参数
    /// </summary>
    public class IfqParameter : INotifyPropertyChanged
    {
        private double _frequency = 103.9;
        private double _ifbw = 500000;
        private double _span = 10000000;
        private double _squelchThreshold = -1;
        private string _demodMode = "FM";
        private double _rfattenuation = 0;
        private string _polarization = "vertical";
        private string _detector = "avg";
        private bool _itu = true;
        private string _ifbwMode = "XdB";
        private double _ifbwParam = 26;
        private bool _audio = true;
        private bool _hasSquelchThreshold = false;
        private bool _hasRfattenuation = false;
        private string _frequencyUnit = "MHz";
        private string _ifbwUnit = "Hz";
        private string _spanUnit = "Hz";
        private string _gainmode = "agc";
        private int _gainparam = 10;
        private int _detectorparam = 128;

        /// <summary>
        /// 监测频率
        /// </summary>
        public double Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                OnPropertyChanged("Frequency");
            }
        }

        public string FrequencyUnit
        {
            get { return _frequencyUnit; }
            set { _frequencyUnit = value; OnPropertyChanged("FrequencyUnit"); }
        }

        /// <summary>
        /// 中频带宽
        /// </summary>
        public double Ifbw
        {
            get { return _ifbw; }
            set { _ifbw = value; OnPropertyChanged("Ifbw"); }
        }

        public string IfbwUnit
        {
            get { return _ifbwUnit; }
            set { _ifbwUnit = value; OnPropertyChanged("IfbwUnit"); }
        }

        /// <summary>
        /// 跨距
        /// </summary>
        public double Span
        {
            get { return _span; }
            set { _span = value; OnPropertyChanged("Span"); }
        }

        public string SpanUnit
        {
            get { return _spanUnit; }
            set { _spanUnit = value; OnPropertyChanged("SpanUnit"); }
        }
        /// <summary>
        /// 是否开启静噪门限
        /// </summary>
        public bool HasSquelchThreshold
        {
            get { return _hasSquelchThreshold; }
            set { _hasSquelchThreshold = value; OnPropertyChanged("HasSquelchThreshold"); }
        }

        /// <summary>
        /// 静噪门限，指定音频数据采集的电平门限
        /// </summary>
        public double SquelchThreshold
        {
            get { return _squelchThreshold; }
            set { _squelchThreshold = value; OnPropertyChanged("SquelchThreshold"); }
        }

        /// <summary>
        /// 解调方式
        /// </summary>
        public string DemodMode
        {
            get { return _demodMode; }
            set { _demodMode = value; OnPropertyChanged("DemodMode"); }
        }
        /// <summary>
        /// 是否开启射频衰减
        /// </summary>
        public bool HasRfattenuation
        {
            get { return _hasRfattenuation; }
            set { _hasRfattenuation = value; OnPropertyChanged("HasRfattenuation"); }
        }

        /// <summary>
        /// 射频衰减开/关
        /// </summary>
        public double Rfattenuation
        {
            get { return _rfattenuation; }
            set { _rfattenuation = value; OnPropertyChanged("Rfattenuation"); }
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
        public string Detector
        {
            get
            {
                var p = Enum.Parse(typeof(Detector), _detector, true);
                var item = from i in RmtpDefaultCollection.DetectorSource where i.Value.Equals(p) select i.Key;
                return item.First();
            }
            set
            {
                _detector = RmtpDefaultCollection.DetectorSource[value].ToString();
                OnPropertyChanged("Detector");
            }
        }
        /// <summary>
        /// 检波参数，平均参数
        /// </summary>
        public int DetectorParam
        {
            get
            {
                return _detectorparam;
            }
            set
            {
                _detectorparam = value;
                OnPropertyChanged("DetectorParam");
            }
        }
        /// <summary>
        /// ITU测量开关
        /// </summary>
        public bool Itu
        {
            get { return _itu; }
            set { _itu = value; OnPropertyChanged("Itu"); }
        }

        /// <summary>
        /// 带宽测量模式
        /// </summary>
        public string IfbwMode
        {
            get { return _ifbwMode; }
            set { _ifbwMode = value; OnPropertyChanged("IfbwMode"); }
        }

        /// <summary>
        /// 带宽测量参数值
        /// </summary>
        public double IfbwParam
        {
            get { return _ifbwParam; }
            set { _ifbwParam = value; OnPropertyChanged("IfbwParam"); }
        }

        /// <summary>
        /// 音频监听开关
        /// </summary>
        public bool Audio
        {
            get { return _audio; }
            set { _audio = value; OnPropertyChanged("Audio"); }
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
            Debug.Assert(!string.IsNullOrWhiteSpace(FrequencyUnit)
                && !string.IsNullOrWhiteSpace(IfbwUnit)
                && !string.IsNullOrWhiteSpace(SpanUnit)
                && !string.IsNullOrWhiteSpace(DemodMode)
                && !string.IsNullOrWhiteSpace(IfbwMode));
            var paramList = new List<Tuple<string, string>>
                    {
                         new Tuple<string,string>("frequency",string.Format("{0}MHz",WMonitorUtile.ConvertFreqValue(FrequencyUnit,"mhz", Frequency))),
                         new Tuple<string,string>("ifbw",string.Format("{0}kHz",WMonitorUtile.ConvertFreqValue(IfbwUnit,"khz", Ifbw))),
                         new Tuple<string,string>("span",string.Format("{0}kHz",WMonitorUtile.ConvertFreqValue(SpanUnit,"khz", Span))),
                         new Tuple<string,string>("squelchthreshold",HasSquelchThreshold?string.Format("{0}dBuV", SquelchThreshold):"off"),
                         new Tuple<string,string>("demodmode",DemodMode),
                         new Tuple<string,string>("rfattenuation",HasRfattenuation?string.Format("{0}dB", Rfattenuation):"off"),
                         new Tuple<string,string>("polarization",RmtpDefaultCollection.PolarizationSource[Polarization].ToString()),
                         new Tuple<string,string>("detector", RmtpDefaultCollection.DetectorSource[Detector].ToString()),
                         new Tuple<string, string>("itu", Itu?"on":"off"),
                         new Tuple<string,string>("ifbwmode", IfbwMode.ToLower()),
                         new Tuple<string, string>("ifbwparam", string.Format("{0}{1}", IfbwParam, IfbwMode.ToLower() == "xdb" ? "db" : "%")),
                         new Tuple<string, string>("audio",Audio?"on":"off"),
                         new Tuple<string, string>("storage","on"),
                         new Tuple<string,string>("gainmode", RmtpDefaultCollection.GainModeSource[GainMode].ToString()),  
                    };

            if (RmtpDefaultCollection.GainModeSource[GainMode] == Rmtp.GainMode.mgc)
            {
                paramList.Add(new Tuple<string, string>("gainparam", string.Format("{0}dBuV", GainParam)));
            } 

            return paramList;
        }
    }
}
