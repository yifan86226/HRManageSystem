using AT_BC.Common;
using AT_BC.Data;
using CO_IA.Data;
using CO_IA.UI.MAP;
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

namespace CO_IA.UI.MonitorTask.Task
{
    /// <summary>
    /// TaskEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TaskEditWindow : Window
    {
        public Action<CO_IA.Data.Task> OnSaveNewTask;
        private ActivityPlaceMap activityMap = new ActivityPlaceMap();
        public TaskEditWindow()
        {
            InitializeComponent();
            this.Loaded += TaskEditWindow_Loaded;
            this.taskEditControl.OnOrgSelected += org =>
                {
                    this.activityMap.ShowMap.setElementExtent(string.Format("{0}{1}", CO_IA.Client.MapGroupTypes.MonitorGroup_, org.GUID));
                };
        }

        private void TaskEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.borderMapContainer.Child = this.activityMap.ShowMap.MainMap;
            this.activityMap.ShowMap.MapInitialized += MapInitialized;
        }

        private void MapInitialized(bool obj)
        {
            if (obj)
            {
                Dictionary<string, ActivityPlaceInfo> dic = new Dictionary<string, ActivityPlaceInfo>();
                var placeInfos = CO_IA.Client.Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                foreach (var placeInfo in placeInfos)
                {
                    dic.Add(placeInfo.Guid, placeInfo);
                }
                //dic.Add(placeInfo.Guid, placeInfo);
                this.activityMap.PlaceLocation = dic;


                ToMapHelper.DrawOrgsToMap(this.activityMap.ShowMap, CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityStage);

                this.activityMap.ShowMap.SetAllGraphicsExtent();
            }
        }
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            var task=this.DataContext as CO_IA.Data.Task;
            if (task != null)
            {
                bool isNew = (task.FormState == AT_BC.Data.FormState.None);
                this.taskEditControl.SaveTask(FormState.Tabulation);
                if (isNew && this.OnSaveNewTask != null)
                {
                    this.OnSaveNewTask(task);
                }
            }
        }

        private void buttonSubmit_Click(object sender, RoutedEventArgs e)
        {
            var task = this.DataContext as CO_IA.Data.Task;
            if (task != null)
            {
                bool isNew = (task.FormState == AT_BC.Data.FormState.None);
                this.taskEditControl.SaveTask(FormState.Check);
                if (isNew && this.OnSaveNewTask != null)
                {
                    this.OnSaveNewTask(task);
                }
            }
            this.DialogResult = true;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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
