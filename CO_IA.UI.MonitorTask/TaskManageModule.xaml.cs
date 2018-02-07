using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.MonitorTask
{
    /// <summary>
    /// TaskManageModule.xaml 的交互逻辑
    /// </summary>
    public partial class TaskManageModule : UserControl
    {
        public List<PP_OrgInfo> OrgList
        {
            get;
            set;
        }
        public TaskManageModule()
        {
            InitializeComponent();
            this.Loaded += TaskManageModule_Loaded;
        }

        private void TaskManageModule_Loaded(object sender, RoutedEventArgs e)
        {

            var existTasks = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.Task[]>(channel =>
                {
                    return channel.GetTasksByActivityGuid(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });
            this.dataGridTask.ItemsSource = new ObservableCollection<CO_IA.Data.Task>(existTasks);
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    this.OrgList = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });

            //this.gridTask.Children.Add(new Task.TaskEditControl());
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            CO_IA.Data.Task task = new Data.Task();
            task.ActivityGuid = RiasPortal.ModuleContainer.Activity.Guid;
            task.FormState = AT_BC.Data.FormState.None;
            task.Key = Utility.NewGuid();
            task.TaskType = TaskType.Normal;
            task.Urgency = TaskUrgency.Normal;
            var wnd = new Task.TaskEditWindow();
            wnd.DataContext = task;
            wnd.OnSaveNewTask += savedTask =>
                {
                    (this.dataGridTask.ItemsSource as System.Collections.ObjectModel.ObservableCollection<CO_IA.Data.Task>).Add(savedTask);
                };
            wnd.ShowDialog(this);
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var task = this.dataGridTask.SelectedValue as CO_IA.Data.Task;
            if (task == null)
            {
                MessageBox.Show("请先选择要删除的任务");
                return;
            }
            if (task.FormState == AT_BC.Data.FormState.None || task.FormState == AT_BC.Data.FormState.Tabulation)
            {
                if (MessageBox.Show("确实要删除选中的任务吗?", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.DeleteTask(task.Key);
                        });
                    (this.dataGridTask.ItemsSource as System.Collections.ObjectModel.ObservableCollection<CO_IA.Data.Task>).Remove(task);
                }
            }
            else
            {
                MessageBox.Show("已经提交的任务不能删除");
            }
        }

        private void dataGridTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void dataGridTask_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                            wnd = new Task.TaskExecutedWindow();
                        }
                        else
                        {
                            wnd = new Task.TaskEditWindow();
                        }
                        wnd.DataContext = task;
                        wnd.ShowDialog(this);
                    }
                }
            }
        }

        private void dataGridTask_LayoutUpdated(object sender, EventArgs e)
        {
            this.dataGridTask.RowHeight = double.NaN;
        }

        private void MenuItemNormalTask_Click(object sender, RoutedEventArgs e)
        {
            this.NewTask(TaskType.Normal);
        }

        private void MenuItemDisturbTask_Click(object sender, RoutedEventArgs e)
        {
            this.NewTask(TaskType.Disturb);
        }

        private void MenuItemBroadcastTask_Click(object sender, RoutedEventArgs e)
        {
            this.NewTask(TaskType.Broadcast);
        }

        private void NewTask(TaskType taskType)
        {
            CO_IA.Data.Task task = new Data.Task();
            task.ActivityGuid = RiasPortal.ModuleContainer.Activity.Guid;
            task.FormState = AT_BC.Data.FormState.None;
            task.Key = Utility.NewGuid();
            task.TaskType = taskType;
            task.Urgency = TaskUrgency.Normal;
            if (taskType == TaskType.Disturb)
            {
                task.DisturbInfo = new TaskDisturbInfo();
            }
            var wnd = new Task.TaskEditWindow();
            wnd.DataContext = task;
            wnd.OnSaveNewTask += savedTask =>
            {
                (this.dataGridTask.ItemsSource as System.Collections.ObjectModel.ObservableCollection<CO_IA.Data.Task>).Add(savedTask);
            };
            wnd.ShowDialog(this);
        }
    }

    public class OrgNameMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                if (values[1] is IList<PP_OrgInfo>)
                {
                    IList<PP_OrgInfo> list = values[1] as IList<PP_OrgInfo>;
                    string code = values[0] as string;
                    foreach (var org in list)
                    {
                        if (org.GUID == code)
                        {
                            return org.NAME;
                        }
                    }
                    return code;
                }
            }
            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ExecutorListSelector : DataTemplateSelector
    {
        private DataTemplate submittedTemplate = null;

        public DataTemplate SubmittedTemplate
        {
            get { return submittedTemplate; }
            set { submittedTemplate = value; }
        }

        private DataTemplate unSubmittedTemplate = null;
        public DataTemplate UnSubmittedTemplate
        {
            get { return unSubmittedTemplate; }
            set { unSubmittedTemplate = value; }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is CO_IA.Data.Task)
            {
                if ((item as CO_IA.Data.Task).FormState == AT_BC.Data.FormState.Check)
                {
                    return this.SubmittedTemplate;
                }
                else
                {
                    return this.UnSubmittedTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

    }
}
