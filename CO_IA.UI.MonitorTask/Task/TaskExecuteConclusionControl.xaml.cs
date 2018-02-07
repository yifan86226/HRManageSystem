using CO_IA.Data;
using CO_IA.Types;
using AT_BC.Types;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using AT_BC.Client.Extensions;
using System.Collections.ObjectModel;
using CO_IA.Client;

namespace CO_IA.UI.MonitorTask.Task
{
    /// <summary>
    /// TaskExecuteConclusionControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskExecuteConclusionControl : EditableUserControl
    {
        public TaskExecuteConclusionControl()
        {
            InitializeComponent();
            TaskCompleteState[] taskCompleteStates = AT_BC.Data.Helpers.EnumHelper.GetEnumValues<TaskCompleteState>();
            foreach (var taskCompleteState in taskCompleteStates)
            {
                var radioButton = new RadioButton() { DataContext = taskCompleteState, Content = taskCompleteState.GetDisplayNameFromEnumDisplayNameAttribute() };
                this.wrapPanelCompleteState.Children.Add(radioButton);
            }
            //wrapPanelDisturbDisposeType

            DisturbDisposeType[] disposeTypes = AT_BC.Data.Helpers.EnumHelper.GetEnumValues<DisturbDisposeType>(DisturbDisposeType.None);
            foreach (var disposeType in disposeTypes)
            {
                var radioButton = new RadioButton() { DataContext = disposeType, Content = disposeType.GetDisplayNameFromEnumDisplayNameAttribute() };
                this.wrapPanelDisturbDisposeType.Children.Add(radioButton);
            }
            //wrapPanelDisturbDisposeType
            this.DataContextChanged += TaskExecuteConclusionControl_DataContextChanged;
        }

        private void TaskExecuteConclusionControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var taskExecuteConclusion = e.NewValue as TaskExecuteConclusion;
            if (taskExecuteConclusion != null)
            {
                if (taskExecuteConclusion.StuffList != null)
                {
                    this.listBoxTaskStuff.ItemsSource = new ObservableCollection<TaskStuff>(taskExecuteConclusion.StuffList);
                }
                else
                {
                    this.listBoxTaskStuff.ItemsSource = new ObservableCollection<TaskStuff>();
                }
                if (taskExecuteConclusion.Executed)
                {
                    foreach (var child in this.wrapPanelCompleteState.Children)
                    {
                        var radioButton = child as RadioButton;
                        if (taskExecuteConclusion.CompleteState.Equals(radioButton.DataContext))
                        {
                            radioButton.IsChecked = true;
                            break;
                        }
                    }
                    if (taskExecuteConclusion.TaskType == TaskType.Disturb)
                    {
                        foreach (var child in this.wrapPanelDisturbDisposeType.Children)
                        {
                            var radioButton = child as RadioButton;
                            if (taskExecuteConclusion.DisposeType.Equals(radioButton.DataContext))
                            {
                                radioButton.IsChecked = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var child in this.wrapPanelDisturbDisposeType.Children)
                        {
                            var radioButton = child as RadioButton;
                            radioButton.IsChecked = false;
                        }
                    }
                }
                else
                { 
                    foreach (var child in this.wrapPanelCompleteState.Children)
                    {
                        var radioButton = child as RadioButton;
                        radioButton.IsChecked = false;
                    }
                    foreach (var child in this.wrapPanelDisturbDisposeType.Children)
                    {
                        var radioButton = child as RadioButton;
                        radioButton.IsChecked = false;
                    }
                }
            }
        }

        private void buttonTaskStuffDownload_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink != null)
            {
                TaskStuff taskStuff = hyperlink.DataContext as TaskStuff;
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.FileName = taskStuff.Name;
                dialog.DefaultExt = System.IO.Path.GetExtension(taskStuff.Name);
                dialog.Filter = string.Format("*.{0}|*.{0}", dialog.DefaultExt);
                if (dialog.ShowDialog() == true)
                {
                    Stream fs = null;
                    try
                    {
                        fs = dialog.OpenFile();
                        if (taskStuff.IsLoaded)
                        {
                            fs.Write(taskStuff.Content, 0, taskStuff.Content.Length);
                        }
                        else
                        {
                            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                            {
                                var result = channel.GetTaskStuffByGuid(taskStuff.Key);
                                if (result.Content != null && result.Content.Length > 0)
                                {
                                    taskStuff.Content = result.Content;
                                    taskStuff.IsLoaded = true;
                                    fs.Write(result.Content, 0, result.Content.Length);
                                }
                            });
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }
            }
        }

