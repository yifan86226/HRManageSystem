using CO_IA.Data;
 
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

namespace CO_IA.Client.Orgs
{
    /// <summary>
    /// MonitorGroup.xaml 的交互逻辑
    /// </summary>
    public partial class OrgToMapStyle : UserControl, DevExpress.Xpf.Core.IFrameworkElement
    {
        public PP_OrgInfo OrgInfo = new PP_OrgInfo();
        public OrgToMapStyle(PP_OrgInfo orgInfo)
        {
            InitializeComponent();
            OrgInfo = orgInfo;
            checkBorder.DataContext = OrgInfo;
            if (orgInfo == null)
                return;
            //txtGroupname.Text = orgInfo.NAME;
            ElementId = MapGroupTypes.MonitorGroup_.ToString()+ orgInfo.GUID;
            
        }
        //private ClientInfo clientinfo;
        //public ClientInfo clientInfo
        //{
        //    get { return clientinfo; }
        //    set
        //    {
        //        clientinfo = value;
        //        //if (clientinfo != null&&!string.IsNullOrEmpty(clientinfo.ActivityPlaceGuid)&&!string.IsNullOrEmpty(clientinfo.ClientAddress))
        //        //{
        //        //    SetContextMenu(true);
        //        //}
        //        //else
        //        //{
        //        //    SetContextMenu(false);
        //        //}
        //    }
        //}
        
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
