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
using CO_IA.Client;
using CO_IA.Data;
using System.IO;

namespace CO_IA.UI.ActivitySummarize
{
    /// <summary>
    /// ImportTask.xaml 的交互逻辑
    /// </summary>
    public partial class ImportTask : Window
    {
        #region 变量
        /// <summary>
        /// 活动信息
        /// </summary>
        private Activity _activityInfo = new Activity();
        public event Action RefreshListEvent;
        private GuaranteeProcess gp = new GuaranteeProcess();
        #endregion
        public ImportTask(GuaranteeProcess _gp)
        {
            InitializeComponent();
            gp = _gp;
            InitPlaceList();

        }
        /// <summary>
        /// 初始化任务列表
        /// </summary>
        private void InitPlaceList()
        {

            //获取活动信息并绑定到页面
            _activityInfo = GetActivity();
            List<Task> listTask = new List<Task>();
            //Task[] tasks = 
            var existTasks = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.Task[]>(channel =>
            {
                return channel.GetTasksByActivityGuid(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            for (int i = 0; i < existTasks.Length; i++)
            {
                var taskStuff = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, List<CO_IA.Data.TaskStuff>>(channel =>
                {
                    return channel.GetTaskStuffByTaskGuid(existTasks[i].Key);
                });
                for (int j = 0; j < taskStuff.Count(); j++)
                {
                    if (IsPicture(taskStuff[j].Name))
                    {
                        listTask.Add(existTasks[i]);
                        break;
                    }
                }
            }

            Task[] tasks = listTask.ToArray();
            this.listPlace.Items.Clear();
            this.listPlace.ItemsSource = tasks;
            this.listPlace.SelectedIndex = 0;
        }
        /// <summary>
        /// 获取活动信息
        /// </summary>
        /// <returns>活动信息</returns>
        private static CO_IA.Data.Activity GetActivity()
        {
            Activity activity = new Activity();
            activity.Guid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            activity.Icon = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Icon;
            activity.Name = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Name;
            activity.ShortHand = CO_IA.Client.RiasPortal.ModuleContainer.Activity.ShortHand;
            activity.DateFrom = CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateFrom;
            activity.DateTo = CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateTo;
            activity.Organizer = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Organizer;
            activity.ActivityStage = CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityStage;
            activity.Description = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Description;
            activity.Creator = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Creator;
            activity.CreateTime = CO_IA.Client.RiasPortal.ModuleContainer.Activity.CreateTime;
            activity.ActivityType = CO_IA.Client.RiasPortal.ModuleContainer.Activity.ActivityType;
            return activity;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TaskStuffItemsSource == null)
            {
                MessageBox.Show("请选择需要导入的任务图片", "消息提示", MessageBoxButton.OK);
                return;
            }
            int checkcount = TaskStuffItemsSource.Count(r => r.IsChecked == true);
            if (checkcount == 0)
            {
                MessageBox.Show("请选择需要导入的任务图片", "消息提示", MessageBoxButton.OK);
                return;
            }
            List<GuaranteeProcess> listGp = new List<GuaranteeProcess>();
            for (int i = 0; i < TaskStuffItemsSource.Count(); i++)
            {
                if (TaskStuffItemsSource[i].IsChecked)
                {
                    GuaranteeProcess tempGp = new GuaranteeProcess();
                    //tempGp.GUID = Utility.NewGuid();
                    var taskStuff = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.TaskStuff>(channel =>
                    {
                        return channel.GetTaskStuffContent(TaskStuffItemsSource[i].Key);
                    });
                    tempGp.GUID = TaskStuffItemsSource[i].Key;
                    tempGp.PHOTO = taskStuff.Content;
                    tempGp.NAME = TaskStuffItemsSource[i].Name;
                    tempGp.ACTIVITY_GUID = gp.ACTIVITY_GUID;
                    tempGp.TYPE = gp.TYPE;
                    listGp.Add(tempGp);
                }
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(
                channel =>
                {
                    channel.SaveGuaranteeProcessList(listGp);
                    MessageBox.Show("导入成功", "提示", MessageBoxButton.OK);
                    if (RefreshListEvent != null)
                    {
                        RefreshListEvent();
                    }
                    this.Close();
                });
        }
        #region 全选
        private CheckBox chkAll;
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            if (TaskStuffItemsSource != null)
            {
                CheckBox chk = sender as CheckBox;
                bool ischecked = chk.IsChecked.Value;

                foreach (TaskStuff item in TaskStuffItemsSource)
                {
                    item.IsChecked = ischecked;
                }
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.TaskStuffItemsSource != null)
            {
                chkAll.IsChecked = TaskStuffItemsSource.Any(item => item.IsChecked);
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            if (TaskStuffItemsSource != null)
            {
                int checkcount = TaskStuffItemsSource.Count(r => r.IsChecked == true);
                if (checkcount == TaskStuffItemsSource.Length)
                {
                    chkAll.IsChecked = true;
                }
                else if (checkcount == 0)
                {
                    chkAll.IsChecked = false;
                }
                else
                {
                    chkAll.IsChecked = null;
                }
            }
        }

        public TaskStuff[] TaskStuffItemsSource
        {
            get { return (TaskStuff[])GetValue(TaskStuffItemsSourceProperty); }
            set { SetValue(TaskStuffItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty TaskStuffItemsSourceProperty =
    DependencyProperty.Register("TaskStuffItemsSource", typeof(TaskStuff[]), typeof(ImportTask), new PropertyMetadata(null, null));

        private Task[] taskDataContext
        {
            get;
            set;
        }
        #endregion

        #region 事件
        private void listPlace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string guid = ((Task)listPlace.SelectedItem).Key;
            var taskStuff = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, List<CO_IA.Data.TaskStuff>>(channel =>
            {
                return channel.GetTaskStuffByTaskGuid(guid);
            });
            //获取该活动已经添加的保障图片
             List<GuaranteeProcess> guarateeList = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize, List<CO_IA.Data.GuaranteeProcess>>(
                channel =>
                {
                    return channel.GetGuaranteeProcessList(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, "");
                });
            
            List<CO_IA.Data.TaskStuff> temp = new List<TaskStuff>();
            for (int i = 0; i < taskStuff.Count(); i++)
            {
                if (IsPicture(taskStuff[i].Name) && guarateeList.Where(o => o.GUID == taskStuff[i].Key).Count() == 0)
                {
                    string id = taskStuff[i].Key;
                    var taskStuffContent = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.TaskStuff>(channel =>
                    {
                        return channel.GetTaskStuffContent(id);
                    });
                    if (taskStuffContent != null)
                    {
                        taskStuff[i].Content = taskStuffContent.Content;
                    }
                    temp.Add(taskStuff[i]);
                }
            }
            this.dataGridTask.ItemsSource = temp;
            //TaskStuff[] 
            TaskStuffItemsSource = temp.ToArray();
        }
        #endregion
        
        ///// <summary>
        ///// 图片预览
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnPreview_Click(object sender, RoutedEventArgs e)
        //{
        //    string id = ((Button)sender).Uid.ToString();
        //    var taskStuff = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.TaskStuff>(channel =>
        //    {
        //        return channel.GetTaskStuffContent(id);
        //    });
        //    MemoryStream stream = new MemoryStream(taskStuff.Content);
        //    BitmapImage bmp = new BitmapImage();
        //    bmp.BeginInit();//初始化
        //    bmp.StreamSource = stream;//设置源
        //    bmp.EndInit();//初始化结束
        //    this.taskImg.Source = bmp;//设置图像Source
        //}

        /// <summary>
        /// 判断文件是否是图片
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns> 
        public bool IsPicture(string fileName)
        {
            string strFilter = ".jpeg|.gif|.jpg|.png|.bmp|.pic|.tiff|.ico|.iff|.lbm|.mag|.mac|.mpt|.opt|";
            char[] separtor = { '|' };
            string[] tempFileds = strFilter.Split(separtor);
            foreach (string str in tempFileds)
            {
                if (str.ToUpper() == fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")).ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
