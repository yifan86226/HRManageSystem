using AT_BC.Common;
using CO_IA.Client;
using CO_IA.Client.Orgs;
using CO_IA.Data;
using CO_IA.Data.MonitorPlan;
using I_GS_MapBase.Portal.Types;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.Group
{
    /// <summary>
    /// GroupList.xaml 的交互逻辑
    /// </summary>
    public partial class GroupList : UserControl
    {
        /// <summary>
        /// 绑定TreeView的数据源
        /// </summary>
        List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();
        public int orgCount=0;
        public GroupList()
        {
            InitializeComponent();
        }
        public void IniData()
        {
            itemList.Clear();
            if (Obj.Activity == null || string.IsNullOrEmpty(Obj.Activity.Guid) == true)
            {
                //创建默认的组织结构组
                //CreateAndSaveDefaultOrgInfos();
            }
            else
            {
                List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    nodes = channel.GetPP_OrgInfos(Obj.Activity.Guid);
                });

                //新建还是读取
                if (nodes == null || nodes.Count == 0)
                {
                    //创建默认的组织结构组
                    //CreateAndSaveDefaultOrgInfos();
                }
                else
                {
                    orgCount = nodes.Count;
                    PP_OrgInfo tempOrgInfo = new PP_OrgInfo();

                    foreach (PP_OrgInfo oinfo in nodes)
                    {
                        if (string.IsNullOrEmpty(oinfo.PARENT_GUID))
                        {
                            tempOrgInfo = oinfo;
                            break;
                        }
                    }
                    ForeachPropertyNode(tempOrgInfo, tempOrgInfo.GUID, nodes);
                    itemList.Add(tempOrgInfo);
                    this.tv_PersonPlan.ItemsSource = null;
                    this.tv_PersonPlan.ItemsSource = itemList;

                }

                
            }  
        }
        /// <summary>
        /// 修改监测组状态
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="online"></param>
        public void  ChangeGroupState(string groupId, bool online)
        {
            if (itemList.Count > 0)
            {
                if (string.IsNullOrEmpty(groupId))
                {
                    ChangeAllGroupOfflineState(online);
                    return;
                }
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (itemList[i].GUID == groupId)
                    {
                        itemList[i].OnLine = online;
                        break;
                    }
                    if (itemList[i].Children != null && itemList[i].Children.Count > 0)
                    {
                        for (int j = 0; j < itemList[i].Children.Count; j++)
                        {
                            if (itemList[i].Children[j].GUID == groupId)
                            {
                                itemList[i].Children[j].OnLine = online;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void ChangeAllGroupOfflineState(bool online)
        {
            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    itemList[i].OnLine = online;
                       
                    
                    if (itemList[i].Children != null && itemList[i].Children.Count > 0)
                    {
                        for (int j = 0; j < itemList[i].Children.Count; j++)
                        {
                            itemList[i].Children[j].OnLine = online;
                            
                        }
                    }
                }
            }
        }
        private void ForeachPropertyNode(PP_OrgInfo node, string pid, List<PP_OrgInfo> nodes)
        {
            foreach (PP_OrgInfo tempNode in nodes)
            {
                if (tempNode.PARENT_GUID == pid)
                {
                    ForeachPropertyNode(tempNode, tempNode.GUID, nodes);
                    node.Children.Add(tempNode);
                }
            }
        }

        private void LocationItem_Click(object sender, RoutedEventArgs e)
        {
            if (tv_PersonPlan.SelectedItem != null)
            {
                PP_OrgInfo orginfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
                if (orginfo != null)
                {
                    var org = Obj.screenMap.DrawElementList.Where(item=>item.Key == MapGroupTypes.MonitorGroup_.ToString()+orginfo.GUID).ToArray();
                    if (org != null && org.Length == 1)
                    {
                        OrgToMapStyle orgPoint = org[0].Value as OrgToMapStyle;
                        if (orgPoint != null)
                        {
                            MapPointEx p = orgPoint.ElementTag as MapPointEx;
                            if (p != null)
                            {
                                Obj.screenMap.setExtent(p);
                            }
                        }
                    }
                }
            }
        }

        private void itemGroupInfo_Click(object sender, RoutedEventArgs e)
        {
            if (tv_PersonPlan.SelectedItem != null)
            {
                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
                if (orgInfo != null)
                {
                    Group.GroupDialog groupDialog = new Group.GroupDialog(new List<PP_OrgInfo>() { orgInfo });
                    groupDialog.ShowDialog(this);
                }
            }
        }

        private void itemTask_Click(object sender, RoutedEventArgs e)
        {
            if (tv_PersonPlan.SelectedItem != null)
            {
                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
                if (orgInfo != null)
                {
                    Task.TaskAllList taskList = new Task.TaskAllList(new string[] { orgInfo.GUID });
                    taskList.ShowDialog(this);
                }
            }
        }

        private void itemLIVEMonitor_Click(object sender, RoutedEventArgs e)
        {
            if (tv_PersonPlan.SelectedItem != null)
            {
                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
                if (orgInfo != null)
                {
                    ScheduleDetail[] scheduleDetails = Utility.getOrgGroupsBySchedule(Obj.Activity.Guid, Obj.Activity.ActivityStage);
                    var obj = scheduleDetails.Where(itm =>
                    {
                        if (itm.ScheduleOrgs != null && itm.ScheduleOrgs.Length > 0 && itm.ScheduleOrgs[0].OrgInfo.GUID == orgInfo.GUID)
                            return true;
                        else
                            return false;
                    }).ToArray();
                    if (obj != null && obj.Length == 1)
                    {
                        string areaid = obj[0].ScheduleOrgs[0].AREA_GUID;
                        Monitor.MonitorView rh = new Monitor.MonitorView(areaid, new List<PP_OrgInfo> { orgInfo });
                        rh.Title = "现场监测情况";
                        rh.ShowDialog();
                    }
                }
            }
        }

        private void itemTrack_Click(object sender, RoutedEventArgs e)
        {
            if (tv_PersonPlan.SelectedItem != null)
            {
                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
                if (orgInfo != null)
                {
                    //先查有没有车，或者车牌
                    PP_VehicleInfo itemVehicle = null;
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        itemVehicle = channel.GetPP_VehicleInfo(orgInfo.GUID);
                    });
                    if (itemVehicle != null && !string.IsNullOrEmpty(itemVehicle.VEHICLE_NUMB))
                    {
                        OrgToMapStyle group = new OrgToMapStyle(orgInfo);
                        Track.TrackCondition conTrack = new Track.TrackCondition(group);
                        conTrack.VehicleNum = itemVehicle.VEHICLE_NUMB;
                        conTrack.Owner = VisualTreeHelperExtension.GetParentObject<System.Windows.Window>(this);
                        conTrack.Show();
                    }
                    else
                    {
                        MessageBox.Show("没有查询到车辆信息！");
                    }
                }
            }
        }

        private void tv_PersonPlan_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LocationItem_Click(null,null);
        }
    }
}
