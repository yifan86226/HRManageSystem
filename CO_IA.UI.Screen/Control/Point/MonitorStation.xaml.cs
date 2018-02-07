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
    /// MonitorStation.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorStation : UserControl, IFrameworkElement
    {
        public FixedStationInfo StationInfo;
        public MonitorStation(FixedStationInfo stationInfo)
        {
            InitializeComponent();
            StationInfo = stationInfo;
            if (StationInfo == null)
                return;
            txtGroupname.Text = StationInfo.Name;
            ElementId = MapGroupTypes.FixMonitorPoint_.ToString() + stationInfo.Guid;
           
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
