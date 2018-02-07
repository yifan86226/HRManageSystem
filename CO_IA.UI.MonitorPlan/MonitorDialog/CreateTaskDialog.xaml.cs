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
using CO_IA.Data;
using CO_IA.Data.Monitor;
using CO_IA.Data.MonitorPlan;

namespace CO_IA.UI.MonitorPlan.MonitorDialog
{
    /// <summary>
    /// CreateTaskDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CreateTaskDialog : Window
    {
        /// <summary>
        /// 活动的主体
        /// </summary>
        private string activityGuid;
        public event Action<DetailMonitorPlan> OKButtonClick;
        private DetailMonitorPlan _currentTask=new DetailMonitorPlan();
        private ActivityPlaceInfo _currentPlace = new ActivityPlaceInfo();
        List<Sealed_PP_OrgInfo> sealed_pp_org = new List<Sealed_PP_OrgInfo>();
        public CreateTaskDialog(DetailMonitorPlan p_task,ActivityPlaceInfo p_placeinfo)
        {
            InitializeComponent();
            _beginDate.SelectedDate = DateTime.Now;
            _endDate.SelectedDate = DateTime.Now;
            activityGuid = p_task.ACTIVITY_GUID;
            this._currentTask = p_task;
            this._currentPlace = p_placeinfo;
            this.DataContext = _currentTask;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string errorMsg = string.Empty;
            if (_beginDate.SelectedDate == null)
            {
                errorMsg += "任务起始日期,";
            }
            if (_endDate.SelectedDate == null)
            {
                errorMsg += "任务结束日期,";
            }
            if (string.IsNullOrEmpty(_workerTBox.Text))
            {
                errorMsg += "人员分组";
            }
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(errorMsg + "不能为空");
                return;
            }
            if (DateTime.Compare(Convert.ToDateTime(_beginDate.SelectedDate), Convert.ToDateTime(_endDate.SelectedDate)) > 0)
            {
                MessageBox.Show("任务开始时间不能大于结束时间！");
                return;
            }
            _currentTask.STARTTASKDATE =Convert.ToDateTime(_beginDate.SelectedDate);
            _currentTask.ENDTASKDATE = Convert.ToDateTime(_endDate.SelectedDate);
            _currentTask.WORKCONTENT = _workDescribeTBox.Text;
            if (OKButtonClick != null)
            {
                OKButtonClick(_currentTask);
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        /// <summary>
        /// 选择监测组信息（显示组及组内人员信息）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PersonSelectBtn_Click(object sender, RoutedEventArgs e)
        {
            //所得所有已存的人员组织结构
            List<PP_OrgInfo> list = new List<PP_OrgInfo>();
            List<PP_OrgInfo> listOrgType = new List<PP_OrgInfo>();
            List<Sealed_PP_OrgInfo> sealedList = new List<Sealed_PP_OrgInfo>();
           // activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;

            //新建还是读取
            if (string.IsNullOrEmpty(activityGuid) == true)
            {

            }
            else
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    list = channel.GetPP_OrgInfos(activityGuid);
                });
                if (list.Count() > 0)
                {
                    listOrgType = list.Where(p => 1 == 1).ToList();
                    foreach (PP_OrgInfo orginfo in listOrgType)
                    {
                        Sealed_PP_OrgInfo currentinfo = new Sealed_PP_OrgInfo();
                        currentinfo.Children = orginfo.Children;
                        currentinfo.GUID = orginfo.GUID;
                        currentinfo.ACTIVITY_GUID = activityGuid;
                       
                        currentinfo.NAME = orginfo.NAME;
                        currentinfo.PARENT_GUID = orginfo.PARENT_GUID;
                       
                        currentinfo.Persons = PrototypeDatas.GetOrgOfPerson(orginfo.GUID);
                        sealed_pp_org.Add(currentinfo);
                    }
                }

                var workerDialog = new WorkerSelectedDialog(sealed_pp_org);
                workerDialog.OKButtonClick += (personGroupList) =>
                {
                    string personDisplay = string.Empty;
                    if (personGroupList != null)
                    {
                        foreach (Sealed_PP_OrgInfo ppinfo in personGroupList)
                        {
                            personDisplay = ppinfo.NAME;//监测组名
                            if (ppinfo.Persons.Count() > 0)
                            {
                                personDisplay += "{";
                                foreach (PP_PersonInfo personInfo in ppinfo.Persons)
                                {
                                    personDisplay += personInfo.NAME + ",";
                                }
                                personDisplay = personDisplay.Substring(0, personDisplay.Length - 1);
                                personDisplay += "}";
                            }
                            _workerTBox.Text = personDisplay;
                            _workerTBox.Tag = personGroupList;
                            _currentTask.SENDGROUPIDS = ppinfo.GUID;//监测组id
                            _currentTask.SENDGROUPS = personDisplay;
                        }
                    }
                };
                workerDialog.Show();
            }
        }


        /// <summary>
        /// 选择，位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _placeSelect_Click_1(object sender, RoutedEventArgs e)
        {
            ActivityListDialog positionDialog = new ActivityListDialog(_currentPlace);
            positionDialog.LocationSelectionChanged += positionDialog_LocationSelectionChanged;
            positionDialog.ShowDialog(this);
        }

        void positionDialog_LocationSelectionChanged(ActivityPlaceLocation p_position)
        {
            _positionTBox.Text = p_position.LocationName.ToString();
            positionId.Text = p_position.GUID;
            _currentTask.POSITIONID = p_position.GUID;//选择的位置id
            _currentTask.POSITIONINFO = p_position.LocationName.ToString();
        }
    }
}
