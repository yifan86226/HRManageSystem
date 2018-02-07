#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案主体界面
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using I_GS_MapBase.Portal.Types;

using System.Collections.ObjectModel;


namespace CO_IA.UI.PersonSchedule.Foreign
{
    /// <summary>
    /// PersonPlanModule.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPlanDialog : AT_BC.Client.Extensions.EditableUserControl
    {
        #region 变量
        /// <summary>
        /// 设备行的高度
        /// </summary>
        private GridLength equipLength;

        /// <summary>
        /// 绑定TreeView的数据源
        /// </summary>
        List<PP_OrgInfo> itemList = new List<PP_OrgInfo>();

        ///// <summary>
        ///// 活动的主ID
        ///// </summary>
        //private string activityGUID = "";

        /// <summary>
        /// 活动的主体
        /// </summary>
        private Activity activity;

        /// <summary>
        /// 当前组织ID
        /// </summary>
        private PP_OrgInfo itemOrgInfo = new PP_OrgInfo();

        /// <summary>
        /// 暂存的组织信息
        /// </summary>
        private PP_OrgInfo itemOrgInfoForComp = new PP_OrgInfo();


        /// <summary>
        /// 每个节点获取的人员列表
        /// </summary>
        List<PP_PersonInfo> itemPersonList = new List<PP_PersonInfo>();

        /// <summary>
        /// 暂存的节点人员信息
        /// </summary>
        List<PP_PersonInfo> itemPersonListForComp = new List<PP_PersonInfo>();

        /// <summary>
        /// 职责
        /// </summary>
        List<PP_Duty> itemDutyList = new List<PP_Duty>();
        /// <summary>
        /// 当前组员列表
        /// </summary>
        List<PP_PersonInfo> itemGrouperList = new List<PP_PersonInfo>();


        /// <summary>
        /// 每个节点获取的车辆信息
        /// </summary>
        List<MonitorStationEquInfo> itemEquipList = new List<MonitorStationEquInfo>();

        /// <summary>
        /// 暂存的节点设备信息
        /// </summary>
        List<PP_EquipInfo> itemEquipListForComp = new List<PP_EquipInfo>();

        /// <summary>
        /// 每个节点获取的设备列表
        /// </summary>
        PP_VehicleInfo itemVehicle = new PP_VehicleInfo();

        /// <summary>
        /// 暂存的点车辆节信息
        /// </summary>
        PP_VehicleInfo itemVehicleForComp = new PP_VehicleInfo();


        /// <summary>
        /// 是否是默认新建的人员预案
        /// </summary>
        bool isNewPlan = false;
        #endregion


        public PersonPlanDialog(List<PP_OrgInfo> nodes)
        {
            InitializeComponent();

            List<PP_Duty> dutyList = GetDutyList();
            foreach (PP_Duty duty in dutyList)
            {
                CheckBox chk = new CheckBox();
                chk.Content = duty.Name;
                chk.Margin = new Thickness(10);
                chk.ToolTip = insertEnterString(duty.Description);
                chk.Tag = duty.Key;
                chk.DataContext = duty;
                this.wp_dutylist.Children.Add(chk);
            }

            equipLength = grid_Main.RowDefinitions[2].Height;
            grid_EquipList.Visibility = System.Windows.Visibility.Hidden;

            //新建还是读取
            if (nodes.Count == 0)
            {

            }
            else
            {
                tv_PersonPlan.ItemsSource = null;
                PP_OrgInfo tempOrgInfo = new PP_OrgInfo();

                foreach (PP_OrgInfo oinfo in nodes)
                {
                    if (string.IsNullOrEmpty(oinfo.PARENT_GUID))
                    {
                        tempOrgInfo = oinfo;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(tempOrgInfo.GUID))
                {
                    itemList = nodes;
                }
                else
                {
                    ForeachPropertyNode(tempOrgInfo, tempOrgInfo.GUID, nodes);
                    itemList.Add(tempOrgInfo);
                }
                
                this.tv_PersonPlan.ItemsSource = null;
                this.tv_PersonPlan.ItemsSource = itemList;
            }
        }
           


        //无限接循环子节点添加到根节点下面
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

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.tv_PersonPlan.Items.Count > 0)
            {
                TreeViewItem tvi_1 = tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(tv_PersonPlan.Items[0]) as TreeViewItem;
                if (tvi_1 != null)
                    tvi_1.IsSelected = true;
            }

        }

    
        /// <summary>
        /// 根据组织结构信息读取该组织详细信息
        /// </summary>
        /// <param name="selectOrgInfo"></param>
        private void SetOrgInfoDetails(PP_OrgInfo selectOrgInfo)
        {

            //清除历史节点保留信息
            ClearHistoryItemInfos();

            if (selectOrgInfo != null)
            {
                //取得当前节点的数据
                itemOrgInfo = selectOrgInfo;

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemPersonList = channel.GetPP_PersonInfos(selectOrgInfo.GUID);
                });

