using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using CO_IA.UI.MAP;
using AT_BC.Types;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.MonitorTask.Task
{
    /// <summary>
    /// TaskEditControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskEditControl : AT_BC.Client.Extensions.EditableUserControl
    {
        public event Action<PP_OrgInfo> OnOrgSelected;
        private ActivityPlaceMap activityMap = new ActivityPlaceMap();
        
        public TaskEditControl()
        {
            InitializeComponent();
            TaskUrgency[] urgencies = AT_BC.Data.Helpers.EnumHelper.GetEnumValues<TaskUrgency>();
            foreach (var urgency in urgencies)
            {
                var radioButton = new RadioButton() { DataContext = urgency, Content = urgency.GetDisplayNameFromEnumDisplayNameAttribute() };
                this.wrapPanelUrgency.Children.Add(radioButton);
            }
            this.comboBoxDisturbLevel.ItemsSource = AT_BC.Data.Helpers.EnumHelper.GetEnumDictionary<DisturbLevel>();
            this.comboBoxDisturbMode.ItemsSource = AT_BC.Data.Helpers.EnumHelper.GetEnumDictionary<DisturbType>();
            this.Loaded += TaskEditControl_Loaded;
        }

        private void TaskEditControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is CO_IA.Data.Task)
            {
                var task = e.NewValue as CO_IA.Data.Task;
                //this.SetupOrgTree(task.ActivityGuid);
                this.UpdateTask(task);
                this.IsReadOnly = (task.FormState == AT_BC.Data.FormState.Check);
            }
        }

        private void UpdateTask(CO_IA.Data.Task task)
        {
            if (task == null)
            {
                return;
            }
            this.comboBoxTaskPalce.SelectedValue = task.TaskPlaceID;
            foreach (var child in this.wrapPanelUrgency.Children)
            {
                var radioButton = child as RadioButton;
                if (task.Urgency.Equals(radioButton.DataContext))
                {
                    radioButton.IsChecked = true;
                    break;
                }
            }
            //this.SetupOrgTree(task.ActivityGuid);
            //this.UpdateUIState(task);

            if (task.Executors != null && task.Executors.Length > 0)
            {
                foreach (var executor in task.Executors)
                {
                    foreach (var org in this.orgList)
                    {
                        if (org.GUID == executor.Executor)
                        {
                            org.IsChecked = true;
                            break;
                        }
                    }
                }
            }
            if (task.TaskType == TaskType.Disturb)
            {
                TaskDisturbInfo disturbInfo= task.DisturbInfo;
                this.textBoxOrg.Text = disturbInfo.DisturbedOrg;
                this.textBoxContact.Text = disturbInfo.Contact;
                this.textBoxPhone.Text = disturbInfo.Phone;
                this.textBoxDisturbedMHzFreq.Text = disturbInfo.DisturbedMHzFreq.ToString();
                this.textBoxEquipmentModel.Text = disturbInfo.EquipmentModel;
                this.comboBoxDisturbLevel.SelectedValue = disturbInfo.DisturbLevel;
                this.comboBoxDisturbMode.SelectedValue = disturbInfo.DisturbType;
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
            {
                this.listBoxTaskStuff.ItemsSource = new ObservableCollection<TaskStuff>(channel.GetTaskStuffs(task.Key, AT_BC.Data.MultipleBoolen.False));
            });
        }

        //private void UpdateUIState(CO_IA.Data.Task task)
        //{
        //    if (string.IsNullOrWhiteSpace(task.Creator))
        //    {
        //        this.textBlockCreator.Text = string.Empty;
        //        this.textBlockCreateTime.Text = string.Empty;
        //    }
        //    else
        //    {
        //        this.textBlockCreator.Text = task.Creator;
        //        this.textBlockCreateTime.Text = task.CreateTime.ToString("yyyy-MM-dd HH:mm");
        //    }
        //    if (string.IsNullOrWhiteSpace(task.Submitter))
        //    {
        //        this.textBlockSubmitter.Text = string.Empty;
        //        this.textBlockSubmitTime.Text = string.Empty;
        //    }
        //    else
        //    {
        //        this.textBlockSubmitter.Text = task.Submitter;
        //        if (task.SubmitTime.HasValue)
        //        {
        //            this.textBlockSubmitTime.Text = task.SubmitTime.Value.ToString("yyyy-MM-dd HH:mm");
        //        }
        //    }
        //}

        private void SetupOrgTree(string activityGuid)
        {
            orgList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, List<PP_OrgInfo>>(channel =>
            {
                return channel.GetPP_OrgInfos(activityGuid);
            });
            this.SetupOrgTreeView(orgList);
        }

        private List<PP_OrgInfo> orgList;

        public void SaveTask(AT_BC.Data.FormState completeState)
        {
            var task = this.DataContext as CO_IA.Data.Task;
            if (task != null)
            {
                string errorInfo;
                if (!CheckTask(task.TaskType,out errorInfo))
                {
                    throw new Exception(errorInfo);
                }
                var tempTask = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<CO_IA.Data.Task>(task);
                if (tempTask.FormState == AT_BC.Data.FormState.None)
                {
                    tempTask.Creator = CO_IA.Client.RiasPortal.Current.UserSetting.UserName;
                    tempTask.CreateTime = CO_IA.Client.Utility.GetServerTime();
                }
                if (completeState == AT_BC.Data.FormState.Check)
                {
                    tempTask.Submitter = CO_IA.Client.RiasPortal.Current.UserSetting.UserName;
                    tempTask.SubmitTime = CO_IA.Client.Utility.GetServerTime();
                }
                foreach (var child in this.wrapPanelUrgency.Children)
                {
                    var radioButton = child as RadioButton;
                    if (radioButton.IsChecked==true)
                    {
                        tempTask.Urgency = (TaskUrgency)radioButton.DataContext;
                        break;
                    }
                }
                if (this.comboBoxTaskPalce.SelectedIndex>=0)
                {
                    tempTask.TaskPlaceID = this.comboBoxTaskPalce.SelectedValue.ToString();
                }
                tempTask.FormState = completeState;
                tempTask.Executors = (from data in this.orgList where data.IsChecked select new ExecutorCompleteState { Executor = data.GUID }).ToArray();
                tempTask.Description = this.textBoxDescription.Text;
                if (tempTask.TaskType == TaskType.Disturb)
                {
                    this.SaveDisturbInfo(tempTask);
                }
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                {
                    channel.SaveTask(tempTask);
                });
                if (task.FormState == AT_BC.Data.FormState.None)
                {
                    task.CreateTime = tempTask.CreateTime;
                    task.Creator = tempTask.Creator;
                }
                if (completeState == AT_BC.Data.FormState.Check)
                {
                    task.Submitter=tempTask.Submitter;
                    task.SubmitTime=tempTask.SubmitTime;
                }
                task.FormState = tempTask.FormState;
                task.Executors = tempTask.Executors;
                task.Description = tempTask.Description;
                task.Urgency = tempTask.Urgency;
                task.TaskPlaceID = tempTask.TaskPlaceID;
                if (task.TaskType == TaskType.Disturb)
                {
                    task.DisturbInfo = tempTask.DisturbInfo;
                }
                MessageBox.Show("任务保存成功");
            }
        }

        private void SaveDisturbInfo(CO_IA.Data.Task task)
        {
            if (task.TaskType != TaskType.Disturb)
            {
                return;
            }
            var disturbInfo = task.DisturbInfo;
            disturbInfo.Contact = this.textBoxContact.Text;
            disturbInfo.DisturbedOrg = this.textBoxOrg.Text;
            disturbInfo.Phone = this.textBoxPhone.Text;
            disturbInfo.EquipmentModel = this.textBoxEquipmentModel.Text;
            double disturbedFreq;
            double.TryParse(this.textBoxDisturbedMHzFreq.Text, out disturbedFreq);
            disturbInfo.DisturbedMHzFreq = disturbedFreq;
            disturbInfo.DisturbLevel = (DisturbLevel)this.comboBoxDisturbLevel.SelectedValue;
            disturbInfo.DisturbType = (DisturbType)this.comboBoxDisturbMode.SelectedValue;
        }

        private bool CheckTask(TaskType taskType,out string errorInfo)
        {
            errorInfo = string.Empty;
            var executors = (from data in this.orgList where data.IsChecked select data.GUID).ToArray();
            if (string.IsNullOrWhiteSpace(textBoxDescription.Text))
            {
                errorInfo="任务描述不能为空!";
                return false;
            }
            if (executors.Length == 0)
            {
                errorInfo="任务必须选择执行者";
                return false;
            }
            if (taskType == TaskType.Broadcast)
            {
                return true;
            }
            if (this.comboBoxTaskPalce.SelectedIndex <0)
            {
                errorInfo="必须指定任务所属区域";
                return false;
            }
            if (taskType == TaskType.Normal)
            {
                return true;
            }
            if (string.IsNullOrWhiteSpace(this.textBoxOrg.Text))
            {
                errorInfo="必须指定干扰申报单位";
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.textBoxContact.Text))
            {
                errorInfo="必须指定干扰申报单位联系人";
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.textBoxPhone.Text))
            {
                errorInfo="必须指定干扰申报单位联系方式";
                return false;
            }
            if (string.IsNullOrWhiteSpace(this.textBoxDisturbedMHzFreq.Text))
            {
                errorInfo="必须指定被干扰频率";
                return false;
            }
            double disturbedMHzFreq;
            if (!double.TryParse(this.textBoxDisturbedMHzFreq.Text, out disturbedMHzFreq))
            {
                errorInfo = "被干扰频率必须是有效的数字";
                return false;
            }
            if (disturbedMHzFreq <= 0d)
            {
                errorInfo = "被干扰频率必须是大于0的数字";
                return false;
            }
            return true;
        }

        //无限接循环子节点添加到根节点下面
        private void ForeachPropertyNode(PP_OrgInfo node, string pid, List<PP_OrgInfo> nodes)
        {
            foreach (PP_OrgInfo tempNode in nodes)
            {
                if (tempNode.PARENT_GUID == pid)
                {
                    ForeachPropertyNode(tempNode, tempNode.GUID, nodes);
                    node.Children.Add(tempNode);
                }
            }
        }

        private void SetupOrgTreeView(List<PP_OrgInfo> nodes)
        {
            if (nodes != null && nodes.Count > 0)
            {
                this.treeViewOrg.ItemsSource = null;
                PP_OrgInfo tempOrgInfo = new PP_OrgInfo();

                foreach (PP_OrgInfo oinfo in nodes)
                {
                    if (string.IsNullOrEmpty(oinfo.PARENT_GUID))
                    {
                        tempOrgInfo = oinfo;
                        break;
                    }
                }
                ForeachPropertyNode(tempOrgInfo, tempOrgInfo.GUID, nodes);

                List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();
                itemList.Add(tempOrgInfo);
                this.treeViewOrg.ItemsSource = itemList;
            }
        }

        private void TaskEditControl_Loaded(object sender, RoutedEventArgs e)
        {
            var activityPlaces=CO_IA.Client.Utility.GetPlaces(RiasPortal.ModuleContainer.Activity.Guid);
            this.comboBoxTaskPalce.ItemsSource = activityPlaces;
            //this.comboBoxTaskPalce.ItemsSource
            if (this.DataContext != null && this.DataContext is CO_IA.Data.Task)
            {
                var task = this.DataContext as CO_IA.Data.Task;
                this.SetupOrgTree(task.ActivityGuid);
                this.UpdateTask(task);
                this.IsReadOnly = (task.FormState == AT_BC.Data.FormState.Check);
                this.DataContextChanged += this.TaskEditControl_DataContextChanged;
            }
        }

        //private void MapInitialized(bool obj)
        //{
        //    if (obj)
        //    {
        //        Dictionary<string, ActivityPlaceInfo> dic = new Dictionary<string, ActivityPlaceInfo>();
        //        var placeInfos = CO_IA.Client.Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
        //        foreach (var placeInfo in placeInfos)
        //        {
        //            dic.Add(placeInfo.Guid, placeInfo);
        //        }
        //        //dic.Add(placeInfo.Guid, placeInfo);
        //        this.activityMap.PlaceLocation = dic;
        //        this.activityMap.ShowMap.SetAllGraphicsExtent();
        //        this.UpdateTask(this.DataContext as CO_IA.Data.Task);
        //        this.DataContextChanged += TaskEditControl_DataContextChanged;
        //    }
        //}

        private void CheckBoxGroups_Click(object sender, RoutedEventArgs e)
        {

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

        private void buttonTaskStuffAdd_Click(object sender, RoutedEventArgs e)
        {
            var task = this.DataContext as CO_IA.Data.Task;
            if (task != null)
            {
                string taskGuid = task.Key;
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
                        taskStuff.IsResultStuff = false;
                        taskStuff.IsLoaded = true;
                        taskStuff.Content = bytes;
                        taskStuff.ContentLength = bytes.Length;
                        taskStuff.Name = dialog.SafeFileName;
                        taskStuff.OwnerGuid = null;
                        taskStuff.TaskGuid = task.Key;
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

        private void treeViewOrg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is PP_OrgInfo)
            {
                if (OnOrgSelected!=null)
                {
                    OnOrgSelected(e.NewValue as PP_OrgInfo);
                }
            }
        }

        private void checkBoxAudio_Checked(object sender, RoutedEventArgs e)
        {


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

        //protected override bool UpdateIsReadOnly(DependencyObject obj, bool newValue)
        //{
        //    if (obj == this.toggleButtonDisturbInfo)
        //    {
        //        return true;
        //    }
        //    return base.UpdateIsReadOnly(obj, newValue);
        //}
    }
}
