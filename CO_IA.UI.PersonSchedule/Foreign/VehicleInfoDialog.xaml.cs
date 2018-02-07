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

namespace CO_IA.UI.PersonSchedule.Foreign
{
    /// <summary>
    /// VehicleInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleInfoDialog : Window
    {
        string OrgID = "";
        public VehicleInfoDialog(PP_VehicleInfo info)
        {
            InitializeComponent();
            this.DataContext = info;
        }
    }
}
