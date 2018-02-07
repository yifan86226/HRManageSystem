using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.Screen.MainPage;
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
using System.Windows.Threading;

namespace CO_IA.UI.Screen
{
    /// <summary>
    /// StagePanel.xaml 的交互逻辑
    /// </summary>
    public partial class StagePanel : UserControl
    {
        bool show = false;
        DateTimeValue dateTimeValue;
        DispatcherTimer timer;

        DateTime BeginTime;
        DateTime EndTime;

        DateTime ExecTime;
        public StagePanel()
        {
            InitializeComponent();
            this.border_stage.DataContext = Obj.Activity;
            dateTimeValue = new DateTimeValue();
            g_time.DataContext = dateTimeValue;

            dateTimeValue.Day = "000";
            dateTimeValue.Hour = "00";
            dateTimeValue.Minute = "00";
            dateTimeValue.Second = "00";

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,1);
            timer.Tick += timer_Tick;

            this.Loaded += StagePanel_Loaded;
        }

        void StagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            //找相应日程，开始倒计时
            Schedule[] infos = Utility.getScheduleInfo();
            if (infos == null || infos.Length == 0)
            {//如果没有日程，根据活动开始、结束时间
                DateTime.TryParse(Obj.Activity.DateFrom.ToString("yyyy-MM-dd 00:00:01"), out BeginTime);
                DateTime.TryParse(Obj.Activity.DateTo.ToString("yyyy-MM-dd 23:23:59"), out EndTime);
                Start();
                return;
            }
            DateTime dt = DateTime.Now;
            var schedule = infos.Where(item=>
                {
                    if (dt >= item.STARTTIME && dt < item.STOPTIME)
                        return true;
                    return false;
                }).ToArray();
            if (schedule != null && schedule.Length == 1)
            {
                BeginTime = schedule[0].STARTTIME;
                EndTime = schedule[0].STOPTIME;
                Start(schedule[0].NAME);
            }
            else
            {
                Schedule[] info = infos.OrderBy(item=>item.STARTTIME).ToArray();
                if (dt < infos[0].STARTTIME)
                {
                    BeginTime = info[0].STARTTIME;
                    EndTime = info[0].STOPTIME;
                    Start(info[0].NAME);
                }
                else
                {
                    timeContent.Text =  "活动时间已过";
                    timeContent.ToolTip = timeContent.Text;
                    border_type.Background = new SolidColorBrush(Color.FromArgb(255,17,83,41));//#FF115329 #FFC34023
                }
                
            }
        }
        private void Start()
        {
            string flag = "开始";
            if (DateTime.Now >= EndTime)
            {
                //MessageBox.Show("此倒计时时间已过！");
                timeContent.Text = "活动时间已过！";
                border_type.Background = new SolidColorBrush(Color.FromArgb(255, 195, 64, 35));
                timeContent.ToolTip = timeContent.Text;
                return;
            }
            if (DateTime.Now >= BeginTime)
            {
                ExecTime = EndTime;
                flag = "结束";
                border_type.Background = new SolidColorBrush(Color.FromArgb(255, 195, 64, 35));
            }
            else
            {
                ExecTime = BeginTime;
                flag = "开始";
                border_type.Background = new SolidColorBrush(Color.FromArgb(255, 17, 83, 41));
            }
            timeContent.Text = "距活动" + flag + "时间";
            timeContent.ToolTip = timeContent.Text;
            timer.Start();
        }
        private void Start(string name)
        {
            string flag = "开始";
            if (DateTime.Now >= EndTime)
            {
                //MessageBox.Show("此倒计时时间已过！");
                timeContent.Text = "日程"+name+"时间已过！";
                timeContent.ToolTip = timeContent.Text;
                return;
            }
            if (DateTime.Now >= BeginTime)
            {
                ExecTime = EndTime;
                flag = "结束";
                border_type.Background = new SolidColorBrush(Color.FromArgb(255, 195, 64, 35));
            }
            else
            {
                ExecTime = BeginTime;
                flag = "开始";
                border_type.Background = new SolidColorBrush(Color.FromArgb(255, 17, 83, 41));
            }
            timeContent.Text = "距日程[" + name +"]" + flag + "时间";
            timeContent.ToolTip = timeContent.Text;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            if (ExecTime < DateTime.Now)
            {
                timer.Stop();
                StagePanel_Loaded(null,null);
                return;
            }
            TimeSpan span = ExecTime - DateTime.Now;
            dateTimeValue.Day = span.Days.ToString().PadLeft(3, '0');
            dateTimeValue.Hour = span.Hours.ToString().PadLeft(2, '0');
            dateTimeValue.Minute = span.Minutes.ToString().PadLeft(2, '0');
            dateTimeValue.Second = span.Seconds.ToString().PadLeft(2, '0');

            
        }

        #region UI控制
        public void setTimeUI(bool v)
        {
            TranslateTransform sctr = new TranslateTransform();
            if (v)
            {
                sctr.Y = 0;
                //SetBottomContent(true);
            }
            else
            {
                sctr.Y = -168;
                //SetBottomContent(false);
            }
            TransformGroup trfg = new TransformGroup();
            trfg.Children.Add(sctr);
            bgTime.RenderTransform = trfg;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if(!show)
                setTimeUI(true);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!show)
                setTimeUI(false);
        }

        /// <summary>
        /// 停靠按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (!show)
            {
                RotateTransform sctr = new RotateTransform();
                sctr.Angle = -45;
                TransformGroup trfg = new TransformGroup();
                trfg.Children.Add(sctr);
                img.RenderTransform = trfg;                
                show = true;
                setTimeUI(show);
                img.ToolTip = "隐藏";
            }
            else
            {
                RotateTransform sctr = new RotateTransform();
                sctr.Angle = 0;
                TransformGroup trfg = new TransformGroup();
                trfg.Children.Add(sctr);
                img.RenderTransform = trfg;               
                show = false;
                setTimeUI(show);
                img.ToolTip = "展开";
            }

        }


        //private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (border_type.Tag == null || border_type.Tag.ToString() == "0")
        //    {
        //        SetBottomContent(true);
        //        border_type.Tag = "1";
        //    }
        //    else
        //    {
        //        SetBottomContent(false);
        //        border_type.Tag = "0";
        //    }
        //}
        private void SetBottomContent(bool v)
        {
            TranslateTransform sctr = new TranslateTransform();
            if (v)
            {
                sctr.Y = 22;
            }
            else
            {
                sctr.Y = 0;
            }
            TransformGroup trfg = new TransformGroup();
            trfg.Children.Add(sctr);
            border_type.RenderTransform = trfg;
        
        }
        #endregion

        private void ImageEdit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            ScheduleSelect win = new ScheduleSelect();
            if (win.ShowDialog(this) == true)
            {
                Schedule schedule = win.selectSchedule;
                if (schedule != null)
                {
                    BeginTime = schedule.STARTTIME;
                    EndTime = schedule.STOPTIME;
                    Start(schedule.NAME);
                }
            }
        }
    }

    public class DateTimeValue : NotifyPropertyChangedObject
    {
        private string day;
        public string Day
        {
            get
            {
                return day;
            }
            set
            {
                day = value;
                NotifyPropertyChanged("Day");
            }
        }
        private string hour;
        public string Hour
        {
            get
            {
                return hour;
            }
            set
            {
                hour = value;
                NotifyPropertyChanged("Hour");
            }
        }
        private string minute;
        public string Minute
        {
            get
            {
                return minute;
            }
            set
            {
                minute = value;
                NotifyPropertyChanged("Minute");
            }
        }

        private string second;
        public string Second
        {
            get
            {
                return second;
            }
            set
            {
                second = value;
                NotifyPropertyChanged("Second");
            }
        }
    }
}
