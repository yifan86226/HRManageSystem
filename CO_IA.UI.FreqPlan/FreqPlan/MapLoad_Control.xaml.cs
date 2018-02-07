using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.MAP;
using CO_IA_Data;
using I_GS_MapBase.Portal;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// MapLoad_Control.xaml 的交互逻辑
    /// </summary>
    public partial class MapLoad_Control : UserControl
    {
        /// <summary>
        /// 台站
        /// </summary>
        private const string stationGroup = "station_";
        private const string freqPlanGroup = "freqPlan_";
        /// <summary>
        /// 地点显示用地图
        /// </summary>
        private ActivityPlaceInfo _activityPlaceInfo = null;
        private MapGIS ShowMap = new MapGIS();

        public MapLoad_Control()
        {
            InitializeComponent();
        }

        public MapLoad_Control(UserControl pSubControl)
        {
            InitializeComponent();
            this.xControlContainer.Child = pSubControl;
        }
        public UserControl UpSubControl
        {
            get { return (UserControl)this.xControlContainer.Child; }
            set
            {
                if (this.xControlContainer.Child != value)
                {
                    RemoveFreqAndStatInfo();
                    this.xControlContainer.Child = value;
                    if (value is FreqPartPlan_Control)
                    {
                        ((FreqPartPlan_Control)this.xControlContainer.Child).xfreqPartPlanList.OnAreaSelect += xfreqPartPlanList_OnAreaSelect;
                        ((FreqPartPlan_Control)this.xControlContainer.Child).xfreqPartPlanList.OnShowAroundArea += xfreqPartPlanList_OnShowAroundArea;
                        ((FreqPartPlan_Control)this.xControlContainer.Child).OnInitAreaSelect += MapLoad_Control_OnInitAreaSelect;
                        ((FreqPartPlan_Control)this.xControlContainer.Child).ActivityPlaceId = _activityPlaceInfo.Guid;
                    }
                    else if (value is RoundStatAnalyse_Control)
                    {
                        ((RoundStatAnalyse_Control)this.xControlContainer.Child).OnDrawStationToMap += DrawStationToMap;
                        ((RoundStatAnalyse_Control)this.xControlContainer.Child).xStationListControl.OnShowAroundArea += xfreqPartPlanList_OnShowAroundArea;
                        ((RoundStatAnalyse_Control)this.xControlContainer.Child).ActivityPlaceId = _activityPlaceInfo.Guid;
                    }
                }
            }
        }

        /// <summary>
        /// 处理为选择周围台站区域的，赋予默认区域
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void MapLoad_Control_OnInitAreaSelect(FreqPlanActivity arg1, Action arg2)
        {
            if (arg1.Points != null && arg1.Points.Length > 0)
            {
                return;
            }
            CreateAroundStationArea(freqPlanGroup+arg1.Guid, arg1.Distance, (calls, call1, call2) =>
            {
                if (calls == null || calls.Length == 0)//xgh
                {
                    if (arg2 != null)
                        arg2();
                    return;
                }
                arg1.Points = new CustomPoint[calls.Length];
                for (int i = 0; i < calls.Length; i++)
                {
                    arg1.Points[i] = new CustomPoint();
                    arg1.Points[i].X = calls[i].X;
                    arg1.Points[i].Y = calls[i].Y;
                }
                arg1.StartPoint.X = call1.X;
                arg1.StartPoint.Y = call1.Y;
                arg1.EndPoint.X = call2.X;
                arg1.EndPoint.Y = call2.Y;
                if (_activityPlaceInfo != null)
                    arg1.PlaceId = _activityPlaceInfo.Guid;
                if (arg2 != null)
                    arg2();

            });
        }
        /// <summary>
        /// 设定单个频率规划周围台站区域
        /// </summary>
        /// <param name="obj"></param>

        void xfreqPartPlanList_OnAreaSelect(FreqPlanActivity obj)
        {
            busyIndicator.IsBusy = true;
            ShowMap.RemoveSymbolElement(freqPlanGroup+obj.Guid);

            CreateAroundStationArea(freqPlanGroup+obj.Guid, obj.Distance, (calls, call1, call2) =>
            {
                busyIndicator.IsBusy = false;
                if (calls != null && calls.Length > 0)//xgh
                {
                    obj.Points = new CustomPoint[calls.Length];
                    for (int i = 0; i < calls.Length; i++)
                    {
                        obj.Points[i] = new CustomPoint();
                        obj.Points[i].X = calls[i].X;
                        obj.Points[i].Y = calls[i].Y;
                    }
                    obj.StartPoint.X = call1.X;
                    obj.StartPoint.Y = call1.Y;
                    obj.EndPoint.X = call2.X;
                    obj.EndPoint.Y = call2.Y;
                    if (_activityPlaceInfo != null)
                        obj.PlaceId = _activityPlaceInfo.Guid;
                }
            });
        }
        /// <summary>
        /// 点击显示周围台站区域
        /// </summary>
        /// <param name="obj"></param>

        void xfreqPartPlanList_OnShowAroundArea(FreqPlanActivity obj)
        {
            var clearObj = ShowMap.DrawList.Where(itm => itm.Key.StartsWith(freqPlanGroup)).Select(p => p.Key).ToList();
            if (clearObj != null && clearObj.Count > 0)
                for (int i = 0; i < clearObj.Count; i++)
                    ShowMap.RemoveSymbolElement(clearObj[i]);
            if (obj.Points != null && obj.Points.Length > 0)
            {
                List<MapPointEx> ps = new List<MapPointEx>();
                for (int i = 0; i < obj.Points.Length; i++)
                {
                    ps.Add(ShowMap.MainMap.MapPointFactory.Create(obj.Points[i].X, obj.Points[i].Y));
                }
                ShowMap.DrawPolygon(ps, freqPlanGroup + obj.Guid, null, null);
            }
        }
        public void ClearSubControl()
        {
            this.xControlContainer.Child = null;
        }

        void MainMap_Initialized(bool obj)
        {
            DrawPlaceMap();
        }
        public ActivityPlaceInfo LoadActivityPlace 
        {
            set
            {
                _activityPlaceInfo = value;
                ShowMap.MainMap.IsOverviewVisible = false;
                ShowMap.MainMap.IsZoomLineVisible = false;
                if (xMapContainer.Child != null)
                {
                    DrawPlaceMap();
                }
                ShowMap.MainMap.Initialized += MainMap_Initialized;
                this.xMapContainer.Child = ShowMap.MainMap;
            }
        }
        private void RemoveFreqAndStatInfo()
        {
            var obj = ShowMap.DrawList.Where(p => p.Key.StartsWith(freqPlanGroup)).Select(p=>p.Key).ToList();
            if (obj != null && obj.Count >0)
                for (int i = 0;i<obj.Count;i++)
                    ShowMap.RemoveSymbolElement(obj[i]);
            obj = ShowMap.DrawList.Where(p => p.Key.StartsWith(stationGroup)).Select(p => p.Key).ToList();
            if (obj != null && obj.Count > 0)
                for (int i = 0; i < obj.Count; i++)
                    ShowMap.RemoveSymbolElement(obj[i]);
        }
        private void DrawPlaceMap()
        {
            if (_activityPlaceInfo != null && !string.IsNullOrEmpty(_activityPlaceInfo.Graphics))
            {
                ShowMap.RemoveSymbolElement();
                //DrawMap(_activityPlaceInfo, ShowMap);

                DrawPlaceLocation(_activityPlaceInfo.Guid, _activityPlaceInfo.Graphics, ShowMap);
                ShowMap.SetAllGraphicsExtent();
            }
            else
                ShowMap.RemoveSymbolElement();
        }

        #region 根据字符串获取图形结构
        /// <summary>
        /// 根据字符串获取图形类信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static GraphicInfo getDrawInfo(string info, MapGIS mapgis)
        {
            if (mapgis == null)
                return null;
            info = info.Replace(" ", "").Replace("\n", "");
            string[] gInfoArr = null;
            string[] gInfo = info.Split(':');
            if (gInfo.Length == 2)
            {
                switch (gInfo[0])
                {
                    case "polyline":
                        PolylineInfo lineInfo = new PolylineInfo();
                        gInfoArr = gInfo[1].Split('|');
                        if (gInfoArr.Length == 2)
                        {
                            //样式
                            string[] gStyle = gInfoArr[0].Split(',');
                            if (gStyle.Length == 2)
                            {
                                lineInfo.BorderColor = gStyle[0];
                                double.TryParse(gStyle[1], out lineInfo.BorderWidth);
                            }
                            //点
                            string[] gPoints = gInfoArr[1].Split('&');
                            if (gPoints.Length > 0)
                            {
                                lineInfo.Points = new List<I_GS_MapBase.Portal.Types.MapPointEx>();
                                for (int i = 0; i < gPoints.Length; i++)
                                {
                                    string[] p = gPoints[i].Split(',');
                                    lineInfo.Points.Add(mapgis.MainMap.MapPointFactory.Create(p[0].TryToDoubleZero(), p[1].TryToDoubleZero()));
                                }
                            }
                        }
                        return lineInfo;
                    case "polygon":
                        PolygonInfo gonInfo = new PolygonInfo();
                        gInfoArr = gInfo[1].Split('|');
                        if (gInfoArr.Length == 2)
                        {
                            //样式
                            string[] gStyle = gInfoArr[0].Split(',');
                            if (gStyle.Length == 3)
                            {
                                gonInfo.BorderColor = gStyle[0];
                                double.TryParse(gStyle[1], out gonInfo.BorderWidth);
                                gonInfo.FillColor = gStyle[2];
                            }
                            //点
                            string[] gPoints = gInfoArr[1].Split('&');
                            if (gPoints.Length > 0)
                            {
                                gonInfo.Points = new List<I_GS_MapBase.Portal.Types.MapPointEx>();
                                for (int i = 0; i < gPoints.Length; i++)
                                {
                                    string[] p = gPoints[i].Split(',');
                                    gonInfo.Points.Add(mapgis.MainMap.MapPointFactory.Create(p[0].TryToDoubleZero(), p[1].TryToDoubleZero()));
                                }
                            }
                        }
                        return gonInfo;
                    case "circle":
                        CircleInfo circleInfo = new CircleInfo();
                        gInfoArr = gInfo[1].Split('|');
                        if (gInfoArr.Length == 2)
                        {
                            //样式
                            string[] gStyle = gInfoArr[0].Split(',');
                            if (gStyle.Length == 3)
                            {
                                circleInfo.BorderColor = gStyle[0];
                                double.TryParse(gStyle[1], out circleInfo.BorderWidth);
                                circleInfo.FillColor = gStyle[2];
                            }
                            //中心点 半径
                            string[] gPoints = gInfoArr[1].Split('&');
                            if (gPoints.Length == 2)
                            {
                                string[] p = gPoints[0].Split(',');
                                circleInfo.CenterPoint = mapgis.MainMap.MapPointFactory.Create(p[0].TryToDoubleZero(), p[1].TryToDoubleZero());
                                circleInfo.Radius = gPoints[1].TryToDoubleZero();
                            }
                        }
                        return circleInfo;
                    case "rectangle":
                        RectangleInfo rectangleInfo = new RectangleInfo();
                        gInfoArr = gInfo[1].Split('|');
                        if (gInfoArr.Length == 2)
                        {
                            //样式
                            string[] gStyle = gInfoArr[0].Split(',');
                            if (gStyle.Length == 3)
                            {
                                rectangleInfo.BorderColor = gStyle[0];
                                double.TryParse(gStyle[1], out rectangleInfo.BorderWidth);
                                rectangleInfo.FillColor = gStyle[2];
                            }

                            string[] gPoints = gInfoArr[1].Split('&');
                            if (gPoints.Length == 3)
                            {
                                string[] p = gPoints[0].Split(',');
                                rectangleInfo.LeftTopPoint = mapgis.MainMap.MapPointFactory.Create(p[0].TryToDoubleZero(), p[1].TryToDoubleZero());
                                rectangleInfo.RectWidth = gPoints[1].TryToDoubleZero();
                                rectangleInfo.RectHeight = gPoints[2].TryToDoubleZero();
                            }
                            if (gPoints.Length == 2)
                            {
                                string[] p = gPoints[0].Split(',');
                                rectangleInfo.LeftTopPoint = mapgis.MainMap.MapPointFactory.Create(p[0].TryToDoubleZero(), p[1].TryToDoubleZero());
                                string[] p2 = gPoints[1].Split(',');
                                rectangleInfo.RightBottomPoint = mapgis.MainMap.MapPointFactory.Create(p2[0].TryToDoubleZero(), p2[1].TryToDoubleZero());
                            }
                        }
                        return rectangleInfo;
                }
            }
            return null;
        }
        #endregion

        int NameId = 0;
        /// <summary>
        /// 解析字符串并绘制
        /// </summary>
        /// <param name="GraphicId">id</param>
        /// <param name="GraphicInfo">图形信息 格式：polyline:blue,2|127,1,42.2&126.8&41.9</param>
        /// <param name="mapControl">需要绘制的地图对象，为空则往ShowMap上绘制</param>
        private void DrawPlaceLocation(string GraphicId, string GraphicInfo, MapGIS mapgis = null)
        {
            if (string.IsNullOrEmpty(GraphicId) || string.IsNullOrEmpty(GraphicInfo))
                return;
            //各形状用*分隔
            //polyline:blue,2|127,1,42.2&126.8,41.9
            //polygon:blue,2,red|127,1,42.2&126.8,41.9
            //circle:blue,2,red|127,1,42.2&2000
            //rectangle:blue,2,red|127,1,42.2&127,1,42.2
            //rectangle:blue,2,red|127,1,42.2&2000&1000
            if (mapgis == null)
                mapgis = ShowMap;
          
            string[] info = GraphicInfo.Split('*');
            for (int i = 0; i < info.Length; i++)
            {
                NameId++;
                GraphicInfo ginfo = getDrawInfo(info[i], mapgis);
                if (ginfo is PolylineInfo)
                {
                    PolylineInfo g = ginfo as PolylineInfo;
                    if (g == null)
                        continue;
                    mapgis.DrawPolyline(g.Points, GraphicId + "-" + NameId.ToString(), new List<KeyValuePair<string, object>> { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                        }, null);
                }
                if (ginfo is PolygonInfo)
                {
                    PolygonInfo g = ginfo as PolygonInfo;
                    if (g == null)
                        continue;
                    mapgis.DrawPolygon(g.Points, GraphicId + "-" + NameId.ToString(), new List<KeyValuePair<string, object>> { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                            new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                        }, null);
                }
                if (ginfo is CircleInfo)
                {
                    CircleInfo g = ginfo as CircleInfo;
                    if (g == null)
                        continue;
                    mapgis.DrawCircle(g.CenterPoint, g.Radius, GraphicId + "-" + NameId.ToString(), new List<KeyValuePair<string, object>> { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                            new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                    }, null);
                }
                if (ginfo is RectangleInfo)
                {
                    RectangleInfo g = ginfo as RectangleInfo;
                    if (g == null)
                        continue;
                    if (g.RightBottomPoint != null)
                    {
                        mapgis.DrawRectangle(g.LeftTopPoint, g.RightBottomPoint, GraphicId + "-" + NameId.ToString(), new List<KeyValuePair<string, object>> { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                            new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                    }, null);
                    }
                    else
                    mapgis.DrawRectangle(g.LeftTopPoint, g.RectWidth, g.RectHeight, GraphicId + "-" + NameId.ToString(), new List<KeyValuePair<string, object>> { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                            new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                    }, null);
                }
            }
        }
        /// <summary>
        /// 绘制地图
        /// </summary>
        /// <param name="place">地点信息</param>
        /// <param name="mapgis">地图</param>
        private void DrawMap(ActivityPlaceInfo place, MapGIS mapgis = null)
        {
            GraphicInfo ginfo = getDrawInfo(place.Graphics, mapgis);
            if (ginfo is PolylineInfo)
            {
                PolylineInfo g = ginfo as PolylineInfo;
                if (g == null)
                    return;
                mapgis.DrawPolyline(g.Points, place.Guid + "-0", new List<KeyValuePair<string, object>> { 
                        new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                        new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                    }, null);
            }
            if (ginfo is PolygonInfo)
            {
                PolygonInfo g = ginfo as PolygonInfo;
                if (g == null)
                    return;
                mapgis.DrawPolygon(g.Points, place.Guid + "-0", new List<KeyValuePair<string, object>> { 
                        new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                        new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                        new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                    }, null);
            }
            if (ginfo is CircleInfo)
            {
                CircleInfo g = ginfo as CircleInfo;
                if (g == null)
                    return;
                mapgis.DrawCircle(g.CenterPoint, g.Radius, place.Guid + "-0" + "", new List<KeyValuePair<string, object>> { 
                        new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                        new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                        new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                }, null);
            }
            if (ginfo is RectangleInfo)
            {
                RectangleInfo g = ginfo as RectangleInfo;
                if (g == null)
                    return;
                if (g.RightBottomPoint != null)
                {
                    mapgis.DrawRectangle(g.LeftTopPoint, g.RightBottomPoint, place.Guid + "-0", new List<KeyValuePair<string, object>> { 
                        new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                        new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                        new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                }, null);
                }
                else

                mapgis.DrawRectangle(g.LeftTopPoint, g.RectWidth, g.RectHeight, place.Guid + "-0", new List<KeyValuePair<string, object>> { 
                        new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                        new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                        new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                }, null);
            }
        }

        /// <summary>
        /// 获取扩展地点信息
        /// </summary>
        /// <param name="pPoints"></param>
        /// <param name="pQBehavior"></param>
        /// <param name="Distance"></param>
        /// <param name="action"></param>
        private void GetAroundStationArea(List<MapPointEx> pPoints, QueryBehavior pQBehavior, double Distance, Action<Point[], Point, Point> action)
        {
            int Re = ShowMap.MainMap.GeometryBuffer(pPoints, pQBehavior, Distance, new Action<List<MapPointEx>>((points) =>
            {
                if (points != null && points.Count > 0)
                {
                    Point[] list = new Point[points.Count - 1];
                    double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (x1 == 0)
                        {
                            x1 = x2 = points[i].X;
                            y1 = y2 = points[i].Y;
                        }
                        x1 = x1 < points[i].X ? x1 : points[i].X;
                        x2 = x2 < points[i].X ? points[i].X : x2;
                        y1 = y1 < points[i].Y ? points[i].Y : y1;
                        y2 = y2 < points[i].Y ? y2 : points[i].Y;
                        if (i == points.Count - 1)
                            break;
                        list[i] = new Point(points[i].X, points[i].Y);
                    }
                    if (action != null)
                    {
                        action(list, new Point(x1, y1), new Point(x2, y2));
                    }
                }
                else//xgh
                {
                    if (action != null)
                    {
                        action(null, new Point(0, 0), new Point(0, 0));
                    }
                }
            }), RiasPortal.GetMapGeometryServerUrl());  ////获得几何服务地址
        }
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            //CreateAroundStationArea(500, null);
        }

        private void CreateAroundStationArea(string pGuid,double pDistance, Action<Point[], Point, Point> pAction)
        {
            //获得几何服务地址
            //GeometryServerUrl = RiasPortal.GetMapGeometryServerUrl();

            var obj = ShowMap.DrawList.Where(itm => itm.Key.StartsWith(_activityPlaceInfo.Guid + "-")).ToList();
            if (obj == null || obj.Count == 0) return;
            for (int i = 0; i < obj.Count; i++)//xgh  地点会存在多个图形的情况
            {
                ReturnDrawGraphicInfo info = obj[i].Value as ReturnDrawGraphicInfo;
                //info.GraphicType  类型
                List<MapPointEx> points = null;

                if (info.QueryObject is PolygonQueryBehaviorEventArgs)
                {
                    points = (info.QueryObject as PolygonQueryBehaviorEventArgs).Points;
                    //polygon.Points
                    //  polygon.QueryBehavior  类型
                }
                if (info.QueryObject is CircleBehaviorEventArgs)
                {
                    points = (info.QueryObject as CircleBehaviorEventArgs).Points;
                }
                if (info.QueryObject is RectangleQueryBehaviorEventArgs)
                {
                    points = (info.QueryObject as RectangleQueryBehaviorEventArgs).Points;
                }
                if (points != null)
                {
                    GetAroundStationArea(points, info.QueryObject.QueryBehavior, pDistance, (pCall, p1, p2) =>
                        {
                            if (pAction != null)
                                pAction(pCall, p1, p2);
                            if (pCall != null && pCall.Length > 0) //xgh
                                DrawAroundStationArea(pGuid, pCall);
                        });
                }
            }
        }

        private void DrawAroundStationArea(string pGuid, Point[] pPoints)
        {
            List<MapPointEx> ps = new List<MapPointEx>();
            for (int i = 0; i < pPoints.Length; i++)
            {
                ps.Add(ShowMap.MainMap.MapPointFactory.Create(pPoints[i].X, pPoints[i].Y));
            }
            ShowMap.DrawPolygon(ps, pGuid, null, null);
        }

        #region 台站在地图上面绘制
        private void DrawStationToMap(List<StationInfo> stations)
        {
            if (stations == null)   
                return;
            for (int i = 0; i < stations.Count; i++)
            {
                StationInfo info = stations[i];
                ShowMap.RemoveSymbolElement(stationGroup + info.STATGUID);
                //if (CheckCoordinateByRange(info.STAT_LG, info.STAT_LA))
                //{
                ShowMap.DrawPoint(ShowMap.GetMapPointEx(info.STAT_LG, info.STAT_LA), new SymbolElement(stationGroup + info.STATGUID)
                    {
                       DataSources = new List<KeyValuePair<string, object>> { 
                        new KeyValuePair<string,object>(GraphicStyle.ImageSource.ToString(),"/CO_IA.UI.Display;component/Images/station_blue/gbds.png"),
                        new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),"5"),
                        new KeyValuePair<string, object>(GraphicStyle.ToolTipText.ToString(),info.STAT_NAME),
                        new KeyValuePair<string, object>("data",info)
                       }
                });
                //}
            }
        }
        /// <summary>
        /// 校验经纬度是否在地图范围内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CheckCoordinateByRange(double x, double y)
        {
            if (CheckCoordinate(x, y))
            {
                if (ShowMap.MapRange.Little.Longitude == 0 || ShowMap.MapRange.Little.Latitude == 0 || ShowMap.MapRange.Great.Latitude == 0 || ShowMap.MapRange.Great.Longitude == 0)
                    return false;
                if (x >= ShowMap.MapRange.Little.Longitude && x <= ShowMap.MapRange.Great.Longitude && y >= ShowMap.MapRange.Great.Latitude && y <= ShowMap.MapRange.Little.Latitude)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 校验经纬度是否有效
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CheckCoordinate(double x, double y)
        {
            if (x == 0 || y == 0)
                return false;
            if (x > 180 || x < 0)
                return false;
            if (y > 90 || y < 0)
                return false;
            return true;
        }
        #endregion
    }
}
