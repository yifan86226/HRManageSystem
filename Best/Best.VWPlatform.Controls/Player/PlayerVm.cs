using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Utility;
using Color = System.Drawing.Color;

namespace Best.VWPlatform.Controls.Player
{
    public class PlayerVm : NotificationObject
    {
        private const double MAX_SPEED = 8;
        private const double MIN_SPEED = 0.125;
        private int _currTime;
        /// <summary>
        /// 单位（秒）
        /// </summary>
        public int CurrTime
        {
            get { return _currTime; }
            set
            {
                if (_currTime != value)
                {
                    _currTime = value;
                    RaisePropertychanged("CurrTime");                    
                }
            }
        }
        private int _totalTime;
        /// <summary>
        /// 单位（秒）
        /// </summary>
        public int TotalTime
        {
            get { return _totalTime; }
            set
            {
                _totalTime = value;
                RaisePropertychanged("TotalTime");
            }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertychanged("IsBusy");
            }
        }

        private double _speed = 1;
        public double Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                RaisePropertychanged("Speed");
                SpeedString = string.Format("{0}X", _speed);
            }
        }

        private string _speedString;
        public string SpeedString
        {
            get { return _speedString; }
            set
            {
                _speedString = value;
                RaisePropertychanged("SpeedString");
            }
        }

        public DelegateCommand PlayAndPauseCommand { get; set; }

        public DelegateCommand FFCommand { get; set; }

        public DelegateCommand FWCommand { get; set; }

        public DelegateCommand SpeedChangedCommand { get; set; }

        public DelegateCommand NextCommand { get; set; }

        public DelegateCommand PreviousCommand { get; set; }

        public DelegateCommand ValueChangedCommand { get; set; }
        public PlayerVm()
        {
            PlayAndPauseCommand = new DelegateCommand();
            FFCommand = new DelegateCommand();
            FFCommand.ExecuteAction = new Action<object>(FF);
            FWCommand = new DelegateCommand();
            FWCommand.ExecuteAction = new Action<object>(FW);
            NextCommand = new DelegateCommand();
            PreviousCommand = new DelegateCommand();
            ValueChangedCommand = new DelegateCommand();
            SpeedChangedCommand = new DelegateCommand();
        }
        private void FF(object parameter)
        {
            if (Speed < MAX_SPEED)
            {
                Speed *= 2;
                if (SpeedChangedCommand != null)
                {
                    SpeedChangedCommand.Execute(Speed);
                }
            }

        }
        private void FW(object parameter)
        {
            if (Speed > MIN_SPEED)
            {
                Speed /= 2;
                if (SpeedChangedCommand != null)
                {
                    SpeedChangedCommand.Execute(Speed);
                }
            }
        }
    }

    class SpeedColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double v = (double)value;
            if (v >= 1)
            {
                return new SolidColorBrush(Colors.LawnGreen);
            }
            else
            {
                return new SolidColorBrush(Colors.Tomato);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }
    }
    class SpeedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("{0}X", value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }
    }
    class TimeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int v = (int)value;
            string s = Utile.SecondToMinute(v);
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }
    }
}
