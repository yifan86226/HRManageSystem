using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 频率集
    /// </summary>
    public class Frequency : INotifyPropertyChanged, IComparable<Frequency>
    {
        private double _frequence = 87;
        private bool _isActive = false;
        private double _ifbw = 200;
        private string _demodulationMode = "FM";
        private string _polarizationMode = "vertical";
        private double _dwellTime;
        private double _measureDuration;
        private int _smoothMode = 1;
        private int _lineSmooth = 512;
        private string _remark;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        /// <summary>
        /// 频率(MHz)
        /// </summary>
        public double Frequence
        {
            get { return _frequence; }
            set
            {
                _frequence = value;
                RaisePropertyChanged("Frequence");
            }
        }

        /// <summary>
        /// 中频带宽(kHz)
        /// </summary>
        public double Ifbw
        {
            get { return _ifbw; }
            set
            {
                _ifbw = value;
                RaisePropertyChanged("Ifbw");
            }
        }

        /// <summary>
        /// 解调方式
        /// </summary>
        public string DemodulationMode
        {
            get { return _demodulationMode; }
            set
            {
                _demodulationMode = value;
                RaisePropertyChanged("DemodulationMode");
            }
        }

        /// <summary>
        /// 极化方式
        /// </summary>
        public string PolarizationMode
        {
            get { return _polarizationMode; }
            set
            {
                _polarizationMode = value;
                RaisePropertyChanged("PolarizationMode");
            }
        }

        /// <summary>
        /// 检波方式
        /// </summary>
        public int SmoothMode
        {
            get { return _smoothMode; }
            set
            {
                _smoothMode = value;
                RaisePropertyChanged("SmoothMode");
            }
        }

        /// <summary>
        /// 平均次数
        /// </summary>
        public int LineSmooth
        {
            get { return _lineSmooth; }
            set
            {
                _lineSmooth = value;
                RaisePropertyChanged("LineSmooth");
            }
        }

        /// <summary>
        /// 测量时间
        /// </summary>
        public double MeasureDuration
        {
            get { return _measureDuration; }
            set
            {
                _measureDuration = value;
                RaisePropertyChanged("MeasureDuration");
            }
        }

        /// <summary>
        /// 驻留时间
        /// </summary>
        public double DwellTime
        {
            get { return _dwellTime; }
            set
            {
                _dwellTime = value;
                RaisePropertyChanged("DwellTime");
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set
            {
                _remark = value;
                RaisePropertyChanged("Remark");
            }
        }

        public static string[] DemodulationModes
        {
            get
            {
                return new[] { "FM", "AM", "PM", "LSB", "USB", "SSB", "CW" };
            }
        }

        public static Dictionary<string, string> PolarizationModes
        {
            get
            {
                var dic = new Dictionary<string, string> { { "horizontal", "水平" }, { "vertical", "垂直" }, { "circle", "圆极化" } };
                return dic;
            }
        }

        //检波方式 1-实时；2-均方根；3-峰值
        public static Dictionary<int, string> SmoothModes
        {
            get
            {
                var dic = new Dictionary<int, string> { { 1, "实时" }, { 2, "均方根" }, { 3, "峰值" } };
                return dic;
            }
        }

        public static int[] LineSmooths
        {
            get
            {
                return new[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 };
            }
        }

        /// <summary>
        /// 输入是否正确
        /// </summary>
        /// <param name="func">arg1 频率, arg2 中频带宽</param>
        /// <returns>true/false</returns>
        public bool IsValid(Func<double, double, bool> func)
        {
            return func(Frequence, Ifbw);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IComparable<Frequency> 成员

        int IComparable<Frequency>.CompareTo(Frequency other)
        {
            return Frequence.CompareTo(other.Frequence);
        }

        #endregion
    }
}
