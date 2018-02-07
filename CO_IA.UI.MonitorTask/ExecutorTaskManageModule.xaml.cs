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
using System.Threading;
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
    public partial class ExecutorTaskManageModule : UserControl,CO_IA.Client.IMessageReceiver
    {
        private List<CO_IA.Data.Task> taskList = new List<Data.Task>();
        private CollectionViewSource executableTasks = new CollectionViewSource();
        private CollectionViewSource executedTasks = new CollectionViewSource();
        public List<PP_OrgInfo> OrgList
        {
            get;
            set;
        }
        public ExecutorTaskManageModule()
        {
            InitializeComponent();
            Binding binding = new Binding();
            binding.Source = this.executableTasks;
            this.dataGridExecutableTask.SetBinding(DataGrid.ItemsSourceProperty, binding);
            //this.dataGridExecutableTask.ItemsSource = this.executableTasks.View;
            binding = new Binding();
            binding.Source = this.executedTasks;
            this.dataGridExecutedTask.SetBinding(DataGrid.ItemsSourceProperty, binding);
            //this.dataGridExecutedTask.ItemsSource = this.executedTasks.View;
            this.executableTasks.Filter += executableTasks_Filter;
            this.executedTasks.Filter += executedTasks_Filter;
            this.executableTasks.Source = this.taskList;
            this.executedTasks.Source = this.taskList;
            this.Loaded += TaskManageModule_Loaded;
        }

        void executedTasks_Filter(object sender, FilterEventArgs e)
        {
            var task = e.Item as CO_IA.Data.Task;
            if (task != null)
            {
                e.Accepted = task.Executors[0].Executed;
            }
            else
            {
                e.Accepted = false;
            }
        }

        void executableTasks_Filter(object sender, FilterEventArgs e)
        {
            var task = e.Item as CO_IA.Data.Task;
            if (task != null)
            {
                e.Accepted = !task.Executors[0].Executed;
            }
            else
            {
                e.Accepted=true;
            }
        }

        private void TaskManageModule_Loaded(object sender, RoutedEventArgs e)
        {
            //CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid
            //CO_IA.Client.RiasPortal.ModuleContainer.GetExecutorLoginInfo().LoginOrg.GUID
            var existTasks = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.Task[]>(channel =>
                {
                    return channel.GetTasksByExecutorGuid(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, CO_IA.Client.RiasPortal.ModuleContainer.GetExecutorLoginInfo().LoginOrg.GUID);
                });
            this.taskList.Clear();
            if (existTasks != null)
            {
                this.taskList.AddRange(existTasks);
            }
            this.executedTasks.View.Refresh();
            this.executableTasks.View.Refresh();
            //this.executableTasks.Source = existTasks;
            //this.executedTasks.Source = existTasks;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                this.OrgList = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            CO_IA.Client.Utility.RegisterMessageReceiver(this);
            //this.executableTasks.View.Filter = current =>
            //{
            //    var task = current as CO_IA.Data.Task;
            //    if (task != null)
            //    {
            //        return !task.Executors[0].Executed;
            //    }
            //    return true;
            //};
            //this.executedTasks.View.Filter = current =>
            //{
            //    var task = current as CO_IA.Data.Task;
            //    if (task != null)
            //    {
            //        return task.Executors[0].Executed;
            //    }
            //    return false;
            //};

            //var executableTasks = (from task in existTasks where !task.Executors[0].Executed select task).ToArray();
            //this.dataGridExecutableTask.ItemsSource = new ObservableCollection<CO_IA.Data.Task>(executableTasks);
            //var executedTasks = (from task in existTasks where task.Executors[0].Executed select task).ToArray();
            //this.dataGridExecutedTask.ItemsSource = new ObservableCollection<CO_IA.Data.Task>(executedTasks);


            //this.gridTask.Children.Add(new Task.TaskEditControl());
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            var task=this.dataGridExecutableTask.SelectedValue as CO_IA.Data.Task;
            if (task == null)
            {
                MessageBox.Show("请先选择要删除的任务");
                return;
            }
            if (task.FormState== AT_BC.Data.FormState.None || task.FormState== AT_BC.Data.FormState.Tabulation)
            {
                if (MessageBox.Show("确实要删除选中的任务吗?", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.DeleteTask(task.Key);
                        });
                    (this.dataGridExecutableTask.ItemsSource as System.Collections.ObjectModel.ObservableCollection<CO_IA.Data.Task>).Remove(task);
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
                            wnd = new Task.ExecutorTaskConclusionWindow();
                            wnd.DataContext = task;
                            if (wnd.ShowDialog(this) == true)
                            {
                                this.executedTasks.View.Refresh();
                                this.executableTasks.View.Refresh();
                            }
                        }
                    }
                }
            }
        }

        private void dataGridTask_LayoutUpdated(object sender, EventArgs e)
        {
            //this.dataGridExecutableTask.RowHeight = double.NaN;
        }

        public void Receive(ActivityMessage message)
        {
            bool notifyUI = false;
            if (message.MessageType == "Task")
            {
                var messageTask = AT_BC.Data.Helpers.DataContractSerializeHelper.DeSerialize<CO_IA.Data.Task>(message.Content);
                bool bExist = false;
                foreach (var task in this.taskList)
                {
                    if (messageTask.Key == task.Key)
                    {
                        task.FormState = messageTask.FormState;
                        task.Description = messageTask.Description;
                        task.Creator = messageTask.Creator;
                        task.CreateTime = messageTask.CreateTime;
                        task.DisturbInfo = messageTask.DisturbInfo;
                        task.Executors = messageTask.Executors;
                        task.Submitter = messageTask.Submitter;
                        task.SubmitTime = messageTask.SubmitTime;
                        task.TaskPlaceID = messageTask.TaskPlaceID;
                        task.TaskType = messageTask.TaskType;
                        task.Urgency = messageTask.Urgency;
                        bExist = true;
                        break;
                    }
                }
                if (!bExist)
                {
                    this.taskList.Add(messageTask);
                    notifyUI = true;
                }
            }
            else if (message.MessageType == "TaskExecuteConclusion")
            {
                var conclusion = AT_BC.Data.Helpers.DataContractSerializeHelper.DeSerialize<CO_IA.Data.TaskExecuteConclusion>(message.Content);
                foreach (var task in this.taskList)
                {
                    if (conclusion.TaskGuid == task.Key)
                    {
                        if (task.Executors[0].Executor== conclusion.Executor)
                        {
                            task.Executors[0].Executed = conclusion.Executed;
                            notifyUI = true;
                        }
                    }
                }
            }
            if (notifyUI)
            {
                this.executedTasks.View.Refresh();
                this.executableTasks.View.Refresh();
            }
        }

        private SynchronizationContext syncContext = System.Threading.SynchronizationContext.Current;
        public System.Threading.SynchronizationContext SyncContext
        {
            get 
            {
                return this.syncContext;
            }
        }
    }
}
