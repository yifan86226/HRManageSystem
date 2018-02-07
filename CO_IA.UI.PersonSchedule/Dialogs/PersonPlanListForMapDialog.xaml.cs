#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案小组列表弹出窗口
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
    /// PersonPlanListForMapDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPlanListForMapDialog : UserControl
    {
        public PersonPlanListForMapDialog(List<string> orgList)
        {
            InitializeComponent();


            foreach (string orgId in orgList)
            {
                PP_GroupGennelUC uc = new PP_GroupGennelUC(orgId);

                this.sp_Groups.Children.Add(uc);
            }
        }
   
   
    }
}
