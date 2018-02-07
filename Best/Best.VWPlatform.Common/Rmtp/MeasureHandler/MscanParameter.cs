using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 离散扫描测量参数
    /// </summary>
    public class MscanParameter : INotifyPropertyChanged
    {
        private string _polarization = "vertical";
        private bool _hasDwellTime = false;
        private int _dwellTime = 0;
        private string _ifbwMode = "XdB";
        private double _ifbwParam = 26;
        private bool _hasThreshold = true;
        private string _thresholdtype = "horizontal";
        private double _signalThreshold = 0;
        private bool _storage = false;

        public MscanParameter()
        {
            Frequencies = new SmartCollection<Frequency>();
        }

        /// <summary>
        /// 频率集
        /// </summary>
        public SmartCollection<Frequency> Frequencies { get; set; }

        /// <summary>
        /// 极化方式
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
        /// 启用驻留时间
        /// </summary>
        public bool HasDwellTime
        {
            get { return _hasDwellTime; }
            set
            {
                _hasDwellTime = value;
                OnPropertyChanged("HasDwellTime");
            }
        }

        /// <summary>
        /// 驻留时间
        /// </summary>
        public int DwellTime
        {
            get { return _dwellTime; }
            set
            {
                _dwellTime = value;
                OnPropertyChanged("DwellTime");
            }
        }

        /// <summary>
        /// 带宽测量模式
        /// </summary>
        public string IfbwMode
        {
            get { return _ifbwMode; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;
                _ifbwMode = value;
                OnPropertyChanged("IfbwMode");
            }
        }

        /// <summary>
        /// 带宽测量参数值
        /// </summary>
        public double IfbwParam
        {
            get { return _ifbwParam; }
            set
            {
                _ifbwParam = value;
                OnPropertyChanged("IfbwParam");
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
                if (string.IsNullOrWhiteSpace(value))
                    return;
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
            set
            {
                _signalThreshold = value;
                OnPropertyChanged("SignalThreshold");
            }
        }

        /// <summary>
        /// 存储开关
        /// </summary>
        public bool Storage
        {
            get { return _storage; }
            set
            {
                _storage = value;
                OnPropertyChanged("Storage");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
        #endregion

        public List<Tuple<string, string>> ToParameterList()
        {
            var pms = new List<Tuple<string, string>>
                          {
                              new Tuple<string, string>("frequency",
                                                        string.Format("{0}MHz",
                                                                      string.Join("MHz;", from f in Frequencies
                                                                                          where f.IsActive
                                                                                          select f.Frequence))),
                              new Tuple<string, string>("ifbw",
                                                        string.Format("{0}kHz",
                                                                      string.Join("kHz;",
                                                                                  from f in Frequencies
                                                                                  where f.IsActive
                                                                                  select f.Ifbw))),
                              new Tuple<string, string>("polarization", Polarization),
                              new Tuple<string, string>("hasdwelltime", HasDwellTime ? "on" : "off"),
                              new Tuple<string, string>("ifbwmode", IfbwMode.ToLower()),
                              new Tuple<string, string>("ifbwparam",
                                                        string.Format("{0}{1}", IfbwParam,
                                                                      IfbwMode.ToLower() == "xdb" ? "db" : "%")),
                              new Tuple<string, string>("hasthreshold", HasThreshold ? "on" : "off"),
                              //new Tuple<string, string>("storage", Storage ? "on" : "off")
                              new Tuple<string, string>("storage","on")
                          };
            if (HasThreshold)
            {
                pms.Add(new Tuple<string, string>("thresholdtype", "horizontal"));
                pms.Add(new Tuple<string, string>("signalthreshold", SignalThreshold + "dBμV"));
            }

            if(HasDwellTime)
            {
                pms.Add(new Tuple<string, string>("holdtime", DwellTime.ToString()));
            }

            return pms;
        }
    }
}
