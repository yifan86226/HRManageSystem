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
using CO_IA.Data.Collection;
using CO_IA_Data;

namespace CO_IA.MonitoringCollecting.Views
{
    /// <summary>
    /// StationBaseDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StationBaseDetailWindow : Window
    {
        public StationBaseDetailWindow(RoundStationInfo p_stationBase)
        {
            InitializeComponent();
            _activityNameTBlock.Text = SystemLoginService.CurrentActivity.Name;
            _activityAddressTBlock.Text = SystemLoginService.CurrentActivityPlace.Name;
            _statInfoGrid.DataContext = p_stationBase;
        }
    }
}
