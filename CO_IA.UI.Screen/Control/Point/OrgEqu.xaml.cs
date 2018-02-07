using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using I_GS_MapBase.Portal;
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

namespace CO_IA.UI.Screen.Control
{
    /// <summary>
    /// OrgEqu.xaml 的交互逻辑
    /// </summary>
    public partial class OrgEqu : UserControl, IFrameworkElement
    {
        public ActivityOrganization Orginfo;
        public ActivityEquipment Equinfo;
        public OrgEqu(ActivityOrganization orginfo,ActivityEquipment equinfo)
        {
            InitializeComponent();
            if (orginfo == null||equinfo==null)
                return;
            Orginfo = orginfo;
            Equinfo = equinfo;
            txtOrgname.Text = orginfo.Name;
            txtEquname.Text = equinfo.Name;
            this.ToolTip = orginfo.Name;
            ElementId = MapGroupTypes.OrgEqu_.ToString() + orginfo.Guid;
        }
        public string ElementId
        {
            get;
            set;
        }
        public object ElementTag
        {
            get;
            set;
        }
       
    }
}
