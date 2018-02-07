using CO_IA.Client;
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

namespace CO_IA.UI.Screen.Group
{
    /// <summary>
    /// GroupDialog.xaml 的交互逻辑
    /// </summary>
    public partial class GroupDialog : Window
    {
        public GroupDialog(List<PP_OrgInfo> nodes)
        {
            InitializeComponent();
            PersonSchedule.Foreign.PersonPlanDialog groupDialog = new PersonSchedule.Foreign.PersonPlanDialog(nodes);
            groupDialog.IsReadOnly = true;
            g.Children.Add(groupDialog);
        }
    }
}
