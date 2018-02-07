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
    /// CDateTimePicker.xaml 的交互逻辑
    /// </summary>
    public partial class CDateTimePicker : UserControl, INotifyPropertyChanged
    {
        public CDateTimePicker()
        {
            InitializeComponent();
            Init(DateTime.Now);
            DataContext = this;
        }

        private DateTime _defaultTime;

        private List<string> _years = new List<string>();
        private List<string> _months = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        private List<string> _days = new List<string>();
        private List<string> _days1 = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"};
        private List<string> _days2 = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30" };
        private List<string> _days22 = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28" };
        private List<string> _days21 = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29"};
        private List<string> _hours = new List<string> { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"};
        private List<string> _mintues = new List<string>
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", 
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"
        };

        private string _yearSelected;
        private string _monthSelected;
        private string _daySelected;

        private string _dateString;
        public string DateString
        {
            get { return _dateString; }
            set
            {
                _dateString = value; 
                OnPropertyChanged("DateString");
            }
        }

        private string _hourSelected;
        private string _minSelected;

        private string _timeString;
        public string TimeString
        {
            get { return _timeString; }
            set
            {
                _timeString = value;
                OnPropertyChanged("TimeString");
            }
        }

        public DateTime DateTimeSelected
        {
            get
            {
                return new DateTime(int.Parse(_yearSelected), int.Parse(_monthSelected), int.Parse(_daySelected),
                    int.Parse(_hourSelected), int.Parse(_minSelected), 0);
            }
        }

        public static readonly new DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof (bool), typeof (CDateTimePicker), new PropertyMetadata(default(bool), NewIsEnabledPropertyChangedCallback));

        private static void NewIsEnabledPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = d as CDateTimePicker;

            if ((bool)e.NewValue)
            {
                v.XBdDate.Visibility = v.XBdTime.Visibility = Visibility.Collapsed;
            }
            else
            {
                v.XBdDate.Visibility = v.XBdTime.Visibility = Visibility.Visible;
            }
        }

        public new bool IsEnabled
        {
            get { return (bool) GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public void Init(DateTime pDateTime)
        {
            _defaultTime = pDateTime;
            _years.Clear();
            int curYear = _defaultTime.Year;
            for (int i = curYear; i >= curYear - 9; i--)
            {
                _years.Add(i.ToString());
            }
            YearPicker.ItemsSource = _years;
            MonthPicker.ItemsSource = _months;
            HourPicker.ItemsSource = _hours;
            MinutePicker.ItemsSource = _mintues;

            YearPicker.SelectedIndex = 0;
            MonthPicker.SelectedIndex = _defaultTime.Month - 1;
            DayPicker.SelectedIndex = _defaultTime.Day - 1;
            HourPicker.SelectedIndex = _defaultTime.Hour;
            MinutePicker.SelectedIndex = _defaultTime.Minute;
        }

        private void UpdateDayPickerSource()
        {
            if (YearPicker.SelectedIndex == -1 || MonthPicker.SelectedIndex == -1)
            {
                return;
            }

            int y = int.Parse(YearPicker.SelectedItem.ToString());
            int m = int.Parse(MonthPicker.SelectedItem.ToString());

            if (y%4 == 0)
            {
                if (m == 2)
                {
                    _days = _days21;
                }
                else if (m == 4 || m == 6 || m == 9 || m == 11)
                {
                    _days = _days2;
                }
                else
                {
                    _days = _days1;
                }
            }
            else
            {
                if (m == 2)
                {
                    _days = _days22;
                }
                else if (m == 4 || m == 6 || m == 9 || m == 11)
                {
                    _days = _days2;
                }
                else
                {
                    _days = _days1;
                }
            }

            DayPicker.ItemsSource = _days;
        }

        private void XtBtnDate_OnClick(object sender, RoutedEventArgs e)
        {
            if (XtBtnDate.IsChecked == true)
            {
                XtBtnTime.IsChecked = false;
            }
        }

        private void XtBtnTime_OnClick(object sender, RoutedEventArgs e)
        {
            if (XtBtnTime.IsChecked == true)
            {
                XtBtnDate.IsChecked = false;
            }
        }

        private void YearPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDayPickerSource();

            _yearSelected = ((ComboBox) sender).SelectedItem.ToString();
            UpdateDateString();
        }

        private void MonthPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDayPickerSource();

            _monthSelected = ((ComboBox)sender).SelectedItem.ToString();
            UpdateDateString();
        }

        private void DayPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _daySelected = ((ComboBox)sender).SelectedItem.ToString();
            UpdateDateString();
        }

        private void HourPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _hourSelected = ((ComboBox)sender).SelectedItem.ToString();
            UpdateTimeString();
        }

        private void MinutePicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _minSelected = ((ComboBox)sender).SelectedItem.ToString();
            UpdateTimeString();
        }

        private void UpdateDateString()
        {
            DateString = _yearSelected + "-" + _monthSelected + "-" + _daySelected;
        }

        private void UpdateTimeString()
        {
            TimeString = _hourSelected + ":" + _minSelected;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }
}
