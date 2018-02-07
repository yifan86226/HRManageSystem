using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CO_IA.Data.TaskManage;
using Microsoft.Win32;

namespace CO_IA.Scene.Task
{
    /// <summary>
    /// TaskDetailInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class TaskDetailInfoDialog : Window
    {
        private TaskListInfo _currentTask = null;
        private PageTypes _pageType = PageTypes.Search;
        private List<RuleFile> _currentRulesFiiles = new List<RuleFile>();
        /// <summary>
        /// 参数0：当前页面 参数1：操作类型-新增/修改/删除 参数2：任务信息 taskinfo 或distrubtaskinfo 参数3：附件列表
        /// </summary>
        public event Action<TaskDetailInfoDialog,PageTypes, object,TaskListInfo, List<RuleFile>> SaveTaskClick;
        public TaskListInfo CurrentTask
        {
            get { return _currentTask; }
            set { _currentTask = value; }
        }
        public TaskDetailInfoDialog(TaskListInfo taskListInfo,PageTypes p_pageType)
        {
            InitializeComponent();
            this._currentTask = taskListInfo;
            this._pageType = p_pageType;
            SetToolBarVisible(p_pageType);
            LoadStyleTest();
            InitData();
            LoadViewDisplay();
            
        }

        private void LoadStyleTest()
        {
            Style txtStyle = new System.Windows.Style() { TargetType = typeof(TextBox) };
            txtStyle.Setters.Add(new Setter() { Property = TextBox.IsReadOnlyProperty, Value = _pageType == PageTypes.Search?true:false});
            txtStyle.Setters.Add(new EventSetter(TextBox.TextChangedEvent, new TextChangedEventHandler(TextBox_TextChanged)));

            LayOutGrid.Resources.Add(typeof(TextBox), txtStyle);
            Style rBtnStyle = new System.Windows.Style() { TargetType = typeof(RadioButton) };
            rBtnStyle.Setters.Add(new Setter() { Property = RadioButton.IsEnabledProperty, Value = _pageType== PageTypes.Search?false:true });
            LayOutGrid.Resources.Add(typeof(RadioButton), rBtnStyle);

        }

        protected void InitData()
        {
            Dictionary<int, string> mydicform = new Dictionary<int, string>() { 
            {0,"语音"},
            {1,"数据"},
            {2,"噪音"},
            {3,"其他"}
            };
            cbdisturb.ItemsSource = mydicform;
            cbdisturb.SelectedValuePath = "Key";
            cbdisturb.DisplayMemberPath = "Value";

            Dictionary<int, string> mydicLevel = new Dictionary<int, string>() { 
            {0,"一般"},
            {1,"轻微"},
            {2,"严重"}
            };
            cbLevel.ItemsSource = mydicLevel;
            cbLevel.SelectedValuePath = "Key";
            cbLevel.DisplayMemberPath = "Value";
            DisturbM.ItemsSource = new string[] { "MHz", "GHz", "kHz" };
            _sendToGroup.Text = SystemLoginService.UserOrgInfo.NAME;

            if (_pageType == PageTypes.Create)
            {
                _genericTask.IsEnabled = true;
                _disturbTask.IsEnabled = true;
            }
            else
            {
                _genericTask.IsEnabled = false;
                _disturbTask.IsEnabled = false;
            }
        }

        private void LoadViewDisplay()
        {
            if (_currentTask.TASKTYPE == 0)
            {
                _genericTask.IsChecked = true;
                taskType.Visibility = Visibility.Visible;
                distrubTask.Visibility = Visibility.Collapsed;

            }
            else if (_currentTask.TASKTYPE == 1)
            {
                _disturbTask.IsChecked = true;
                distrubTask.Visibility = Visibility.Visible;
                taskType.Visibility = Visibility.Collapsed;
            }
            //RuleFile[] rulefiles = GetRulesFiilesInfo(_currentTask.CHILDGUID);
            //List<RuleFile> filesource = rulefiles.Where(r => r.MAINGUID == _currentTask.CHILDGUID).ToList();
            //_currentRulesFiiles = filesource;
            //AddHyperlinkFileTiltle(filesource);
        }
        void AddHyperlinkFileTiltle(List<RuleFile> p_filesource)
        {
            if (_currentTask.TASKTYPE == 0)
            {
                _generalFileSp.Children.Clear();
            }
            else
            {
                _disturbeFileSp.Children.Clear();
            }
            foreach (RuleFile fiile in p_filesource)
            {
                Image image = new Image() { Height=16,Width=16,VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Source = new BitmapImage(new Uri("/CO_IA.Scene;component/Images/delete.png", UriKind.RelativeOrAbsolute)) };
                image.Tag = fiile;
                image.MouseLeftButtonUp += DeleteAttach_MouseLeftButtonUp;
                StackPanel sp = new StackPanel() { Orientation= Orientation.Horizontal};
                TextBlock textBlock = new TextBlock();
                Hyperlink link = new Hyperlink() { DataContext = fiile };
                link.Click += link_Click;
                link.Inlines.Add(new Run() { Text = fiile.FILENAME });
                if (_currentTask.TASKTYPE == 0)
                {
                    textBlock.Inlines.Add(link);
                    sp.Children.Add(textBlock);
                    if (_pageType != PageTypes.Search)
                    sp.Children.Add(image);
                    _generalFileSp.Children.Add(sp);
                }
                else
                {
                    textBlock.Inlines.Add(link);
                    sp.Children.Add(textBlock);
                    if (_pageType != PageTypes.Search)
                    sp.Children.Add(image);
                    _disturbeFileSp.Children.Add(sp);
                }
            }
        }

        void DeleteAttach_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            RuleFile rulesFiile = image.Tag as RuleFile;
            _currentRulesFiiles.Remove(rulesFiile);
            AddHyperlinkFileTiltle(_currentRulesFiiles);
        }

