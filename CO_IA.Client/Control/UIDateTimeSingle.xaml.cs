using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.Client
{
    /// <summary>
    /// UIDateTimeSingle.xaml 的交互逻辑
    /// </summary>
    public partial class UIDateTimeSingle : UserControl
    {
        public delegate void DataChangedHandle(object sender, DateTime dt);
        public event DataChangedHandle DataChanged;

        public static readonly DependencyProperty DateTimeValueProperty = DependencyProperty.Register("DateTimeValue", typeof(DateTime), typeof(UIDateTimeSingle),
          new PropertyMetadata(new PropertyChangedCallback(DateTimeValueChangedCallback)));
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DateTimeValue
        {
            get { return (DateTime)GetValue(DateTimeValueProperty); }
            set { SetValue(DateTimeValueProperty, value); }
        }



        public static readonly DependencyProperty DateValueProperty = DependencyProperty.Register("DateValue", typeof(DateTime), typeof(UIDateTimeSingle), new PropertyMetadata(DateValueChangedCallback)); //属性默认值
        public DateTime DateValue
        {
            get { return (DateTime)GetValue(DateValueProperty); }
            set { SetValue(DateValueProperty, value); }
        }

        public static readonly DependencyProperty HourValueProperty = DependencyProperty.Register("HourValue", typeof(string), typeof(UIDateTimeSingle), new PropertyMetadata(HourValueChangedCallback)); //属性默认值
        public string HourValue
        {
            get { return (string)GetValue(HourValueProperty); }
            set { SetValue(HourValueProperty, value); }
        }


        public static readonly DependencyProperty MinitValueProperty = DependencyProperty.Register("MinitValue", typeof(string), typeof(UIDateTimeSingle), new PropertyMetadata(MinitValueChangedCallback)); //属性默认值
        public string MinitValue
        {
            get { return (string)GetValue(MinitValueProperty); }
            set { SetValue(MinitValueProperty, value); }
        }


        private static void DateTimeValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTime dtNew = DateTime.Now;
            string newValue = e.NewValue == null ? "" : e.NewValue.ToString();
            if (DateTime.TryParse(newValue.ToString(), out dtNew))
            {

            }
            UIDateTimeSingle thisDateTime = d as UIDateTimeSingle;
            string hor = dtNew.Hour.ToString().PadLeft(2, '0');
            string minit = dtNew.Minute.ToString().PadLeft(2, '0');

            thisDateTime.SetValue(DateValueProperty, dtNew);
            thisDateTime.SetValue(HourValueProperty, hor);
            thisDateTime.SetValue(MinitValueProperty, minit);

            if (thisDateTime.DataChanged != null)
            {
                thisDateTime.DataChanged(thisDateTime,(DateTime)thisDateTime.GetValue(DateValueProperty));
            }
            
        }
        private static void DateValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue(d as UIDateTimeSingle);
        }

        private static void HourValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue(d as UIDateTimeSingle);
        }
        private static void MinitValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue(d as UIDateTimeSingle);
        }

        private static void UpdateNewValue(UIDateTimeSingle UIDateTimeCtrl)
        {
            if (UIDateTimeCtrl != null)
            {
                DateTime dValue = (DateTime)UIDateTimeCtrl.GetValue(DateValueProperty);
                string hValue = (string)UIDateTimeCtrl.GetValue(HourValueProperty);
                string mValue = (string)UIDateTimeCtrl.GetValue(MinitValueProperty);

                hValue = hValue == null ? "00" : hValue;
                mValue = mValue == null ? "00" : mValue;
                if (DateTime.TryParse(dValue.ToString("yyyy-MM-dd") + " " + hValue + ":" + mValue, out dValue))
                {

                }
                UIDateTimeCtrl.SetValue(DateTimeValueProperty, dValue);
            }
        }
      
        public UIDateTimeSingle()
        {
            InitializeComponent();
            List<string> hor = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                hor.Add(i.ToString().PadLeft(2, '0'));
            }
            cmbHor.ItemsSource = hor;

            List<string> minit = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                minit.Add(i.ToString().PadLeft(2, '0'));
            }
            cmbMinit.ItemsSource = minit;
           
        }
    }
}
