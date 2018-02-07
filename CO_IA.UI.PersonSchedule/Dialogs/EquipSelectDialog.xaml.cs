#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：设备选择弹出窗口
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
    /// EquipSelectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipSelectDialog : Window
    {
        private List<PP_EquipInfo> equpList = new List<PP_EquipInfo>();
        public EquipSelectDialog()
        {
            InitializeComponent();

            PP_EquipInfo e1 = new PP_EquipInfo();

            e1.NAME = "接收线";
            e1.MODEL = "PR100";
            e1.EQUIP_NUMB = "1300012";

            equpList.Add(e1);

            PP_EquipInfo e2 = new PP_EquipInfo();

            e2.NAME = "馈线";
            e2.MODEL = "";
            e2.EQUIP_NUMB = "1300013";

            equpList.Add(e2);

            PP_EquipInfo e3 = new PP_EquipInfo();

            e3.NAME = "笔记本";
            e3.MODEL = "联想1130";
            e3.EQUIP_NUMB = "1300015";


            equpList.Add(e3);

            this.dg_EquipList.ItemsSource = equpList;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