        TaskInfo CreateTaskInfoInstance()
        {
           return new TaskInfo() { ActivityGuid = SystemLoginService.CurrentActivity.Guid, 
               GROUPID = SystemLoginService.UserOrgInfo.GUID, 
              GROUPNAME = SystemLoginService.UserOrgInfo.NAME,
               GUID = _currentTask.CHILDGUID,
                                   ResultSenderID = SystemLoginService.UserOrgInfo.GUID,
                                   ResultSendDate = DateTime.Now
           };
        }
        DisturbTaskInfo CreateDisturbTaskInfoInstance()
        {
           return new DisturbTaskInfo() { FREQENCYUNIT = "GHz", ActivityGuid = SystemLoginService.CurrentActivity.Guid,
               GROUPID = SystemLoginService.UserOrgInfo.GUID,
               GROUPNAME = SystemLoginService.UserOrgInfo.NAME,
               Guid = _currentTask.CHILDGUID,
               GUID = _currentTask.CHILDGUID, 
               ResultSenderID = SystemLoginService.UserOrgInfo.GUID,
               ResultSendDate = DateTime.Now};
        }
        void link_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;
            RuleFile file = link.DataContext as RuleFile;
            if (file.FILEPATH == null || file.FILEPATH.Length == 0)
            {
                MessageBox.Show("没找到附件内容");
                return;
            }
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog .FileName = file.FILENAME;
            dialog.DefaultExt = file.FILETYPE;
            System.Windows.Forms.DialogResult dialogResult = dialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK || dialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                byte[] result = file.FILEPATH;
                try
                {
                    using (Stream stream = dialog.OpenFile())
                    {
                        stream.Write(result, 0, result.Length);
                        stream.Close();
                        MessageBox.Show("导出成功");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }


        /// <summary>
        /// 查询，一般任务
        /// </summary>
        /// <returns></returns>
        private CO_IA.Data.TaskManage.TaskInfo[] GetTaskInfo(string p_taskGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.TaskInfo[]>(
                 channel =>
                 {
                     TaskInfo[] taskInfoArray = channel.GetTaskInfo(p_taskGuid);
                     taskInfoArray.ToList().ForEach(p => p.ActivityGuid = _currentTask.ACTIVITY_GUID );
                     taskInfoArray.ToList().ForEach(p => p.TASKSTATE = _currentTask.TASKSTATE);
                     return taskInfoArray;
                 });
        }
        /// <summary>
        /// 查询，干扰任务
        /// </summary>
        /// <returns></returns>
        private CO_IA.Data.TaskManage.DisturbTaskInfo[] GetDisturbTaskInfo(string p_taskGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.DisturbTaskInfo[]>(
                 channel =>
                 {
                     DisturbTaskInfo[] taskInfoArray = channel.GetDisturbTaskInfo(p_taskGuid);
                     taskInfoArray.ToList().ForEach(p => p.ActivityGuid = _currentTask.ACTIVITY_GUID);
                     taskInfoArray.ToList().ForEach(p => p.TASKSTATE = _currentTask.TASKSTATE);
                     return taskInfoArray;
                 });
        }
        /// <summary>
        /// 获取附件信息
        /// </summary>
        /// <returns></returns>
        private CO_IA.Data.TaskManage.RuleFile[] GetRulesFiilesInfo(string p_taskGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.RuleFile[]>(
                 channel =>
                 {
                     return channel.GetRulesFile(p_taskGuid);
                 });
        }

        private void SetToolBarVisible(PageTypes p_pageTypes)
        {
            //文件上传 干扰任务
            xDisturbAttachBtn.Visibility = p_pageTypes == PageTypes.Modify ? Visibility.Visible : Visibility.Collapsed;
            //删除附件 干扰任务
            //_delAttachBtn.Visibility = p_pageTypes == PageTypes.Modify ? Visibility.Visible : Visibility.Collapsed;
            //文件上传 一般任务
            xGeneralAttachBtn.Visibility = p_pageTypes == PageTypes.Modify ? Visibility.Visible : Visibility.Collapsed;
            //删除附件 一般任务
            //deletesamefile.Visibility = p_pageTypes == PageTypes.Modify ? Visibility.Visible : Visibility.Collapsed;

            if (p_pageTypes == PageTypes.Modify || p_pageTypes == PageTypes.Create)
            {
                xDisturbAttachBtn.Visibility = Visibility.Visible;
                //_delAttachBtn.Visibility = Visibility.Visible;
                xGeneralAttachBtn.Visibility = Visibility.Visible;
                //deletesamefile.Visibility = Visibility.Visible;
                _saveSp.Visibility = Visibility.Visible;
            }
            else
            {
                xDisturbAttachBtn.Visibility = Visibility.Collapsed;
                //_delAttachBtn.Visibility = Visibility.Collapsed;
                xGeneralAttachBtn.Visibility = Visibility.Collapsed;
                //deletesamefile.Visibility = Visibility.Collapsed;
                _saveSp.Visibility = Visibility.Collapsed;
            }
           
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null) return;

            string errorStr = VerifyTaskInfo();
            if (!string.IsNullOrEmpty(errorStr))
            {
                MessageBox.Show(errorStr);
                return;
            }
            LoadTaskListInfo();
            if (SaveTaskClick != null)
            {
                SaveTaskClick(this,_pageType, this.DataContext,_currentTask, _currentRulesFiiles);
            }
        }

