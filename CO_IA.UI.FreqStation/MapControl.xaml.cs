using CO_IA.Data;
using CO_IA.UI.MAP;
using CO_IA_Data;
using I_GS_MapBase.Portal.Types;
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

namespace CO_IA.UI.FreqStation
{
    /// <summary>
    /// MapControl.xaml 的交互逻辑
    /// </summary>
    public partial class MapControl : UserControl
    {

        public ActivityPlaceInfo CurrentPlaceInfo
        {
            get;
            set;
        }

        private ActivityPlaceMap activityMap = new ActivityPlaceMap();

        public MapControl()
        {
            InitializeComponent();
            this.Loaded += MapControl_Loaded;
        }

        private void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.borderMapContainer.Child = activityMap.ShowMap.MainMap;
            activityMap.ShowMap.MapInitialized += MapInitialized;
        }

        private void MapInitialized(bool obj)
        {

        }

        public void DrawStations(List<ActivitySurroundStation> stations)
        {            
            activityMap.ShowMap.MapInitialized += new Action<bool>((b) =>
            {
                DrawArea();
                DrawStation(stations);
            });
            if (activityMap.ShowMap.initialized)
            {
                DrawStation(stations);
            }
        }

        public void StationSelectionChanges(ActivitySurroundStation station)
        {
            activityMap.ShowMap.SelectStation(station);
        }

        public void DrawPlaceFreqPlanExtendRange(PlaceFreqPlan freqPlan)
        {
            this.activityMap.ShowMap.RemoveSymbolElement("freqPlan");
            if (freqPlan.RangePointList != null && freqPlan.RangePointList.Count > 1)
            {
                List<I_GS_MapBase.Portal.Types.MapPointEx> list = new List<I_GS_MapBase.Portal.Types.MapPointEx>(freqPlan.RangePointList.Count);
                foreach (var pt in freqPlan.RangePointList)
                {
                    list.Add(activityMap.ShowMap.MainMap.MapPointFactory.Create(pt.Longitude, pt.Latitude));
                }
                this.activityMap.ShowMap.DrawPolygon(list, "freqPlan");
                this.activityMap.ShowMap.SetAllGraphicsExtent();
            }
        }

        private void DrawArea()
        {
            Dictionary<string, ActivityPlaceInfo> dic = new Dictionary<string, ActivityPlaceInfo>();
            dic.Add(CurrentPlaceInfo.Guid, CurrentPlaceInfo);
            this.activityMap.PlaceLocation = dic;
            this.activityMap.ShowMap.SetAllGraphicsExtent();
        }

        private void DrawStation(List<ActivitySurroundStation> stations)
        {
            activityMap.ShowMap.ClearStation();
            activityMap.ShowMap.SetCluster(true);
            foreach (ActivitySurroundStation station in stations)
            {
                MapPointEx point = activityMap.ShowMap.MainMap.MapPointFactory.Create(station.STAT_LG,
                   station.STAT_LA);
                activityMap.ShowMap.DrawStationPoint(point,true, station);
            }
        }
    }
}