        private void buttonTaskStuffDelete_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink != null)
            {
                TaskStuff taskStuff = hyperlink.DataContext as TaskStuff;
                if (taskStuff != null)
                {
                    if (MessageBox.Show("确实要删除选中的资料吗?", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.DeleteTaskStuff(taskStuff.Key);
                        });
                        (this.listBoxTaskStuff.ItemsSource as ObservableCollection<TaskStuff>).Remove(taskStuff);
                    }
                }
            }
        }

        private void checkBoxAudio_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                var taskStuff = checkBox.DataContext as TaskStuff;
                if (taskStuff == null)
                {
                    return;
                }
                if (checkBox.IsChecked == true)
                {
                    string displayFile = TaskStuffFileHelper.GetDisplayFile(taskStuff);
                    if (displayFile.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) || displayFile.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                    {
                        CO_IA.Client.AudioPlayer.Play(checkBox,new Uri(displayFile, UriKind.RelativeOrAbsolute), () => { checkBox.IsChecked = false; });
                    }
                }
                else
                {
                    CO_IA.Client.AudioPlayer.Stop();
                }
            }
        }

        private void buttonTaskStuffAdd_Click(object sender, RoutedEventArgs e)
        {
            var taskConclusion = this.DataContext as CO_IA.Data.TaskExecuteConclusion;
            if (taskConclusion != null)
            {
                byte[] bytes = null;
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    Stream fs = null;
                    try
                    {
                        fs = dialog.OpenFile();
                        bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                    if (bytes != null)
                    {
                        TaskStuff taskStuff = new TaskStuff();
                        taskStuff.Key = Utility.NewGuid();
                        taskStuff.IsResultStuff = true;
                        taskStuff.IsLoaded = true;
                        taskStuff.OwnerGuid = taskConclusion.Executor;
                        taskStuff.Content = bytes;
                        taskStuff.ContentLength = bytes.Length;
                        taskStuff.Name = dialog.SafeFileName;
                        taskStuff.OwnerGuid = taskConclusion.Executor;
                        taskStuff.TaskGuid = taskConclusion.TaskGuid;
                        taskStuff.SubmitUser = RiasPortal.Current.UserSetting.UserName;
                        taskStuff.SubmitTime = DateTime.Now;
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.SaveTaskStuff(taskStuff);
                        });
                        (this.listBoxTaskStuff.ItemsSource as ObservableCollection<TaskStuff>).Add(taskStuff);
                    }
                }
            }
        }

        public bool SubmitConclusion()
        {
            if (this.CheckConclusion())
            {
                var conclusion = this.DataContext as Data.TaskExecuteConclusion;
                if (conclusion != null)
                {
                    conclusion.CompleteDescription = this.textBoxCompleteDescription.Text;
                    foreach (var child in this.wrapPanelCompleteState.Children)
                    {
                        var radioButton = child as RadioButton;
                        if (radioButton.IsChecked == true)
                        {
                            conclusion.CompleteState = (TaskCompleteState)radioButton.DataContext;
                            break;
                        }
                    }
                    if (conclusion.TaskType == TaskType.Disturb)
                    {
                        foreach (var child in this.wrapPanelDisturbDisposeType.Children)
                        {
                            var radioButton = child as RadioButton;
                            if (radioButton.IsChecked == true)
                            {
                                conclusion.DisposeType = (DisturbDisposeType)radioButton.DataContext;
                                break;
                            }
                        }
                    }
                    conclusion.Submitter = RiasPortal.Current.UserSetting.UserName;
                    conclusion.SubmitTime = Utility.GetServerTime();
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.SaveTaskExecuteConclusion(conclusion);
                        });
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckConclusion()
        {
            var taskExecuteConclusion = this.DataContext as TaskExecuteConclusion;
            if (taskExecuteConclusion != null)
            {
                if (string.IsNullOrWhiteSpace(this.textBoxCompleteDescription.Text))
                {
                    MessageBox.Show("执行结果必须填写");
                    return false;
                }
                bool existChecked = false;
                foreach (var child in this.wrapPanelCompleteState.Children)
                {
                    var radioButton = child as RadioButton;
                    if (radioButton.IsChecked == true)
                    {
                        existChecked = true;
                        break;
                    }
                }
                if (!existChecked)
                {
                    MessageBox.Show("未指定完成状态");
                    return false;
                }
                if (taskExecuteConclusion.TaskType == TaskType.Disturb)
                {
                    existChecked = false;
                    foreach (var child in this.wrapPanelDisturbDisposeType.Children)
                    {
                        var radioButton = child as RadioButton;
                        if (radioButton.IsChecked == true)
                        {
                            existChecked = true;
                            break;
                        }
                    }
                    if (!existChecked)
                    {
                        MessageBox.Show("未选择干扰处理方式");
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
    }
}