        private void LoadTaskListInfo()
        {
            if (this.DataContext is TaskInfo)
            {
                TaskInfo taskinfo = this.DataContext as TaskInfo;
                
                _currentTask.ACTIVITY_GUID = taskinfo.ActivityGuid;
                _currentTask.EXPANDS = taskinfo.GENERICDESCRIBE;
                _currentTask.GROUPID = taskinfo.GROUPID;
                _currentTask.GROUPNAME = taskinfo.ResultSenderID;
                _currentTask.ReceiptMsg = taskinfo.MONITORRESULT;
                _currentTask.TASKNAME = taskinfo.GENERICNAME;
                _currentTask.TASKSTATE = taskinfo.TASKSTATE;
                _currentTask.TASKTYPE = 0;
                _currentTask.URGENCY = taskinfo.URGENCY;
            }
            else if (this.DataContext is DisturbTaskInfo)
            {
                DisturbTaskInfo taskinfo = this.DataContext as DisturbTaskInfo;
                _currentTask.ACTIVITY_GUID = taskinfo.ActivityGuid;
                if (!string.IsNullOrEmpty(taskinfo.CHECKRESULT))
                {
                    _currentTask.CheckResult = int.Parse(taskinfo.CHECKRESULT);
                }
                _currentTask.EXPANDS = taskinfo.DISTRUBDESCRIBE;
                _currentTask.GROUPID = taskinfo.GROUPID;
                _currentTask.GROUPNAME = taskinfo.ResultSenderID;
                _currentTask.ReceiptMsg = taskinfo.EXPLAIN;
                _currentTask.TASKNAME = taskinfo.DISTRUBNAME;
                _currentTask.TASKSTATE = taskinfo.TASKSTATE;
                _currentTask.TASKTYPE = 1;
                _currentTask.URGENCY = -1;//干扰任务没有紧急程度
            }
        }

