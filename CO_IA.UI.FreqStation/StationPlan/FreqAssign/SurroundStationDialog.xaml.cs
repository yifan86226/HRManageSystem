using CO_IA.Data;
using CO_IA_Data;
using I_CO_IA.FreqStation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// SurroundStationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SurroundStationDialog : Window
    {
        private ActivityPlaceInfo CurrentActivityPlace;
        public SurroundStationDialog(ActivityPlaceInfo pActivityPlace)
        {
            InitializeComponent();
            CurrentActivityPlace = pActivityPlace;
            mapcontrol.CurrentPlaceInfo = CurrentActivityPlace;
            surroundStationListControl.StationSelectionChanged += surroundStationListControl_StationSelectionChanged;
            QuerySurroundStationFromDB();
        }

        private void QuerySurroundStationFromDB()
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                List<ActivitySurroundStation> stations = channel.GetActivitySurroundStations(this.CurrentActivityPlace.ActivityGuid, this.CurrentActivityPlace.Guid, null);

                ObservableCollection<ActivitySurroundStation> stationsources = new ObservableCollection<ActivitySurroundStation>(stations);
                surroundStationListControl.DataContext = stationsources;
                mapcontrol.DrawStations(stations);
            });
        }


        private void surroundStationListControl_StationSelectionChanged(ActivitySurroundStation obj)
        {
            mapcontrol.StationSelectionChanges(obj);
        }
    }
}
