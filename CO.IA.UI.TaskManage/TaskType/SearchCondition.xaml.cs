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
using CO_IA.Data.TaskManage;

namespace CO.IA.UI.TaskManage.TaskType
{
    /// <summary>
    /// SearchCondition.xaml 的交互逻辑
    /// </summary>
    public partial class SearchCondition : Window
    {
        public event Action AfterSaveEvent;
        //任务状态
        private int _tasktate = 0;
        public int TaskState
        {
            get{return _tasktate;}
            set{_tasktate=value;}
        }
        //任务类型
        private int _tasktype = 0;
        public int TaskType
        {
            get { return _tasktype; }
            set { _tasktype = value; }
        }
        private TaskQueryList taskQueryList;
        public event Action<TaskQueryList> OntaskQueryList;

        public SearchCondition()
        {
            InitializeComponent();

            InitData();
       
        }

        private void InitData()
        {
            Dictionary<int, string> mydicform = new Dictionary<int, string>() { 
            {0,"一般任务"},
            {1,"干扰任务"}
            };
            taskType.ItemsSource = mydicform;
            taskType.SelectedValuePath = "Key";
            taskType.DisplayMemberPath = "Value";
            taskType.SelectedIndex = 0;
        }

        public SearchCondition(TaskQueryList queryList)
        {
            InitializeComponent();
            InitData();
            taskQueryList = queryList;
            this.DataContext = taskQueryList;
        }

        private void btnSearch_Click_1(object sender, RoutedEventArgs e)
        {
            if (OntaskQueryList != null)
            {
                OntaskQueryList(taskQueryList);
            }
            this.Close();
            
        }
        /// <summary>
        /// 任务状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void starting_Click_1(object sender, RoutedEventArgs e)
        {
            //if (starting.IsChecked == true)
            //{
            //    taskQueryList.TASKSTATE = 0;
            //}
            //else
            //{
            //    taskQueryList.TASKSTATE = 1;
            //}
        }

        private void GroupSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            var groupDialog = new GroupSelectName();
            groupDialog.OKButtonClick += (GroupNameList) =>
            {
                groupname.Text = GroupNameList.NAME;
                taskQueryList.GROUPID = GroupNameList.GUID;
            };
            groupDialog.Show();
        }


    }
}
