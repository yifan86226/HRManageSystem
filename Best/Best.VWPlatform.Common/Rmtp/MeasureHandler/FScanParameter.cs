using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 频段扫描测量参数
    /// </summary>
    public class FScanParameter : INotifyPropertyChanged
    {
        public FScanParameter()
        {
            WhiteLists = new SmartCollection<WhiteFreq>();
        }

        /// <summary>
        /// 信号白名单
        /// </summary>
        public SmartCollection<WhiteFreq> WhiteLists { get; set; }
        /// <summary>
        /// 检波方式
        /// </summary>
        private string _detectionMode = "峰值";
        /// <summary>
        /// 终止频率
        /// </summary>
        private double _endFrequency = 108;
        private string _endFreqUnit = "MHz";
        /// <summary>
        /// 中频带宽
        /// </summary>
        private double _ifbw = 200;
        private string _ifbwUnit = "kHz";
        /// <summary>
        /// 极化方式
        /// </summary>
        private string _polarization = "垂直极化";
        /// <summary>
        /// 起始频率
        /// </summary>
        private double _startFrequency = 88;
        private string _startFreqUnit = "MHz";
        /// <summary>
        /// 步进
        /// </summary>
        private double _step = 100;
        private string _stepUnit = "kHz";
        /// <summary>
        /// 统计窗口时间
        /// </summary>
        private int _statwindowtime = 5;
        /// <summary>
        /// 启用信号门限
        /// </summary>
        private bool _hasThreshold = true;
        /// <summary>
        /// 监测信号门限类型
        /// </summary>
        private string _thresholdtype = "horizontal";
        /// <summary>
        /// 监测信号门限
        /// </summary>
        private double _signalThreshold = 0;
        /// <summary>
        /// 步进最大值
        /// </summary>
        private double _maxStep = 100;
        /// <summary>
        /// 起始频率 startfreq Hz
        /// </summary>
        public double StartFrequency
        {
            get { return _startFrequency; }
            set
            {
                _startFrequency = value;
                var maxStep = Math.Floor(WMonitorUtile.ConvertFreqValue(EndFreqUnit, "hz", _endFrequency - _startFrequency));
                MaxStep = WMonitorUtile.ConvertFreqValue("hz", StepUnit, maxStep);
                OnPropertyChanged("StartFrequency");
                OnPropertyChanged("MaxStep");
            }
        }
        /// <summary>
        /// 测量起始频率单位
        /// </summary>
        public string StartFreqUnit
        {
            get { return _startFreqUnit; }
            set 
            { 
                _startFreqUnit = value; 
                OnPropertyChanged("StartFreqUnit"); 
            }
        }
        /// <summary>
        /// 终止频率 stopfreq Hz
        /// </summary>
        public double EndFrequency
        {
            get { return _endFrequency; }
            set
            {
                _endFrequency = value;
                var maxStep = Math.Floor(WMonitorUtile.ConvertFreqValue(EndFreqUnit, "hz", _endFrequency - _startFrequency));
                MaxStep = WMonitorUtile.ConvertFreqValue("hz", StepUnit, maxStep);
                OnPropertyChanged("EndFrequency");
                OnPropertyChanged("MaxStep");
            }
        }
        /// <summary>
        /// 测量终止频率单位
        /// </summary>
        public string EndFreqUnit
        {
            get { return _endFreqUnit; }
            set
            {
                _endFreqUnit = value;
                OnPropertyChanged("EndFreqUnit");
                if (_endFreqUnit == null) return;
                var maxStep = Math.Floor(WMonitorUtile.ConvertFreqValue(_endFreqUnit, "hz", _endFrequency - _startFrequency));
                MaxStep = WMonitorUtile.ConvertFreqValue("hz", StepUnit, maxStep);
                OnPropertyChanged("MaxStep");
            }
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
        /// 步进 step最大值 Hz
        /// </summary>
        public double MaxStep
        {
            get { return _maxStep; }
            set
            {
                _maxStep = value;
                OnPropertyChanged("MaxStep");
            }
        }

        /// <summary>
        /// 中频带宽  Hz
        /// </summary>
        public double Ifbw
        {
            get { return _ifbw; }
            set
            {
                _ifbw = value;
                OnPropertyChanged("Ifbw");
            }
        }

        /// <summary>
        /// 中频带宽的单位
        /// </summary>
        public string IfbwUnit
        {
            get { return _ifbwUnit; }
            set
            {
                _ifbwUnit = value;
                OnPropertyChanged("IfbwUnit");
            }
        }

        /// <summary>
        /// 检波方式 detector
        /// 可选取值：peak / avg    / fast   / rms    / qbk
        /// 分别表示：峰值 / 平均值 / 实时值 / 均方根 / 准峰值
        /// </summary>
        public string DetectionMode
        {
            get { return _detectionMode; }
            set
            {
                _detectionMode = value;
                OnPropertyChanged("DetectionMode");
            }
        }

        /// <summary>
        /// 极化方式 horizontal / vertical / circle
        /// </summary>
        public string Polarization
        {
            get { return _polarization; }
            set
            {
                _polarization = value;
                OnPropertyChanged("Polarization");
            }
        }

        /// <summary>
        /// 统计时间窗
        /// </summary>
        public int Statwindowtime
        {
            get { return _statwindowtime; }
            set
            {
                _statwindowtime = value;
                OnPropertyChanged("Statwindowtime");
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
                    Thresholdtype = "";
                OnPropertyChanged("HasThreshold");
                OnPropertyChanged("ThresholdType");
            }
        }

        /// <summary>
        /// 信号门限类型
        /// </summary>
        public string Thresholdtype
        {
            get 
            {
                if (!_hasThreshold) return "";
                if (_thresholdtype == "") _thresholdtype = "horizontal";
                return _thresholdtype; 
            }
            set
            {
                _thresholdtype = HasThreshold ? "horizontal" : "";
                OnPropertyChanged("Thresholdtype");
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
        /// 存储开关
        /// </summary>
        public bool Storage { get; set; }
        public string ToCmdParamString()
        {
            var sb = new StringBuilder();
            sb.Append("RMTP: PARAM:");
            sb.AppendFormat("startfreq={0}MHz", StartFrequency);
            sb.AppendFormat(",stopfreq={0}MHz", EndFrequency);
            double step = Step;
            switch (StepUnit.ToUpper())
            {
                case "GHZ":
                    step = step * 1000000;
                    break;
                case "MHZ":
                    step = step * 1000;
                    break;
                case "HZ":
                    step = step / 1000;
                    break;
            }

            double ifbw = Ifbw;
            switch (IfbwUnit.ToUpper())
            {
                case "GHZ":
                    ifbw = ifbw * 1000000;
                    break;
                case "MHZ":
                    ifbw = ifbw * 1000;
                    break;
                case "HZ":
                    ifbw = ifbw / 1000;
                    break;
            }

            sb.AppendFormat(",step={0}kHz", step);
            sb.AppendFormat(",ifbw={0}kHz", ifbw);
            sb.AppendFormat(",detector={0}", RmtpDefaultCollection.DetectorSource[DetectionMode]);
            sb.AppendFormat(",polarization={0}", RmtpDefaultCollection.PolarizationSource[Polarization]);
            sb.Append("\\n");

            return sb.ToString();
        }

        public List<Tuple<string, string>> ToTupleString()
        {
            var lst = new List<Tuple<string, string>>();
            lst.Add(new Tuple<string, string>("startfreq", string.Format("{0}MHz", WMonitorUtile.ConvertFreqValue(StartFreqUnit, "mhz", StartFrequency))));
            lst.Add(new Tuple<string, string>("stopfreq", string.Format("{0}MHz", WMonitorUtile.ConvertFreqValue(EndFreqUnit, "mhz", EndFrequency))));

            double step = Step;
            switch (StepUnit.ToUpper())
            {
                case "GHZ":
                    step = step * 1000000;
                    break;
                case "MHZ":
                    step = step * 1000;
                    break;
                case "HZ":
                    step = step / 1000;
                    break;
            }
            lst.Add(new Tuple<string, string>("step", string.Format("{0}kHz", step)));

            double ifbw = Ifbw;
            switch (IfbwUnit.ToUpper())
            {
                case "GHZ":
                    ifbw = ifbw * 1000000;
                    break;
                case "MHZ":
                    ifbw = ifbw * 1000;
                    break;
                case "HZ":
                    ifbw = ifbw / 1000;
                    break;
            }

            lst.Add(new Tuple<string, string>("ifbw", string.Format("{0}kHz", ifbw)));
            lst.Add(new Tuple<string, string>("detector", RmtpDefaultCollection.DetectorSource[DetectionMode].ToString()));
            lst.Add(new Tuple<string, string>("polarization", RmtpDefaultCollection.PolarizationSource[Polarization].ToString()));
            lst.Add(new Tuple<string, string>("statwindowtime", Statwindowtime.ToString()));
            lst.Add(new Tuple<string, string>("storage", Storage ? "on" : "off"));

            if (HasThreshold)
            {
                string signalthreshold = string.Format("{0}dBμV", SignalThreshold);
                lst.Add(new Tuple<string, string>("thresholdtype", Thresholdtype));
                lst.Add(new Tuple<string, string>("signalthreshold", signalthreshold));
            }

            lst.Add(new Tuple<string, string>("ifbwmode", "xdb"));
            lst.Add(new Tuple<string, string>("ifbwparam", "26"));

            return lst;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class WhiteFreq : INotifyPropertyChanged
    {
        private double _frequency;
        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public double Frequence
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                OnPropertyChanged("Frequence");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
