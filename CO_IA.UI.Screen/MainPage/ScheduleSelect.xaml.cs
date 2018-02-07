using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.PersonSchedule;
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
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.MainPage
{
    /// <summary>
    /// ScheduleSelect.xaml 的交互逻辑
    /// </summary>
    public partial class ScheduleSelect : Window
    {
        public Schedule selectSchedule;
        Schedule[] allSchedule = null;
        public ScheduleSelect()
        {
            InitializeComponent();
            this.busyIndicator.IsBusy = true;
          //  this.Busy("正在查询，请稍候...");
            this.Loaded += ScheduleSelect_Loaded;
            
        }

        void ScheduleSelect_Loaded(object sender, RoutedEventArgs e)
        {
            
            IniUI();
            this.busyIndicator.IsBusy = false;
        }
        private void IniUI()
        {
            sp_content.Children.Clear();
            allSchedule = Utility.getScheduleInfo();
            if (allSchedule != null && allSchedule.Length > 0)
            {
                bool show = false;
                foreach (var item in allSchedule)
                {
                    if (item.STARTTIME <= DateTime.Now && item.STOPTIME >= DateTime.Now)
                    {
                        show = true;
                    }
                    else
                        show = false;
                    ScheduleItem scheduleItem = new ScheduleItem(item,false,show);
                    
                    sp_content.Children.Add(scheduleItem);
                }
            }
        }
       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in sp_content.Children)
            {
                ScheduleItem scheduleItem = item as ScheduleItem;
                if (scheduleItem != null && scheduleItem.ScheduleCheckState)
                {
                    selectSchedule = scheduleItem._schedule;
                    break;
                }
            }
            this.DialogResult = true;
        }
    }
}
