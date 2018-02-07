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

using System.Data;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// PersonPlanModule.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPlanModule : AT_BC.Client.Extensions.EditableUserControl
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
        /// 是否是默认新建的人员预案
        /// </summary>
        bool isNewPlan = false;
        #endregion

        private bool isRename;
        public PersonPlanModule()
        {
            InitializeComponent();
            
            
            //PersonPlanInitializeDialog dg = new PersonPlanInitializeDialog();
            //dg.ShowDialog();

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


            activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;

            //新建还是读取
            if (activity == null || string.IsNullOrEmpty(activity.Guid) == true)
            {
                //创建默认的组织结构组
                CreateAndSaveDefaultOrgInfos();
            }
            else
            {
                //所得所有已存的人员组织结构
                List<PP_OrgInfo> nodes = new List<PP_OrgInfo>();



                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    nodes = channel.GetPP_OrgInfos(activity.Guid);
                });


                //新建还是读取
                if (nodes.Count == 0)
                {
                    //创建默认的组织结构组
                    CreateAndSaveDefaultOrgInfos();
                }
                else
                {
                    tv_PersonPlan.ItemsSource = null;
                    //tv_PersonPlan.Items.Clear();//加载根节点前先清除Treeview控件项
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

            //if (this.tv_PersonPlan.Items.Count > 0)
            //{
            //    itemOrgInfo = this.tv_PersonPlan.Items[0] as PP_OrgInfo;

            //    //界面赋值
            //    SetOrgInfoDetails(itemOrgInfo);
            //}
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
            if (this.IsReadOnly)
            {
                img_OrgLeader.ContextMenu = null;
                img_Coordinator.ContextMenu = null;
                img_GroupLeader.ContextMenu = null;
                img_SLeader.ContextMenu = null;
            }
            if (this.tv_PersonPlan.Items.Count > 0)
            {
                TreeViewItem tvi_1 = tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(tv_PersonPlan.Items[0]) as TreeViewItem;
                if (tvi_1 != null)
                    tvi_1.IsSelected = true;
            }

        }

        /// <summary>
        /// 创建默认的组织结构组
        /// </summary>
        private void CreateAndSaveDefaultOrgInfos()
        {
            //新的人员预案
            isNewPlan = true;

            //重大活动安全保障办公室
            string rootGUID =CO_IA.Client.Utility.NewGuid();
            
            //频率台站第一小组
            string pinlvSubGUID1 =CO_IA.Client.Utility.NewGuid();
            //频率台站第二小组
            string pinlvSubGUID2 =CO_IA.Client.Utility.NewGuid();
            //监测组第一小组
            string jianceSubGUID1 =CO_IA.Client.Utility.NewGuid();
            //监测组第二小组
            string jianceSubGUID2 =CO_IA.Client.Utility.NewGuid();

            List<PP_OrgInfo> nodes = new List<PP_OrgInfo>()
            {
                new PP_OrgInfo { GUID = rootGUID,  ACTIVITY_GUID=activity.Guid, NAME = "重大活动安全保障办公室" ,DUTY="01"},  
                new PP_OrgInfo { GUID = jianceSubGUID1, ACTIVITY_GUID=activity.Guid, NAME = "监测第一小组", PARENT_GUID = rootGUID ,DUTY="05"},
                new PP_OrgInfo { GUID = jianceSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "监测第二小组", PARENT_GUID = rootGUID ,DUTY="05"},
                new PP_OrgInfo { GUID = pinlvSubGUID1,ACTIVITY_GUID=activity.Guid,  NAME = "频率第一小组", PARENT_GUID = rootGUID,DUTY="04"},
                new PP_OrgInfo { GUID = pinlvSubGUID2,ACTIVITY_GUID=activity.Guid,  NAME = "频率第二小组", PARENT_GUID = rootGUID,DUTY="04"},
                
            };

            bool isSuccesful = false;
            //保存相关数据信息
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                isSuccesful = channel.SavePP_OrgInfoList(nodes);
            });

            if (isSuccesful == false)
            {
                MessageBox.Show("系统默认创建和保存活动组织结构失败。");
            }


            tv_PersonPlan.Items.Clear();//加载根节点前先清除Treeview控件项

            //PP_OrgInfo node = new PP_OrgInfo { GUID = rootGUID, ACTIVITY_GUID = activity.Guid, NAME = "重大活动安全保障办公室" };

            ForeachPropertyNode(nodes[0], nodes[0].GUID, nodes);
            itemList.Add(nodes[0]);

            this.tv_PersonPlan.ItemsSource = null;
            this.tv_PersonPlan.ItemsSource = itemList;
        }


        /// <summary>
        /// 根据组织结构信息读取该组织详细信息
        /// </summary>
        /// <param name="selectOrgInfo"></param>
        private void SetOrgInfoDetails(PP_OrgInfo selectOrgInfo)
        {
            tabControl1.SelectedIndex = 0;
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
                        p0.GUID = CO_IA.Client.Utility.NewGuid();
                        p0.ORG_GUID = selectOrgInfo.GUID;
                        p0.ADD_TYPE = 1;
                        p0.PERSON_TYPE = 0;

                        itemPersonList.Add(p0);

                        PP_PersonInfo p3 = new PP_PersonInfo();
                        p3.GUID = CO_IA.Client.Utility.NewGuid();
                        p3.ORG_GUID = selectOrgInfo.GUID;
                        p3.ADD_TYPE = 1;
                        p3.PERSON_TYPE = 3;
                        itemPersonList.Add(p3);
                    }
                    else
                    {
                        PP_PersonInfo p1 = new PP_PersonInfo();

                        p1.GUID = CO_IA.Client.Utility.NewGuid();
                        p1.ORG_GUID = selectOrgInfo.GUID;
                        p1.ADD_TYPE = 1;
                        p1.PERSON_TYPE = 1;

                        itemPersonList.Add(p1);


                        PP_PersonInfo p2 = new PP_PersonInfo();
                        p2.GUID = CO_IA.Client.Utility.NewGuid();
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

              

                #region  保存比对信息

                //保存组织信息
                itemOrgInfoForComp = CopeOrgInfo(itemOrgInfo);

                // 复制保存当前的人员信息
                itemPersonListForComp = CopePersonList(itemPersonList);

           

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
                npinfo.VOICEID = pinfo.VOICEID;
                tempPersonList.Add(npinfo);
            }


            return tempPersonList;
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
                uc.MouseRightButtonUp += uc_MouseRightButtonUp;
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
        /// 添加新的组织
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            { }
            else
            {
                //有节点选中,添加当前选中节点的子节点

                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;

                if (orgInfo != null)
                {

                    PP_OrgInfo tempOrgInfo = new PP_OrgInfo();
                    tempOrgInfo.GUID =CO_IA.Client.Utility.NewGuid();
                    tempOrgInfo.ACTIVITY_GUID = orgInfo.ACTIVITY_GUID;
                    tempOrgInfo.PARENT_GUID = orgInfo.GUID;
                    tempOrgInfo.NAME = "待重命名";

                    orgInfo.Children.Add(tempOrgInfo);

                    //保存到数据库
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        channel.SavePP_OrgInfo(tempOrgInfo);
                    });

                    this.tv_PersonPlan.ItemsSource = null;
                    this.tv_PersonPlan.ItemsSource = itemList;

                    //界面赋值
                    SetOrgInfoDetails(tempOrgInfo);
                }
            }
        }


        /// <summary>
        /// 移除相关节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            { }
            else
            {//有节点选中,添加当前选中节点的子节点

                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;

                if (orgInfo != null)
                {
                    if (orgInfo.PARENT_GUID != "")
                    {
                        MessageBoxResult result = MessageBox.Show("您确认要删除当前节点么？", "删除当前节点", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                        {

                            RemovePPOrginfoFromeList(orgInfo.PARENT_GUID, orgInfo, this.itemList);


                            bool isAllSuccessfull = true;

                            //从数据库删除数据
                            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                            {
                                //删除当前节点
                                isAllSuccessfull = channel.DeletePP_OrgInfo(orgInfo.GUID);
                            });


                            if (isAllSuccessfull == true)
                            {
                                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                                {
                                    //删除当前节点子节点
                                    channel.DeletePP_OrgInfoByParentID(orgInfo.GUID);
                                });


                                this.tv_PersonPlan.ItemsSource = null;
                                this.tv_PersonPlan.ItemsSource = itemList;

                                if (this.tv_PersonPlan.Items.Count > 0)
                                {
                                    itemOrgInfo = this.tv_PersonPlan.Items[0] as PP_OrgInfo;

                                    //界面赋值
                                    SetOrgInfoDetails(itemOrgInfo);

                                }
                            }
                            else
                            {
                                MessageBox.Show("删除节点信息失败！");
                            }

                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("不允许删除根节点！", "删除根节点", MessageBoxButton.OK);

                    }
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
        /// 对当前TreeViewItem进行重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReNameTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            this.isRename = true;
            //获取在TreeView.ItemTemplate中定义的TextBox控件

            tempTextBox = FindVisualChild<TextBox>(item as DependencyObject);

            //设置该TextBox的Visibility 属性为Visible

            tempTextBox.Visibility = Visibility.Visible;

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


        /// <summary>
        /// 当TextBox失去焦点时发生此事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void renametextbox_LostFous(object sender, RoutedEventArgs e)
        {
            tempTextBox.Visibility = Visibility.Collapsed;

            if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            { }
            else
            {//有节点选中,添加当前选中节点的子节点

                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;

                if (orgInfo != null)
                {
                    //保存到数据库
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        channel.SavePP_OrgInfo(orgInfo);
                    });
                }
            }
            this.isRename = false;
        }

        #endregion

        /// <summary>
        /// 保存组织结构信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SaveInfo_Click(object sender, RoutedEventArgs e)
        {
            this.tv_PersonPlan.UpdateLayout();
            var text = this.tv_PersonPlan.ItemContainerGenerator.ContainerFromItem(this.tv_PersonPlan.SelectedItem as PP_OrgInfo);
            // 循环更新组织信息
            //SavePersonPlanOrgInfos(tv_PersonPlan.Items);

            if (!CheckInput())
                return;
            if (SaveOrgInfoDetailInfo() == true)
            {

                #region  保存比对信息
                //保存组织信息
                itemOrgInfoForComp = CopeOrgInfo(itemOrgInfo);

                // 复制保存当前的人员信息
                itemPersonListForComp = CopePersonList(itemPersonList);


                #endregion


                MessageBox.Show("信息保存完毕！");
            }
            else
            {
                MessageBox.Show("部分信息保存失败，请确认！");
            }
        }

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
                if (ItemDetailDataChanged() == true)
                {
                    MessageBoxResult result = MessageBox.Show("本页面信息已经被修改，你是否想保存数据后在离开？", "确认信息", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                    {
                        if (!CheckInput())
                            return;
                        //保存当前组织信息
                        if (SaveOrgInfoDetailInfo() == false)
                        {
                            MessageBox.Show("部分信息保存失败，请确认！");
                        }

                    }
                    else if (result == MessageBoxResult.No)
                    {

                    }
                }

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

                    //在地图上设置选择'高亮'
                    //selectplaceUC.SelectionLocation(itemOrgInfo.GUID);
                }
            }
            catch
            {
                throw;
            }
        }
        private bool CheckInput()
        {
            itemOrgInfo.DUTY = GetDutyData();
            if(string.IsNullOrEmpty(itemOrgInfo.DUTY))
            {
                MessageBox.Show("职责不能为空！");
                return false;
            }
          
            bool doubleName = false;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].NAME.Trim() == this.itemOrgInfo.NAME.Trim() && itemList[i].GUID != itemOrgInfo.GUID)
                {
                    doubleName = true;
                    break;
                }
                if (itemList[i].Children != null && itemList[i].Children.Count > 0)
                {
                    for (int j = 0; j < itemList[i].Children.Count; j++)
                    {
                        if (itemList[i].Children[j].NAME.Trim() == this.itemOrgInfo.NAME.Trim() && itemList[i].Children[j].GUID != itemOrgInfo.GUID)
                        {
                            doubleName = true;
                            break;
                        }
                    }
                }
            }
            if (doubleName)
            {
                MessageBox.Show("组名称不能重复！");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 保存当前组织信息
        /// </summary>
        private bool SaveOrgInfoDetailInfo()
        {
            
            bool isAllSuccessful = true;
            //对比组织信息
            if (CompareOrgInfoData() == true)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    isAllSuccessful = channel.SavePP_OrgInfo(this.itemOrgInfo) && isAllSuccessful ? true : false;
                });

            }

            //比对人员
            if (ComparePersonData() == true)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    List<PP_PersonInfo> list = this.itemPersonList.Where(item => !string.IsNullOrEmpty(item.NAME)).ToList();;
                    if (list.Count == 0)
                    {
                        PP_PersonInfo p0 = new PP_PersonInfo();
                        p0.GUID = CO_IA.Client.Utility.NewGuid();
                        p0.ORG_GUID = itemOrgInfo.GUID;
                        p0.ADD_TYPE = 1;
                        p0.PERSON_TYPE = 0;

                        list.Add(p0);
                    }
                    
                    isAllSuccessful = channel.SavePP_PersonInfoList(list) && isAllSuccessful ? true : false;
                });
            }

           

            return isAllSuccessful;
        }

        #region  比对方法
        /// <summary>
        /// 判断是否存在数据变化
        /// </summary>
        /// <returns></returns>
        private bool ItemDetailDataChanged()
        {
            if (IsReadOnly)
            {
                return false;
            }
            if (itemPersonList.Count != itemPersonListForComp.Count )
            {
                return true;
            }

            //对比组织信息
            if (CompareOrgInfoData() == true)
            {
                return true;
            }

            //比对人员
            if (ComparePersonData() == true)
            {
                return true;
            }

            //比对车辆
           
            return false;

        }


        /// <summary>
        /// 对比组织信息
        /// </summary>
        /// <returns></returns>
        private bool CompareOrgInfoData()
        {
            
            if (itemOrgInfo.DUTY != itemOrgInfoForComp.DUTY)
            {
                return true;
            }
            if (itemOrgInfo.NAME != itemOrgInfoForComp.NAME)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 比对人员
        /// </summary>
        /// <returns></returns>
        private bool ComparePersonData()
        {
            if (itemPersonList.Count == itemPersonListForComp.Count)
            {

                var tempP1 = itemPersonList.OrderBy(x => x.GUID);
                List<PP_PersonInfo> listP1 = new List<PP_PersonInfo>();

                foreach (PP_PersonInfo tempinfo in tempP1)
                {
                    listP1.Add(tempinfo);
                }

                var tempP2 = itemPersonListForComp.OrderBy(x => x.GUID);
                List<PP_PersonInfo> listP2 = new List<PP_PersonInfo>();

                foreach (PP_PersonInfo tempinfo in tempP2)
                {
                    listP2.Add(tempinfo);
                }

                for (int i = 0; i < listP1.Count; i++)
                {
                    //listP1[i].ISCHECKED != listP2[i].ISCHECKED ||
                    if (listP1[i].GUID != listP2[i].GUID ||
                    listP1[i].NAME != listP2[i].NAME ||
                    listP1[i].ORG_GUID != listP2[i].ORG_GUID ||
                    listP1[i].PERSON_TYPE != listP2[i].PERSON_TYPE ||
                    listP1[i].PHONE != listP2[i].PHONE ||
                    listP1[i].PHOTO != listP2[i].PHOTO ||
                    listP1[i].TASK != listP2[i].TASK ||
                    listP1[i].UNIT != listP2[i].UNIT ||
                    listP1[i].ADD_TYPE != listP2[i].ADD_TYPE ||
                    listP1[i].DUTY != listP2[i].DUTY||
                        listP1[i].VOICEID != listP2[i].VOICEID)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }



        /// <summary>
        /// 获取界面中的职责ID
        /// </summary>
        /// <returns></returns>
        private string GetDutyData()
        {
            string dutys = "";
            foreach (var ele in wp_dutylist.Children)
            {
                CheckBox chk = ele as CheckBox;
                if (chk != null)
                {
                    if (chk.IsChecked == true)
                    {
                        dutys += chk.Tag.ToString() + ",";
                    }
                }
            }
            return dutys.Trim(',');
        }
        private bool CompareDutyData()
        {
            if (wp_dutylist.Children.Count > 0)
            {
                List<PP_Duty> selectDuty = new List<PP_Duty>();
                foreach (var ele in wp_dutylist.Children)
                {
                    CheckBox chk = ele as CheckBox;
                    if (chk != null)
                    {
                        if (chk.IsChecked == true)
                        {
                            selectDuty.Add(chk.DataContext as PP_Duty);
                        }
                    }
                }
                if (itemDutyList.Count == selectDuty.Count)
                {
                    var tempP1 = itemDutyList.OrderBy(x => x.Key).ToList<PP_Duty>();
                    var tempP2 = selectDuty.OrderBy(x => x.Key).ToList<PP_Duty>();
                    for (int i = 0; i < tempP1.Count; i++)
                    {
                        if (tempP1[i].Key != tempP2[i].Key)
                        {
                            return true;
                        }
                    }
                }
                else
                    return true;
            }
            return false;
        }
        #endregion

        #region  人员相关操作方法

        /// <summary>
        /// 主任选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OrgLeaderAdd_Click(object sender, RoutedEventArgs e)
        {
            // 选择和设定特定人员
            SetSpecialPerson(0);
        }

        /// <summary>
        /// 选择协调员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CoordinatorAdd_Click(object sender, RoutedEventArgs e)
        {
            // 选择和设定特定人员
            SetSpecialPerson(3);
        }

        /// <summary>
        /// 选择组长
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_GroupLeaderAdd_Click(object sender, RoutedEventArgs e)
        {
            // 选择和设定特定人员
            SetSpecialPerson(1);
        }

        //选择副组长
        private void btn_SubGroupLeaderAdd_Click(object sender, RoutedEventArgs e)
        {
            // 选择和设定特定人员
            SetSpecialPerson(2);
        }


        /// <summary>
        /// 弹出选择添加人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_LevelAdd_Click(object sender, RoutedEventArgs e)
        {
            List<string> exsitUserIDList = GetExistUserList();
            if (itemPersonList.Count > 0)
            {
                itemPersonList.ForEach(item => {
                    if (!exsitUserIDList.Contains(item.GUID))
                        exsitUserIDList.Add(item.GUID);
                });
            }

            SelectOrgPersonDialog dialog = new SelectOrgPersonDialog(true, false, exsitUserIDList);

            dialog.ShowDialog(this);

            if (dialog.DialogResult == true)
            {
                foreach (PP_PersonInfo pPerson in dialog.SelectPersoninfolist)
                {
                    pPerson.ORG_GUID = itemOrgInfo.GUID;
                    pPerson.PERSON_TYPE = 4;
                    itemGrouperList.Add(pPerson);
                    itemPersonList.Add(pPerson);
                }

                //刷新人员界面
                FillGrouperWrapPanel(itemGrouperList);
            }
        }

        /// <summary>
        /// 选择和设定特定人员
        /// </summary>
        /// <param name="p"></param>
        private void SetSpecialPerson(int persontype)
        {
            List<string> exsitUserIDList = GetExistUserList();
            if (itemPersonList.Count > 0)
            {
                itemPersonList.ForEach(item =>
                {
                    if (!exsitUserIDList.Contains(item.GUID))
                        exsitUserIDList.Add(item.GUID);
                });
            }

            //只允许单选
            SelectOrgPersonDialog dialog = new SelectOrgPersonDialog(false, false, exsitUserIDList);

            dialog.ShowDialog(this);

            if (dialog.DialogResult == true)
            {
                itemPersonList = itemPersonList.Where(p => p.PERSON_TYPE != persontype).ToList<PP_PersonInfo>();

                foreach (PP_PersonInfo pPerson in dialog.SelectPersoninfolist)
                {
                    pPerson.ORG_GUID = itemOrgInfo.GUID;
                    pPerson.PERSON_TYPE = persontype;

                    itemPersonList.Add(pPerson);

                    if (pPerson.PERSON_TYPE == 0)
                    {
                        grid_OrgLeader.DataContext = null;
                        grid_OrgLeader.DataContext = pPerson;
                    }
                    else if (pPerson.PERSON_TYPE == 3)
                    {
                        grid_PSO_Coordinator.DataContext = null;
                        grid_PSO_Coordinator.DataContext = pPerson;
                    }
                    else if (pPerson.PERSON_TYPE == 1)
                    {
                        grid_GroupLeaderDetail.DataContext = null;
                        grid_GroupLeaderDetail.DataContext = pPerson;

                        grid_GroupLeaderImage.DataContext = null;
                        grid_GroupLeaderImage.DataContext = pPerson;
                    }
                    else if (pPerson.PERSON_TYPE == 2)
                    {
                        grid_SLeaderInfo.DataContext = null;
                        grid_SLeaderInfo.DataContext = pPerson;

                        grid_SLeaderImage.DataContext = null;
                        grid_SLeaderImage.DataContext = pPerson;
                    }
                }

                //刷新人员界面
                //FillGrouperWrapPanel(itemGrouperList);
            }
        }

        /// <summary>
        /// 判断过滤人员逻辑
        /// </summary>
        /// <returns></returns>
        private List<string> GetExistUserList()
        {
            List<string> existUserIDList = new List<string>();
           
            List<PP_PersonInfo> enpointPersonList = new List<PP_PersonInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                enpointPersonList = channel.GetEndPointPP_PersonInfos(this.activity.Guid);
            });

            foreach (PP_PersonInfo perinfo in enpointPersonList)
            {
                existUserIDList.Add(perinfo.GUID);
            }
           

            return existUserIDList;
        }
        /// <summary>
        /// 判断过滤人员逻辑
        /// </summary>
        /// <returns></returns>
        private List<string> GetExistVehicleList(string groupID=null)
        {
            List<string> existVehicleNumList = new List<string>();
           
            List<PP_VehicleInfo> enpointPersonList = new List<PP_VehicleInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                enpointPersonList = channel.GetEndPointPP_VehicleInfos(this.activity.Guid);
            });
            foreach (PP_VehicleInfo perinfo in enpointPersonList)
            {
                if (!string.IsNullOrEmpty(groupID) && groupID == perinfo.ORG_GUID)
                    continue;
                existVehicleNumList.Add(perinfo.VEHICLE_NUMB);
            }

            return existVehicleNumList;
        }
        /// <summary>
        /// 判断过滤人员逻辑
        /// </summary>
        /// <returns></returns>
        private List<string> GetExistEquList()
        {
            List<string> existEquList = new List<string>();
           
            List<PP_EquipInfo> enpointEquList = new List<PP_EquipInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                //更新当前节点
                enpointEquList = channel.GetEndPointPP_EquipInfos(this.activity.Guid);
            });

            foreach (PP_EquipInfo perinfo in enpointEquList)
            {
                existEquList.Add(perinfo.GUID);
            }
            

            return existEquList;
        }

        /// <summary>
        /// 新增人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void btn_PersonAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    PersonDetailInfoDialog dg = new PersonDetailInfoDialog(itemOrgInfo.GUID);
        //    dg.ShowDialog(this);
        //    if (dg.DialogResult == true)
        //    {
        //        //添加到人员列表
        //        this.itemPersonList.Add(dg.PersonInfoData);
        //        this.itemGrouperList.Add(dg.PersonInfoData);

        //        //刷新人员界面
        //        FillGrouperWrapPanel(itemGrouperList);
        //    }
        //}

        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_PersonModify_Click(object sender, RoutedEventArgs e)
        {

            foreach (PP_PersonInfo personinfo in this.itemGrouperList)
            {
                if (personinfo.ISCHECKED == true)
                {
                    PersonDetailInfoDialog dg = new PersonDetailInfoDialog(personinfo);
                    dg.ShowDialog(this);

                    if (dg.DialogResult == true)
                    {
                        //修改数据
                        foreach (PP_PersonInfo pinfo in this.itemPersonList)
                        {
                            if (pinfo.GUID == dg.PersonInfoData.GUID)
                            {
                                pinfo.NAME = dg.PersonInfoData.NAME;
                                pinfo.DUTY = dg.PersonInfoData.DUTY;
                                pinfo.UNIT = dg.PersonInfoData.UNIT;
                                pinfo.PHONE = dg.PersonInfoData.PHONE;
                                pinfo.PHOTO = dg.PersonInfoData.PHOTO;
                                pinfo.ISCHECKED = false;
                                break;
                            }
                        }

                        foreach (PP_PersonInfo pinfo in itemGrouperList)
                        {
                            if (pinfo.GUID == dg.PersonInfoData.GUID)
                            {
                                pinfo.NAME = dg.PersonInfoData.NAME;
                                pinfo.DUTY = dg.PersonInfoData.DUTY;
                                pinfo.UNIT = dg.PersonInfoData.UNIT;
                                pinfo.PHONE = dg.PersonInfoData.PHONE;
                                pinfo.PHOTO = dg.PersonInfoData.PHOTO;
                                pinfo.ISCHECKED = false;

                                break;
                            }

                        }
                        //刷新人员界面
                        FillGrouperWrapPanel(itemGrouperList);
                    }
                    break;
                }

            }
        }

        /// <summary>
        /// 删除人员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_PersonDelete_Click(object sender, RoutedEventArgs e)
        {
            List<PP_PersonInfo> tempPersonList = new List<PP_PersonInfo>();

            if (cb_Grouper.IsChecked == true)
            {
                foreach (PP_PersonInfo pInfo in this.itemGrouperList)
                {
                    if (pInfo.ISCHECKED == true)
                    {
                        tempPersonList.Add(pInfo);
                    }
                }
            }
            else
            {
                foreach (PP_GrouperUC uc in this.wp_GrouperListr.Children)
                {
                    if (uc.cb_PersonName.IsChecked == true)
                    {
                        tempPersonList.Add((PP_PersonInfo)uc.Tag);
                    }
                }
            }

            foreach (PP_PersonInfo pInfo in tempPersonList)
            {

                try
                {
                    this.itemGrouperList.Remove(pInfo);
                }
                catch { }

                try
                {
                    this.itemPersonList.Remove(pInfo);
                }
                catch { }
            }
            //itemGrouperList = tempPersonList;

            //填充数据
            FillGrouperWrapPanel(itemGrouperList);

        }

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

        /// <summary>
        /// 控制人员修改按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_SingleGrouper_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;

            foreach (PP_PersonInfo pinfo in this.itemGrouperList)
            {

                if (pinfo.ISCHECKED == true)
                {
                    i++;
                }
            }
        }

        #endregion

      


        #region 图片上传
        private void btn_Coordinator_Upload(object sender, MouseButtonEventArgs e)
        {
            PP_PersonInfo temp = grid_PSO_Coordinator.DataContext as PP_PersonInfo;
            if (temp == null || string.IsNullOrEmpty(temp.NAME))
            {
                //return;
            }
            //协调员
            UploadPersonImage(this.img_Coordinator);
        }

        private void btn_GroupLeader_Upload(object sender, MouseButtonEventArgs e)
        {
            PP_PersonInfo temp = grid_GroupLeaderDetail.DataContext as PP_PersonInfo;
            if (temp == null || string.IsNullOrEmpty(temp.NAME))
            {
                //return;
            }
            //组长
            UploadPersonImage(this.img_GroupLeader);
        }

        private void btn_SLeader_Upload(object sender, MouseButtonEventArgs e)
        {
            PP_PersonInfo temp = grid_SLeaderInfo.DataContext as PP_PersonInfo;
            if (temp == null || string.IsNullOrEmpty(temp.NAME))
            {
                //return;
            }
            //副组长
            UploadPersonImage(this.img_SLeader);
        }

        private void btn_OrgLeader_Upload(object sender, MouseButtonEventArgs e)
        {
            PP_PersonInfo temp = grid_OrgLeader.DataContext as PP_PersonInfo;
            if (temp == null || string.IsNullOrEmpty(temp.NAME))
            {
                //MessageBox.Show("姓名不能为空！");
                //return;
            }
            //办公室主任
            UploadPersonImage(this.img_OrgLeader);
        }
        /// <summary>
        /// 人员图片上传（办公室主任）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OrgLeader_Upload(object sender, RoutedEventArgs e)
        {
            UploadPersonImage(this.img_OrgLeader);
        }
        /// <summary>
        /// 人员图片上传（组长）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_GroupLeader_Upload(object sender, RoutedEventArgs e)
        {
            UploadPersonImage(this.img_GroupLeader);

        }
        /// <summary>
        /// 人员图片上传（副组长）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SLeader_Upload(object sender, RoutedEventArgs e)
        {
            UploadPersonImage(this.img_SLeader);

        }
        /// <summary>
        /// 人员图片上传（协调员）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Coordinator_Upload(object sender, RoutedEventArgs e)
        {
            UploadPersonImage(this.img_Coordinator);

        }
        /// <summary>
        /// 填充图片
        /// </summary>
        /// <param name="image"></param>
        private void UploadPersonImage(Image image)
        {
            if (IsReadOnly)
                return;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图片|*.jpg;*.png;*.jpeg;*.bmp";//过滤器
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



        private void AddTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            //添加树
            if (tv_PersonPlan.Items.Count > 0)
            {
                PP_OrgInfo orgInfo = tv_PersonPlan.Items[0] as PP_OrgInfo;
                if (orgInfo != null)
                {

                    PP_OrgInfo tempOrgInfo = new PP_OrgInfo();
                    tempOrgInfo.GUID =CO_IA.Client.Utility.NewGuid();
                    tempOrgInfo.ACTIVITY_GUID = orgInfo.ACTIVITY_GUID;
                    tempOrgInfo.PARENT_GUID = orgInfo.GUID;
                    tempOrgInfo.NAME = getDefaultName(0);// "待重命名";
                    tempOrgInfo.DUTY = "05";
                    orgInfo.Children.Add(tempOrgInfo);


                    //保存到数据库
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                    {
                        //更新当前节点
                        channel.SavePP_OrgInfo(tempOrgInfo);
                    });

                    this.tv_PersonPlan.ItemsSource = null;
                    this.tv_PersonPlan.ItemsSource = itemList;

                    //界面赋值
                    SetOrgInfoDetails(tempOrgInfo);
                }
            }
        }
        private string getDefaultName(int nameindex)
        {
            string defaultname = "待重命名";
            if (nameindex != 0)
                defaultname = defaultname + nameindex.ToString();
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].NAME.Trim() == defaultname)
                {
                    return getDefaultName(nameindex+1);
                    
                }
                if (itemList[i].Children != null && itemList[i].Children.Count > 0)
                {
                    for (int j = 0; j < itemList[i].Children.Count; j++)
                    {
                        if (itemList[i].Children[j].NAME.Trim() == defaultname)
                        {
                            return getDefaultName(nameindex + 1);
                            
                        }
                    }
                }
            }
            return defaultname ;
        }
        private void DelTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.tv_PersonPlan.SelectedItem == null)//检查是否有节点选中
            { }
            else
            {//有节点选中,添加当前选中节点的子节点

                PP_OrgInfo orgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
                if (orgInfo != null)
                {
                    if (orgInfo.PARENT_GUID != "")
                    {
                        MessageBoxResult result = MessageBox.Show("您确认要删除当前节点么？", "删除当前节点", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)  
                        {
                            RemovePPOrginfoFromeList(orgInfo.PARENT_GUID, orgInfo, this.itemList);
                            bool isAllSuccessfull = true;
                            //从数据库删除数据
                            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                            {
                                //删除当前节点
                                isAllSuccessfull = channel.DeletePP_OrgInfo(orgInfo.GUID);
                            });

                            if (isAllSuccessfull == true)
                            {
                                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                                {
                                    //删除当前节点子节点
                                    channel.DeletePP_OrgInfoByParentID(orgInfo.GUID);
                                });

                                this.tv_PersonPlan.ItemsSource = null;
                                this.tv_PersonPlan.ItemsSource = itemList;

                                if (this.tv_PersonPlan.Items.Count > 0)
                                {
                                    itemOrgInfo = this.tv_PersonPlan.Items[0] as PP_OrgInfo;
                                    //界面赋值
                                    SetOrgInfoDetails(itemOrgInfo);
                                }
                            }
                            else
                            {
                                MessageBox.Show("删除节点信息失败！");
                            }
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("不允许删除根节点！", "删除根节点", MessageBoxButton.OK);
                    }
                }
            }
        }

        private void StackPanel_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.IsReadOnly)
            {
                e.Handled = true;
            }
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

     


        ContextMenu setMenu;
        private void dg_GrouperList_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            //grid 组员 右键事件
            GetContextMenu();
            dg_GrouperList.ContextMenu = setMenu;

        }
        private void uc_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            GetContextMenu();
            PP_GrouperUC uc = sender as PP_GrouperUC;
            if(uc!=null)
                uc.ContextMenu = setMenu;
        }
        private ContextMenu GetContextMenu()
        {
            if (IsReadOnly)
                return null;
            if (setMenu != null)
                return null;
            setMenu = new ContextMenu();
            setMenu.Opened += menu_Opened;
            MenuItem item = null;
            
            item = new MenuItem();
            item.Name = "zhuren";
            item.Tag = "0";
            item.Header = "设置为主任";
            item.Click += item_Click;
            setMenu.Items.Add(item);

            item = new MenuItem();
            item.Name = "xietiaoyuan";
            item.Tag = "0";
            item.Header = "设置为协调员";
            item.Click += item_Click;
            setMenu.Items.Add(item);
           
            item = new MenuItem();
            item.Name = "zuzhang";
            item.Tag = "1";
            item.Header = "设置为组长";
            item.Click += item_Click;
            setMenu.Items.Add(item);

            item = new MenuItem();
            item.Name = "fuzuzhang";
            item.Tag = "1";
            item.Header = "设置为副组长";
            item.Click += item_Click;
            setMenu.Items.Add(item);

            return setMenu;
        }

        void menu_Opened(object sender, RoutedEventArgs e)
        {
            PP_OrgInfo itemOrgInfo = tv_PersonPlan.SelectedItem as PP_OrgInfo;
            if(tv_PersonPlan.SelectedItem==null||itemOrgInfo==null)
            {
                e.Handled = true;
                return;
            }
            
            foreach (var obj in setMenu.Items)
            {
                MenuItem item = obj as MenuItem;
                if (item != null&&item.Tag!=null)
                {
                    if(item.Tag.ToString()=="0")
                    {
                        item.Visibility = string.IsNullOrEmpty(itemOrgInfo.PARENT_GUID)? Visibility.Visible:Visibility.Collapsed;
                    }
                    if (item.Tag.ToString() == "1")
                    {
                        item.Visibility = string.IsNullOrEmpty(itemOrgInfo.PARENT_GUID) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;
            var element = ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(menuitem));

            PP_PersonInfo info = null ;
            //获取选中的对象
            if (element is DataGrid)
            {
                info = dg_GrouperList.SelectedItem as PP_PersonInfo; 
            }
            if (element is PP_GrouperUC)
            {
                PP_GrouperUC uc = element as PP_GrouperUC;
                info = uc.Tag as PP_PersonInfo;
            }
            if (info == null)
                return;
            switch (menuitem.Name)
            { 
                case "zhuren":
                    if(grid_OrgLeader.DataContext!=null)
                    {
                        PP_PersonInfo temp = grid_OrgLeader.DataContext as PP_PersonInfo;
                        grid_OrgLeader.DataContext = null;
                        if (string.IsNullOrEmpty(temp.NAME))
                        {
                            itemPersonList.Remove(temp);
                        }
                        else
                        {
                            temp.PERSON_TYPE = 4;
                            itemGrouperList.Add(temp);
                        }
                    }
                    info.PERSON_TYPE = 0;                    
                    grid_OrgLeader.DataContext = info;
                    
                    break;
                case "xietiaoyuan":
                    if (grid_PSO_Coordinator.DataContext != null)
                    {
                        PP_PersonInfo temp = grid_PSO_Coordinator.DataContext as PP_PersonInfo;
                        grid_PSO_Coordinator.DataContext = null;
                        if (string.IsNullOrEmpty(temp.NAME))
                        {
                            itemPersonList.Remove(temp);
                        }
                        else
                        {
                            temp.PERSON_TYPE = 4;
                            itemGrouperList.Add(temp);
                        }
                    }
                    info.PERSON_TYPE = 3;                    
                    grid_PSO_Coordinator.DataContext = info;
                    break;
                case "zuzhang":
                    if (grid_GroupLeaderDetail.DataContext != null)
                    {
                        PP_PersonInfo temp = grid_GroupLeaderDetail.DataContext as PP_PersonInfo;
                        grid_GroupLeaderDetail.DataContext = null;
                        if (string.IsNullOrEmpty(temp.NAME))
                        {
                            itemPersonList.Remove(temp);
                        }
                        else
                        {
                            temp.PERSON_TYPE = 4;
                            itemGrouperList.Add(temp);
                        }
                    }
                    info.PERSON_TYPE = 1;                    
                    grid_GroupLeaderDetail.DataContext = info;

                    grid_GroupLeaderImage.DataContext = null;
                    grid_GroupLeaderImage.DataContext = info;
                    break;
                case "fuzuzhang":
                    if (grid_SLeaderInfo.DataContext != null)
                    {
                        PP_PersonInfo temp = grid_SLeaderInfo.DataContext as PP_PersonInfo;
                        grid_SLeaderInfo.DataContext = null;
                        if (string.IsNullOrEmpty(temp.NAME))
                        {
                            itemPersonList.Remove(temp);
                        }
                        else
                        {
                            temp.PERSON_TYPE = 4;
                            itemGrouperList.Add(temp);
                        }
                    }
                    info.PERSON_TYPE = 2;                   
                    grid_SLeaderInfo.DataContext = info;
                    grid_SLeaderImage.DataContext = null;
                    grid_SLeaderImage.DataContext = info;
                    break;
            }
            itemGrouperList.Remove(info);
            FillGrouperWrapPanel(itemGrouperList);
        }

        private void menuItemSetGrouper_Click(object sender, RoutedEventArgs e)
        {
            //设为组员
            var element = ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(sender as MenuItem));
            Image img = element as Image;
            PP_PersonInfo info = img.DataContext as PP_PersonInfo;
            if (info != null)
            {
                if (string.IsNullOrEmpty(info.NAME))
                    return;
                switch (info.PERSON_TYPE)
                { 
                    case 0://主任
                        grid_OrgLeader.DataContext = null;
                        PP_PersonInfo p0 = new PP_PersonInfo();
                        p0.GUID =CO_IA.Client.Utility.NewGuid();
                        p0.ORG_GUID = info.ORG_GUID;
                        p0.ADD_TYPE = 1;
                        p0.PERSON_TYPE = 0;

                        itemPersonList.Add(p0);
                        grid_OrgLeader.DataContext = p0;
                        break;
                    case 3://协调员
                        grid_PSO_Coordinator.DataContext = null;
                        PP_PersonInfo p3 = new PP_PersonInfo();
                        p3.GUID =CO_IA.Client.Utility.NewGuid();
                        p3.ORG_GUID = info.ORG_GUID;
                        p3.ADD_TYPE = 1;
                        p3.PERSON_TYPE = 3;
                        itemPersonList.Add(p3);
                        grid_PSO_Coordinator.DataContext = p3;
                        break;
                    case 1://组长
                        grid_GroupLeaderDetail.DataContext = null;
                        grid_GroupLeaderImage.DataContext = null;
                        PP_PersonInfo p1 = new PP_PersonInfo();

                        p1.GUID =CO_IA.Client.Utility.NewGuid();
                        p1.ORG_GUID = info.ORG_GUID;
                        p1.ADD_TYPE = 1;
                        p1.PERSON_TYPE = 1;

                        itemPersonList.Add(p1);
                        grid_GroupLeaderDetail.DataContext = p1;
                        grid_GroupLeaderImage.DataContext = p1;
                        break;
                    case 2://副组长
                        grid_SLeaderInfo.DataContext = null;
                        grid_SLeaderImage.DataContext = null;
                        PP_PersonInfo p2 = new PP_PersonInfo();
                        p2.GUID =CO_IA.Client.Utility.NewGuid();
                        p2.ORG_GUID = info.ORG_GUID;
                        p2.ADD_TYPE = 1;
                        p2.PERSON_TYPE = 2;
                        itemPersonList.Add(p2);
                        grid_SLeaderInfo.DataContext = p2;
                        grid_SLeaderImage.DataContext = p2;
                        break;
                }
                info.PERSON_TYPE = 4;
                itemGrouperList.Add(info);
                FillGrouperWrapPanel(itemGrouperList);
            }

        }

        private void menuItemClearGrouper_Click(object sender, RoutedEventArgs e)
        {
            //清除
            var element = ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(sender as MenuItem));
            Image img = element as Image;
            PP_PersonInfo info = img.DataContext as PP_PersonInfo;
            if (info != null)
            {
                if (string.IsNullOrEmpty(info.NAME))
                    return;
                switch (info.PERSON_TYPE)
                {
                    case 0://主任
                        grid_OrgLeader.DataContext = null;
                        PP_PersonInfo p0 = new PP_PersonInfo();
                        p0.GUID = CO_IA.Client.Utility.NewGuid();
                        p0.ORG_GUID = info.ORG_GUID;
                        p0.ADD_TYPE = 1;
                        p0.PERSON_TYPE = 0;

                        itemPersonList.Add(p0);
                        grid_OrgLeader.DataContext = p0;
                        break;
                    case 3://协调员
                        grid_PSO_Coordinator.DataContext = null;
                        PP_PersonInfo p3 = new PP_PersonInfo();
                        p3.GUID = CO_IA.Client.Utility.NewGuid();
                        p3.ORG_GUID = info.ORG_GUID;
                        p3.ADD_TYPE = 1;
                        p3.PERSON_TYPE = 3;
                        itemPersonList.Add(p3);
                        grid_PSO_Coordinator.DataContext = p3;
                        break;
                    case 1://组长
                        grid_GroupLeaderDetail.DataContext = null;
                        grid_GroupLeaderImage.DataContext = null;
                        PP_PersonInfo p1 = new PP_PersonInfo();

                        p1.GUID = CO_IA.Client.Utility.NewGuid();
                        p1.ORG_GUID = info.ORG_GUID;
                        p1.ADD_TYPE = 1;
                        p1.PERSON_TYPE = 1;

                        itemPersonList.Add(p1);
                        grid_GroupLeaderDetail.DataContext = p1;
                        grid_GroupLeaderImage.DataContext = p1;
                        break;
                    case 2://副组长
                        grid_SLeaderInfo.DataContext = null;
                        grid_SLeaderImage.DataContext = null;
                        PP_PersonInfo p2 = new PP_PersonInfo();
                        p2.GUID = CO_IA.Client.Utility.NewGuid();
                        p2.ORG_GUID = info.ORG_GUID;
                        p2.ADD_TYPE = 1;
                        p2.PERSON_TYPE = 2;
                        itemPersonList.Add(p2);
                        grid_SLeaderInfo.DataContext = p2;
                        grid_SLeaderImage.DataContext = p2;
                        break;
                }
                itemPersonList.Remove(info);
            }
        }

        private void btn_Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel文件(*.xls)|*.xls";
            dialog.DefaultExt = "xls";
            if (dialog.ShowDialog() == true)
            {
                string[] columns = new string[]{"人员名称","性别","单位","部门","职责","电话号码","语音呼叫号码","备注","照片"};
                DataTable importDt = ExcelImportHelper.LoadDataFromExcel(dialog.FileName);
                if (importDt != null && importDt.Rows.Count > 0)
                {
                    List<PP_PersonInfo> persons = new List<PP_PersonInfo>();
                    for (int i = 0; i < importDt.Rows.Count; i++)
                    {
                        if (importDt.Columns.Contains("人员名称"))
                        {
                            if (string.IsNullOrEmpty(importDt.Rows[i]["人员名称"].ToString()))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                        PP_PersonInfo info = new PP_PersonInfo();
                        info.GUID = Utility.NewGuid();
                        info.ORG_GUID = itemOrgInfo.GUID;
                        info.NAME = importDt.Rows[i]["人员名称"].ToString().Trim();
                        if (importDt.Columns.Contains("性别"))
                        {
                            info.SEX = importDt.Rows[i]["性别"].ToString().Trim();
                        }
                        if (importDt.Columns.Contains("单位"))
                        {
                            info.UNIT = importDt.Rows[i]["单位"].ToString().Trim();
                        }
                        if (importDt.Columns.Contains("部门"))
                        {
                            info.DEPT = importDt.Rows[i]["部门"].ToString().Trim();
                        }
                        if (importDt.Columns.Contains("职责"))
                        {
                            info.DUTY = importDt.Rows[i]["职责"].ToString().Trim();
                        }
                        if (importDt.Columns.Contains("电话号码"))
                        {
                            info.PHONE = importDt.Rows[i]["电话号码"].ToString().Trim();
                        }
                        if (importDt.Columns.Contains("语音呼叫号码"))
                        {
                            info.VOICEID = importDt.Rows[i]["语音呼叫号码"].ToString().Trim();

                        }
                        if (importDt.Columns.Contains("备注"))
                        {
                            info.TASK = importDt.Rows[i]["备注"].ToString().Trim();

                        }
                        if (importDt.Columns.Contains("照片"))
                        {
                            if (!string.IsNullOrEmpty(importDt.Rows[i][8].ToString()))
                                info.PHOTO = GetPhotoByPath(dialog.FileName, importDt.Rows[i][8].ToString().Trim());
                        }
                        
                        info.PERSON_TYPE = 4;
                        
                        persons.Add(info);
                    }
                    if (persons.Count > 0)
                    {
                        itemGrouperList.AddRange(persons);
                        itemPersonList.AddRange(persons);
                        FillGrouperWrapPanel(itemGrouperList);
                        MessageBox.Show("导入成功，成功添加"+persons.Count.ToString()+"条记录！");
                        return;
                    }

                }
                MessageBox.Show("没有获取到记录！");
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
            if (this.IsReadOnly)
            {
                if (obj == this.img1 || obj == this.img3 || obj == this.img2 || obj == this.img4)
                {
                    Image img = obj as Image;
                    img.IsEnabled = false;
                }
            }
            //if (obj == this.groupBoxStation)
            //{
            //    System.Windows.Controls.CheckBox chkbox = groupBoxStation.Header as System.Windows.Controls.CheckBox;
            //    if (chkbox != null)
            //        chkbox.IsEnabled = !newValue;
            //}
           
            return base.UpdateIsReadOnly(obj, newValue);
        }
        private byte[] GetPhotoByPath(string filePath,string fileName)
        {
            string imgPath = filePath.Substring(0,filePath.LastIndexOf('\\')+1)+"images\\"+fileName;

            if (File.Exists(imgPath))
            {
                FileStream fs = new FileStream(imgPath, FileMode.Open);//可以是其他重载方法
                byte[] byData = new byte[fs.Length];
                fs.Read(byData, 0, byData.Length);
                fs.Close();
                return byData;
            }
            return null;
        }

        private void btn_DownLoad_Click(object sender, RoutedEventArgs e)
        {
            string tempFile = AppDomain.CurrentDomain.BaseDirectory + "Template\\person";
            if (!Directory.Exists(tempFile))
            {
                Directory.CreateDirectory(tempFile);
                MessageBox.Show("模板文件夹中不存在模板文件！");
                return;
            }
            DirectoryInfo TheFolder=new DirectoryInfo(tempFile);
            FileInfo[] files = TheFolder.GetFiles();
            if (files == null || files.Length == 0)
            {
                MessageBox.Show("模板文件夹中不存在模板文件！");
                return;
            }
            foreach (FileInfo file in TheFolder.GetFiles())
            {
                if (file.Extension.ToLower() == ".zip")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "zip files(*.zip)|*.zip|All files(*.*)|*.*";
                    saveFileDialog.RestoreDirectory = true;
                    bool? result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        string localFilePath = saveFileDialog.FileName.ToString();
                        try
                        {
                            file.CopyTo(localFilePath, true);
                            MessageBox.Show("保存模板成功！");
                        }
                        catch
                        {
                            MessageBox.Show("保存模板失败！");
                        }
                    }
                }
                //if (file.Extension.ToLower() == ".xls" || file.Extension.ToLower() == ".xlsx")
                //{
                //    SaveFileDialog saveFileDialog = new SaveFileDialog();
                //    saveFileDialog.Filter = "xls files(*.xls)|*.xls|xlsx files(*.xlsx)|*.xlsx|All files(*.*)|*.*";
                //    saveFileDialog.RestoreDirectory = true;
                //    bool? result = saveFileDialog.ShowDialog();
                //    if (result == true)
                //    {
                //        string localFilePath = saveFileDialog.FileName.ToString();
                //        try
                //        {
                //            file.CopyTo(localFilePath, true);
                //            Directory.CreateDirectory("");
                //            MessageBox.Show("保存模板成功！");
                //        }
                //        catch
                //        {
                //            MessageBox.Show("保存模板失败！");
                //        }
                //    }
                //}
            }
        }


        private void ImgDel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            PP_PersonInfo info = img.DataContext as PP_PersonInfo;
            info.PHOTO = null;
        }

        private void ImgDelV_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            PP_VehicleInfo info = img.DataContext as PP_VehicleInfo;
            info.PICTURE = null;
        }
      


    }
}