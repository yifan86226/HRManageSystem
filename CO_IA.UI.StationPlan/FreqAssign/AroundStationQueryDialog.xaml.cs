using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using CO_IA.UI.StationManage;
using CO_IA_Data;
using I_CO_IA.FreqPlan;
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

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// AroundStationQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AroundStationQueryDialog : Window
    {
        private ActivityPlaceMap ActivityMap = new ActivityPlaceMap();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stations"></param>
        /// <param name="isLoadMap">add by wrx 161118</param>
        public AroundStationQueryDialog( List<RoundStationInfo> stations ,bool isLoadMap = true)
        {
            InitializeComponent();
            if (isLoadMap)
            {
                InitMap();
            }
            stationdatagrid.ItemsSource = stations;
            DrawRoundStationsToMap(stations);
        }

        private void InitMap()
        {
            ActivityPlaceInfo[] places = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            Dictionary<string, ActivityPlaceInfo> dicPlaces = new Dictionary<string, ActivityPlaceInfo>();
            foreach (ActivityPlaceInfo place in places)
            {
                dicPlaces.Add(place.Guid, place);
            }
            ActivityMap.PlaceLocation = dicPlaces;
            this.xMapContainer.Child = (ActivityMap.ShowMap.MainMap);
        }
        
        private void Stationdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoundStationInfo selectstation = this.stationdatagrid.SelectedItem as RoundStationInfo;
            StationDetailDialog dialog = new StationDetailDialog(selectstation.STATGUID);
            dialog.ShowDialog(this);
        }


        private void DrawRoundStationsToMap(List<RoundStationInfo> stations)
        {
            if (stations != null && stations.Count > 0)
            {
                //如果地图未初始化完成,在初始化完成的回调方法里绘制周围台站
                //这种情况只有在初始化地图未完成时发生
                if (!ActivityMap.ShowMap.initialized)
                {
                    ActivityMap.ShowMap.MapInitialized += (initialized) =>
                    {
                        if (initialized)
                        {
                            foreach (RoundStationInfo station in stations)
                            {
                                DrawStationToMap(station);
                            }
                        }
                    };

                }
                else
                {
                    foreach (RoundStationInfo station in stations)
                    {
                        DrawStationToMap(station);
                    }
                }
            }
        }

        private void DrawStationToMap(RoundStationInfo station)
        {
            string imgUrl = "/CO_IA.UI.StationPlan;component/Images/station.png";
            string id = station.STATGUID;
            ActivityMap.ShowMap.RemoveSymbolElement(id);
            //重写
            //ActivityMap.ShowMap.DrawPoint(station.STAT_LG, station.STAT_LA, imgUrl, id, new List<KeyValuePair<string, object>> { 
            //    new KeyValuePair<string,object>(GraphicStyle.ImageSource.ToString(),imgUrl),
            //    new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),"6"),
            //    new KeyValuePair<string, object>(GraphicStyle.ToolTipText.ToString(),station.STAT_NAME),
            //    new KeyValuePair<string, object>("data",station)
            //});
        }
    }
}
