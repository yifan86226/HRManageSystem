#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：小组概要信息控件
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using CO_IA.Data;
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

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// PP_GroupGennelUC.xaml 的交互逻辑
    /// </summary>
    public partial class PP_GroupGennelUC : UserControl
    {



        /// <summary>
        /// 当前组织ID
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



        public PP_GroupGennelUC(string orgid)
        {
            InitializeComponent();

            //根据组织ID获取当前ID

           
            if (orgid != null)
            {

                //读取人员信息
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemOrgInfo = channel.GetPP_OrgInfo(orgid);
                });

                if (itemOrgInfo == null)
                {
                    MessageBox.Show("未成功取得组织详细信息，请检查数据是否正确！");
                    return;
                }

                //读取人员信息
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemPersonList = channel.GetPP_PersonInfos(itemOrgInfo.GUID);
                });

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemEquipList = channel.GetPP_EqupInfos(itemOrgInfo.GUID);
                });
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    //更新当前节点
                    itemVehicle = channel.GetPP_VehicleInfo(itemOrgInfo.GUID);
                });



                if (itemOrgInfo.ORG_TYPE == 1)
                {
                    this.tb_GroupName.Text = itemOrgInfo.NAME + "(监测组)";
                }
                else
                {
                    this.tb_GroupName.Text = itemOrgInfo.NAME;
                }

                string detailStr = "";

                //根节点
                if (itemOrgInfo.PARENT_GUID == "")
                {
                    //便利人员列表并赋值人员类型：0.主任 1.组长；2.副组长3.协调裕安。4.组员
                    foreach (PP_PersonInfo tempPersonInfo in itemPersonList)
                    {
                        if (tempPersonInfo.PERSON_TYPE == 4)
                        {
                            itemGrouperList.Add(tempPersonInfo);
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 0)
                        {
                            this.tb_GroupLeader.Text = tempPersonInfo.NAME;

                            //设置人员照片
                            SetImageSource(tempPersonInfo.PHOTO);
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 3)
                        {
                            detailStr += "协调员：" + tempPersonInfo.NAME + "; ";
                        }
                    }
                }
                //其他组织节点
                else
                {
                    //便利人员列表并赋值人员类型：0.主任 1.组长；2.副组长3.协调裕安。4.组员
                    foreach (PP_PersonInfo tempPersonInfo in itemPersonList)
                    {
                        if (tempPersonInfo.PERSON_TYPE == 4)
                        {
                            itemGrouperList.Add(tempPersonInfo);

                        }
                        else if (tempPersonInfo.PERSON_TYPE == 1)
                        {
                            this.tb_GroupLeader.Text = tempPersonInfo.NAME;

                            //设置人员照片
                            SetImageSource(tempPersonInfo.PHOTO);
                        }
                        else if (tempPersonInfo.PERSON_TYPE == 2)
                        {
                            detailStr += "副组长：" + tempPersonInfo.NAME + "; ";
                        }
                    }
                }

                detailStr += "组员" + itemGrouperList.Count + "名; " + "设备：" + itemEquipList.Count + "台; " + "车牌号：" + itemVehicle.VEHICLE_NUMB + ".";
                this.tb_GroupDetail.Text = detailStr;
            }
        }
      
        /// <summary>
        /// 设置照片
        /// </summary>
        /// <param name="p"></param>
        private void SetImageSource(byte[] imageData)
        {
            if (imageData != null && imageData.Length > 0)
            {
                //byte[] imageData = File.ReadAllBytes(fileName);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData);//imageData是从数据库中读取出来的字节数组

                ms.Seek(0, System.IO.SeekOrigin.Begin);

                BitmapImage newBitmapImage = new BitmapImage();

                newBitmapImage.BeginInit();

                newBitmapImage.StreamSource = ms;

                newBitmapImage.EndInit();

                this.img_GroupLeader.Source = newBitmapImage;
            }
        }


        /// <summary>
        /// 开启进一步详细信息显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PersonPlanForMapDialog dg = new PersonPlanForMapDialog(itemOrgInfo.GUID);
            dg.ShowDialog(this);
        }
    }
}
