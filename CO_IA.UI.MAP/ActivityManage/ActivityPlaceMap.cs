#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：活动地点绘制类，提供绘制方法，保存图形格式等
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using GS_MapBase;
using I_GS_MapBase.Portal;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AT_BC.Types;
namespace CO_IA.UI.MAP
{
    public class ActivityPlaceMap
    {
        /// <summary>
        /// 当前选中的地点ID
        /// </summary>
        private string _currentPlaceId = "";
        /// <summary>
        /// 当前选中的地点的ID,如果为空或者null则清除高亮的点 此操作只限ShowMap
        /// </summary>
        public string CurrentPlaceId
        {
            get
            {
                return _currentPlaceId;
            }
            set
            {
                _currentPlaceId = value;
                ShowMap.MapInitialized += ((obj) =>
                {
                    if(obj)
                        SelectedPlace();
                });

                if (ShowMap.initialized)
                {
                    SelectedPlace();
                }
            }
        }
        /// <summary>
        /// 存储所有地点的绘制信息 key为地点的ID，value为点集的字符串格式
        /// </summary>
        private Dictionary<string, ActivityPlaceInfo> _placeLocation = new Dictionary<string, ActivityPlaceInfo>();
        /// <summary>
        /// 存储地点的绘制信息 key为地点的ID，value为点集的字符串格式
        /// </summary>
        public Dictionary<string, ActivityPlaceInfo> PlaceLocation
        {
            get { return _placeLocation; }
            set
            {
                _placeLocation = value;
                if (_placeLocation != null)
                {
                    DrawPlace(ShowMap);                    
                }
            }
        }
        /// <summary>
        /// 地点显示用地图
        /// </summary>
        public MapGIS ShowMap;
        /// <summary>
        /// 地点编辑用地图
        /// </summary>
        private UMap MapEdit;
        public ActivityPlaceMap()
        {
            ShowMap = new MapGIS();
            ShowMap.MainMap.IsOverviewVisible = false;
            ShowMap.MainMap.IsZoomLineVisible = false;
            ShowMap.MapInitialized += ShowMap_Initialized; 
        }
        void ShowMap_Initialized(bool obj)
        {
            if (_currentPlaceId != "")
            {
                CurrentPlaceId = _currentPlaceId;
            }
        }


