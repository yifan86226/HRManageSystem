#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：设备信息弹出窗口
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
using System.Windows.Shapes;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// EquipDetailInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipDetailInfoDialog : Window
    {

        public int mode = 0;//0:新增； 1：修改

        public PP_EquipInfo EquipInfoData = new PP_EquipInfo();


        private string tempOrgGuid = "";



        public EquipDetailInfoDialog(string orgGuid)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            tempOrgGuid = orgGuid;
        }

        public EquipDetailInfoDialog(PP_EquipInfo equipInfo)
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            mode = 1;//修改
            EquipInfoData = equipInfo;
            tb_Name.Text = equipInfo.NAME;
            tb_Mode.Text = equipInfo.MODEL;
            tb_Numb.Text = equipInfo.EQUIP_NUMB;
        }



        /// <summary>
        /// 点击保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (mode == 0)
            {
                EquipInfoData.GUID =CO_IA.Client.Utility.NewGuid();
                EquipInfoData.ORG_GUID = tempOrgGuid;          
                EquipInfoData.ADD_TYPE = 2;
            }

            EquipInfoData.NAME = tb_Name.Text.Trim();
            EquipInfoData.MODEL = tb_Mode.Text.Trim();
            EquipInfoData.EQUIP_NUMB = tb_Numb.Text.Trim();
            this.DialogResult = true;
            this.Close();

        }

        /// <summary>
        /// 点击取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }

    }
}
