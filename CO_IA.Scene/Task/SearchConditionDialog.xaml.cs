using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CO_IA.Data.TaskManage;

namespace CO_IA.Scene.Task
{
    /// <summary>
    /// SearchConditionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SearchConditionDialog : Window
    {
        public event Action AfterSaveEvent;
        //任务状态
        private int _tasktate = 0;
        //任务类型
        private int _tasktype = 0;
        private TaskQueryList _searchCondition = TaskSearchConditionFactory.GetTaskSearchCondition();// TaskQueryList() { TASKSTATE = 0, TASKTYPE = 0, GROUPID = SystemLoginService.UserOrgInfo.GUID, GROUPNAME = SystemLoginService .UserOrgInfo.NAME};

        public int TaskState
        {
            get { return _tasktate; }
            set { _tasktate = value; }
        }
        
        public int TaskType
        {
            get { return _tasktype; }
            set { _tasktype = value; }
        }
        
        public event Action<TaskQueryList> OntaskQueryList;


        public SearchConditionDialog(TaskQueryList p_searchCondition = null)
        {
            InitializeComponent();
            if (p_searchCondition != null)
            {
                _searchCondition = p_searchCondition;
            }
            this.DataContext = _searchCondition;
            InitData();
        }

        private void InitData()
        {
            Dictionary<int, string> mydicform = new Dictionary<int, string>() { 
            {-1,"全部"},
            {0,"一般任务"},
            {1,"干扰任务"}
            };
            taskType.ItemsSource = mydicform;
            taskType.SelectedIndex = 0;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (OntaskQueryList != null)
            {
                OntaskQueryList(_searchCondition);
            }
            this.Close();

        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            _searchCondition = TaskSearchConditionFactory.GetTaskSearchCondition();//new TaskQueryList() { TASKSTATE = 0, TASKTYPE = 0, GROUPID = SystemLoginService.UserOrgInfo.GUID, GROUPNAME = SystemLoginService .UserOrgInfo.NAME};
            this.DataContext = _searchCondition;
        }

    }
}
