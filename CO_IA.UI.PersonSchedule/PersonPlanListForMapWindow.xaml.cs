#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：弹出人员小组列表
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
    /// PersonPlanListForMapWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPlanListForMapWindow : Window
    {
        public PersonPlanListForMapWindow(List<string>  orgList)
        {
            InitializeComponent();

            PersonPlanListForMapDialog dialog = new PersonPlanListForMapDialog(orgList);
            this.grid_main.Children.Add(dialog);
        }
    }
}
