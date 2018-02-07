using CO_IA.Client;
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
using System.IO;
namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// ScheduleModule.xaml 的交互逻辑
    /// </summary>
    public partial class ScheduleModule : UserControl
    {
        Schedule[] allSchedule=null;
        //ScheduleItemDialog itemDialog = null;
        public ScheduleModule()
        {
            InitializeComponent();
            IniUI();
        }
        private void IniUI()
        {
            sp_content.Children.Clear();
            allSchedule = Utility.getScheduleInfo();
            if (allSchedule != null && allSchedule.Length > 0)
            {
                foreach (var item in allSchedule)
                {
                    ScheduleItem scheduleItem = new ScheduleItem(item);
                    scheduleItem.AddAction += AddItem;
                    scheduleItem.ModifyAction += ModifyItem;
                    scheduleItem.DeleteAction += DeleteItem;
                    sp_content.Children.Add(scheduleItem);
                }
            }
        }
        

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            AddItem();
        }
        private void ItemChanged(bool success)
        {
            if (success)
            {
                IniUI();
            }
        }

        private void AddItem()
        {
            Schedule newSchedule = new Schedule()
            {
                NAME = "新建日程",
                ACTIVITY_GUID = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid,
                STARTTIME = DateTime.Now,
                STOPTIME = DateTime.Now.AddDays(3),
                GUID = Guid.NewGuid().ToString(),
                MEMO = "",
                ScheduleDetailInfos = new ScheduleDetail[0]
            };
            ScheduleItemDialog itemDialog = new ScheduleItemDialog(newSchedule);
            //itemDialog.allSchedule = allSchedule;
            itemDialog.ItemChanged = ItemChanged;
            itemDialog.ShowDialog(this);
        }
        private void ModifyItem(Schedule item)
        {
            ScheduleItemDialog itemDialog = new ScheduleItemDialog(item);
            //itemDialog.allSchedule = allSchedule;
            itemDialog.ItemChanged = ItemChanged;
            itemDialog.ShowDialog(this);
        }
        public void DeleteItem(Schedule item)
        {
            if (MessageBox.Show("确定要删除此日程吗", "询问", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    channel.DeleteScheduleByGuid(item.GUID);
                    IniUI();                    
                });

            }
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            Utility.PrintElement(mainGrid);
        }

    }
}
