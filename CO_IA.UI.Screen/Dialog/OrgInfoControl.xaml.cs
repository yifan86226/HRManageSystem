using AT_BC.Client.Extensions;
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

namespace CO_IA.UI.Screen.Dialog
{

    /// <summary>
    /// OrgInfo.xaml 的交互逻辑
    /// </summary>
    public partial class OrgInfoControl : EditableUserControl
    {
        SecurityClass[] securityclasses;

        private ActivityOrganization orgdatacontent;

        Activity activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;

        public ActivityOrganization ORGDataContent
        {
            get
            {
                return orgdatacontent;
            }
            set
            {
                orgdatacontent = value;
            }
        }
        public OrgInfoControl()
        {
            InitializeComponent();
            this.DataContext = this;
            InitSecurityClass();
        }
        private void InitSecurityClass()
        {
            securityclasses = CO_IA.Client.Utility.GetSecurityClasses();
            combClass.ItemsSource = securityclasses;
            if (securityclasses == null || securityclasses.Length == 0)
            {
                MessageBox.Show("请现在基础数据设置中增加保障级别");
            }
        }
    }
}
