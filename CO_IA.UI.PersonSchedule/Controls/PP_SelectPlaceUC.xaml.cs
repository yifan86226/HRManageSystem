using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>
    /// PP_SelectPlaceUC.xaml 的交互逻辑
    /// </summary>
    public partial class PP_SelectPlaceUC : UserControl
    {
        /// <summary>
        /// 组信息
        /// </summary>
        public PP_OrgInfo OrgInfo
        {
            get;
            set;
        }
 
        private MapPointEx MapPoint { get; set; }

        public PP_SelectPlaceUC()
        {
            InitializeComponent();
            InitMap();
            QueryAllLocation();
            ActivityMap.ShowMap.MainMap.Drop += MainMap_Drop;
        }

        private void InitMap()
        {
            xMapContainer.Child = ActivityMap.ShowMap.MainMap;
            ActivityPlaceInfo[] places = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            Dictionary<string, ActivityPlaceInfo> dicPlaces = new Dictionary<string, ActivityPlaceInfo>();
            foreach (ActivityPlaceInfo place in places)
            {
                dicPlaces.Add(place.Guid, place);
            }
            ActivityMap.PlaceLocation = dicPlaces;
        }

        private void QueryAllLocation()
        {
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            //{
            //    List<PP_OrgInfo> orgs = channel.GetMonitorPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid, false);
            //    if (ActivityMap.ShowMap.initialized)
            //    {
            //        if (orgs != null && orgs.Count > 0)
            //        {
            //            foreach (PP_OrgInfo org in orgs)
            //            {
            //                MapPlace place = new MapPlace();
            //                place.ORGinfo = org;
            //                place.PlaceGuid = "";
            //                MapPointEx point = ActivityMap.ShowMap.MainMap.MapPointFactory.Create(double.Parse(org.LONGITUDE), double.Parse(org.LATITUDE));
            //                place.MapPoint = point;
            //                place.BeforeDragPlaceEvent += OnBeforeDragPlace;
            //                place.DeletePlaceEvent += place_DeletePlaceEvent;
            //                DrawORGToMap(place, point);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        ActivityMap.ShowMap.MapInitialized += (initialized) =>
            //        {
            //            if (initialized)
            //            {
            //                if (orgs != null && orgs.Count > 0)
            //                {
            //                    foreach (PP_OrgInfo org in orgs)
            //                    {
            //                        MapPlace place = new MapPlace();
            //                        place.ORGinfo = org;
            //                        place.PlaceGuid = "";
            //                        MapPointEx point = ActivityMap.ShowMap.MainMap.MapPointFactory.Create(double.Parse(org.LONGITUDE), double.Parse(org.LATITUDE));
            //                        place.MapPoint = point;
            //                        place.BeforeDragPlaceEvent += OnBeforeDragPlace;
            //                        place.DeletePlaceEvent += place_DeletePlaceEvent;
            //                        DrawORGToMap(place, point);
            //                    }
            //                }
            //            }
            //        };
            //    }
            //});

        }

        private void MainMap_Drop(object sender, DragEventArgs e)
        {
            if (OrgInfo != null)
            {
                Point scpoint = e.GetPosition(this);
                MapPointEx currentmappoint = ActivityMap.ShowMap.MainMap.ScreenToMap(new PointEx(scpoint.X, scpoint.Y));
                string placeid = ActivityMap.GetMouseLocationPlaceID(currentmappoint.X, currentmappoint.Y);
                if (string.IsNullOrEmpty(placeid))
                {
                    MessageBoxResult msgresult = MessageBox.Show("当前地点不是活动区域,是否手动选择地点？", "提示", MessageBoxButton.YesNo);
                    if (msgresult == MessageBoxResult.Yes)
                    {
                        SelectPlaceDialog selectplace = new SelectPlaceDialog();
                        selectplace.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        selectplace.SelectedPlaceChangeEvent += (currentplaceId) =>
                        {
                            ActivityMap.CurrentPlaceId = currentplaceId;
                        };
                        selectplace.SelectedPlaceEvent += (placeguid) =>
                        {
                            placeid = placeguid;
                            ActivityMap.CurrentPlaceId = null;
                        };
                        selectplace.ShowDialog(this);
                    }
                }

                MapPlace place = ActivityMap.ShowMap.DrawElementList.FirstOrDefault(r => r.Key == OrgInfo.GUID).Value as MapPlace;
                if (place == null)
                {
                    place = new MapPlace();
                    place.BeforeDragPlaceEvent += OnBeforeDragPlace;
                    place.DeletePlaceEvent += place_DeletePlaceEvent;
                }
                place.ORGinfo = OrgInfo;
                place.PlaceGuid = placeid;
                place.MapPoint = currentmappoint;

                SaveORGPLocation(placeid, currentmappoint);
                DrawORGToMap(place, currentmappoint);
            }
        }

        private void OnBeforeDragPlace(PP_OrgInfo orginfo)
        {
            OrgInfo = orginfo;
        }

        private void DrawORGToMap(MapPlace place, MapPointEx mappoint)
        {
            ActivityMap.ShowMap.RemoveElement(place.ElementId);
            ActivityMap.ShowMap.AddElement(place, ActivityMap.ShowMap.GetMapPointEx(mappoint.X, mappoint.Y) );
        }

        private void place_DeletePlaceEvent(PP_OrgInfo obj)
        {
            MessageBoxResult result = MessageBox.Show(string.Format("确认要在地图上移除{0}?", obj.NAME), "提示", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                ActivityMap.ShowMap.RemoveElement(obj.GUID);

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    channel.SavePP_OrgInfoLocation(obj.GUID, null, null);
                });
            }

            ActivityMap.ShowMap.MainMap.PanEnabled = false;
        }

        private void SaveORGPLocation(string placeid, MapPointEx mappoint)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                channel.SavePP_OrgInfoLocation(OrgInfo.GUID, mappoint.X.ToString(), mappoint.Y.ToString());
            });
        }

        public bool ExistMapLocation(string orgguid)
        {
            MapPlace place = ActivityMap.ShowMap.DrawElementList.FirstOrDefault(r => r.Key == orgguid).Value as MapPlace;
            if (place != null)
            {
                ActivityMap.ShowMap.MainMap.Location(place.MapPoint);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SelectionLocation(string orgguid)
        {
            if (!string.IsNullOrEmpty(orgguid))
            {
                foreach (KeyValuePair<string, object> item in ActivityMap.ShowMap.DrawElementList)
                {
                    MapPlace place = item.Value as MapPlace;
                    if (place != null)
                    {
                        if (place.ElementId == orgguid)
                        {
                            place.IsChecked = true;
                            ActivityMap.ShowMap.MainMap.Location(place.MapPoint);
                        }
                        else
                        {
                            place.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void OnAfterDragPlace()
        {
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1000);
                ActivityMap.ShowMap.MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ActivityMap.ShowMap.MainMap.PanEnabled = true;
                }));
            })) { IsBackground = true }.Start();
        }
    }
}
