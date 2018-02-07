using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CO_IA.UI.PersonSchedule
{
    
    /// <summary>
    /// UIDateTime.xaml 的交互逻辑
    /// </summary>
    public partial class UIDateTime : UserControl
    {
        //public static readonly DependencyProperty DateTimeValueProperty = DependencyProperty.Register("DateTimeValue", typeof(string), typeof(UIDateTime),
        //    new PropertyMetadata(new PropertyChangedCallback(DateTimeValueChangedCallback))); 
        
        //public string DateTimeValue 
        //{
        //    get { return (string)GetValue(DateTimeValueProperty); }
        //    set { SetValue(DateTimeValueProperty, value); }
        //}
        DateTime baseTime ;
        public Action<DateTime> BeginDateTimeChanged;
        public Action<DateTime> StopDateTimeChanged;

        public static readonly DependencyProperty DateTimeTypeProperty = DependencyProperty.Register("DateTimeType", typeof(int), typeof(UIDateTime),
            new PropertyMetadata(-1,new PropertyChangedCallback(DateTimeTypeChangedCallback))); //

        public int DateTimeType
        {
            get { return (int)GetValue(DateTimeTypeProperty); }
            set { SetValue(DateTimeTypeProperty, value); }
        }

        public static readonly DependencyProperty StartDateTimeValueProperty = DependencyProperty.Register("StartDateTimeValue", typeof(DateTime), typeof(UIDateTime),
           new PropertyMetadata(new PropertyChangedCallback(StartDateTimeValueChangedCallback))); 
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDateTimeValue
        {
            get { return (DateTime)GetValue(StartDateTimeValueProperty); }
            set { SetValue(StartDateTimeValueProperty, value); }
        }

        public static readonly DependencyProperty StopDateTimeValueProperty = DependencyProperty.Register("StopDateTimeValue", typeof(DateTime), typeof(UIDateTime),
           new PropertyMetadata(new PropertyChangedCallback(StopDateTimeValueChangedCallback))); 
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime StopDateTimeValue
        {
            get { return (DateTime)GetValue(StopDateTimeValueProperty); }
            set { SetValue(StopDateTimeValueProperty, value); }
        }



        public static readonly DependencyProperty BeginDateValue_allProperty = DependencyProperty.Register("BeginDateValue_all", typeof(DateTime), typeof(UIDateTime), new PropertyMetadata(BeginDateValue_allChangedCallback)); //属性默认值
        public DateTime BeginDateValue_all
        {
            get { return (DateTime)GetValue(BeginDateValue_allProperty); }
            set { SetValue(BeginDateValue_allProperty, value); }
        }

        public static readonly DependencyProperty StopDateValue_allProperty = DependencyProperty.Register("StopDateValue_all", typeof(DateTime), typeof(UIDateTime), new PropertyMetadata(StopDateValue_allChangedCallback)); //属性默认值
        public DateTime StopDateValue_all
        {
            get { return (DateTime)GetValue(StopDateValue_allProperty); }
            set { SetValue(StopDateValue_allProperty, value); }
        }

        public static readonly DependencyProperty HourValue_allProperty = DependencyProperty.Register("HourValue_all", typeof(string), typeof(UIDateTime), new PropertyMetadata(HourValue_allChangedCallback)); //属性默认值
        public string HourValue_all
        {
            get { return (string)GetValue(HourValue_allProperty); }
            set { SetValue(HourValue_allProperty, value); }
        }


        public static readonly DependencyProperty MinitValue_allProperty = DependencyProperty.Register("MinitValue_all", typeof(string), typeof(UIDateTime), new PropertyMetadata(MinitValue_allChangedCallback)); //属性默认值
        public string MinitValue_all
        {
            get { return (string)GetValue(MinitValue_allProperty); }
            set { SetValue(MinitValue_allProperty, value); }            
        }





        public static readonly DependencyProperty DateValue_customProperty = DependencyProperty.Register("DateValue_custom", typeof(DateTime), typeof(UIDateTime), new PropertyMetadata(DateValue_customChangedCallback)); //属性默认值
        public DateTime DateValue_custom
        {
            get { return (DateTime)GetValue(DateValue_customProperty); }
            set { SetValue(DateValue_customProperty, value); }
        }

        public static readonly DependencyProperty BeginHourValue_customProperty = DependencyProperty.Register("BeginHourValue_custom", typeof(string), typeof(UIDateTime), new PropertyMetadata(BeginHourValue_customChangedCallback)); //属性默认值
        public string BeginHourValue_custom
        {
            get { return (string)GetValue(BeginHourValue_customProperty); }
            set { SetValue(BeginHourValue_customProperty, value); }
        }
        
        public static readonly DependencyProperty BeginMinitValue_customProperty = DependencyProperty.Register("BeginMinitValue_custom", typeof(string), typeof(UIDateTime), new PropertyMetadata(BeginMinitValue_customChangedCallback)); //属性默认值
        public string BeginMinitValue_custom
        {
            get { return (string)GetValue(BeginMinitValue_customProperty); }
            set { SetValue(BeginMinitValue_customProperty, value); }
        }
        public static readonly DependencyProperty StopHourValue_customProperty = DependencyProperty.Register("StopHourValue_custom", typeof(string), typeof(UIDateTime), new PropertyMetadata(StopHourValue_customChangedCallback)); //属性默认值
        public string StopHourValue_custom
        {
            get { return (string)GetValue(StopHourValue_customProperty); }
            set { SetValue(StopHourValue_customProperty, value); }
        }


        public static readonly DependencyProperty StopMinitValue_customProperty = DependencyProperty.Register("StopMinitValue_custom", typeof(string), typeof(UIDateTime), new PropertyMetadata(StopMinitValue_customChangedCallback)); //属性默认值
        public string StopMinitValue_custom
        {
            get { return (string)GetValue(StopMinitValue_customProperty); }
            set { SetValue(StopMinitValue_customProperty, value); }
        }

        public UIDateTime()
        {
            InitializeComponent();
            baseTime = DateTime.Parse("1999-01-01");
            List<string> hor = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                hor.Add(i.ToString().PadLeft(2,'0'));
            }
            cmbHor_all.ItemsSource = hor;
            cmbHor_custom1.ItemsSource = hor;
            cmbHor_custom2.ItemsSource = hor;

            List<string> minit = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                minit.Add(i.ToString().PadLeft(2, '0'));
            }
            cmbMinit_all.ItemsSource = minit;
            cmbMinit_custom1.ItemsSource = minit;
            cmbMinit_custom2.ItemsSource = minit;

            //MainGrid.DataContext = this;
        }
       
        //private static void DateTimeValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    UIDateTime thisDateTime = d as UIDateTime;
        //    string newValue = e.NewValue == null ? "" : e.NewValue.ToString();
        //    string[] t = newValue.Split(' ');
        //    if (t.Length == 2)
        //    {
        //        thisDateTime.SetValue(DateValueProperty, t[0]);
        //        string[] t2 = t[1].Split(':');
        //        if (t2.Length == 2)
        //        {
        //            thisDateTime.SetValue(HourValueProperty, t2[0]);
        //            thisDateTime.SetValue(MinitValueProperty, t2[1]);
        //        }
        //    }
        //}

        /// <summary>
        /// 根据类型来设置界面显示
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DateTimeTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            UIDateTime thisDateTime = d as UIDateTime;
            bool b = e.NewValue == e.OldValue;
            string newValue = e.NewValue == null ? "" : e.NewValue.ToString();
            int type = 0;
            if (!int.TryParse(newValue, out type))//0：全天 1：自定义时间段
            {
                type = 0;
            }
            if (type == 0)
            {
                thisDateTime.all_Grid.Visibility = Visibility.Visible;
                thisDateTime.custom_Grid.Visibility = Visibility.Collapsed;
                
            }
            else
            {
                thisDateTime.all_Grid.Visibility = Visibility.Collapsed;
                thisDateTime.custom_Grid.Visibility = Visibility.Visible;
            }
            if (!b)
            {
                thisDateTime.SetValue(StartDateTimeValueProperty,thisDateTime.StartDateTimeValue.AddSeconds(1));
                thisDateTime.SetValue(StopDateTimeValueProperty, thisDateTime.StopDateTimeValue.AddSeconds(1));
            }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void StartDateTimeValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTime dtNew = DateTime.Now;
            string newValue = e.NewValue == null ? "" : e.NewValue.ToString();
            if (DateTime.TryParse(newValue.ToString(), out dtNew))
            {
                
            }
            UIDateTime thisDateTime = d as UIDateTime;
            if (dtNew < thisDateTime.baseTime)
                return;
            string hor = dtNew.Hour.ToString().PadLeft(2, '0');
            string minit = dtNew.Minute.ToString().PadLeft(2, '0');

            
            int type = (int)thisDateTime.GetValue(DateTimeTypeProperty);
            //if (type == -1)
            //{
            //    thisDateTime.SetValue(BeginDateValue_allProperty, dtNew);
            //    thisDateTime.SetValue(HourValue_allProperty, hor);
            //    thisDateTime.SetValue(MinitValue_allProperty, minit);
            //    thisDateTime.SetValue(DateValue_customProperty, dtNew);
            //    thisDateTime.SetValue(BeginHourValue_customProperty, hor);
            //    thisDateTime.SetValue(BeginMinitValue_customProperty, minit);
            //}

            if (type == 0)
            {
                thisDateTime.SetValue(BeginDateValue_allProperty, dtNew);                
                thisDateTime.SetValue(HourValue_allProperty, hor);
                thisDateTime.SetValue(MinitValue_allProperty, minit);
            }
            else
            {
                thisDateTime.SetValue(DateValue_customProperty,dtNew);
                thisDateTime.SetValue(BeginHourValue_customProperty, hor);
                thisDateTime.SetValue(BeginMinitValue_customProperty, minit);
            }
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void StopDateTimeValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTime dtNew = DateTime.Now;
            string newValue = e.NewValue == null ? "" : e.NewValue.ToString();
            if (DateTime.TryParse(newValue.ToString(), out dtNew))
            {

            }
            UIDateTime thisDateTime = d as UIDateTime;
            if (dtNew < thisDateTime.baseTime)
                return;
            string hor = dtNew.Hour.ToString().PadLeft(2, '0');
            string minit = dtNew.Minute.ToString().PadLeft(2, '0');

            
            int type = (int)thisDateTime.GetValue(DateTimeTypeProperty);

            //if (type == -1)
            //{
            //    thisDateTime.SetValue(BeginDateValue_allProperty, dtNew);
            //    thisDateTime.SetValue(HourValue_allProperty, hor);
            //    thisDateTime.SetValue(MinitValue_allProperty, minit);
            //    //thisDateTime.SetValue(DateValue_customProperty, dtNew);
            //    thisDateTime.SetValue(BeginHourValue_customProperty, hor);
            //    thisDateTime.SetValue(BeginMinitValue_customProperty, minit);
            //}


            if (type == 0)
            {
                thisDateTime.SetValue(StopDateValue_allProperty, dtNew);
                thisDateTime.SetValue(HourValue_allProperty, hor);
                thisDateTime.SetValue(MinitValue_allProperty, minit);
            }
            else
            {
                thisDateTime.SetValue(DateValue_customProperty, dtNew);
                thisDateTime.SetValue(StopHourValue_customProperty, hor);
                thisDateTime.SetValue(StopMinitValue_customProperty, minit);
            }
            //UIDateTime thisDateTime = d as UIDateTime;
            //string newValue = e.NewValue == null ? "" : e.NewValue.ToString();
            //string[] t = newValue.Split(' ');
            //if (t.Length == 2)
            //{
            //    thisDateTime.SetValue(DateValueProperty, t[0]);
            //    string[] t2 = t[1].Split(':');
            //    if (t2.Length == 2)
            //    {
            //        thisDateTime.SetValue(HourValueProperty, t2[0]);
            //        thisDateTime.SetValue(MinitValueProperty, t2[1]);
            //    }
            //}
        }


        private static void BeginDateValue_allChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_all(d as UIDateTime);
        }
        private static void StopDateValue_allChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //UpdateNewValue(d as UIDateTime);
        }
        private static void HourValue_allChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_all(d as UIDateTime);
        }
        private static void MinitValue_allChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_all(d as UIDateTime);
        }





        private static void DateValue_customChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_custom(d as UIDateTime);
        }
        private static void BeginHourValue_customChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_custom(d as UIDateTime);
        }
        private static void BeginMinitValue_customChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_custom(d as UIDateTime);
        }
        private static void StopHourValue_customChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_custom(d as UIDateTime);
        }
        private static void StopMinitValue_customChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateNewValue_custom(d as UIDateTime);
        }



        private static void UpdateNewValue_all(UIDateTime UIDateTimeCtrl)
        {
            if (UIDateTimeCtrl != null)
            {
                DateTime dValue_begin = (DateTime)UIDateTimeCtrl.GetValue(BeginDateValue_allProperty);
                DateTime dValue_stop = (DateTime)UIDateTimeCtrl.GetValue(StopDateValue_allProperty);
                string hValue = (string)UIDateTimeCtrl.GetValue(HourValue_allProperty);
                string mValue = (string)UIDateTimeCtrl.GetValue(MinitValue_allProperty);                
                
                hValue = hValue == null ? "00" : hValue;
                mValue = mValue == null ? "00" : mValue;
                if (DateTime.TryParse(dValue_begin.ToString("yyyy-MM-dd")+" "+hValue+":"+mValue, out dValue_begin))
                {

                }
                if (DateTime.TryParse(dValue_stop.ToString("yyyy-MM-dd") + " " + hValue + ":" + mValue, out dValue_stop))
                {

                }
                UIDateTimeCtrl.SetValue(StartDateTimeValueProperty, dValue_begin);
                UIDateTimeCtrl.SetValue(StopDateTimeValueProperty, dValue_stop);
                if (UIDateTimeCtrl.BeginDateTimeChanged != null)
                    UIDateTimeCtrl.BeginDateTimeChanged(dValue_begin);
                if (UIDateTimeCtrl.StopDateTimeChanged != null)
                    UIDateTimeCtrl.StopDateTimeChanged(dValue_stop);
            }
        }
        private static void UpdateNewValue_custom(UIDateTime UIDateTimeCtrl)
        {
            if (UIDateTimeCtrl != null)
            {
                DateTime dValue = (DateTime)UIDateTimeCtrl.GetValue(DateValue_customProperty);
                string hValue_begin = (string)UIDateTimeCtrl.GetValue(BeginHourValue_customProperty);
                string mValue_begin = (string)UIDateTimeCtrl.GetValue(BeginMinitValue_customProperty);
                string hValue_stop = (string)UIDateTimeCtrl.GetValue(StopHourValue_customProperty);
                string mValue_stop = (string)UIDateTimeCtrl.GetValue(StopMinitValue_customProperty);

                hValue_begin = hValue_begin == null ? "00" : hValue_begin;
                mValue_begin = mValue_begin == null ? "00" : mValue_begin;
                hValue_stop = hValue_stop == null ? "00" : hValue_stop;
                mValue_stop = mValue_stop == null ? "00" : mValue_stop;

                DateTime BeginTime = DateTime.Now;
                DateTime StopTime = DateTime.Now;
                if (DateTime.TryParse(dValue.ToString("yyyy-MM-dd") + " " + hValue_begin + ":" + mValue_begin, out BeginTime))
                {

                }
                if (DateTime.TryParse(dValue.ToString("yyyy-MM-dd") + " " + hValue_stop + ":" + mValue_stop, out StopTime))
                {

                }
                UIDateTimeCtrl.SetValue(StartDateTimeValueProperty, BeginTime);
                UIDateTimeCtrl.SetValue(StopDateTimeValueProperty, StopTime);
                if (UIDateTimeCtrl.BeginDateTimeChanged != null)
                    UIDateTimeCtrl.BeginDateTimeChanged(BeginTime);
                if (UIDateTimeCtrl.StopDateTimeChanged != null)
                    UIDateTimeCtrl.StopDateTimeChanged(StopTime);
            }
        }
      
    }
   
}