                if (itemPersonList.Count == 0)
                {
                    if (string.IsNullOrEmpty(selectOrgInfo.PARENT_GUID) == true)
                    {
                        PP_PersonInfo p0 = new PP_PersonInfo();
                        p0.GUID =CO_IA.Client.Utility.NewGuid();
                        p0.ORG_GUID = selectOrgInfo.GUID;
                        p0.ADD_TYPE = 1;
                        p0.PERSON_TYPE = 0;

                        itemPersonList.Add(p0);

                        PP_PersonInfo p3 = new PP_PersonInfo();
                        p3.GUID =CO_IA.Client.Utility.NewGuid();
                        p3.ORG_GUID = selectOrgInfo.GUID;
                        p3.ADD_TYPE = 1;
                        p3.PERSON_TYPE = 3;
                        itemPersonList.Add(p3);
                    }
                    else
                    {
                        PP_PersonInfo p1 = new PP_PersonInfo();

                        p1.GUID =CO_IA.Client.Utility.NewGuid();
                        p1.ORG_GUID = selectOrgInfo.GUID;
                        p1.ADD_TYPE = 1;
                        p1.PERSON_TYPE = 1;

                        itemPersonList.Add(p1);


                        PP_PersonInfo p2 = new PP_PersonInfo();
                        p2.GUID =CO_IA.Client.Utility.NewGuid();
                        p2.ORG_GUID = selectOrgInfo.GUID;
                        p2.ADD_TYPE = 1;
                        p2.PERSON_TYPE = 2;
                        itemPersonList.Add(p2);
                    }
                }
                else
                {
                    setDefaultImage();
                }

