using CO_IA.Data;
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

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// ScheduleItem.xaml 的交互逻辑
    /// </summary>
    public partial class ScheduleItem : UserControl
    {
        public bool ScheduleCheckState = false;
        public Action AddAction;
        public Action<Schedule> ModifyAction;
        public Action<Schedule> DeleteAction;
        public Schedule _schedule;
        public ScheduleItem(Schedule schedule)
        {
            InitializeComponent();
            _schedule = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<Schedule>(schedule);
            this.DataContext = _schedule;
            IniUI();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="modify"></param>
        /// <param name="check"></param>
        public ScheduleItem(Schedule schedule,bool modify,bool check)
        {
            InitializeComponent();
            if (!modify)
                stpContent.Visibility = System.Windows.Visibility.Collapsed;
            if (check)
                selectState.Visibility = System.Windows.Visibility.Visible;
            _schedule = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<Schedule>(schedule);
            this.DataContext = _schedule;
            IniUI();
        }
        private void IniUI()
        {
            if (_schedule == null || _schedule.ScheduleDetailInfos==null||_schedule.ScheduleDetailInfos.Length == 0)
                return;

            List<ScheduleDetail[]> scheduleList = GetDetailSchedule(_schedule.ScheduleDetailInfos);
            string start = "", stop = "";
            foreach (var details in scheduleList)
            {
                start = details[0].STARTTIME.ToString("yyyy-MM-dd");
                stop = details[0].STOPTIME.ToString("yyyy-MM-dd");
                if (start == stop)
                {
                    SetBorderToUI(details[0].STARTTIME.ToString("yyyy年MM月dd日"));
                    SetGridToUI(details);
                }
                else
                {
                    SetBorderToUI(details[0].STARTTIME.ToString("yyyy年MM月dd日 - ") + details[0].STOPTIME.ToString("yyyy年MM月dd日"));
                    SetGridToUI(details);
                }
            }
        }
        private void SetGridToUI(ScheduleDetail[] detailsinfo)
        {
            ScheduleDetailItem detailItem = new ScheduleDetailItem(detailsinfo);
            sp_detail.Children.Add(detailItem);
        }
        private void SetBorderToUI(string content)
        {
            Border border = new Border()
            {
                BorderThickness = new Thickness(0, 0, 1, 1),
                BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Height = 30
            };
            TextBlock tb = new TextBlock()
            {
                 FontSize=13,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = content,
                Margin = new Thickness(5,0,0,0)
            };
            border.Child = tb;
            sp_detail.Children.Add(border);
        }
        private List<ScheduleDetail[]> GetDetailSchedule(ScheduleDetail[] details)
        {
            List<ScheduleDetail[]> resultDetail = new List<ScheduleDetail[]>();
            var detail = details.OrderBy(x=>x.STARTTIME).ToList();
            string start = "",start1="";
            string stop = "",stop1="";
            for (int i = 0; i < detail.Count; i++)
            {
                List<ScheduleDetail> subDetails = new List<ScheduleDetail>();
                start = detail[i].STARTTIME.ToString("yyyy-MM-dd");
                stop = detail[i].STOPTIME.ToString("yyyy-MM-dd");
                if (start == stop) //先只考虑同一天的
                {
                    //SetBorderToUI(start);                    
                    //ScheduleDetail d = new ScheduleDetail();
                    //d = detail[i];
                    subDetails.Add(detail[i]);
                    detail.RemoveAt(i);
                    i--;
                    for (int j = 0; j < detail.Count; j++)
                    {
                        start1 = detail[j].STARTTIME.ToString("yyyy-MM-dd");
                        stop1 = detail[j].STOPTIME.ToString("yyyy-MM-dd");
                        if (start1 == stop1&&start==start1)//如果相等，两种可能，
                        {
                            subDetails.Add(detail[j]);
                            detail.RemoveAt(j);
                            j--;
                        }
                    }
                }
                else
                {
                    subDetails.Add(detail[i]);
                    detail.RemoveAt(i);
                    i--;
                    //SetBorderToUI(start+" 至 "+stop);
                }
                resultDetail.Add(subDetails.ToArray());
            }
            return resultDetail;
        }

        private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
        {
            if (AddAction != null)
                AddAction();
        }

        private void MenuItemModify_Click(object sender, RoutedEventArgs e)
        {
            if (ModifyAction != null)
            {
                ModifyAction(_schedule);
            }           
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteAction != null)
            {
                DeleteAction(_schedule);
            }      
        }

        private void editImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ModifyAction != null)
            {
                ModifyAction(_schedule);
            } 
        }

        private void deleteImg_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DeleteAction != null)
            {
                DeleteAction(_schedule);
            } 
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            stpContent.Opacity = 1;
        }

        private void stpContent_MouseLeave(object sender, MouseEventArgs e)
        {
            stpContent.Opacity = 0.2;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectState.IsChecked==true)
                ScheduleCheckState = true;
            else
                ScheduleCheckState = false;
        }
    }
}
