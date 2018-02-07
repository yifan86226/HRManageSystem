using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Controls;
using CO_IA.Data.TaskManage;
using System.Collections.ObjectModel;
using CO_IA.Data;

namespace CO_IA.Scene.Task
{
    /// <summary>
    /// TaskGridCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskGridCtrl : UserControl
    {
        public ObservableCollection<TaskListInfo> GridSource
        {
            get 
            {
                return this._taskgrid.ItemsSource as ObservableCollection<TaskListInfo>; 
            }
        }

        public void SetDataSource(IEnumerable<TaskListInfo> taskListInfo)
        {
            this._taskgrid.ItemsSource = null;
            if (taskListInfo != null)
            {
                ObservableCollection<TaskListInfo> taskList = new ObservableCollection<TaskListInfo>(taskListInfo);
                this._taskgrid.ItemsSource = taskList;
                if (taskList.Count > 0)
                {
                    _taskgrid.SelectedIndex = 0;
                }
            }
        }
        
        public TaskGridCtrl()
        {
            InitializeComponent();
            _orgInfos = GetPP_OrgInfos();
            this.DataContext = this;
        }

        private void RecordPlay_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image.Tag == null) return;
            byte[] bytes = image.Tag as byte[];
            Stream stream = new MemoryStream(bytes);
            SoundPlayer player = new SoundPlayer(stream);
            player.Play();

        }
        public DataGrid TaskDataGrid { get { return _taskgrid; } }

        private static List<PP_OrgInfo> _orgInfos = new List<PP_OrgInfo>();

        public static List<PP_OrgInfo> OrgInfos
        {
            get { return TaskGridCtrl._orgInfos; }
            set 
            { 
                TaskGridCtrl._orgInfos = value; 
            }
        }

        private static List<PP_OrgInfo> GetPP_OrgInfos()
        {
            return DataOperator.GetPP_OrgInfosByActivityID(SystemLoginService.CurrentActivity.Guid);
        }
    }
}
