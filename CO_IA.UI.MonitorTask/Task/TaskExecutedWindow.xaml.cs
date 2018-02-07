using AT_BC.Common;
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
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;

namespace CO_IA.UI.MonitorTask.Task
{
    /// <summary>
    /// TaskExecutedWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TaskExecutedWindow : Window
    {
        public TaskExecutedWindow()
        {
            InitializeComponent();
            this.DataContextChanged += TaskExecutedWindow_DataContextChanged;
        }

        private void TaskExecutedWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CO_IA.Data.Task)
            {
                var result = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, TaskExecuteConclusion[]>(channel =>
                {
                    return channel.GetTaskExecuteConclusions((e.NewValue as CO_IA.Data.Task).Key);
                });
                this.tabControlExecuteConclusion.ItemsSource = result;
                this.OrgList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, List<PP_OrgInfo>>(channel =>
                {
                    return channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public List<PP_OrgInfo> OrgList
        {
            get;
            private set;
        }

        private void FileDescriptionSaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is TaskStuff)
            {
                var taskStuff = e.Parameter as TaskStuff;
                if (!taskStuff.IsLoaded)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                    {
                        var result = channel.GetTaskStuffByGuid(taskStuff.Key);
                        if (result.Content != null && result.Content.Length > 0)
                        {
                            taskStuff.Content = result.Content;
                        }
                    });
                }
                FileDescriptionHelper.SaveAs(sender, e);
            }
        }

        private void FileDescriptionOpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            TaskStuffFileHelper.OpenFile(sender, e, this);
        }
    }
}
