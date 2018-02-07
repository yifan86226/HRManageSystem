using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CO_IA.Data.TaskManage;


namespace CO_IA.Scene.Task
{
    /// <summary>
    /// 任务管理
    /// </summary>
    public partial class TemporaryTaskView : UserControl
    {
        private TaskListInfo _selectedTaskInfo;
       private TaskQueryList _searchCondition = TaskSearchConditionFactory.GetTaskSearchCondition();

        public TemporaryTaskView()
        {
            InitializeComponent();
            _taskGrid.TaskDataGrid.SelectionChanged += TaskDataGrid_SelectionChanged;
            _taskGrid.TaskDataGrid.MouseDoubleClick += TaskGrid_MouseDoubleClick;
            LoadTaskGridItems();

        }

        void TaskGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //TaskListInfo task = (sender as DataGrid).SelectedItem as TaskListInfo;
            //TaskDetailInfoDialog dialog = new TaskDetailInfoDialog(task, PageTypes.Search);
            //dialog.SaveTaskClick += CurrentTaskDialog_SaveTaskClick;
            //dialog.Show();
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            TaskListInfo taskListInfo = _taskGrid.TaskDataGrid.SelectedItem as TaskListInfo;
            if (taskListInfo.GROUPNAME != SystemLoginService.UserOrgInfo.GUID)
            {
                MessageBox.Show("没有权限删除非"+SystemLoginService.UserOrgInfo.NAME+"组创建的任务");
                return;
            }
            if (taskListInfo != null || !string.IsNullOrEmpty(taskListInfo.GUID ))
            {
                DeleteTaskInfo(taskListInfo.CHILDGUID, taskListInfo.TASKTYPE);
            }
        }
        /// <summary>
        /// 提交回执信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveReceiptMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            string errorMsg = string.Empty;
            string message = string.Empty;
            if (_selectedTaskInfo == null || string.IsNullOrEmpty(_selectedTaskInfo.GUID))
            {
                errorMsg += "请选择一条任务\r\n";
            }
            else if (string.IsNullOrEmpty(_receiptTBox.Text))
            {
                errorMsg += "请填写回执内容\r\n";
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(errorMsg);
                return;
            }
            else
            {
                _selectedTaskInfo.TASKSTATE = 1;
                _selectedTaskInfo.GROUPNAME = SystemLoginService.UserOrgInfo.GUID;

                //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, bool>
                //  (
                //  channel =>
                //  {
                //      if (channel.InsertOrUpdateReceiptMsg(_selectedTaskInfo))
                //      {
                //          LoadTaskGridItems();
                //          MessageBox.Show("保存成功");
                //          return true;
                //      }
                //      else
                //      {
                //          _selectedTaskInfo.TASKSTATE = 0;
                //          MessageBox.Show("保存失败");
                //          return false;
                //      }
                //  });
            }
        }
       ///// <summary>
       ///// 保存任务处理事件
       ///// </summary>
       ///// <param name="arg1"></param>
       ///// <param name="arg2"></param>
       ///// <param name="arg3"></param>
       // void CurrentTaskDialog_SaveTaskClick(TaskDetailInfoDialog sender,PageTypes arg1, object arg2,TaskListInfo arg3, List<RuleFile> arg4)
       // {
       //      PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, bool>
       //        (
       //        channel =>
       //        {
                  
       //            _selectedTaskInfo = arg3;
       //            bool isSaveSuccess = false;
       //            if(arg2 is TaskInfo)
       //            {
       //                TaskInfo taskInfo = arg2 as TaskInfo;
       //                if (!string.IsNullOrEmpty(taskInfo.MONITORRESULT))
       //                {
       //                    _selectedTaskInfo.TASKSTATE = 1;
       //                    taskInfo.TASKSTATE = 1;
       //                }
       //                isSaveSuccess = channel.InsertOrUpdateTaskInfo(arg1 == PageTypes.Create ? true : false, _selectedTaskInfo, taskInfo, arg4);
       //            }
       //            else  if(arg2 is DisturbTaskInfo)
       //            {
       //                DisturbTaskInfo disTask = arg2 as DisturbTaskInfo;
       //                if (!string.IsNullOrEmpty(disTask.EXPLAIN) || !string.IsNullOrEmpty(disTask.CHECKRESULT))
       //                {
       //                    _selectedTaskInfo.TASKSTATE = 1;
       //                    disTask.TASKSTATE = 1;
       //                }
       //                isSaveSuccess = channel.InsertOrUpdateDisturbTaskInfo(arg1 == PageTypes.Create ? true : false, _selectedTaskInfo, disTask, arg4);
       //            }
       //            if (isSaveSuccess)
       //            {
       //                sender.Close();
       //                LoadTaskGridItems();
       //                MessageBox.Show("保存成功");
       //            }
       //            else
       //            {
       //                MessageBox.Show("保存失败");
       //            }
       //            return isSaveSuccess;
                  
       //        });
       // }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="p_taskGuid"></param>
        /// <param name="p_taskType"></param>
        private void DeleteTaskInfo(string p_taskGuid, int p_taskType)
        {
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task>
            //    (channel =>
            //    {
            //        if (channel.DeleteFullTaskInfo(p_taskGuid, p_taskType))
            //        {
            //            LoadTaskGridItems();
            //            MessageBox.Show("删除成功");
            //        }
            //        else
            //        {
            //            MessageBox.Show("删除失败");
            //        }
            //    });
        }
       
        private void LoadTaskGridItems()
        {
            //626
            //TaskListInfo[] tasks = DataOperator.GetTaskListInfosByParam(_searchCondition);
            //_taskGrid.SetDataSource(tasks);

        }

        void TaskDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid.SelectedItem == null) return;
            
            _selectedTaskInfo = dataGrid.SelectedItem as TaskListInfo;
            _taskDescribeTBlock.Text = _selectedTaskInfo.EXPANDS;
           
            SetReceiptMessage();
            SetDeleteBar();
            
        }
        /// <summary>
        /// 设置回执信息的可读性
        /// </summary>
        private void SetReceiptMessage()
        {
            if (_selectedTaskInfo.TASKTYPE == 0)
            {
                xDisturbTaskSp.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if(_selectedTaskInfo.TASKTYPE == 1)
            {
                xDisturbTaskSp.Visibility = System.Windows.Visibility.Visible;
            }

            if (_selectedTaskInfo.TASKSTATE == 0)
            {
                
                _receiptBtn.Visibility = System.Windows.Visibility.Visible;
                _receiptTBox.IsReadOnly = false;
                xDisturbTaskSp.IsEnabled = true;
            }
            else
            {
                _receiptBtn.Visibility = System.Windows.Visibility.Collapsed;
                _receiptTBox.IsReadOnly = true;
                xDisturbTaskSp.IsEnabled = false;
            }
            _receiptTBox.Text = _selectedTaskInfo.ReceiptMsg;
            _receiptGroupBox.DataContext = _selectedTaskInfo;
        }
        /// <summary>
        /// 控制删除按钮可读性
        /// </summary>
        private void SetDeleteBar()
        {
            if (_selectedTaskInfo.GROUPNAME != SystemLoginService.UserOrgInfo.GUID)
            {
                btnDelete.IsEnabled = false;
                btnUpdate.IsEnabled = false;
            }
            else
            {
                btnDelete.IsEnabled = true;
                btnUpdate.IsEnabled = true;
                if (_selectedTaskInfo.TASKSTATE == 1)
                {//完成状态不能修改
                    btnUpdate.IsEnabled = false;
                }
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            SearchConditionDialog dialog = new SearchConditionDialog(_searchCondition);
            dialog.OntaskQueryList += (condition) =>
            {
                if (condition != null)
                {
                    _searchCondition = condition;
                    //626
                    //var result = DataOperator.GetTaskListInfosByParam(_searchCondition);
                    //this._taskGrid.SetDataSource(result);
                }
            };
            dialog.Show();
        }

      
        /// <summary>
        /// 用于定时接收
        /// </summary>
        /// <param name="taskListInfo"></param>
        public void AddTask(TaskListInfo taskListInfo)
        {
            if (taskListInfo == null)
            {
                return;
            }
            var taskList = this._taskGrid.GridSource;
            if (taskList != null)
            {
                taskList.Insert(0, taskListInfo);
            }
            this._taskGrid.SetDataSource(taskList);
        }
       
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //string guid =CO_IA.Client.Utility.NewGuid();
            //TaskDetailInfoDialog taskDialog = new TaskDetailInfoDialog(new TaskListInfo() { Guid = guid, GUID = guid, CHILDGUID =CO_IA.Client.Utility.NewGuid() }, PageTypes.Create);
            //taskDialog.SaveTaskClick += CurrentTaskDialog_SaveTaskClick;
            //taskDialog.Show();

            GeneralTaskDialog dialog = new GeneralTaskDialog();
            dialog.Show();
        }
        /// <summary>
        /// 应该记住发送任务的人或组？
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //TaskDetailInfoDialog dialog = new TaskDetailInfoDialog(_selectedTaskInfo, PageTypes.Modify);
            //dialog.SaveTaskClick += CurrentTaskDialog_SaveTaskClick;
            //dialog.Show();
        }

    }
    public class TaskSearchConditionFactory
    {
        public static TaskQueryList GetTaskSearchCondition()
        {
            return new TaskQueryList() { ACTIVITY_GUID = SystemLoginService.CurrentActivity.Guid, TASKSTATE = 0, TASKTYPE = -1, GROUPID = SystemLoginService.UserOrgInfo.GUID, GROUPNAME = SystemLoginService.UserOrgInfo.NAME };
        }
    }
}
