using Best.VWPlatform.Common.Rmtp.DataFrames;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 射频全景信号统计项
    /// </summary>
    public partial class SignalStatisticsItem : INotifyPropertyChanged
    {
        private bool _isChecked = false;
        private bool _isVisible = true;
        private int _nature;
        private double _frequency;
        private double _currentFrequency;
        private short _level;
        private short _fieldStrength;
        private Single _density;
        private double _bandwidth;
        private double _currentBandwidth;
        private Int64 _interceptedNumber;
        private Single _occupancyRate;
        private Int64 _duration;
        private DateTime _lastTime;
        private DateTime _firstTime;
        private double _longitude;
        private double _latitude;
        private string _belong;
        private double _referFreq;
        private Int32 _signalId;
        private float _customValue;
        private Visibility _itemVisibility;
        private double _recordFreq;

        public SignalStatisticsItem()
        {
            ItemVisibility = Visibility.Collapsed;
        }
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        /// <summary>
        /// 信号ID
        /// </summary>
        public Int32 SignalId
        {
            get { return _signalId; }
            set
            {
                _signalId = value;
                OnPropertyChanged("SignalId");
            }
        }

        /*
         1 - 已知信号，台站数据库（A库）
         2 - 已知信号，台站数据库（B库）
         3 - 已知信号,未入库
         4 - 未知信号/不明信号
         5 - 非法信号
         6 - 违规信号
         7 - 虚假信号
         8 - 其它信号
        */
        /// <summary>
        /// 性质
        /// </summary>
        public int Nature
        {
            get { return _nature; }
            set
            {
                _nature = value;
                OnPropertyChanged("Nature");
            }
        }
        /// <summary>
        /// Tdoa定位无效
        /// </summary>
        public bool IsTdoaInvalid { get; set; }

        /// <summary>
        /// 频率(Hz)
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

        /// <summary>
        /// 备案频率
        /// </summary>
        public double RecordFreq
        {
            set
            {
                _recordFreq = value;
                OnPropertyChanged("RecordFreq");
            }
            get { return _recordFreq; }
        }

        /// <summary>
        /// 当前频率(Hz)
        /// </summary>
        public double CurrentFrequency
        {
            get { return _currentFrequency; }
            set
            {
                _currentFrequency = value;
                OnPropertyChanged("CurrentFrequency");
            }
        }

        /// <summary>
        /// 电平(dBμV)
        /// </summary>
        public short Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged("Level");
            }
        }

        /// <summary>
        /// 场强(dBμV/m)
        /// </summary>
        public short FieldStrength
        {
            get { return _fieldStrength; }
            set
            {
                _fieldStrength = value;
                OnPropertyChanged("FieldStrength");
            }
        }

        /// <summary>
        /// 功率密度(dBμW/cm²)
        /// </summary>
        public Single Density
        {
            get { return _density; }
            set
            {
                _density = value;
                OnPropertyChanged("Density");
            }
        }
        /// <summary>
        /// 用户数据值，该数据帧的门限或容差(margin)
        /// </summary>
        public Single CustomValue
        {
            get { return _customValue; }
            set
            {
                _customValue = value;
                OnPropertyChanged("CustomValue");
            }
        }

        /// <summary>
        /// 带宽(Hz)
        /// </summary>
        public double Bandwidth
        {
            get { return _bandwidth; }
            set
            {
                _bandwidth = value;
                OnPropertyChanged("Bandwidth");
            }
        }

        /// <summary>
        /// 当前带宽(Hz)
        /// </summary>
        public double CurrentBandwidth
        {
            get { return _currentBandwidth; }
            set
            {
                _currentBandwidth = value;
                OnPropertyChanged("CurrentBandwidth");
            }
        }

        /// <summary>
        /// 截获次数
        /// </summary>
        public Int64 InterceptedNumber
        {
            get { return _interceptedNumber; }
            set
            {
                _interceptedNumber = value;
                OnPropertyChanged("InterceptedNumber");
            }
        }

        /// <summary>
        /// 占用度(%)
        /// </summary>
        public Single OccupancyRate
        {
            get { return _occupancyRate; }
            set
            {
                _occupancyRate = value;
                OnPropertyChanged("OccupancyRate");
            }
        }

        /// <summary>
        /// 持续时间(毫秒)
        /// </summary>
        public Int64 Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }
        /// <summary>
        /// 第一次捕获时间
        /// </summary>
        public DateTime FirstTime
        {
            get { return _firstTime; }
            set
            {
                _firstTime = value;
                OnPropertyChanged("FirstTime");
            }
        }
        /// <summary>
        /// 最后一次出现时间
        /// </summary>
        public DateTime LastTime
        {
            get { return _lastTime; }
            set
            {
                _lastTime = value;
                OnPropertyChanged("LastTime");
            }
        }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                OnPropertyChanged("Longitude");
            }
        }

        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                OnPropertyChanged("Latitude");
            }
        }

        /// <summary>
        /// 归属
        /// </summary>
        public string Belong
        {
            get { return _belong; }
            set
            {
                _belong = value;
                OnPropertyChanged("Belong");
            }
        }
        /// <summary>
        /// 参考频率
        /// </summary>
        public double ReferFreq
        {
            get { return _referFreq; }
            set
            {
                _referFreq = value;
                OnPropertyChanged("ReferFreq");
            }
        }

        public TdoaDataFrame DataFrame { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }

        public ICommand IffqCommand { get; set; }

        public ICommand ItuCommand { get; set; }

        public Visibility ItemVisibility
        {
            get { return _itemVisibility; }
            set
            {
                _itemVisibility = value;
                OnPropertyChanged("ItemVisibility");
            }
        }
    }
}
