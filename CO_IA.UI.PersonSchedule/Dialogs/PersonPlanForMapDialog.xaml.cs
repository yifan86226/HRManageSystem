#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案地图弹出窗口
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using CO_IA.Data;
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
using System.Windows.Shapes;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// PersonPlanForMapDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPlanForMapDialog : Window
    {
        /// <summary>
        /// 当前组织
        /// </summary>
        private PP_OrgInfo itemOrgInfo = new PP_OrgInfo();


        /// <summary>
        /// 每个节点获取的人员列表
        /// </summary>
        List<PP_PersonInfo> itemPersonList = new List<PP_PersonInfo>();


        /// <summary>
        /// 当前组员列表
        /// </summary>
        List<PP_PersonInfo> itemGrouperList = new List<PP_PersonInfo>();


        /// <summary>
        /// 每个节点获取的车辆信息
        /// </summary>
        List<PP_EquipInfo> itemEquipList = new List<PP_EquipInfo>();



        /// <summary>
        /// 每个节点获取的设备列表
        /// </summary>
        PP_VehicleInfo itemVehicle = new PP_VehicleInfo();




        public PersonPlanForMapDialog( )
        {
            InitializeComponent();
        }


        public PersonPlanForMapDialog(string orgid)
        {
            InitializeComponent();
            if (orgid != null)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemOrgInfo = channel.GetPP_OrgInfo(orgid);
                });

                //界面赋值
                SetOrgInfoDetails(itemOrgInfo);
            }
        }
 

        /// <summary>
        /// 根据组织结构信息读取改组织详细信息
        /// </summary>
        /// <param name="selectOrgInfo"></param>
        private void SetOrgInfoDetails(PP_OrgInfo selectOrgInfo)
        {

            if (selectOrgInfo != null)
            {
                grid_GroupLeaderHeader.DataContext = null;
                grid_GroupLeaderHeader.DataContext = selectOrgInfo;

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemPersonList = channel.GetPP_PersonInfos(selectOrgInfo.GUID);
                });

            
                //根节点
                if (selectOrgInfo.PARENT_GUID == "")
                {
                    grid_EquipList.Visibility = System.Windows.Visibility.Hidden;
                    //便利人员列表并赋值人员类型：0.主任 1.组长；2.副组长3.协调裕安。4.组员
                    foreach (PP_PersonInfo tempPersonInfo in itemPersonList)
                    {
                        if (tempPersonInfo.PERSON_TYPE == 4)
                        {
                            itemGrouperList.Add(tempPersonInfo);
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 0)
                        {
                            grid_GroupLeaderDetail.DataContext = null;
                            grid_GroupLeaderDetail.DataContext = tempPersonInfo;


                            grid_GroupLeaderImage.DataContext = null;
                            grid_GroupLeaderImage.DataContext = tempPersonInfo;
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 3)
                        {
                            grid_SLeaderInfo.DataContext = null;
                            grid_SLeaderInfo.DataContext = tempPersonInfo;

                            grid_SLeaderImage.DataContext = null;
                            grid_SLeaderImage.DataContext = tempPersonInfo;
                        }
                    }


                }
                //其他组织节点
                else
                {
                    grid_EquipList.Visibility = System.Windows.Visibility.Visible;

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
#warning 设备已修改
                        //itemEquipList = channel.GetPP_EqupInfos(selectOrgInfo.GUID);
                    });


                    this.dg_EquipList.ItemsSource = null;
                    this.dg_EquipList.ItemsSource = itemEquipList;
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


                this.gird_VehicleInfo.DataContext = null;
                this.gird_VehicleInfo.DataContext = itemVehicle;

     

            }
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
                PP_GrouperUC uc = new PP_GrouperUC(tempPersonInfo,false);
                uc.Tag = tempPersonInfo;
                this.wp_GrouperListr.Children.Add(uc);
            }
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


    }
}
