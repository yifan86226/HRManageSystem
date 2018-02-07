using CO_IA.Client;
using CO_IA.Data;
using CO_IA_Data;
using I_CO_IA.FreqStation;
using I_GS_MapBase.Portal.Types;
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

namespace CO_IA.UI.Screen.Dialog
{
    /// <summary>
    /// SurroundStationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SurroundStationDialog : Window
    {
        string AreaID;
        private ObservableCollection<CO_IA_Data.ActivitySurroundStation> activitystationitemssource = new ObservableCollection<ActivitySurroundStation>();

        public ObservableCollection<ActivitySurroundStation> ActivityStationItemsSource
        {
            get
            {
                return activitystationitemssource;
            }
            set
            {
                activitystationitemssource = value;
            }
        }
        public SurroundStationDialog(string areaID)
        {
            InitializeComponent();
            AreaID = areaID;
            QuerySurroundStations();
            surroundStationListControl.StationSelectionChanged += surroundStationListControl_StationSelectionChanged;
        }
        private void QuerySurroundStations()
        {
            List<ActivitySurroundStation> stations = QuerySurroundStationFromDB(null);
            ActivityStationItemsSource = new ObservableCollection<CO_IA_Data.ActivitySurroundStation>(stations);
            //foreach (ActivitySurroundStation item in stations)
            //{
            //    ActivityStationItemsSource.Add(item);
            //}

            SetStationListItemsSource();
        }
        private List<ActivitySurroundStation> QuerySurroundStationFromDB(List<string> freqs)
        {
            ActivityStationItemsSource.Clear();

            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<ActivitySurroundStation>>(channel =>
            {
                return channel.GetActivitySurroundStations(Obj.Activity.Guid, AreaID, freqs);
            });
        }

        private void SetStationListItemsSource()
        {
            surroundStationListControl.DataContext = ActivityStationItemsSource;
            DrawStations(ActivityStationItemsSource.ToList());
        }

        private void surroundStationListControl_StationSelectionChanged(ActivitySurroundStation obj)
        {
            Obj.screenMap.SelectStation(obj);
        }
        public void DrawStations(List<ActivitySurroundStation> stations)
        {
            if (Obj.screenMap.initialized)
            {
                DrawStation(stations);
            }
            else
            {
                Obj.screenMap.MapInitialized += new Action<bool>((b) =>
                {
                    DrawStation(stations);
                });
            }
        }
        private void DrawStation(List<ActivitySurroundStation> stations)
        {
            Obj.screenMap.ClearStation();
            Obj.screenMap.SetCluster(true);
            foreach (ActivitySurroundStation station in stations)
            {
                MapPointEx point = Obj.screenMap.MainMap.MapPointFactory.Create(station.STAT_LG, station.STAT_LA);
                Obj.screenMap.DrawStationPoint(point,true, station);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Obj.screenMap.SelectStationLayer.ClearSymbolElements();
            Obj.screenMap.ClearStation();
        }
    }
}
