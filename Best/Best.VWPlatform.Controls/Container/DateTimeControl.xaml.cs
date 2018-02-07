using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Best.VWPlatform.Controls.Container
{
    /// <summary>
    /// DateTimeControl.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeControl : UserControl, INotifyPropertyChanged
    {
        private DateTime _datetimeDefault;
        private TextBox _currentTextBox, _oldTextBox;
        //第一次加载时的属性
        private Int32 _selectedHour ;
        private Int32 _selectedMinute;
        private Int32 _selectedSecond;
        public DateTimeControl(DateTime datetimeDefault)
        {
            _datetimeDefault = datetimeDefault;
            fir = true;

            SelectedHour = _datetimeDefault.Hour;
            SelectedMinute = _datetimeDefault.Minute;
            SelectedSecond = _datetimeDefault.Second;
           
            InitializeComponent();

            DatePickerObj.SelectedDate = _datetimeDefault;

            _oldTextBox = _currentTextBox = TextBox_Seconds;
            _currentTextBox.Background = Brushes.White;
            //this.SendPropertyChanged("SelectedDateTimeString");
        }

        public DateTimeControl()
        {
           _datetimeDefault = DateTime.Now;
           //fir = true;
           InitializeComponent();

           DatePickerObj.SelectedDate = _datetimeDefault;
           SelectedHour = _datetimeDefault.Hour;
           SelectedMinute = _datetimeDefault.Minute;
           SelectedSecond = _datetimeDefault.Second;

           _oldTextBox = _currentTextBox = TextBox_Seconds;
           _currentTextBox.Background = Brushes.White;
           //this.SendPropertyChanged("SelectedDateTimeString");
       }

       private void ContentSite_MouseDoubleClick(object sender, MouseButtonEventArgs e)
       {
           fir = false;
           this.SendPropertyChanged("SelectedDateTimeString");
       }
       public bool fir
       {
           get;
           set;
       }
        public DateTime SelectedDateTime
        {
            get
            {
                DateTime obj = ((DatePickerObj == null) ? _datetimeDefault : DatePickerObj.SelectedDate ?? _datetimeDefault);
                return obj.Add(new TimeSpan(SelectedHour - obj.Hour, SelectedMinute - obj.Minute, SelectedSecond - obj.Second));
            }
            set
            {
                DateTime obj = value;
            }
       
        }

         public string SelectedDateTimesFromTextbox="";
        
         public String SelectedDateTimeString
         { 
                get
                {
                    if (fir == false)
                    {
                        return "";
                    }
                    else
                    {
                        return SelectedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
               set
               {
                   SelectedDateTime = DateTime.Parse(value);
               }
            }

        #region private methods
        private void TextBox_Hours_GotFocus(object sender, RoutedEventArgs e)
        {            
            _oldTextBox.Background = Brushes.Transparent;  
            _oldTextBox = _currentTextBox = TextBox_Hours;
            _currentTextBox.Background = Brushes.White;
        }
        private void TextBox_Minutes_GotFocus(object sender, RoutedEventArgs e)
        {
            _oldTextBox.Background = Brushes.Transparent;
            _oldTextBox = _currentTextBox = TextBox_Minutes;
            _currentTextBox.Background = Brushes.White;
        }
        private void TextBox_Seconds_GotFocus(object sender, RoutedEventArgs e)
        {
            _oldTextBox.Background = Brushes.Transparent;
            _oldTextBox = _currentTextBox = TextBox_Seconds;
            _currentTextBox.Background = Brushes.White;
        }
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            fir = true;
            this.SendPropertyChanged("SelectedDateTime");
            this.SendPropertyChanged("SelectedDateTimeString");
        }

        private void BTN_IncreaseTime_Click(object sender, RoutedEventArgs e)
        {
            Int32 result;
            if (Int32.TryParse(_currentTextBox.Text, out result))
            {
                result++;
                if (result < 60) 
                
                {
                    if (_currentTextBox.Equals(TextBox_Hours))
                    {
                        if (result < 24)
                        {
                            _currentTextBox.Text = result.ToString();
                        }
                    }
                    else
                    {
                        _currentTextBox.Text = result.ToString();
                    }
                }
               
            }
        }
        private void BTN_DecrementTime_Click(object sender, RoutedEventArgs e)
        {
            Int32 result;
            if (Int32.TryParse(_currentTextBox.Text, out result))
            {
                result--;
                if (result >= 0)
                {
                   
                    _currentTextBox.Text = result.ToString();
                  
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void SendPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region private area
        public Int32 SelectedHour
        {
            get { return _selectedHour; }
            set
            {
                _selectedHour = value;
                SendPropertyChanged("SelectedHour");
            }
        }
        public Int32 SelectedMinute
        {
            get { return _selectedMinute; }
            set
            {
                _selectedMinute = value;
                SendPropertyChanged("SelectedMinute");
            }
        }
        public Int32 SelectedSecond
        {
            get { return _selectedSecond; }
            set
            {
                _selectedSecond = value;
                SendPropertyChanged("SelectedSecond");
            }
        }      
        #endregion

        private void ContentSite_TextChanged(object sender, TextChangedEventArgs e)
        {
            SelectedDateTimesFromTextbox = ((TextBox)sender).Text;
        }
    }
    public class Int32ToStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object obj, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return obj.ToString();
        }
        object IValueConverter.ConvertBack(object obj, Type type, object parameter, System.Globalization.CultureInfo culture)
        {
            return Int32.Parse((String)obj);
        }
    }
    public class TimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            Int32 iValue;
            if (Int32.TryParse((String)value, out iValue))
            {
                if (iValue < _minValue || iValue > _maxValue)
                    return new ValidationResult(false, "Out of range!");
                else
                    return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, null);
            }
        }

        public Int32 MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }
        public Int32 MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        private Int32 _minValue = 0;
        private Int32 _maxValue = 59;
    }
}
