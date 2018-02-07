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
using CO_IA.UI.MonitorTask.Task;
using System.Collections.ObjectModel;

namespace CO_IA.UI.Screen.Task
{
    /// <summary>
    /// TaskedList.xaml 的交互逻辑
    /// </summary>
    public partial class TaskedList : UserControl
    {
        public TaskedList()
        {
            InitializeComponent();
            this.Loaded += TaskManageModule_Loaded;
        }
        private void TaskManageModule_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGridTask.ItemsSource = null;
            this.dataGridTask.ItemsSource = Obj.taskData.Tasked;

        }
        //private void LoadData()
        //{
        //    var existTasks = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.Task[]>(channel =>
        //    {
        //        return channel.GetTasksByActivityGuid(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
        //    });
        //    this.dataGridTask.ItemsSource = null;
        //    this.dataGridTask.ItemsSource = new ObservableCollection<CO_IA.Data.Task>(existTasks);
        //}

        private void dataGridTask_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    CO_IA.Data.Task task = dgr.DataContext as CO_IA.Data.Task;
                    if (task != null)
                    {
                        Window wnd;
                        if (task.FormState == AT_BC.Data.FormState.Check)
                        {
                            wnd = new TaskExecutedWindow();
                        }
                        else
                        {
                            wnd = new TaskEditWindow();
                           
                            
                        }
                        wnd.DataContext = task;
                        wnd.ShowDialog(this);
                    }
                }
            }
        }

    }
}