        /// <summary>
        /// 获取鼠标位置的地点ID
        /// </summary>
        /// <param name="p_point"></param>
        public string GetMouseLocationPlaceID(double x,double y)
        {
            if (_placeLocation == null || _placeLocation.Count == 0)
                return "";
            ClientUtile clientUtile = ClientUtile.Create();
            foreach (var itm in _placeLocation)
            {
                string placeid = itm.Key;
                var list = ShowMap.DrawList.Where(item => item.Key.StartsWith(placeid + "-")).ToList();
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ReturnDrawGraphicInfo info = list[i].Value as ReturnDrawGraphicInfo;
                        if (info != null)
                        {
                            if (x >= info.GraphicExtent.Xy1.X && x <= info.GraphicExtent.Xy2.X && y >= info.GraphicExtent.Xy2.Y && y <= info.GraphicExtent.Xy1.Y)
                            {
                                List<MapPointEx> points = null;

                                if (info.QueryObject is PolygonQueryBehaviorEventArgs)
                                {
                                    points = (info.QueryObject as PolygonQueryBehaviorEventArgs).Points;
                                }
                                if (info.QueryObject is CircleBehaviorEventArgs)
                                {
                                    points = (info.QueryObject as CircleBehaviorEventArgs).Points;
                                }
                                if (info.QueryObject is RectangleQueryBehaviorEventArgs)
                                {
                                    points = (info.QueryObject as RectangleQueryBehaviorEventArgs).Points;
                                }
                                if (points == null || points.Count == 0)
                                    continue;
                                List<Point> pList = new List<Point>();
                                points.ForEach(item=>pList.Add(new Point(item.X,item.Y)));
                                if (clientUtile.CalcPointPolygonRelation(new Point(x, y), pList.ToArray()))
                                {
                                    return placeid;
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }
        
        
        /// <summary>
        /// 绘制选择的位置图形
        /// </summary>
        private void SelectedPlace()
        {
            //if (!ShowMap.DrawList.Where(itm => itm.Key.StartsWith(MapGroupTypes.AreaRange_.ToString())).Any())
            //{
            //    if (ShowMap.DrawElementList.Count > 0)
            //    { 
                
            //    }
            //    return;
            //}
            var item = ShowMap.DrawList.Where(itm => itm.Key.StartsWith("selected_")).ToList();
            if (item.Count > 0)
            {
                item.ForEach(g => ShowMap.RemoveSymbolElement(g.Key));
            }

            if (!string.IsNullOrEmpty(_currentPlaceId))
            {
                if (_placeLocation.ContainsKey(_currentPlaceId))
                {
                    ActivityPlaceInfo placeInfo = _placeLocation[_currentPlaceId] as ActivityPlaceInfo;
                    if (placeInfo != null)
                    {
                        MapExtent extent = null;
                        if (placeInfo.Graphics != null)
                        {
                            ShowMap.DrawArea("selected_" + _currentPlaceId, placeInfo.Graphics);
                            extent = ShowMap.GetGraphicExtentByID(new string[] { MapGroupTypes.AreaRange_.ToString() + _currentPlaceId });
                           }
                        else
                        {
                            if (placeInfo.Locations != null && placeInfo.Locations.Length > 0)
                            {
                                List<string> ids = new List<string>();
                                foreach (var location in placeInfo.Locations)
                                {
                                    ids.Add(MapGroupTypes.Location_.ToString() + location.GUID);
                                }
                                extent = ShowMap.GetGraphicExtentByID(ids.ToArray());
                            }
                        }
                        if (extent != null && extent.Xy1.X != 0)
                            ShowMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = extent.Xy2.X, Latitude = extent.Xy2.Y }, true);
                        
                    }
                }
            }
        }

        #region 根据位置信息绘制
        /// <summary>
        /// 绘制地点
        /// </summary>
        /// <param name="mapgis"></param>
        private void DrawPlace(MapGIS mapgis = null)
        {
            if (_placeLocation != null&&_placeLocation.Count>0)
            {
                if (mapgis == null)
                    mapgis = ShowMap;

                mapgis.MapInitialized += ((b) =>
                {
                    if (!b)
                        return;
                    mapgis.RemoveSymbolElementByFlag(MapGroupTypes.AreaRange_.ToString());
                    mapgis.RemoveElementByFlag(MapGroupTypes.Location_.ToString());
                    foreach (var itm in _placeLocation)
                    {
                        ActivityPlaceInfo placeInfo = itm.Value as ActivityPlaceInfo;
                        if (placeInfo != null)
                        {
                            mapgis.DrawArea(MapGroupTypes.AreaRange_.ToString() + itm.Key, placeInfo.Graphics);
                            if (placeInfo.Locations != null && placeInfo.Locations.Length > 0)
                            {
                                mapgis.DrawLocation(placeInfo.Locations);
                            }
                        }
                    }
                    if (_currentPlaceId != "")
                    {
                        CurrentPlaceId = _currentPlaceId;
                    }
                });
                if (mapgis.initialized)
                {
                    mapgis.RemoveSymbolElementByFlag(MapGroupTypes.AreaRange_.ToString());
                    mapgis.RemoveElementByFlag(MapGroupTypes.Location_.ToString());
                    foreach (var itm in _placeLocation)
                    {
                        ActivityPlaceInfo placeInfo = itm.Value as ActivityPlaceInfo;
                        if (placeInfo != null)
                        {
                            mapgis.DrawArea(MapGroupTypes.AreaRange_.ToString() + itm.Key, placeInfo.Graphics);
                            if (placeInfo.Locations != null && placeInfo.Locations.Length > 0)
                            {
                                mapgis.DrawLocation(placeInfo.Locations);
                            }
                        }
                    }
                    if (_currentPlaceId != "")
                    {
                        CurrentPlaceId = _currentPlaceId;
                    }
                }
               
            }
        }
        
        
        #endregion        



        #region 编辑部分
        /// <summary>
        /// 调用绘制编辑界面
        /// </summary>
        public void ShowEditMap()
        { 
            if(string.IsNullOrEmpty(_currentPlaceId))
            {
                MessageBox.Show("当前没有选择的区域！");
                return;
            }
            MapEdit = new UMap(this);
            MapEdit.mapGis.MainMap.ServiceUrl = RiasPortal.Current.MapConfig.ElectricUrl;
            MapEdit.mapGis.ShowBar = true;
            MapEdit.mapGis.MapInitialized += ((b) => {
                if (_placeLocation.ContainsKey(_currentPlaceId))
                {
                    ActivityPlaceInfo placeInfo = _placeLocation[_currentPlaceId] as ActivityPlaceInfo;
                    if (placeInfo != null)
                    {
                        MapEdit.mapGis.DrawArea(_currentPlaceId, placeInfo.Graphics);
                    }
                    MapEdit.mapGis.SetAllGraphicsExtent();
                    MapEdit.mapGis.BarGraphicChange(true);
                } 
            });
            
            MapEdit.ShowDialog();

        }
        /// <summary>
        /// 编辑之后更新图形
        /// </summary>
        /// <param name="gString"></param>
        public void UpdateLocation(string gString)
        {
            if (gString == null)
                return;
            if (_placeLocation != null && _placeLocation.ContainsKey(_currentPlaceId))
            {
                ActivityPlaceInfo placeInfo = _placeLocation[_currentPlaceId] as ActivityPlaceInfo;
                if (placeInfo != null)
                {
                    placeInfo.Graphics = gString;
                }
            }         
            PlaceLocation = _placeLocation;
        }
        #endregion

    }
}
