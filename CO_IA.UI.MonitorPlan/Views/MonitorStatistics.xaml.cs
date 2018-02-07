using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CO_IA.Data;
using CO_IA.Data.Monitor;
using CO_IA.Data.MonitorPlan;

namespace CO_IA.UI.MonitorPlan.Views
{
    /// <summary>
    /// MonitorStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorStatistics : UserControl
    {
        #region 变量
        DetailMonitorPlan _curretnMonitorPlan = new DetailMonitorPlan();
        List<DetailMonitorPlan> originalMontiorList = new List<DetailMonitorPlan>();
        List<DetailMonitorPlan> curentMontiorList = new List<DetailMonitorPlan>();
        List<PP_OrgInfo> list = new List<PP_OrgInfo>();
        List<PP_OrgInfo> listOrgType = new List<PP_OrgInfo>();
        string activityid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
        Sealed_PP_OrgInfo sealedorginfo = new Sealed_PP_OrgInfo();
        string activityId = "";
        public List<DetailMonitorPlan> EquipmentItemsSource
        {
            get { return _taskDataGrid.ItemsSource as List<DetailMonitorPlan>; }
            set { _taskDataGrid.ItemsSource = null; _taskDataGrid.ItemsSource = value; }
        }
        #endregion
        public MonitorStatistics(string placeid,string p_activityid)
        {
            InitializeComponent();
            activityId=p_activityid;
           // _currentPlan = PrototypeDatas.GetMonitorPlan();
            DetailMonitorPlan[] getAllDetailMonitor = PrototypeDatas.GetDetailMonitorPlan(placeid, activityid);
            if (getAllDetailMonitor!=null && getAllDetailMonitor.Count() > 0)
            {
                foreach (var personList in getAllDetailMonitor)
                {
                    DetailMonitorPlan orgMonitor = new DetailMonitorPlan();
                    orgMonitor.SENDGROUPIDS = personList.SENDGROUPIDS;
                    orgMonitor.WORKCONTENT = personList.WORKCONTENT;
                    orgMonitor.Persons = PrototypeDatas.GetOrgOfPerson(personList.SENDGROUPIDS);
                    List<string> liststr = new List<string>();
                    if (personList.IMPORTFREQUENCYRANGE.Contains(',') == true)
                    {
                        liststr = personList.IMPORTFREQUENCYRANGE.Split(',').ToList();
                    }
                    else
                    {
                        liststr.Add(personList.IMPORTFREQUENCYRANGE);
                    }
                    orgMonitor.FrequencyRange = liststr;
                    originalMontiorList.Add(orgMonitor);
                }
                var groupAllList = originalMontiorList.GroupBy(p => p.SENDGROUPIDS).ToList();//以监测组id进行分组操作
                foreach (var group in groupAllList)
                {
                    var personList = group.ToList();
                    foreach (var person in personList)
                    {
                        DetailMonitorPlan orgMonitor = new DetailMonitorPlan();
                        orgMonitor.SENDGROUPIDS = person.SENDGROUPIDS;
                        orgMonitor.WORKCONTENT = person.WORKCONTENT;
                        orgMonitor.Persons =person.Persons;
                        orgMonitor.FrequencyRange = person.FrequencyRange;
                        curentMontiorList.Add(orgMonitor);
                    }
                }
            }
            _taskDataGrid.Items.Clear();
            _taskDataGrid.ItemsSource = curentMontiorList;
            LoadedSearchCondition();
           // LoadedViewContent(_currentPlan.Tasks);
        }

        #region 方法

        /// <summary>
        /// 初始加载,监测组和监测地点
        /// </summary>
        private void LoadedSearchCondition()
        {
            //绑定监测组
            _groupCbox.ItemsSource = MonitorHelper.GetDistinctGroupByPlan(sealedorginfo);
            //绑定监测地点
            //string activityId = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            ActivityPlaceInfo[] getActivityPlace = PrototypeDatas.GetPlacesByActivityId(activityId);
            _workAddressCbox.ItemsSource = getActivityPlace.ToList();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 统计事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatisticsBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           
        }

        /// <summary>
        /// 监测地点选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _workAddressCbox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ActivityPlaceInfo place = (sender as ComboBox).SelectedItem as ActivityPlaceInfo;
            _curretnMonitorPlan.WORKPLACEID = place.Guid;
        }
        /// <summary>
        /// 监测小组选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupCbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Sealed_PP_OrgInfo group = (sender as ComboBox).SelectedItem as Sealed_PP_OrgInfo;
            _curretnMonitorPlan.SENDGROUPIDS = group.GUID;
            _curretnMonitorPlan.Persons = group.Persons;
        }
        private void stationdatagrid_LayoutUpdated(object sender, EventArgs e)
        {
            _taskDataGrid.RowHeight = double.NaN;
        }
        #endregion

        private void btn_Export_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string path = CO_IA.Client.ExcelHelper.GetPath();

            if (path != null)
            {
                object[,] source = SourceToObject(EquipmentItemsSource);
                if (source != null)
                {
                    bool exportresult = CO_IA.Client.ExcelHelper.ExportToExcel(source, path, "频率预案列表");
                    if (exportresult)
                    {
                        MessageBox.Show("导出成功!");
                    }
                    else
                    {
                        MessageBox.Show("导出失败!");
                    }
                }
                else
                {
                    MessageBox.Show("没有需要导出的数据!");
                }
            }
        }
        private object[,] SourceToObject(List<DetailMonitorPlan> equs)
        {
            object[,] obj = null;
            int rows = equs.Count;
            ObservableCollection<DataGridColumn> columns = _taskDataGrid.Columns;
            int cols = columns.Count;
            obj = new object[rows + 1, cols];

            for (int c = 1; c < cols; c++)
            {
                DataGridColumn column = columns[c];
                obj[0, c - 1] = column.Header;
            }
            for (int r = 0; r < rows; r++)
            {
                DetailMonitorPlan equ = equs[r];
                obj[r + 1, 0] = equ.SENDGROUPIDS;//工作组
                for (int i = 0; i < equ.Persons.Count;i++ )
                {

                }
                obj[r + 1, 1] = equ.Persons;
                //obj[r + 1, 2] = equ.FrequencyRange;
                obj[r + 1, 3] = equ.WORKCONTENT; 

            }

            return obj;
        }

      

    }
}
