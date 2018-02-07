using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CO_IA.Data;
using CO_IA.Data.Monitor;
using CO_IA.Data.MonitorPlan;
using CO_IA.UI.MonitorPlan.MonitorDialog;

namespace CO_IA.UI.MonitorPlan.MonitorCtrl
{
    /// <summary>
    /// 活动列表，实际根据活动预案调用得到
    /// </summary>
    public partial class ActivityList : UserControl
    {
        #region 变量
        //所得所有已存的人员组织结构
        List<PP_OrgInfo> list = new List<PP_OrgInfo>();
        List<PP_OrgInfo> listOrgType = new List<PP_OrgInfo>();
        string activityid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
        private ActivityPlaceInfo _activityPlace = new ActivityPlaceInfo();
        public List<GroupAndLocation> getgroupList = new List<GroupAndLocation>();
        public GroupAndLocation grouplocaction = new GroupAndLocation();
        List<string> groupids = new List<string>();
        #endregion

        #region 构造函数
        public ActivityList(ActivityPlaceInfo p_activityPlace)
        {
            InitializeComponent();
            if (CO_IA.Client.RiasPortal.ModuleContainer.Activity != null)
            {
                _beginDate.Text = CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateFrom.ToShortDateString();
                _endDate.Text = CO_IA.Client.RiasPortal.ModuleContainer.Activity.DateTo.ToShortDateString();
            }
            else
            {
                _beginDate.Text = DateTime.Now.ToShortDateString();
                _endDate.Text = DateTime.Now.ToShortDateString();
            }
            //监测地点的位置信息
            _activityPlace = p_activityPlace;
            //获取对应地点的活动位置信息
            ActivityPlaceLocation[] getLocations = _activityPlace.Locations;
            if (activityid != "")
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    list = channel.GetPP_OrgInfos(activityid);
                });
                if (list.Count() > 0)
                {
                    listOrgType = list.Where(p => 1 == 1).ToList();
                    var groupList = listOrgType.GroupBy(p => p.NAME).ToList();//进行分组操作
                    foreach (var group in groupList)
                    {
                        var personList = group.ToList();
                        grouplocaction = new GroupAndLocation();
                        grouplocaction.GroupGuid = personList[0].GUID;
                        groupids.Add(personList[0].GUID);
                        grouplocaction.GroupName = personList[0].NAME;
                        grouplocaction.Locations = PrototypeDatas.GetLocations(_activityPlace.Guid);
                        grouplocaction.IsChecked = true;
                        grouplocaction.GUID = "";
                        grouplocaction.LocationName = "";
                        //grouplocaction.BeginTime =Convert.ToDateTime(_beginDate.SelectedDate);
                        //grouplocaction.EndTime = Convert.ToDateTime(_endDate.SelectedDate);
                        grouplocaction.BeginTime = Convert.ToDateTime(_beginDate.Text);
                        grouplocaction.EndTime = Convert.ToDateTime(_endDate.Text);
                        getgroupList.Add(grouplocaction);
                    }
                    grouplocaction.GroupIDs = groupids;
                    _freqPlaceLBox.ItemsSource = getgroupList;
                    _groupGrid.DataContext = this;
                }
            }
        }
        #endregion

        #region 事件
        private void PLaceCheckBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox cBox = sender as CheckBox;
            GroupAndLocation freqRange = cBox.Tag as GroupAndLocation;
        }
        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaceCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            grouplocaction = (sender as CheckBox).Tag as GroupAndLocation;
            grouplocaction.IsChecked = true;
            getgroupList.Add(grouplocaction);
        }
        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaceCheckBox_UnChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            grouplocaction = (sender as CheckBox).Tag as GroupAndLocation;
            grouplocaction.IsChecked = false;
            getgroupList.Remove(getgroupList.Where(p => p.GroupGuid == grouplocaction.GroupGuid).FirstOrDefault());

            //getgroupList.Where(p => p.GroupGuid == grouplocaction.GroupGuid).FirstOrDefault().Locations.ToList().Clear();
            //getgroupList.Where(p => p.GroupGuid == grouplocaction.GroupGuid).FirstOrDefault().Locations = PrototypeDatas.GetLocations(_activityPlace.Guid);
        }
        /// <summary>
        /// 选择，位置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _locaName_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combox = sender as ComboBox;
            if (grouplocaction.IsChecked == true)
            {
                ActivityPlaceLocation groupL = combox.SelectedItem as ActivityPlaceLocation;
                grouplocaction.GUID = groupL.GUID;//位置信息id
                grouplocaction.LocationName = groupL.LocationName;//监测位置名称
                if (getgroupList.Count > 0)
                {
                    getgroupList.Remove(getgroupList.Where(p => p.GroupGuid == grouplocaction.GroupGuid && p.IsChecked == true).FirstOrDefault());
                    getgroupList.Add(grouplocaction);
                }
            }
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        #endregion
    }

}
