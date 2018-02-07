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
    public partial class UTimeControl : UserControl
    {
        public delegate void DataChangedHandle(object sender, DateTime dt);
        public event DataChangedHandle DataChanged;

        public static readonly DependencyProperty TimeValueProperty = DependencyProperty.Register("TimeValue", typeof(DateTime), typeof(UTimeControl),
          new PropertyMetadata(new PropertyChangedCallback(DateTimeValueChangedCallback)));
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime TimeValue
        {
            get { return (DateTime)GetValue(TimeValueProperty); }
            set { SetValue(TimeValueProperty, value); }
        }



        public static readonly DependencyProperty DateValueProperty = DependencyProperty.Register("DateValue", typeof(DateTime), typeof(UTimeControl), new PropertyMetadata(DateValueChangedCallback)); //属性默认值
        public DateTime DateValue
        {
            get { return (DateTime)GetValue(DateValueProperty); }
            set { SetValue(DateValueProperty, value); }
        }

        public static readonly DependencyProperty HourValueProperty = DependencyProperty.Register("HourValue", typeof(string), typeof(UTimeControl), new PropertyMetadata(HourValueChangedCallback)); //属性默认值
        public string HourValue
        {
            get { return (string)GetValue(HourValueProperty); }
            set { SetValue(HourValueProperty, value); }
        }


        public static readonly DependencyProperty MinitValueProperty = DependencyProperty.Register("MinitValue", typeof(string), typeof(UTimeControl), new PropertyMetadata(MinitValueChangedCallback)); //属性默认值
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
            UTimeControl thisDateTime = d as UTimeControl;
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
            UpdateNewValue(d as UTimeControl);
        }

        private static void HourValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue(d as UTimeControl);
        }
        private static void MinitValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue(d as UTimeControl);
        }

        private static void UpdateNewValue(UTimeControl UIDateTimeCtrl)
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
                UIDateTimeCtrl.SetValue(TimeValueProperty, dValue);
            }
        }

        public UTimeControl()
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
