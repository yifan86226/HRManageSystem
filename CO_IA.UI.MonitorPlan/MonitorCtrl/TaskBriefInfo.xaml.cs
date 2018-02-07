using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CO_IA.Data.Monitor;

namespace CO_IA.UI.MonitorPlan.MonitorCtrl
{
    /// <summary>
    /// 任务简要信息
    /// </summary>
    public partial class TaskBriefInfo : UserControl
    {
        public static readonly DependencyProperty DataSourceProperty =
          DependencyProperty.Register(
          "DataSource",
          typeof(List<MonitorTask>),
          typeof(TaskBriefInfo),
          new PropertyMetadata(new PropertyChangedCallback(DataSourcePropertyChangedCallBack)));
        int lBoxIndex = 0;
        private static void DataSourcePropertyChangedCallBack(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var antGrpCtrl = sender as TaskBriefInfo;
            var list = args.NewValue as List<MonitorTask>;
            if (antGrpCtrl != null && list != null)
            {
                antGrpCtrl.SetValue(DataSourceProperty, list);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public List<MonitorTask> DataSource
        {
            get
            {
                return (List<MonitorTask>)GetValue(DataSourceProperty);
            }
            set
            {
                SetValue(DataSourceProperty, value);
            }
        }

        public TaskBriefInfo()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Width = LayoutRoot.ActualWidth - 50;
            lBoxIndex++;
            if (lBoxIndex % 2 == 0)
            {
                grid.Background = new SolidColorBrush(new Color() { A = 255, R = 255, G = 255, B = 224 });
            }
            else
            {
                grid.Background = new SolidColorBrush(new Color() { A = 255, R = 241, G = 244, B = 248 });
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MonitorTask task = _listBoxCtrl.SelectedItem as MonitorTask;
            OpenTaskDetailDialog(task);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            string taskID = (sender as System.Windows.Documents.Hyperlink).Tag.ToString();
            List<MonitorTask> taskList = DataSource.Where(p => p.TaskID == taskID).ToList();
            if (taskList.Count > 0)
            {
                OpenTaskDetailDialog(taskList[0]);
            }
        }
        /// <summary>
        /// 打开任务详细信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTaskDetailDialog(MonitorTask p_task)
        {
            //var dialog = new TaskDetailDialog(p_task);
            //dialog.AfterOkButtonClick += Dialog_AfterOkButtonClick;
            //dialog.Show();
        }

        void Dialog_AfterOkButtonClick(MonitorTask obj)
        {
            _listBoxCtrl.ItemsSource = null;
            _listBoxCtrl.ItemsSource = DataSource;
        }

        private void CopyBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MonitorTask task = _listBoxCtrl.SelectedItem as MonitorTask;
            AddTaskItem(task);
        }
        public void AddTaskItem(MonitorTask p_task)
        {
            MonitorTask newTask = GetNewMonitorTask(p_task);
            DataSource.Add(newTask);
            _listBoxCtrl.ItemsSource = null;
            _listBoxCtrl.ItemsSource = DataSource;
            _listBoxCtrl.SelectedIndex = DataSource.Count - 1;
        }

        private void PrintBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MonitorTask task = _listBoxCtrl.SelectedItem as MonitorTask;
            //TaskDetailDialog taskDialog = new TaskDetailDialog(task);
            //taskDialog.SetToolBarVisible(false);
            //PrintDialog dialog = new PrintDialog();
            //if (dialog.ShowDialog() == true)
            //{
            //    dialog.PrintVisual(taskDialog._layoutGrid, "任务视图打印");
            //}
        }

        private void DeleteBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MonitorTask task = _listBoxCtrl.SelectedItem as MonitorTask;
            DataSource.Remove(task);
            _listBoxCtrl.ItemsSource = null;
            _listBoxCtrl.ItemsSource = DataSource;
        }
        public MonitorTask GetNewMonitorTask(MonitorTask p_oldTask)
        {
            MonitorTask newTask = new MonitorTask();
            newTask.ProtectFreqPoints.AddRange(p_oldTask.ProtectFreqPoints);
            newTask.ProtectFreqRanges.AddRange(p_oldTask.ProtectFreqRanges);
            newTask.TaskID = p_oldTask.TaskID;
            newTask.TaskType = p_oldTask.TaskType;
            newTask.TimeLength = p_oldTask.TimeLength;
            newTask.WorkAddress = p_oldTask.WorkAddress;
            newTask.WorkDate = p_oldTask.WorkDate;
            newTask.WorkDescribe = p_oldTask.WorkDescribe;
            newTask.WorkerGroup.AddRange(p_oldTask.WorkerGroup);
            return newTask;
        }
    }
}
