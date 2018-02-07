using CO_IA.Client;
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

namespace CO_IA.UI.Screen.Control.Point
{
    /// <summary>
    /// PersonPoint.xaml 的交互逻辑
    /// </summary>
    public partial class PersonPoint : UserControl,IFrameworkElement
    {
        public PP_PersonInfo PersonInfo;
        public PP_OrgInfo OrgInfo;
        public PersonPoint(PP_OrgInfo orgInfo,PP_PersonInfo personInfo)
        {
            InitializeComponent();
            OrgInfo = orgInfo;
            PersonInfo = personInfo;
            if (personInfo != null)
            {
                ElementId = MapGroupTypes.Person_.ToString() + PersonInfo.GUID;
                txtOrgname.Text = OrgInfo.NAME;
                txtPersonname.Text = personInfo.NAME;
            }
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