                grid_OrgLeaderHeader.DataContext = null;
                grid_OrgLeaderHeader.DataContext = selectOrgInfo;
                //根节点
                if (selectOrgInfo.PARENT_GUID == "")
                {
                    grid_OrgLeader.Visibility = System.Windows.Visibility.Visible;
                    tab_equ.Visibility = System.Windows.Visibility.Collapsed;

                    grid_EquipList.Visibility = System.Windows.Visibility.Hidden;
                    grid_GroupLeader.Visibility = System.Windows.Visibility.Hidden;

                    //便利人员列表并赋值人员类型：0.主任 1.组长；2.副组长3.协调裕安。4.组员
                    foreach (PP_PersonInfo tempPersonInfo in itemPersonList)
                    {
                        if (tempPersonInfo.PERSON_TYPE == 4)
                        {
                            itemGrouperList.Add(tempPersonInfo);
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 0)
                        {
                            grid_OrgLeader.DataContext = null;
                            grid_OrgLeader.DataContext = tempPersonInfo;
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 3)
                        {
                            grid_PSO_Coordinator.DataContext = null;
                            grid_PSO_Coordinator.DataContext = tempPersonInfo;
                        }
                    }
                }
                //其他组织节点
                else
                {
                    grid_OrgLeader.Visibility = System.Windows.Visibility.Hidden;
                    tab_equ.Visibility = System.Windows.Visibility.Visible;

                    grid_EquipList.Visibility = System.Windows.Visibility.Visible;
                    grid_GroupLeader.Visibility = System.Windows.Visibility.Visible;


                    //便利人员列表并赋值人员类型：0.主任 1.组长；2.副组长3.协调裕安。4.组员
                    foreach (PP_PersonInfo tempPersonInfo in itemPersonList)
                    {
                        if (tempPersonInfo.PERSON_TYPE == 4)
                        {
                            itemGrouperList.Add(tempPersonInfo);

                        }
                        else if (tempPersonInfo.PERSON_TYPE == 1)
                        {
                            grid_GroupLeaderDetail.DataContext = null;
                            grid_GroupLeaderDetail.DataContext = tempPersonInfo;


                            grid_GroupLeaderImage.DataContext = null;
                            grid_GroupLeaderImage.DataContext = tempPersonInfo;
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 2)
                        {
                            grid_SLeaderInfo.DataContext = null;
                            grid_SLeaderInfo.DataContext = tempPersonInfo;

                            grid_SLeaderImage.DataContext = null;
                            grid_SLeaderImage.DataContext = tempPersonInfo;
                        }
                    }

                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        itemEquipList = channel.GetPP_EqupInfos(selectOrgInfo.GUID);
                    });

                    dataGridMonitorEquipment.ItemsSource = itemEquipList;
                }

                //职责区域
                if (itemOrgInfo!=null)
                {
                    if (!string.IsNullOrEmpty(itemOrgInfo.DUTY))
                    {
                        string[] dutys = itemOrgInfo.DUTY.Split(',');
                        for (int i = 0; i < dutys.Length; i++)
                        {
                            foreach (var ele in wp_dutylist.Children)
                            {
                                CheckBox chk = ele as CheckBox;
                                if (chk != null)
                                {
                                    if (chk.Tag.ToString() == dutys[i])
                                    {
                                        chk.IsChecked = true;
                                    }
                                }
                            }
                        }
                    }
                }
                //填充人员显示列表
                FillGrouperWrapPanel(itemGrouperList);

                #region 如果所有人员都没有照片就显示列表
                try
                {
                    bool isGourperHasImage = false;

                    foreach (PP_PersonInfo pinfo in this.itemGrouperList)
                    {
                        if (pinfo.PHOTO != null && pinfo.PHOTO.Length > 1)
                        {
                            isGourperHasImage = true;
                            break;
                        }
                    }

                    if (isGourperHasImage == false)
                    {
                        cb_Grouper.IsChecked = true;
                        dg_GrouperList.Visibility = System.Windows.Visibility.Visible;

                        sv_Grouper.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        cb_Grouper.IsChecked = false;
                        dg_GrouperList.Visibility = System.Windows.Visibility.Hidden;

                        sv_Grouper.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                catch 
                { }
                #endregion

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemVehicle = channel.GetPP_VehicleInfo(selectOrgInfo.GUID);
                });

                if (itemVehicle == null || itemVehicle.GUID == null)
                {
                    itemVehicle = new PP_VehicleInfo();
                    itemVehicle.GUID =CO_IA.Client.Utility.NewGuid();
                    itemVehicle.ORG_GUID = selectOrgInfo.GUID;
                    itemVehicle.ADD_TYPE = 1;
                    itemVehicle.VEHICLE_TYPE = -1;
                }


                this.gird_VehicleInfo.DataContext = null;
                this.gird_VehicleInfo.DataContext = itemVehicle;

                #region  保存比对信息

                //保存组织信息
                itemOrgInfoForComp = CopeOrgInfo(itemOrgInfo);

                // 复制保存当前的人员信息
                itemPersonListForComp = CopePersonList(itemPersonList);

                // 复制保存当前的设备信息
                itemEquipListForComp = CopeEquipList(itemEquipList); 

                // 复制保存当前的设备信息
                itemVehicleForComp = CopeVehicleInfo(itemVehicle);

                #endregion

            }
        }
        /// <summary>
        /// 在查询结果人员数量不等于0的情况
        /// </summary>
        private void setDefaultImage()
        {
            PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
            if (orgInfo == null)
                return;
            if (string.IsNullOrEmpty(orgInfo.PARENT_GUID) == true)
            {
                if (!itemPersonList.Where(item => item.PERSON_TYPE == 0).Any())
                {
                    PP_PersonInfo p0 = new PP_PersonInfo();
                    p0.GUID = CO_IA.Client.Utility.NewGuid();
                    p0.ORG_GUID = orgInfo.GUID;
                    p0.ADD_TYPE = 1;
                    p0.PERSON_TYPE = 0;
                    grid_OrgLeader.DataContext = p0;
                    itemPersonList.Add(p0);
                }

                if (!itemPersonList.Where(item => item.PERSON_TYPE == 3).Any())
                {
                    PP_PersonInfo p3 = new PP_PersonInfo();
                    p3.GUID = CO_IA.Client.Utility.NewGuid();
                    p3.ORG_GUID = orgInfo.GUID;
                    p3.ADD_TYPE = 1;
                    p3.PERSON_TYPE = 3;
                    grid_PSO_Coordinator.DataContext = p3;
                    itemPersonList.Add(p3);
                }
            }
            else
            {
                if (!itemPersonList.Where(item => item.PERSON_TYPE == 1).Any())
                {
                    PP_PersonInfo p1 = new PP_PersonInfo();
                    p1.GUID = CO_IA.Client.Utility.NewGuid();
                    p1.ORG_GUID = orgInfo.GUID;
                    p1.ADD_TYPE = 1;
                    p1.PERSON_TYPE = 1;
                    grid_GroupLeaderImage.DataContext = p1;
                    itemPersonList.Add(p1);
                }
                if (!itemPersonList.Where(item => item.PERSON_TYPE == 2).Any())
                {
                    PP_PersonInfo p2 = new PP_PersonInfo();
                    p2.GUID = CO_IA.Client.Utility.NewGuid();
                    p2.ORG_GUID = orgInfo.GUID;
                    p2.ADD_TYPE = 1;
                    p2.PERSON_TYPE = 2;
                    grid_SLeaderImage.DataContext = p2;
                    itemPersonList.Add(p2);
                }
            }
        }
        //private void setDefaultImage()
        //{
        //    PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
        //    if (string.IsNullOrEmpty(orgInfo.PARENT_GUID) == true)
        //    {
        //        PP_PersonInfo p0 = new PP_PersonInfo();
        //        p0.GUID = CO_IA.Client.Utility.NewGuid();
        //        p0.ORG_GUID = orgInfo.GUID;
        //        p0.ADD_TYPE = 1;
        //        p0.PERSON_TYPE = 0;

        //        grid_OrgLeader.DataContext = p0;

        //        PP_PersonInfo p3 = new PP_PersonInfo();
        //        p3.GUID = CO_IA.Client.Utility.NewGuid();
        //        p3.ORG_GUID = orgInfo.GUID;
        //        p3.ADD_TYPE = 1;
        //        p3.PERSON_TYPE = 3;
        //        grid_PSO_Coordinator.DataContext = p3;
        //    }
        //    else
        //    {
        //        PP_PersonInfo p1 = new PP_PersonInfo();

        //        p1.GUID = CO_IA.Client.Utility.NewGuid();
        //        p1.ORG_GUID = orgInfo.GUID;
        //        p1.ADD_TYPE = 1;
        //        p1.PERSON_TYPE = 1;

        //        grid_GroupLeaderImage.DataContext = p1;

        //        PP_PersonInfo p2 = new PP_PersonInfo();
        //        p2.GUID = CO_IA.Client.Utility.NewGuid();
        //        p2.ORG_GUID = orgInfo.GUID;
        //        p2.ADD_TYPE = 1;
        //        p2.PERSON_TYPE = 2;
        //        grid_SLeaderImage.DataContext = p2;
        //    }
        //}
        /// <summary>
        /// 复制保存组织信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private PP_OrgInfo CopeOrgInfo(PP_OrgInfo item)
        {
            PP_OrgInfo info = new PP_OrgInfo();

            info.GUID = item.GUID;
            info.NAME = item.NAME;
            info.PARENT_GUID = item.PARENT_GUID;
            info.ACTIVITY_GUID = item.ACTIVITY_GUID;
            info.Children = item.Children;
            info.DUTY = item.DUTY;
            return info;
        }



        /// <summary>
        /// 复制保存当前的人员信息
        /// </summary>
        /// <param name="itemPersonList"></param>
        /// <returns></returns>
        private List<PP_PersonInfo> CopePersonList(List<PP_PersonInfo> list)
        {
            List<PP_PersonInfo> tempPersonList = new List<PP_PersonInfo>();

            foreach (PP_PersonInfo pinfo in list)
            {
                PP_PersonInfo npinfo = new PP_PersonInfo();

                npinfo.GUID = pinfo.GUID;
                npinfo.ISCHECKED = pinfo.ISCHECKED;
                npinfo.NAME = pinfo.NAME;
                npinfo.ORG_GUID = pinfo.ORG_GUID;
                npinfo.PERSON_TYPE = pinfo.PERSON_TYPE;
                npinfo.PHONE = pinfo.PHONE;
                npinfo.PHOTO = pinfo.PHOTO;
                npinfo.TASK = pinfo.TASK;
                npinfo.UNIT = pinfo.UNIT;
                npinfo.ADD_TYPE = pinfo.ADD_TYPE;
                npinfo.DUTY = pinfo.DUTY;
                tempPersonList.Add(npinfo);
            }


            return tempPersonList;
        }

        /// <summary>
        /// 复制保存当前的设备信息
        /// </summary>
        /// <param name="itemPersonList"></param>
        /// <returns></returns>
        private List<PP_EquipInfo> CopeEquipList(List<MonitorStationEquInfo> list)
        {
            List<PP_EquipInfo> tempEquipList = new List<PP_EquipInfo>();

            foreach (MonitorStationEquInfo pinfo in list)
            {
                PP_EquipInfo npinfo = new PP_EquipInfo();
                //npinfo.GUID = Guid.NewGuid().ToString();
                npinfo.ISCHECKED = pinfo.IsChecked;
                npinfo.NAME = pinfo.Name;
                npinfo.ORG_GUID = itemOrgInfo.GUID;
                npinfo.GUID = pinfo.ID;
                //npinfo.MODEL = pinfo.MODEL;
                //npinfo.ADD_TYPE = pinfo.ADD_TYPE;
                //npinfo.EQUIP_NUMB = pinfo.EQUIP_NUMB;
                tempEquipList.Add(npinfo);
            }


            return tempEquipList;
        }

        /// <summary>
        /// 复制保存当前的车辆信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private PP_VehicleInfo CopeVehicleInfo(PP_VehicleInfo item)
        {
            PP_VehicleInfo info = new PP_VehicleInfo();

            info.GUID = item.GUID;
            info.ORG_GUID = item.ORG_GUID;
            info.OTHER_NUMB = item.OTHER_NUMB;
            info.REMARKS = item.REMARKS;
            info.VEHICLE_MODEL = item.VEHICLE_MODEL;
            info.VEHICLE_NUMB = item.VEHICLE_NUMB;
            info.VEHICLE_TYPE = item.VEHICLE_TYPE;
            info.ADD_TYPE = item.ADD_TYPE;
            info.DRIVER = item.DRIVER;
            info.DRIVER_PHONE = item.DRIVER_PHONE;
            info.PICTURE = item.PICTURE;
            return info;
        }



        /// <summary>
        /// 填充人员显示列表
        /// </summary>
        /// <param name="grouperList"></param>
        private void FillGrouperWrapPanel(List<PP_PersonInfo> grouperList)
        {
            this.wp_GrouperListr.Children.Clear();


            this.dg_GrouperList.ItemsSource = null;
            this.dg_GrouperList.ItemsSource = grouperList;

            //填充显示人员
            foreach (PP_PersonInfo tempPersonInfo in grouperList)
            {
                PP_GrouperUC uc = new PP_GrouperUC(tempPersonInfo);
                
                uc.Tag = tempPersonInfo;
                uc.cb_PersonName.Click += cb_PersonName_Click;
                this.wp_GrouperListr.Children.Add(uc);
            }
        }

        

        void cb_PersonName_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;

            foreach (UIElement element in wp_GrouperListr.Children)
            {
                try
                {
                    if (((PP_GrouperUC)element).cb_PersonName.IsChecked == true)
                    {
                        i++;
                    }

                    PP_PersonInfo tempInfo = ((PP_GrouperUC)element).Tag as PP_PersonInfo;

                    tempInfo.ISCHECKED = (bool)((PP_GrouperUC)element).cb_PersonName.IsChecked;
                }

                catch
                { }
            }
        }

        /// <summary>
        /// 清除历史节点保留信息
        /// </summary>
        private void ClearHistoryItemInfos()
        {
            this.itemVehicle = new PP_VehicleInfo();
            this.itemEquipList.Clear();
            this.itemPersonList.Clear();
            this.wp_GrouperListr.Children.Clear();
            this.itemGrouperList.Clear();
            foreach (var ele in wp_dutylist.Children)
            {
                CheckBox chk = ele as CheckBox;
                if (chk != null)
                {
                    chk.IsChecked = false;
                }
            }
        }

        

        
        /// <summary>
        /// 移除特定的节点
        /// </summary>
        /// <param name="PARENT_GUID"></param>
        /// <param name="tarOrgInfo"></param>
        /// <param name="orgList"></param>
        private void RemovePPOrginfoFromeList(string PARENT_GUID, PP_OrgInfo tarOrgInfo, List<PP_OrgInfo> orgList)
        {
            foreach (PP_OrgInfo orginfo in orgList)
            {
                if (orginfo.GUID == PARENT_GUID)
                {
                    orginfo.Children.Remove(tarOrgInfo);
                    break;
                }
                else if (orginfo.Children.Count > 0)
                {
                    RemovePPOrginfoFromeList(PARENT_GUID, tarOrgInfo, orginfo.Children);
                }

            }
        }

        #region  人员组织结构树重命名相关
        /// <summary>
        /// 当前获取的临时TreeviewItem
        /// </summary>
        TreeViewItem item;

        /// <summary>
        /// 当前获取的临时TreeviewItem模板下的TextBox
        /// </summary>
        TextBox tempTextBox;

        /// <summary>
        /// 下面的部分是在鼠标指针位于此元素（TreeViewItem）上并且按下鼠标右键时发生。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_PersonPlan_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            //此处item定义的是一个类的成员变量，是一个TreeViewItem类型
            item = GetParentObjectEx<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (item != null)
            {

                //使当前节点获得焦点
                item.Focus();

                //系统不再处理该操作

                e.Handled = true;

            }
        }
        
       
        /// <summary>
        /// 获取当前TreeView的TreeViewItem
        /// </summary>
        /// <typeparam name="TreeViewItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public TreeViewItem GetParentObjectEx<TreeViewItem>(DependencyObject obj) where TreeViewItem : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is TreeViewItem)
                {
                    return (TreeViewItem)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        /// <summary>
        /// 获取ItemTemplate内部的各种控件
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {

                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)

                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;

                }

            }

            return null;

        }



        #endregion

      

        /// <summary>
        /// 循环保存组织信息
        /// </summary>
        /// <param name="itemCollection"></param>
        /// <returns></returns>
        private bool SavePersonPlanOrgInfos(ItemCollection itemCollection)
        {
            bool isItemSucessful = false;
            bool isChildItemSucessful = false;
            foreach (TreeViewItem tvItem in itemCollection)
            {

                PP_OrgInfo orgInfo = tvItem.DataContext as PP_OrgInfo;

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    isItemSucessful = channel.SavePP_OrgInfo(orgInfo);
                });

                if (tvItem.Items.Count > 0)
                {
                    isChildItemSucessful = SavePersonPlanOrgInfos(tvItem.Items);

                }
            }

            //本身和下属都更新成功了才算成功
            if (isItemSucessful && isChildItemSucessful)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 人员组织树进行切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tv_PersonPlan_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {                
                //取得当前的被选择节点     
                itemOrgInfo = ((System.Windows.Controls.TreeView)(sender)).SelectedItem as PP_OrgInfo;

                if (itemOrgInfo == null)
                {
                    return;
                }
                else
                {                    
                    //界面赋值
                    SetOrgInfoDetails(itemOrgInfo);
                    
                }
            }
            catch
            {
                throw;
            }
        }
      

        #region  人员相关操作方法

       

     

              

      
      
        
        /// <summary>
        /// 切换组员显示图片还是列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Grouper_Click(object sender, RoutedEventArgs e)
        {
            if (cb_Grouper.IsChecked == true)
            {
                dg_GrouperList.Visibility = System.Windows.Visibility.Visible;

                sv_Grouper.Visibility = System.Windows.Visibility.Hidden;
                

            }
            else
            {
                sv_Grouper.Visibility = System.Windows.Visibility.Visible;
                dg_GrouperList.Visibility = System.Windows.Visibility.Hidden;
            }
        }

      

        #endregion

        #region 设备操作方法
        private void dataGridMonitorEquipment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    var monitorEquipnent = dgr.DataContext as MonitorStationEquInfo;
                    if (monitorEquipnent != null)
                    {
                        //MonitorStationEquDialog dialog = new MonitorStationEquDialog(monitorEquipnent);
                        //dialog.Title = "便携式设备信息";
                        //dialog.IsShowDetail = true;
                        //dialog.ShowDialog(this);
                    }
                }
            }
        }
        
       

        
       

        #endregion

       

        #region 图片上传
        private void btn_Coordinator_Upload(object sender, MouseButtonEventArgs e)
        {
            //协调员
            UploadPersonImage(this.img_Coordinator);
        }

        private void btn_GroupLeader_Upload(object sender, MouseButtonEventArgs e)
        {
            //组长
            UploadPersonImage(this.img_GroupLeader);
        }

        private void btn_SLeader_Upload(object sender, MouseButtonEventArgs e)
        {
            //副组长
            UploadPersonImage(this.img_SLeader);
        }

        private void btn_OrgLeader_Upload(object sender, MouseButtonEventArgs e)
        {
            //办公室主任
            UploadPersonImage(this.img_OrgLeader);
        }
        
       
        /// <summary>
        /// 填充图片
        /// </summary>
        /// <param name="image"></param>
        private void UploadPersonImage(Image image)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "jpg|*.jpg|png|*.png|jpeg|*.jpeg";//过滤器
            if (ofd.ShowDialog() == true)
            {
                string fileName = ofd.FileName;//获得文件的完整路径

                byte[] imageData = File.ReadAllBytes(fileName);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData);//imageData是从数据库中读取出来的字节数组

                ms.Seek(0, System.IO.SeekOrigin.Begin);

                BitmapImage newBitmapImage = new BitmapImage();

                newBitmapImage.BeginInit();

                newBitmapImage.StreamSource = ms;

                newBitmapImage.EndInit();

                image.Source = newBitmapImage;
            }

        }

        #endregion

       
        /// <summary>
        /// 获取 职责
        /// </summary>
        /// <returns></returns>
        private List<PP_Duty> GetDutyList()
        {
            List<PP_Duty> list = null;
            //保存相关数据信息
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
            {
                //更新当前节点
                list = channel.Get_DutyList();
            });
            return list;
        }
        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string insertEnterString(string s)
        {
            string Re = s;
            int Long = 40;
            int index = 0;
            for (int i = 0; i < Re.Length; i++)
            {
                index++;
                if (index == Long)
                {
                    Re = Re.Insert(i, Environment.NewLine);
                    i++;
                    index = 0;
                }
            }
            return Re;
        }
       
        private void StackPanel_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            StackPanel sp = sender as StackPanel;
            if (sp.ContextMenu != null)
            { 
                MenuItem item1 = sp.ContextMenu.Items[0] as MenuItem;
                MenuItem item2 = sp.ContextMenu.Items[1] as MenuItem;
                if (sp.Tag == null || string.IsNullOrEmpty(sp.Tag.ToString()))
                {
                    item1.Visibility = System.Windows.Visibility.Visible;
                    item2.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    item1.Visibility = System.Windows.Visibility.Collapsed;
                    item2.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsReadOnly)
                return;
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedIndex == 0)
            {
                setVehicleUI(true);
            }
            else
            {
                setVehicleUI(false);
            }
        }
        private void setVehicleUI(bool v)
        {
            txtNum.IsReadOnly = v;
            txtModel.IsReadOnly = v;
            txtDriver.IsReadOnly = v;
            txtMemo.IsReadOnly = v;
            txtPhone.IsReadOnly = v;
            img_Vehicle.IsEnabled = !v;
            if (v == true)
            {
                itemVehicle.VEHICLE_NUMB = "";
                itemVehicle.VEHICLE_MODEL = "";
                itemVehicle.REMARKS = "";
                itemVehicle.DRIVER = "";
                itemVehicle.DRIVER_PHONE = "";
                itemVehicle.PICTURE = null;
                itemVehicle.OTHER_NUMB = "";
            }
        }

        protected override bool UpdateIsReadOnly(DependencyObject obj, bool newValue)
        {
            if (!newValue)
                return true;
            if (obj == this.cb_Grouper)
            {
                return true;
            }

            return base.UpdateIsReadOnly(obj, newValue);
        }

    }
}