        string VerifyTaskInfo()
        {
            string errorMsg = "";
            if (this.DataContext is TaskInfo)
            {
                TaskInfo task = this.DataContext as TaskInfo;
                if (string.IsNullOrEmpty(task.GENERICNAME))
                {
                    errorMsg += "任务标题不能为空";
                }
            }
            else if (this.DataContext is DisturbTaskInfo)
            {
                DisturbTaskInfo task = this.DataContext as DisturbTaskInfo;
                if (string.IsNullOrEmpty(task.DISTRUBNAME) )
                {
                    errorMsg += "任务标题不能为空\r\n";
                }
                if (string.IsNullOrEmpty(task.DISTRUBDESCRIBE))
                {
                    errorMsg += "任务描述不能为空";
                }
            }
            return errorMsg;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DisturbAttachBtn_Click(object sender, RoutedEventArgs e)
        {
            UploadAttchFile("IntertaskFile");
        }
        private void GeneralAttachBtn_Click(object sender, RoutedEventArgs e)
        {
            UploadAttchFile("SameAsTaskFile");
        }
        void UploadAttchFile(string p_filetype)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;//检查文件是否存在
            dlg.Multiselect = false;//是否允许多选，false表示单选
            dlg.CheckPathExists = true;
            dlg.Filter = "All files (*.*)|*.*";//文件过滤器
            if (dlg.ShowDialog() != true)
            {
                return;
            }
            Stream fileStream = dlg.OpenFile();
            if (fileStream == null)
            {
                return;
            }
            if (fileStream.Length / 1024 / 1024 > 20)
            {
                MessageBox.Show("上传附件不支持超过20M大小的文件。");
                return;
            }
            byte[] arrFile = new byte[fileStream.Length];
            fileStream.Read(arrFile, 0, arrFile.Length);
            RuleFile rulesFiile = new RuleFile(); 
            rulesFiile.GUID =CO_IA.Client.Utility.NewGuid();
            rulesFiile.MAINGUID = _currentTask.CHILDGUID;
            rulesFiile.FILEPATH = arrFile;
            rulesFiile.FILEFORM = dlg.FileName.ToString().Substring(dlg.FileName.ToString().LastIndexOf('.') + 1);
            rulesFiile.FILENAME = dlg.SafeFileName;
            rulesFiile.FILESIZE = fileStream.Length.ToString();
            rulesFiile.FILETYPE = p_filetype; //SameAsTaskFile 一般任务附件 IntertaskFile 干扰任务附件
            if (IsContainsFile(rulesFiile))
            {
                MessageBox.Show("该附件已存在，请重新选择！");
                return;
            }
            _currentRulesFiiles.Add(rulesFiile);
            AddHyperlinkFileTiltle(_currentRulesFiiles);
        }

        bool IsContainsFile(RuleFile p_rulesFiile)
        {
           var list = _currentRulesFiiles.Where(p => p.FILESIZE == p_rulesFiile.FILESIZE && p.FILENAME == p_rulesFiile.FILENAME && p.FILEFORM == p_rulesFiile.FILEFORM).ToList();
           return list.Count > 0 ? true : false;
        }
       
        private void TaskTypeButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Content != null && radioButton.Content.ToString() == "一般任务")
            {
                taskType.Visibility = System.Windows.Visibility.Visible;
                distrubTask.Visibility = System.Windows.Visibility.Collapsed;
                
                //TaskInfo[] array = GetTaskInfo(_currentTask.CHILDGUID);
                //if (array.Length > 0)
                //{
                //    this.DataContext = array[0];
                //}
                //else
                //{
                //    this.DataContext = CreateTaskInfoInstance();
                //}
                _currentTask.TASKTYPE = 0;
            }
            else if (radioButton.Content != null && radioButton.Content.ToString() == "干扰任务")
            {
                distrubTask.Visibility = System.Windows.Visibility.Visible;
                taskType.Visibility = System.Windows.Visibility.Collapsed;
                DisturbTaskInfo[] array = GetDisturbTaskInfo(_currentTask.CHILDGUID);
                if (array.Length > 0)
                {
                    this.DataContext = array[0];
                }
                else
                {
                    this.DataContext = CreateDisturbTaskInfoInstance();
                }
                _currentTask.TASKTYPE = 1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tbox = sender as TextBox;
            if (tbox.Tag == null) return;
            string regStr = tbox.Tag.ToString();
            Regex reg = new Regex(regStr);
            if (!reg.IsMatch(tbox.Text))
            {
                tbox.Background = new SolidColorBrush(Colors.Red);
                tbox.Focus();
            }
            else
            {
                tbox.Background = new SolidColorBrush(Colors.White);
            }
        }

    }

    public enum PageTypes
    { 
        /// <summary>
        /// 修改是，显示附件下载删除等。
        /// </summary>
        Modify,
        /// <summary>
        /// 创建时，不提供提交回执
        /// </summary>
        Create,
        /// <summary>
        /// 查询时，不显示附件下载删除等
        /// </summary>
        Search
    }
}
