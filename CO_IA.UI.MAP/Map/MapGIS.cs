#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：对地图基本操作进行封装，对绘制的对象进行管理，获取地图地址及范围
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using AT_BC.Common;
using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.Map.Control;
using GS_MapBase;
using I_GS_MapBase.Portal;
using I_GS_MapBase.Portal.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.MAP
{
    public class MapGIS 
    {
        
        #region 绘制工具
        /// <summary>
        /// bar中是否进行了操作
        /// </summary>
        public bool Modify = false;
        /// <summary>
        /// 绘制工具栏
        /// </summary>
        private Bar DrawBar = null;
        /// <summary>
        /// 绘制图形change
        /// </summary>
        public Action<bool> BarGraphicChange;
        private bool _showBar = false;
        /// <summary>
        /// 是否显示绘制工具栏
        /// </summary>
        public bool ShowBar
        {
            get { return _showBar; }
            set
            {
                _showBar = value;
                if (initialized && _showBar)
                {
                    AddDrawBar();
                    _showBar = false;
                }
            }
        }
        #endregion

        #region 主地图控件
        private MapControl _MainMap;
        /// <summary>
        /// 地图对象
        /// </summary>
        public MapControl MainMap
        {

            get { return _MainMap; }
        }
        #endregion


        #region 地图初始化

        /// <summary>
        /// 地图是否初始化完成
        /// </summary>
        public bool initialized = false;

        /// <summary>
        /// 地图初始化完成事件
        /// </summary>
        public Action<bool> MapInitialized;

        #endregion

        #region 标绘Action

        /// <summary>
        /// 标绘鼠标左键事件
        /// </summary>
        public Action<object, GraphicEventArgs> OnGraphicMouseLeftButtonUp;
        /// <summary>
        /// 标绘鼠标右键事件
        /// </summary>
        public Action<object, GraphicEventArgs> OnGraphicMouseRightButtonUp;
        /// <summary>
        /// 标绘鼠标进入事件
        /// </summary>
        public Action<object, GraphicEventArgs> OnGraphicMouseEnter;

        #endregion

        #region 地图范围
        /// <summary>
        /// 地图坐标范围
        /// </summary>
        public Range<AT_BC.Data.GeoPoint> MapRange;
        #endregion

        


        /// <summary>
        /// 如果重名，增加此序号
        /// </summary>
        private int nameId = 0;
        /// <summary>
        /// 存储点的ID列表 object为ReturnDrawGraphicInfo对象
        /// </summary>
        public List<KeyValuePair<string, object>> DrawList = new List<KeyValuePair<string, object>>();
        /// <summary>
        /// 存储WPF元素点的ID列表 
        /// </summary>
        public List<KeyValuePair<string, object>> DrawElementList = new List<KeyValuePair<string, object>>();
        /// <summary>
        /// 点类别列表
        /// </summary>
        public LayerList layerList = new LayerList();

        
       

        public string GeometryServerUrl = "";

        

        #region 构造函数+地图初始化

        public MapGIS()
        {
            _MainMap = new MapControl();
            _MainMap.IsShowXY = true;

            ResourceDictionary languageResDic = new ResourceDictionary();
            languageResDic.Source = new Uri(@"\CO_IA.UI.MAP;component\Themes\Generic.xaml", UriKind.Relative);
            MainMap.Resources.MergedDictionaries.Add(languageResDic);

            //设置地图地址
            _MainMap.ServiceUrl = RiasPortal.Current.MapConfig.ElectricUrl;

            _MainMap.Initialized += _MainMap_Initialized;
            //_MainMap.OverviewMargin = new Thickness(0,50,50,0);

            //加载资源
            
            
        }
        
        /// <summary>
        /// 地图初始化完成事件
        /// </summary>
        /// <param name="obj"></param>
        void _MainMap_Initialized(bool obj)
        {
            initialized = true;

            if (MainMap.Parent != null)
            {
                Grid g = MainMap.Parent as Grid;
                if (g != null)
                {//添加背景图片
                    g.Background = new System.Windows.Media.ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/CO_IA.Themes;component/Images/bk.jpg")),
                        Opacity = 0.08
                    };
                }
            }

            //图层列表
            //layerList.mapGis = this;
                        

            //为了生成两个图层，并且Element图层在上
            MainMap.DefaultLayer.ClearSymbolElements();
            MainMap.DefaultLayer.ClearSlElements();
                        

            //标绘鼠标事件
            //_MainMap.DefaultLayer.OnGraphicsMouseLeftButtonUp += DefaultLayer_OnGraphicsMouseLeftButtonUp;
            //_MainMap.DefaultLayer.OnGraphicsMouseRightButtonUp += DefaultLayer_OnGraphicsMouseRightButtonUp;
            //_MainMap.DefaultLayer.OnGraphicsMouseEnter += DefaultLayer_OnGraphicsMouseEnter;

            //设置地图默认显示区域
            MapRange = RiasPortal.GetMapDefaultArea();
            fullExtent();

            //获得几何服务地址
            GeometryServerUrl = RiasPortal.GetMapGeometryServerUrl();
            
            if (ShowBar)
            {
                AddDrawBar();
                _showBar = false;
            }
            if (MapInitialized != null)
                MapInitialized(obj);
        }
        #endregion

        #region 添加工具条

        /// <summary>
        /// 增加绘制工具条
        /// </summary>
        private void AddDrawBar()
        {
            if (!initialized)
                return;
            Grid grid = MainMap.Parent as Grid;
            if (grid != null)
            {
                DrawBar = new Bar(this);

                DrawBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                DrawBar.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                DrawBar.Margin = new Thickness(50, 50, 0, 0);

                DrawBar.GraphicChange += BarGraphicChange;
                MainMap.UserGrid.Children.Add(DrawBar);
            }
        }

        #endregion

        #region 标绘鼠标事件

        /// <summary>
        /// 标绘鼠标左键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DefaultLayer_OnGraphicsMouseLeftButtonUp(object sender, GraphicEventArgs e)
        {
            if (OnGraphicMouseRightButtonUp != null)
                OnGraphicMouseLeftButtonUp(sender, e);
        }
        /// <summary>
        /// 标绘鼠标右键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DefaultLayer_OnGraphicsMouseRightButtonUp(object sender, GraphicEventArgs e)
        {
            if (OnGraphicMouseRightButtonUp != null)
                OnGraphicMouseRightButtonUp(sender, e);
        }
        /// <summary>
        /// 标绘鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DefaultLayer_OnGraphicsMouseEnter(object sender, GraphicEventArgs e)
        {
            if(OnGraphicMouseEnter!=null)
                OnGraphicMouseEnter(sender, e);
        }
        #endregion

        #region 地图范围
        /// <summary>
        /// 全图
        /// </summary>
        public void fullExtent()
        {
            if (!initialized)
                return;
            //设置地图默认显示区域           
            setExtent(MapRange.Little, MapRange.Great);
        }
        /// <summary>
        /// 设置地图范围 输入左上角，右下角坐标 x1<x2 y1>y2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="Range">是否按一定比例缩小地图，true：执行；false：不执行</param>
        public void setExtent(AT_BC.Data.GeoPoint p1, AT_BC.Data.GeoPoint p2,bool Range = false)
        {
            if (!initialized)
                return;
            double x_Offset = 0, y_Offset = 0;
            if (Range)
            {
                x_Offset = Math.Abs(p1.Longitude - p2.Longitude) / 2.5;
                y_Offset = Math.Abs(p1.Latitude - p2.Latitude) / 2;
            }
            MainMap.AreaLocation(MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(p1.Longitude - x_Offset,p1.Latitude + y_Offset),
                                                       MainMap.MapPointFactory.Create(p2.Longitude + x_Offset, p2.Latitude - y_Offset)));
        }
        /// <summary>
        /// 设置地图范围 输入左上角，右下角坐标 x1<x2 y1>y2
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="Range">是否按一定比例缩小地图，true：执行；false：不执行</param>
        public void setExtent(MapPointEx p1, MapPointEx p2, bool Range = false)
        {
            if (!initialized)
                return;
            double x_Offset = 0, y_Offset = 0;
            if (Range)
            {
                x_Offset = Math.Abs(p1.X - p2.X) / 2.5;
                y_Offset = Math.Abs(p1.Y - p2.Y) / 2;
            }
            MainMap.AreaLocation(MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(p1.X - x_Offset, p1.Y + y_Offset),
                                                       MainMap.MapPointFactory.Create(p2.X + x_Offset, p2.Y - y_Offset)));
        }

        /// <summary>
        /// 设置点集范围
        /// </summary>
        /// <param name="points"></param>
        /// <param name="Range"></param>
        public void setExtent(AT_BC.Data.GeoPoint[] points, bool Range = false)
        {
            I_GS_MapBase.Portal.Types.MapExtent extent = GetGraphicExtentByPoints(points);
            if (extent.Xy1 == extent.Xy2)
            {
                setExtent(new AT_BC.Data.GeoPoint() { Longitude=extent.Xy1.X,Latitude = extent.Xy1.Y});
                MainMap.Location(extent.Xy1);
            }
            else
            {
                setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude=extent.Xy2.X,Latitude = extent.Xy2.Y}, Range);
            }
        }
        /// <summary>
        /// 设置点集范围
        /// </summary>
        /// <param name="points"></param>
        /// <param name="Range"></param>
        public void setExtent(MapPointEx[] points, bool Range = false)
        {
            I_GS_MapBase.Portal.Types.MapExtent extent = GetGraphicExtentByPoints(points);
            if (extent.Xy1 == extent.Xy2)
            {
                setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y });
                MainMap.Location(extent.Xy1);
            }
            else
            {
                setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = extent.Xy2.X, Latitude = extent.Xy2.Y }, Range);
            }
        }

        /// <summary>
        /// 点
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="Range">是否按一定比例缩小地图，true：执行；false：不执行</param>
        public void setExtent(AT_BC.Data.GeoPoint p)
        {
            if (!initialized)
                return;
            if (p.Latitude == 0 || p.Longitude == 0)
                return;
            setExtent(MainMap.MapPointFactory.Create(p.Longitude,p.Latitude));            
        }
        /// <summary>
        /// 点
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="Range">是否按一定比例缩小地图，true：执行；false：不执行</param>
        public void setExtent(MapPointEx p)
        {
            if (!initialized)
                return;
            if (p.X == 0 || p.Y == 0)
                return;
            double xoffset = 0.01;
            double yoffset = 0.001;
            I_GS_MapBase.Portal.Types.MapExtent extent = MainMap.CurrentMapExtent;
            double xoffset1 = Math.Abs(extent.Xy2.X - extent.Xy1.X) / 2;
            double yoffset1 = Math.Abs(extent.Xy2.Y - extent.Xy1.Y) / 2;
            if (xoffset > xoffset1 || yoffset > yoffset1)
            {
                MainMap.Location(p);
            }
            else
            {
                MainMap.AreaLocation(MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(p.X - xoffset, p.Y + yoffset), MainMap.MapPointFactory.Create(p.X + xoffset, p.Y - yoffset)));
            }
            //闪烁
            SelectPointShine(p);
        }


        public void setExtent(string id)
        {
            var drawItem = DrawList.Where(item=>item.Key == id).ToList();
            if (drawItem != null && drawItem.Count > 0)
            {
                ReturnDrawGraphicInfo info = drawItem[0].Value as ReturnDrawGraphicInfo;
                if (info != null)
                {
                    setExtent(info.CenterPoint);
                }

            }
        }
        /// <summary>
        /// 根据绘制的控件元素ID来定位
        /// </summary>
        /// <param name="id"></param>
        public void setElementExtent(string id)
        {
            var drawItem = DrawElementList.Where(item => item.Key == id).ToList();
            if (drawItem != null && drawItem.Count > 0)
            {
                IFrameworkElement element = drawItem[0].Value as IFrameworkElement;
                if (element != null&&element.ElementTag!=null)
                {
                    MapPointEx p = element.ElementTag as MapPointEx;
                    setExtent(p);
                }

            }
        }

        /// <summary>
        /// 所有地图元素居中显示
        /// </summary>
        public void SetAllGraphicsExtent()
        {
            MainMap.Initialized += ((b) =>
            {
                if (!b)
                    return;
                I_GS_MapBase.Portal.Types.MapExtent extent = GetAllGraphicExtent();
                if (extent != null)
                {
                    setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = extent.Xy2.X, Latitude = extent.Xy2.Y }, true);
                }
            });
            if (initialized)
            {
                I_GS_MapBase.Portal.Types.MapExtent extent = GetAllGraphicExtent();
                if (extent != null)
                {
                    setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = extent.Xy2.X, Latitude = extent.Xy2.Y }, true);
                }
            }           
        }
        /// <summary>
        /// 所有区域居中显示
        /// </summary>
        public void SetAllAreaGraphicsExtent()
        {
            MainMap.Initialized += ((b) =>
            {
                if (!b)
                    return;
                I_GS_MapBase.Portal.Types.MapExtent extent = GetGraphicExtentByFlag(MapGroupTypes.AreaRange_.ToString());
                if (extent == null)
                {
                    extent = GetGraphicExtentByFlag(MapGroupTypes.Location_.ToString());
                }
                if (extent != null)
                {
                    setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = extent.Xy2.X, Latitude = extent.Xy2.Y }, true);
                }
            });
            if (initialized)
            {
                I_GS_MapBase.Portal.Types.MapExtent extent = GetGraphicExtentByFlag(MapGroupTypes.AreaRange_.ToString());
                if (extent == null)
                {
                    extent = GetGraphicExtentByFlag(MapGroupTypes.Location_.ToString());
                }
                if (extent != null)
                {
                    setExtent(new AT_BC.Data.GeoPoint() { Longitude = extent.Xy1.X, Latitude = extent.Xy1.Y }, new AT_BC.Data.GeoPoint() { Longitude = extent.Xy2.X, Latitude = extent.Xy2.Y }, true);
                }
            }
        }

        #region 获取坐标范围

        #region 根据点集获取坐标范围
        /// <summary>
        /// 根据坐标点集获取坐标范围
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public I_GS_MapBase.Portal.Types.MapExtent GetGraphicExtentByPoints(AT_BC.Data.GeoPoint[] points)
        {
            if (points == null || points.Length == 0)
            {
                return null;
            }
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            for (int i = 0; i < points.Length; i++)
            {
                if (x1 == 0)
                {
                    x1 = points[i].Longitude;
                    y1 = points[i].Latitude;
                    x2 = points[i].Longitude;
                    y2 = points[i].Latitude;
                }
                x1 = x1 > points[i].Longitude ? points[i].Longitude : x1;
                y1 = y1 > points[i].Latitude ? y1 : points[i].Latitude;
                x2 = x2 > points[i].Longitude ? x2 : points[i].Longitude;
                y2 = y2 > points[i].Latitude ? points[i].Latitude : y2;
            }
            return MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(x1, y1), MainMap.MapPointFactory.Create(x2, y2));
        }
        /// <summary>
        /// 根据坐标点集获取坐标范围
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public I_GS_MapBase.Portal.Types.MapExtent GetGraphicExtentByPoints(MapPointEx[] mapPoints)
        {
            if (mapPoints == null || mapPoints.Length == 0)
            {
                return null;
            }
            List<AT_BC.Data.GeoPoint> plist = new List<AT_BC.Data.GeoPoint>();
            for (int i = 0; i < mapPoints.Length; i++)
            {
                plist.Add(new AT_BC.Data.GeoPoint() { Longitude = mapPoints[i].X, Latitude = mapPoints[i].Y });
            }
            AT_BC.Data.GeoPoint[] points = plist.ToArray();

            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;

            for (int i = 0; i < points.Length; i++)
            {
                if (x1 == 0)
                {
                    x1 = points[i].Longitude;
                    y1 = points[i].Latitude;
                    x2 = points[i].Longitude;
                    y2 = points[i].Latitude;
                }
                x1 = x1 > points[i].Longitude ? points[i].Longitude : x1;
                y1 = y1 > points[i].Latitude ? y1 : points[i].Latitude;
                x2 = x2 > points[i].Longitude ? x2 : points[i].Longitude;
                y2 = y2 > points[i].Latitude ? points[i].Latitude : y2;
            }
            return MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(x1, y1), MainMap.MapPointFactory.Create(x2, y2));
        }
        #endregion

        #region 获取已绘制图形的坐标范围
        /// <summary>
        /// 所有地点信息 自适应地图
        /// </summary>
        /// <param name="mapgis"></param>
        /// <returns></returns>
        public I_GS_MapBase.Portal.Types.MapExtent GetAllGraphicExtent()
        {
            if (DrawList.Count == 0&&DrawElementList.Count==0)
            {
                return null;
            }
            List<MapPointEx> elementPoints = new List<MapPointEx>();
            if (DrawElementList.Count != 0)
            {
                DrawElementList.ForEach(item => elementPoints.Add(((IFrameworkElement)item.Value).ElementTag as MapPointEx));
            }
            return GetMapExtentByList(DrawList, elementPoints);
        }
        public I_GS_MapBase.Portal.Types.MapExtent GetGraphicExtentByFlag(string Flag)
        {
            if (DrawList.Count == 0 && DrawElementList.Count == 0)
            {
                return null;
            }
            var list = DrawList.Where(item =>item.Key.StartsWith(Flag)).ToList<KeyValuePair<string, object>>();

            List<MapPointEx> elementPoints = new List<MapPointEx>();
            if (DrawElementList.Count != 0)
            {
                DrawElementList.ForEach(item => 
                    {
                        if(item.Key.StartsWith(Flag))
                        {
                            elementPoints.Add(((IFrameworkElement)item.Value).ElementTag as MapPointEx);
                        }
                    });
            }

            return GetMapExtentByList(list, elementPoints);
        }
        /// <summary>
        /// 根据绘制的图形ID获取图形范围
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public I_GS_MapBase.Portal.Types.MapExtent GetGraphicExtentByID(string[] ids)
        {
            if (DrawList.Count == 0&&DrawElementList.Count==0)
            {
                return null;
            }            
            var list = DrawList.Where(item =>
            {
                foreach (string s in ids)
                {
                    string id = item.Key;
                    if (id.StartsWith(s))
                        return true;
                }
                return false;

            }).ToList<KeyValuePair<string, object>>();

            List<MapPointEx> elementPoints = new List<MapPointEx>();
            DrawElementList.ForEach(item=>
                {
                    foreach (string s in ids)
                    {
                        string id = item.Key;
                        if (id.StartsWith(s))
                             elementPoints.Add(((IFrameworkElement)item.Value).ElementTag as MapPointEx);
                    }
                });
          
            return GetMapExtentByList(list, elementPoints);
        }
        public I_GS_MapBase.Portal.Types.MapExtent GetMapExtentByList(List<KeyValuePair<string, object>> list,List<MapPointEx> elementPoint=null)
        {
            if (list == null && elementPoint == null)
                return null;
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            for (int i = 0; i < list.Count; i++)
            {
                ReturnDrawGraphicInfo info = list[i].Value as ReturnDrawGraphicInfo;
                if (info == null)
                    continue;
                if (x1 == 0)
                {
                    x1 = info.GraphicExtent.Xy1.X;
                    y1 = info.GraphicExtent.Xy1.Y;
                    x2 = info.GraphicExtent.Xy2.X;
                    y2 = info.GraphicExtent.Xy2.Y;
                }
                if (info == null)
                    continue;
                x1 = x1 > info.GraphicExtent.Xy1.X ? info.GraphicExtent.Xy1.X : x1;
                y1 = y1 > info.GraphicExtent.Xy1.Y ? y1 : info.GraphicExtent.Xy1.Y;
                x2 = x2 > info.GraphicExtent.Xy2.X ? x2 : info.GraphicExtent.Xy2.X;
                y2 = y2 > info.GraphicExtent.Xy2.Y ? info.GraphicExtent.Xy2.Y : y2;
            }
            if (elementPoint != null && elementPoint.Count > 0)
            {
                foreach (var point in elementPoint)
                {
                    if (x1 == 0)
                    {
                        x1 = point.X;
                        y1 = point.Y;
                        x2 = point.X;
                        y2 = point.Y;
                    }
                    x1 = x1 > point.X ? point.X : x1;
                    y1 = y1 > point.Y ? y1 : point.Y;
                    x2 = x2 > point.X ? x2 : point.X;
                    y2 = y2 > point.Y ? point.Y : y2;
                }
            }
            return MainMap.MapExtentFactory.Create(MainMap.MapPointFactory.Create(x1, y1), MainMap.MapPointFactory.Create(x2, y2));
        }
        #endregion

        #endregion
      

        #endregion

        #region 存储绘制信息
        /// <summary>
        /// 绘制点时保存点类型 polygon_name1
        /// </summary>
        /// <param name="id"></param>
        /// <param name="key">polygon_name1中polygon为key</param>
        private void SaveDrawList(string id, object obj = null, string key = null)
        {
            //string skey = key;
            //if (string.IsNullOrEmpty(skey))//获取图层类别
            //{
            //    if (id.IndexOf('_') != -1)
            //    {
            //        skey = id.Substring(0, id.IndexOf('_')).Trim();
            //    }
            //}
            //if (!string.IsNullOrEmpty(skey))//向图层管理添加类别
            //{
            //    layerList.Add(GetTitleByLayerKey(skey), skey);
            //}

            List<KeyValuePair<string, object>> place = DrawList.Where(item => item.Key == id).ToList<KeyValuePair<string, object>>();
            if (place.Count == 0)
                DrawList.Add(new KeyValuePair<string, object>(id, obj));
        }
        
        #endregion

        #region 删除标绘
        /// <summary>
        /// 删除元素,并从list中移除
        /// </summary>
        /// <param name="elementId">id为控件或者null时，全部删除</param>
        public void RemoveSymbolElement(string Id = null)
        {
            if (!initialized)
                return;
            if (string.IsNullOrEmpty(Id))
            {
                MainMap.DefaultLayer.ClearSymbolElements();
                DrawList.Clear();
                return;
            }
            MainMap.DefaultLayer.RemoveSymbolElement(Id);
            List<KeyValuePair<string, object>> list = DrawList.Where(item => item.Key == Id).ToList<KeyValuePair<string, object>>();
            if (list.Count != 0)
                DrawList.Remove(list[0]);
        }
        /// <summary>
        /// 删除元素,并从list中移除
        /// </summary>
        /// <param name="elementId">id为控件或者null时，全部删除</param>
        public void RemoveSymbolElementByFlag(string flag)
        {
            if (!initialized)
                return;
            if (string.IsNullOrEmpty(flag))
            {                
                return;
            }
            for (int i = 0; i < DrawList.Count; i++)
            {
                if (DrawList[i].Key.StartsWith(flag))
                {
                    MainMap.DefaultLayer.RemoveSymbolElement(DrawList[i].Key);
                    DrawList.RemoveAt(i);
                    i--;
                }
            } 
        }

        /// <summary>
        /// 删除Element点，地图和存储列表 包括UserGrid的
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveElement(string Id = null)
        {
            if (string.IsNullOrEmpty(Id))
            {
                MainMap.DefaultLayer.ClearSlElements();
                DrawElementList.Clear();
                //MainMap.UserGrid.Children.Clear();
                return;
            }
            MainMap.DefaultLayer.RemoveSlElement(Id);
            List<KeyValuePair<string, object>> list = DrawElementList.Where(item => item.Key == Id).ToList<KeyValuePair<string, object>>();
            if (list.Count != 0)
                DrawElementList.Remove(list[0]);

        }
        /// <summary>
        /// genju 
        /// </summary>
        /// <param name="flag"></param>
        public void RemoveElementByFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag))
            {               
                return;
            }
            for (int i = 0; i < DrawElementList.Count; i++)
            {
                if (DrawElementList[i].Key.StartsWith(flag))
                {
                    MainMap.DefaultLayer.RemoveSlElement(DrawElementList[i].Key);
                    DrawElementList.RemoveAt(i);
                    i--;
                }
            }            

        }
        #endregion

        #region 隐藏、显示元素
        /// <summary>
        ///根据前缀设置符号元素是否可见
        /// </summary>
        /// <param name="Flag">前缀</param>
        /// <param name="v"></param>
        public void SetVisibilityByFlag(string Flag,bool v)
        {
            if (DrawList.Count > 0)
            {
                var item = DrawList.Where(itm => itm.Key.StartsWith(Flag)).ToList();
                if (item.Count > 0)
                {
                    item.ForEach(g => MainMap.DefaultLayer.ChangeSLElement(g.Key, v));
                }
            }
        }
        /// <summary>
        /// 根据前缀设置控件元素是否可见
        /// </summary>
        /// <param name="Flag"></param>
        /// <param name="v"></param>
        public void SetElementVisibilityByFlag(string Flag, bool v)
        {
            if (DrawElementList.Count > 0)
            {
                var item = DrawElementList.Where(itm => itm.Key.StartsWith(Flag)).ToList();
                if (item.Count > 0)
                {
                    item.ForEach(g => MainMap.DefaultLayer.ChangeSLElement(g.Key, v));
                }
            }
        }
        /// <summary>
        /// 根据ID设置符号元素是否可见
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        public void SetVisibilityByID(string id, bool v)
        {
            if (DrawList.Count > 0)
            {
                var item = DrawList.Where(itm => itm.Key==id).ToList();
                if (item.Count > 0)
                {
                    item.ForEach(g => MainMap.DefaultLayer.ChangeSLElement(g.Key, v));
                }
            }
        }
        /// <summary>
        /// 根据ID设置控件元素是否可见
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        public void SetElementVisibilityByID(string id, bool v)
        {
            if (DrawElementList.Count > 0)
            {
                var item = DrawElementList.Where(itm => itm.Key==id).ToList();
                if (item.Count > 0)
                {
                    item.ForEach(g => MainMap.DefaultLayer.ChangeSLElement(g.Key, v));
                }
            }
        }
        #endregion


        #region 绘制地点
        int NameId = 0;
        /// <summary>
        /// 解析字符串并绘制
        /// </summary>
        /// <param name="GraphicId">id</param>
        /// <param name="GraphicInfo">图形信息 格式：polyline:blue,2|127,1,42.2&126.8&41.9</param>
        /// <param name="mapControl">需要绘制的地图对象，为空则往ShowMap上绘制</param>
        public ReturnDrawGraphicInfo[] DrawArea(string GraphicId, string GraphicInfo)
        {
            if (string.IsNullOrEmpty(GraphicId) || string.IsNullOrEmpty(GraphicInfo))
            {
                //SetAllGraphicsExtent();
                return null;
            }
            int zIndex = 5;

            //各形状用*分隔
            //polyline:blue,2|127,1,42.2&126.8,41.9
            //polygon:blue,2,red|127,1,42.2&126.8,41.9
            //circle:blue,2,red|127,1,42.2&2000
            //rectangle:blue,2,red|127,1,42.2&127,1,42.2
            //rectangle:blue,2,red|127,1,42.2&2000&1000

            ControlTemplate controlTemplateLine = null;
            ControlTemplate controlTemplateFill = null;
            if (GraphicId.StartsWith("selected_"))
            {
                controlTemplateLine = _MainMap.Resources["selectedLine"] as ControlTemplate;
                controlTemplateFill = _MainMap.Resources["selectedFill"] as ControlTemplate;
                zIndex++;
            }
            List<ReturnDrawGraphicInfo> Result = new List<ReturnDrawGraphicInfo>();
            string[] info = GraphicInfo.Split('*');
            string id = "";
            for (int i = 0; i < info.Length; i++)
            {
                ReturnDrawGraphicInfo gReInfo = null;
                NameId++;
                id = GraphicId + "-" + NameId.ToString();
                GraphicInfo ginfo = getDrawInfo(info[i]);
                if (ginfo is PolylineInfo)
                {
                    PolylineInfo g = ginfo as PolylineInfo;
                    if (g == null)
                        continue;
                    gReInfo = DrawPolyline(g.Points, new SymbolElement(id)
                    {
                        DataSources = new List<KeyValuePair<string, object>> 
                        { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                            new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),zIndex.ToString()),
                        },
                        ControlTemplate = controlTemplateLine
                    });

                }
                if (ginfo is PolygonInfo)
                {
                    PolygonInfo g = ginfo as PolygonInfo;
                    if (g == null)
                        continue;
                    gReInfo = DrawPolygon(g.Points, new SymbolElement(id)
                    {
                        DataSources = new List<KeyValuePair<string, object>> 
                        { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                            new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                            new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),zIndex.ToString()),
                        },
                        ControlTemplate = controlTemplateFill
                    });
                }
                if (ginfo is CircleInfo)
                {
                    CircleInfo g = ginfo as CircleInfo;
                    if (g == null)
                        continue;
                    gReInfo = DrawCircle(g.CenterPoint, g.Radius, new SymbolElement(id)
                    {
                        DataSources = new List<KeyValuePair<string, object>> 
                        { 
                            new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                            new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                            new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                            new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),zIndex.ToString()),
                        },
                        ControlTemplate = controlTemplateFill
                    });
                }
                if (ginfo is RectangleInfo)
                {
                    RectangleInfo g = ginfo as RectangleInfo;
                    if (g == null)
                        continue;
                    if (g.RightBottomPoint != null)
                    {
                        gReInfo = DrawRectangle(g.LeftTopPoint, g.RightBottomPoint, new SymbolElement(id)
                        {
                            DataSources = new List<KeyValuePair<string, object>> 
                            { 
                                new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                                new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                                new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                                new KeyValuePair<string,object>(GraphicStyle.zIndex.ToString(),zIndex.ToString()),
                            },
                            ControlTemplate = controlTemplateFill
                        });
                    }
                    //else
                    //gReInfo = mapgis.DrawRectangle(g.LeftTopPoint, g.RectWidth, g.RectHeight, GraphicId + "-" + NameId.ToString(), new List<KeyValuePair<string, object>> { 
                    //        new KeyValuePair<string,object>(GraphicStyle.BorderColor.ToString(),g.BorderColor),
                    //        new KeyValuePair<string,object>(GraphicStyle.BorderWidth.ToString(),g.BorderWidth),
                    //        new KeyValuePair<string,object>(GraphicStyle.FillColor.ToString(),g.FillColor),
                    //}, controlTemplateFill);
                }
                if (gReInfo != null)
                    Result.Add(gReInfo);
            }
            if (Result.Count == 0)
                return null;
            return Result.ToArray();
        }
        /// <summary>
        /// 绘制地点
        /// </summary>
        /// <param name="locations"></param>
        public void DrawLocation(ActivityPlaceLocation[] locations)
        {
            if (locations != null && locations.Length > 0)
            {
                for (int i = 0; i < locations.Length; i++)
                {
                    string id = MapGroupTypes.Location_.ToString() + locations[i].GUID;
                    bool showimage = false;
                    if (locations[i].activityPlaceLocationImage == null || locations[i].activityPlaceLocationImage.Count == 0)
                    {
                    }
                    else
                    {
                        showimage = true;
                    }
                    LocationPoint locationpoint = new LocationPoint(showimage);
                    locationpoint.ElementId = id;
                    locationpoint.LocationInfo = locations[i];
                    locationpoint.ShowPlaceImageEvent += p_ShowPlaceImageEvent;
                    AddElement(locationpoint, GetMapPointEx(locations[i].LocationLG, locations[i].LocationLA));

                }
            }
        }
        void p_ShowPlaceImageEvent(ActivityPlaceLocation obj)
        {
            ShowLocationImageDialog managerImage = new ShowLocationImageDialog(obj.activityPlaceLocationImage, "");

            managerImage.ShowDialog(VisualTreeHelperExtension.GetParentObject<System.Windows.Window>(MainMap));
        }
        #region 根据字符串获取图形结构
        /// <summary>
        /// 根据字符串获取图形类信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public GraphicInfo getDrawInfo(string info)
        {
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
                                    lineInfo.Points.Add(GetMapPointEx(p[0].TryToDoubleZero(), p[1].TryToDoubleZero()));
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
                                    gonInfo.Points.Add(GetMapPointEx(p[0].TryToDoubleZero(), p[1].TryToDoubleZero()));
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
                                circleInfo.CenterPoint = GetMapPointEx(p[0].TryToDoubleZero(), p[1].TryToDoubleZero());
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
                                rectangleInfo.LeftTopPoint = GetMapPointEx(p[0].TryToDoubleZero(), p[1].TryToDoubleZero());
                                rectangleInfo.RectWidth = gPoints[1].TryToDoubleZero();
                                rectangleInfo.RectHeight = gPoints[2].TryToDoubleZero();
                            }
                            if (gPoints.Length == 2)
                            {
                                string[] p = gPoints[0].Split(',');
                                rectangleInfo.LeftTopPoint = GetMapPointEx(p[0].TryToDoubleZero(), p[1].TryToDoubleZero());
                                string[] p2 = gPoints[1].Split(',');
                                rectangleInfo.RightBottomPoint = GetMapPointEx(p2[0].TryToDoubleZero(), p2[1].TryToDoubleZero());
                            }
                        }
                        return rectangleInfo;
                }
            }
            return null;
        }
        #endregion

        #region  根据绘制的图形获得位置字符串
        /// <summary>
        /// 根据绘制的图形获得位置字符串  暂支持一个图形，绘制地点时候用
        /// </summary>
        public string GetGraphicString(string key=null)
        {
            //各形状用*分隔
            //polyline:blue,2|127,1,42.2&126.8,41.9 （线：线颜色、宽度|起点&终点）
            //polygon:blue,2,red|127,1,42.2&126.8,41.9 （多边形：边框颜色，宽度，填充颜色|点&点）
            // circle:blue,2,red|127,1,42.2&2000 （圆：边框颜色，宽度，填充颜色|中心点，半径（米））            
            //rectangle:blue,2,red|127,1,42.2&2000&1000 （矩形：边框颜色，宽度，填充颜色|左上角坐标&宽度（米）&高度（米））

            //rectangle:blue,2,red|127,1,42.2&127,1,42.2 （矩形）
            string Locations = "";
            for (int i = 0; i < DrawList.Count; i++)
            {
                string gString = "", borderColor = "", borderWidth = "", fillColor = "", pString = "";
                ReturnDrawGraphicInfo info = DrawList[i].Value as ReturnDrawGraphicInfo;
                if (info != null)
                {
                    foreach (var dataItem in info.DataSource)
                    {
                        if (dataItem.Key == GraphicStyle.BorderColor.ToString())
                        {
                            borderColor = dataItem.Value.ToString();
                        }
                        if (dataItem.Key == GraphicStyle.BorderWidth.ToString())
                        {
                            borderWidth = dataItem.Value.ToString();
                        }
                        if (dataItem.Key == GraphicStyle.FillColor.ToString())
                        {
                            fillColor = dataItem.Value.ToString();
                        }
                    }
                    switch (info.GraphicType)
                    {
                        case QueryBehavior.Polyline:
                            PolygonQueryBehaviorEventArgs polyline = info.QueryObject as PolygonQueryBehaviorEventArgs;
                            pString = "";
                            if (polyline != null && polyline.Points != null)
                            {
                                for (int j = 0; j < polyline.Points.Count; j++)
                                {
                                    pString += polyline.Points[j].X.ToString() + "," + polyline.Points[j].Y.ToString() + "&";
                                }
                                pString = pString.Trim('&');
                            }
                            gString = "polyline:" + borderColor + "," + borderWidth + "|" + pString;
                            break;
                        case QueryBehavior.Polygon:
                            PolygonQueryBehaviorEventArgs polygon = info.QueryObject as PolygonQueryBehaviorEventArgs;
                            pString = "";
                            if (polygon != null && polygon.Points != null)
                            {
                                for (int j = 0; j < polygon.Points.Count; j++)
                                {
                                    pString += polygon.Points[j].X.ToString() + "," + polygon.Points[j].Y.ToString() + "&";
                                }
                                pString = pString.Trim('&');
                            }
                            gString = "polygon:" + borderColor + "," + borderWidth + "," + fillColor + "|" + pString;
                            break;
                        case QueryBehavior.Circle:
                            CircleBehaviorEventArgs circle = info.QueryObject as CircleBehaviorEventArgs;
                            pString = "";
                            if (circle != null)
                            {
                                gString = "circle:" + borderColor + "," + borderWidth + "," + fillColor + "|" + circle.CenterPoint.X.ToString() + "," + circle.CenterPoint.Y.ToString() + "&" + circle.Radius.ToString();
                            }

                            break;
                        case QueryBehavior.Rectangle:
                            RectangleQueryBehaviorEventArgs rectangle = info.QueryObject as RectangleQueryBehaviorEventArgs;
                            if (rectangle != null)
                            {
                                if (rectangle.RightBottomPoint == null)
                                    gString = "rectangle:" + borderColor + "," + borderWidth + "," + fillColor + "|" + rectangle.LeftTopPoint.X.ToString() + "," + rectangle.LeftTopPoint.Y.ToString() + "&" + rectangle.Width + "&" + rectangle.Height;
                                else
                                    gString = "rectangle:" + borderColor + "," + borderWidth + "," + fillColor + "|" + rectangle.LeftTopPoint.X.ToString() + "," + rectangle.LeftTopPoint.Y.ToString() + "&" + rectangle.RightBottomPoint.X.ToString() + "," + rectangle.RightBottomPoint.Y.ToString();
                            }
                            break;
                    }
                    Locations = Locations + "*" + gString;
                }
            }
            return Locations.Trim('*').Trim();
        }

        #endregion

        #endregion

        
        /// <summary>
        /// 4秒钟闪烁一个点
        /// </summary>
        /// <param name="p"></param>
        public void SelectPointShine(MapPointEx p)
        {
            if (p.X <= 0 || p.X > 180)
                return;
            if (p.Y > 90 || p.Y <= 0)
                return;
            string id = "shine_" + Guid.NewGuid().ToString();
            DrawPoint(p, new SymbolElement(id) {
                ControlTemplate = MainMap.Resources["PointShine"],
            });
            
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(2000);
                MainMap.Dispatcher.BeginInvoke(new Action(() =>
                {
                    RemoveSymbolElement(id);
                }));

            })) { IsBackground = true }.Start();  
        }

        #region 画点 Station
        private MapLayer haveStationLayer = null;
        /// <summary>
        /// 绘制的台站图层
        /// </summary>
        public MapLayer HaveStationLayer
        {
            get
            {
                if (haveStationLayer == null)
                    haveStationLayer = MainMap.CreateMapLayer();
               
                return haveStationLayer;
            }
        }

        private MapLayer stationLayer = null;
        /// <summary>
        /// 绘制的台站图层
        /// </summary>
        public MapLayer StationLayer
        {
            get
            {
                if (stationLayer == null)
                    stationLayer = MainMap.CreateMapLayer();
                return stationLayer;
            }
        }
        private MapLayer selectStationLayer;
        /// <summary>
        /// 台站选择图层
        /// </summary>
        public MapLayer SelectStationLayer
        {
            get
            {
                if (haveStationLayer == null)
                    haveStationLayer = MainMap.CreateMapLayer();
                if (stationLayer == null)
                    stationLayer = MainMap.CreateMapLayer();//保证台站图层在下
                if (selectStationLayer == null)
                    selectStationLayer = MainMap.CreateMapLayer();
                return selectStationLayer;
            }
        }
        /// <summary>
        /// 设置是否聚合
        /// </summary>
        /// <param name="b"></param>
        public void SetCluster(bool b)
        {
            StationLayer.IsClusterer = b;
        }
        /// <summary>
        /// 在默认图层上画点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="imgUrl">可以为空，如果不为空，将添加到DC参数中</param>
        /// <param name="Id"></param>
        /// <param name="SymbolName">样式模板</param>
        /// <param name="DC">数据字典，不为空时可以放置图片等信息，以imgUrl为准</param>
        public MapPointEx DrawStationPoint(MapPointEx point, bool bRight, object data, MapLayer Layer = null, bool? selected = null)
        {
            MapPointEx RePoint = null;
            double[] p = new double[] { point.Y,point.X};
            if(bRight)
                p = I_GS_MapBase.Portal.CoordOffset.transform(point.X, point.Y);//进行坐标校正
             
            
            if (Layer == null)
            {
                //if (StationLayer != null)
                //    MainMap.RemoveMapLayer(StationLayer.Id);
                Layer = StationLayer;
            }
            ControlTemplate cTemplate = MainMap.Resources["StationTemplete"] as ControlTemplate;

            string tooltip = null;
            string type = null;
            string id = "";
            string imgName = "VHF6";
            if (data is CO_IA_Data.StationInfo)
            {
                CO_IA_Data.StationInfo stationinfo = data as CO_IA_Data.StationInfo;
                tooltip = stationinfo.STAT_NAME;
                type = stationinfo.NET_SVN.PadRight(4,'0') +"-"+stationinfo.STAT_APP_TYPE;
                id = stationinfo.STATGUID;
            }
            if (data is CO_IA.Data.Setting.JHStation)
            {
                CO_IA.Data.Setting.JHStation stationinfo = data as CO_IA.Data.Setting.JHStation;
                tooltip = stationinfo.NAME;
                type = stationinfo.NET_SVN.PadRight(4, '0') + "-" + stationinfo.ST_TYPE;
                id = stationinfo.GUID;
            }
            if (Utility.StationClassDic.ContainsKey(type))
                imgName = Utility.StationClassDic[type];

            List<KeyValuePair<string, object>> dataSource = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(GraphicStyle.ImageSource.ToString(),string.Format("/CO_IA.UI.Map;component/Images/station/GIS_{0}.png",imgName)),
                new KeyValuePair<string, object>(GraphicStyle.ToolTipText.ToString(),tooltip),
                new KeyValuePair<string, object>("data",data),                                        
            };
            dataSource.Add(new KeyValuePair<string, object>("Selected",selected==true?"1":"0"));

            RePoint = GetMapPointEx(p[1], p[0]);
            ReturnDrawGraphicInfo rInfo = Layer.DrawPoint(RePoint, new SymbolElement(id)
            {
                ControlTemplate = cTemplate,
                DataSources = dataSource
            });
            return RePoint;
        }
        
        /// <summary>
        /// 清空已设台站图层
        /// </summary>
        public void ClearHaveStation()
        {
            if (StationLayer != null)
            {
                HaveStationLayer.ClearSymbolElements();                
            }        
        }
        /// <summary>
        /// 清空台站图层
        /// </summary>
        public void ClearStation()
        {
            if (StationLayer != null)
            {
                StationLayer.ClearSymbolElements();
            }
        }
        /// <summary>
        /// 选中台站
        /// </summary>
        /// <param name="data"></param>
        public void SelectStation(object data)
        {
            SelectStationLayer.ClearSymbolElements();
            if (data == null)
                return;
            MapPointEx point=null;
            if (data is CO_IA_Data.StationInfo)
            {
                CO_IA_Data.StationInfo stationinfo = data as CO_IA_Data.StationInfo;
                point = MainMap.MapPointFactory.Create(stationinfo.STAT_LG,stationinfo.STAT_LA);
            }
            if (data is CO_IA.Data.Setting.JHStation)
            {
                CO_IA.Data.Setting.JHStation stationinfo = data as CO_IA.Data.Setting.JHStation;
                double x = 0; double y = 0;
                if(double.TryParse(stationinfo.STAT_LG,out x)&&double.TryParse(stationinfo.STAT_LA,out y))
                {
                
                }
                point = MainMap.MapPointFactory.Create(x,y);
            }
            
            //MainMap.Location(point,18);
            MapPointEx rightPoint = DrawStationPoint(point, true, data, SelectStationLayer, true);
            setExtent(rightPoint);
            //MainMap.AreaLocation();
            //MainMap.Location(rightPoint, 18);
        
        }
        #endregion

        #region element 操作
        public void AddElement(IFrameworkElement element,MapPointEx point)
        {
            MainMap.DefaultLayer.AddSlElement(element,point);
            element.ElementTag = point;
            SaveElementList(element.ElementId, element);
        }
      
        /// <summary>
        /// 保存Element点
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="obj"></param>
        private void SaveElementList(string Id,object obj)
        {
            DrawElementList.Add(new KeyValuePair<string,object>( Id,obj));
        }      
       
        #endregion

        #region 画点
        /// <summary>
        /// 画点
        /// </summary>
        /// <param name="point">坐标</param>
        /// <param name="symbol"></param>
        public void DrawPoint(MapPointEx point, SymbolElement symbol,MapLayer layer = null)
        {
            MapLayer Layer = layer;
            if (layer == null)
                Layer = MainMap.DefaultLayer;
            ReturnDrawGraphicInfo rInfo = Layer.DrawPoint(point, symbol);
            if(symbol!=null&&layer==null)
                SaveDrawList(symbol.ElementId, rInfo);
        }
        
        /// <summary>
        /// 在默认图层上画点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="imgUrl">可以为空，如果不为空，将添加到DC参数中</param>
        /// <param name="Id"></param>
        /// <param name="SymbolName">样式模板</param>
        /// <param name="DC">数据字典，不为空时可以放置图片等信息，以imgUrl为准</param>
        //public void DrawPoint(double x, double y, string imgUrl, string Id, List<KeyValuePair<string, object>> DC = null, string SymbolName = null,double x_offset=0,double y_offset=0)
        //{
        //    ControlTemplate cTemplate = null;
        //    if (!String.IsNullOrEmpty(SymbolName))
        //        cTemplate = MainMap.Resources[SymbolName] as ControlTemplate;

        //    List<KeyValuePair<string, object>> dataSource = null;
        //    if (DC == null)
        //    {
        //        if (!string.IsNullOrEmpty(imgUrl))
        //        {
        //            dataSource = new List<KeyValuePair<string, object>>
        //            {
        //                new KeyValuePair<string, object>(GraphicStyle.ImageSource.ToString(),imgUrl),
                        
        //            };
        //        }
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(imgUrl))
        //        {
        //            DC.Remove(DC.Where(itm => itm.Key == GraphicStyle.ImageSource.ToString()).FirstOrDefault());
        //            DC.Add(new KeyValuePair<string, object>(GraphicStyle.ImageSource.ToString(), imgUrl));
        //        }

        //        dataSource = DC;
        //    }
        //    if (!String.IsNullOrEmpty(SymbolName))
        //    {//客户端自定义样式
        //        dataSource.Add(new KeyValuePair<string, object>("x_offset", x_offset.ToString()));
        //        dataSource.Add(new KeyValuePair<string, object>("y_offset", y_offset.ToString()));
        //        dataSource.Add(new KeyValuePair<string, object>("xline_offset", (20 - x_offset).ToString()));//(20-x_offset)
        //        dataSource.Add(new KeyValuePair<string, object>("yline_offset", (55 - y_offset).ToString()));//(y_offset-50)
        //        if (x_offset == 0 && y_offset == 0)
        //            dataSource.Add(new KeyValuePair<string, object>("point_Visible", "Hidden"));
        //        else
        //            dataSource.Add(new KeyValuePair<string, object>("point_Visible", "Visible"));
        //    }

        //    ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawPoint(MainMap.MapPointFactory.Create(x, y), new SymbolElement(Id)
        //    {
        //        ControlTemplate = cTemplate,
        //        DataSources = dataSource
        //    });

        //    SaveDrawList(Id, rInfo);
        //}
        #endregion

       

        #region 绘制圆
        public ReturnDrawGraphicInfo DrawCircle(MapPointEx centerPoint, double Radius,SymbolElement symbol)
        {
            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawCircle(centerPoint, Radius, symbol);
            if (symbol != null)
                SaveDrawList(symbol.ElementId, rInfo);
            return rInfo;
        }
        /// <summary>
        /// 绘制圆
        /// </summary>
        /// <param name="centerPoint">中心点坐标</param>
        /// <param name="Radius">半径长度，单位：米</param>
        /// <param name="Id">唯一标识</param>
        /// <param name="DC">数据字典，包含样式</param>
        /// <param name="SymbolName">样式模板</param>
        /// <param name="BorderColor">设置边框颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>
        /// <param name="FillColor">设置填充颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>        
        /// <param name="BorderThickness">设置宽度，默认1，如果参数DC和SymbolName均为null，则此参数有效</param>
        public ReturnDrawGraphicInfo DrawCircle(MapPointEx centerPoint, double Radius, string Id, List<KeyValuePair<string, object>> DC = null, ControlTemplate SymbolName = null, string BorderColor = "Green", string FillColor = "#3300FF00", int BorderThickness = 1)
        {
            ControlTemplate controlTemplate = SymbolName;

            if (controlTemplate == null)
            {
                controlTemplate = MainMap.Resources["DrawFill"] as ControlTemplate;

            }
            List<KeyValuePair<string, object>> dataSource = DC;
            if (dataSource == null)
            {
                dataSource = new List<KeyValuePair<string, object>>
                {                                
                    new KeyValuePair<string, object>(GraphicStyle.FillColor.ToString(), FillColor),
                    new KeyValuePair<string, object>(GraphicStyle.BorderWidth.ToString(), BorderThickness.ToString()),
                    new KeyValuePair<string, object>(GraphicStyle.BorderColor.ToString(), BorderColor)
                };
            }
            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawCircle(centerPoint, Radius, new SymbolElement(Id)
            {
                ControlTemplate = controlTemplate,
                DataSources = dataSource
            });

            SaveDrawList(Id,rInfo);
            return rInfo;
        }
        #endregion

        #region 绘制矩形
        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="LeftTopPoint"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public ReturnDrawGraphicInfo DrawRectangle(MapPointEx LeftTopPoint, double Width, double Height, SymbolElement symbol)
        {
            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawRectangle(LeftTopPoint, Width, Height, symbol);
            if(symbol!=null)
                SaveDrawList(symbol.ElementId, rInfo);
            return rInfo;
        }
        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="LeftTopPoint">左上点坐标</param>
        /// <param name="Width">宽度，单位：米</param>
        /// <param name="Height">高度，单位：米</param>
        /// <param name="Id">唯一标识</param>
        /// <param name="DC">数据字典，包含样式</param>
        /// <param name="SymbolName">样式模板</param>
        /// <param name="BorderColor">设置边框颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>
        /// <param name="FillColor">设置填充颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>        
        /// <param name="BorderThickness">设置宽度，默认1，如果参数DC和SymbolName均为null，则此参数有效</param>
        public ReturnDrawGraphicInfo DrawRectangle(MapPointEx LeftTopPoint, double Width, double Height, string Id, List<KeyValuePair<string, object>> DC = null, ControlTemplate SymbolName = null, string BorderColor = "Green", string FillColor = "#3300FF00", int BorderThickness = 1)
        {
            ControlTemplate controlTemplate = SymbolName;

            if (controlTemplate == null)
            {
                controlTemplate = MainMap.Resources["DrawFill"] as ControlTemplate;

            }
            List<KeyValuePair<string, object>> dataSource = DC;
            if (dataSource == null)
            {
                dataSource = new List<KeyValuePair<string, object>>
                {                                
                    new KeyValuePair<string, object>(GraphicStyle.FillColor.ToString(), FillColor),
                    new KeyValuePair<string, object>(GraphicStyle.BorderWidth.ToString(), BorderThickness.ToString()),
                    new KeyValuePair<string, object>(GraphicStyle.BorderColor.ToString(), BorderColor)
                };
            }

            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawRectangle(LeftTopPoint, Width, Height, new SymbolElement(Id)
            {
                ControlTemplate = controlTemplate,
                DataSources = dataSource
            });

            SaveDrawList(Id,rInfo);
            return rInfo;
        }
        #endregion

        #region 绘制矩形Ex
        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="LeftTopPoint"></param>
        /// <param name="RightBottomPoint"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public ReturnDrawGraphicInfo DrawRectangle(MapPointEx LeftTopPoint, MapPointEx RightBottomPoint, SymbolElement symbol)
        {
            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawRectangle(LeftTopPoint, RightBottomPoint, symbol);
            if(symbol!=null)
                SaveDrawList(symbol.ElementId, rInfo);
            return rInfo;
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="LeftTopPoint">左上点坐标</param>
        /// <param name="Width">宽度，单位：米</param>
        /// <param name="Height">高度，单位：米</param>
        /// <param name="Id">唯一标识</param>
        /// <param name="DC">数据字典，包含样式</param>
        /// <param name="SymbolName">样式模板</param>
        /// <param name="BorderColor">设置边框颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>
        /// <param name="FillColor">设置填充颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>        
        /// <param name="BorderThickness">设置宽度，默认1，如果参数DC和SymbolName均为null，则此参数有效</param>
        public ReturnDrawGraphicInfo DrawRectangle(MapPointEx LeftTopPoint, MapPointEx RightBottomPoint, string Id, List<KeyValuePair<string, object>> DC = null, ControlTemplate SymbolName = null, string BorderColor = "Green", string FillColor = "#3300FF00", int BorderThickness = 1)
        {
            ControlTemplate controlTemplate = SymbolName;

            if (controlTemplate == null)
            {
                controlTemplate = MainMap.Resources["DrawFill"] as ControlTemplate;

            }
            List<KeyValuePair<string, object>> dataSource = DC;
            if (dataSource == null)
            {
                dataSource = new List<KeyValuePair<string, object>>
                {                                
                    new KeyValuePair<string, object>(GraphicStyle.FillColor.ToString(), FillColor),
                    new KeyValuePair<string, object>(GraphicStyle.BorderWidth.ToString(), BorderThickness.ToString()),
                    new KeyValuePair<string, object>(GraphicStyle.BorderColor.ToString(), BorderColor)
                };
            }

            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawRectangle(LeftTopPoint, RightBottomPoint, new SymbolElement(Id)
            {
                ControlTemplate = controlTemplate,
                DataSources = dataSource
            });

            SaveDrawList(Id, rInfo);
            return rInfo;
        }
        #endregion

        #region 绘制多边形
        /// <summary>
        /// 绘制多边形
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public ReturnDrawGraphicInfo DrawPolygon(List<MapPointEx> Points, SymbolElement symbol)
        {
            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawPolygon(Points, symbol);
            if(symbol!=null)
                SaveDrawList(symbol.ElementId, rInfo);
            return rInfo;
        }
        /// <summary>
        /// 绘制多边形
        /// </summary>
        /// <param name="Points">点集</param>
        /// <param name="Id">唯一标识</param>        
        /// <param name="DC">数据字典，包含样式</param>
        /// <param name="SymbolName">样式模板</param>
        /// <param name="BorderColor">设置边框颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>
        /// <param name="FillColor">设置填充颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>        
        /// <param name="BorderThickness">设置宽度，默认1，如果参数DC和SymbolName均为null，则此参数有效</param>
        public ReturnDrawGraphicInfo DrawPolygon(List<MapPointEx> Points, string Id, List<KeyValuePair<string, object>> DC = null, ControlTemplate SymbolName = null, string BorderColor = "Green", string FillColor = "#3300FF00", int BorderThickness = 1)
        {
            ControlTemplate controlTemplate = SymbolName;

            if (controlTemplate == null)
            {
                controlTemplate = MainMap.Resources["DrawFill"] as ControlTemplate;
            }
            List<KeyValuePair<string, object>> dataSource = DC;
            if (dataSource == null)
            {
                dataSource = new List<KeyValuePair<string, object>>
                {                                
                    new KeyValuePair<string, object>(GraphicStyle.FillColor.ToString(), FillColor),
                    new KeyValuePair<string, object>(GraphicStyle.BorderWidth.ToString(), BorderThickness.ToString()),
                    new KeyValuePair<string, object>(GraphicStyle.BorderColor.ToString(), BorderColor)
                };
            }
            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawPolygon(Points, new SymbolElement(Id)
            {
                ControlTemplate = controlTemplate,
                DataSources = dataSource
            });

            SaveDrawList(Id, rInfo);
            return rInfo;
        }
        #endregion

        #region 绘制线
        public ReturnDrawGraphicInfo DrawPolyline(List<MapPointEx> Points, SymbolElement symbol)
        {
            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawPolyLine(Points, symbol);
            if (symbol!=null)
                SaveDrawList(symbol.ElementId, rInfo);
            return rInfo;
        }
        /// <summary>
        /// 绘制线
        /// </summary>
        /// <param name="Points">点集</param>
        /// <param name="Id">唯一标识</param>
        /// <param name="DC">数据字典，包含样式</param>
        /// <param name="SymbolName">样式模板</param>
        /// <param name="BorderColor">设置颜色，默认绿色，如果参数DC和SymbolName均为null，则此参数有效</param>
        /// <param name="BorderThickness">设置宽度，默认1，如果参数DC和SymbolName均为null，则此参数有效</param>
        public ReturnDrawGraphicInfo DrawPolyline(List<MapPointEx> Points, string Id, List<KeyValuePair<string, object>> DC = null, ControlTemplate SymbolName = null, string BorderColor = "Green", int BorderThickness = 1)
        {
            ControlTemplate controlTemplate = SymbolName;

            if (controlTemplate == null)
            {
                controlTemplate = MainMap.Resources["DrawLine"] as ControlTemplate;

            }
            List<KeyValuePair<string, object>> dataSource = DC;
            if (dataSource == null)
            {
                dataSource = new List<KeyValuePair<string, object>>
                {                                
                    new KeyValuePair<string, object>(GraphicStyle.BorderWidth.ToString(), BorderThickness.ToString()),
                    new KeyValuePair<string, object>(GraphicStyle.BorderColor.ToString(), BorderColor)
                };
            }

            //如果有重名的，随机价格后缀
            string eId = Id;
            List<KeyValuePair<string, object>> place = DrawList.Where(item => item.Key == Id).ToList<KeyValuePair<string, object>>();
            if (place.Count > 0)
            {
                nameId++;
                eId = eId + "-" + nameId.ToString();
            }

            ReturnDrawGraphicInfo rInfo = MainMap.DefaultLayer.DrawPolyLine(Points, new SymbolElement(eId)
            {
                ControlTemplate = controlTemplate,
                DataSources = dataSource
            });
            //将真实ID存入Tag
            rInfo.Tag = Id;
            SaveDrawList(eId, rInfo);
            return rInfo;
        }
        #endregion

        /// <summary>
        /// 根据图形信息获取缓冲范围的点集
        /// </summary>
        /// <param name="R">缓冲距离</param>
        /// <param name="GraphicInfo">图形信息，同地点结构的字段</param>
        /// <param name="CallBack">返回点集</param>
        /// <param name="ExtentCallBack">返回图形范围</param>
        public void GetGeometryBuffer(double R, string GraphicInfos, Action<AT_BC.Data.GeoPoint[]> CallBack, Action<I_GS_MapBase.Portal.Types.MapExtent> ExtentCallBack = null)
        {
            QueryBehavior type = QueryBehavior.None;
            List<MapPointEx> inputPoint=null;
            inputPoint = getPointsByGraphics(GraphicInfos, out type);

            //GraphicInfo ginfo = getDrawInfo(GraphicInfo);
            
            //if (ginfo is PolylineInfo)
            //{
            //    PolylineInfo lineInfo = ginfo as PolylineInfo;
            //    type = QueryBehavior.Polyline;
            //    inputPoint = lineInfo.Points;
            //}
            //if (ginfo is PolygonInfo)
            //{
            //    PolygonInfo gonInfo = ginfo as PolygonInfo;
            //    type = QueryBehavior.Polygon;
            //    inputPoint = gonInfo.Points;
            //}
            //if (ginfo is CircleInfo)
            //{
            //    CircleInfo circleInfo = ginfo as CircleInfo;
            //    type = QueryBehavior.Circle;
            //    inputPoint = MainMap.GetPointsByGeometry(new GeoCircle() {  Center=new I_GS_MapBase.Portal.Types.GeoPoint(){ X = circleInfo.CenterPoint.X,Y = circleInfo.CenterPoint.Y}, Radius = circleInfo.Radius});
            //} 
            //if (ginfo is RectangleInfo)
            //{
            //    RectangleInfo rectangleInfo = ginfo as RectangleInfo;
            //    type = QueryBehavior.Polyline;
            //    inputPoint = new List<MapPointEx>() { 
            //        rectangleInfo.LeftTopPoint,
            //        MainMap.MapPointFactory.Create(rectangleInfo.RightBottomPoint.X,rectangleInfo.LeftTopPoint.Y),
            //        rectangleInfo.RightBottomPoint,
            //        MainMap.MapPointFactory.Create(rectangleInfo.LeftTopPoint.X,rectangleInfo.RightBottomPoint.Y),
            //        rectangleInfo.LeftTopPoint
            //    };
            //}
            int Re = MainMap.GeometryBuffer(inputPoint, type, R, new Action<List<MapPointEx>>((points) =>
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
                    if (CallBack != null)
                    {
                        List<AT_BC.Data.GeoPoint> geoList = new List<AT_BC.Data.GeoPoint>();
                        points.ForEach(item => geoList.Add(new AT_BC.Data.GeoPoint() { Longitude=item.X,Latitude=item.Y}));
                        CallBack(geoList.ToArray());
                    }
                }
            }), GeometryServerUrl);
        }
        /// <summary>
        /// 根据图形信息获取点集
        /// </summary>
        /// <param name="GraphicInfo"></param>
        /// <param name="type">返回点类型</param>
        /// <returns></returns>
        public List<MapPointEx> getPointsByGraphics(string GraphicInfos, out QueryBehavior type)
        {
            GraphicInfo ginfo = getDrawInfo(GraphicInfos);
            type = QueryBehavior.None;
            List<MapPointEx> inputPoint = null;
            if (ginfo is PolylineInfo)
            {
                PolylineInfo lineInfo = ginfo as PolylineInfo;
                type = QueryBehavior.Polyline;
                inputPoint = lineInfo.Points;
            }
            if (ginfo is PolygonInfo)
            {
                PolygonInfo gonInfo = ginfo as PolygonInfo;
                type = QueryBehavior.Polygon;
                inputPoint = gonInfo.Points;
            }
            if (ginfo is CircleInfo)
            {
                CircleInfo circleInfo = ginfo as CircleInfo;
                type = QueryBehavior.Circle;
                inputPoint = MainMap.GetPointsByGeometry(new GeoCircle() { Center = new I_GS_MapBase.Portal.Types.GeoPoint() { X = circleInfo.CenterPoint.X, Y = circleInfo.CenterPoint.Y }, Radius = circleInfo.Radius });
            }
            if (ginfo is RectangleInfo)
            {
                RectangleInfo rectangleInfo = ginfo as RectangleInfo;
                type = QueryBehavior.Polyline;
                inputPoint = new List<MapPointEx>() { 
                    rectangleInfo.LeftTopPoint,
                    MainMap.MapPointFactory.Create(rectangleInfo.RightBottomPoint.X,rectangleInfo.LeftTopPoint.Y),
                    rectangleInfo.RightBottomPoint,
                    MainMap.MapPointFactory.Create(rectangleInfo.LeftTopPoint.X,rectangleInfo.RightBottomPoint.Y),
                    rectangleInfo.LeftTopPoint
                };
            }
            return inputPoint;
        }




        public MapPointEx GetMapPointEx(double x, double y)
        {
            return _MainMap.MapPointFactory.Create(x, y);
        }

        /// <summary>
        /// 经纬度校验
        /// </summary>
        /// <param name="xy"></param>
        /// <returns></returns>
        public bool CheckCoordinate(double[] xy)
        {
            if (xy.Length != 2)
                return false;
            if (xy[0] > 0 && xy[0] < 180)
                return true;
            if (xy[1] > 0 || xy[1] < 90)
                return false;
            return false;
        }















        private string GetTitleByLayerKey(string skey)
        {
            string title = "标题";
            switch (skey)
            {
                case "center":
                    title = "指挥中心";
                    break;
                case "site":
                    title = "活动区域";
                    break;
                case "station":
                    title = "台站";
                    break;
                case "monitor":
                    title = "固定监测设备";
                    break;
                case "group":
                    title = "监测小组";
                    break;
                case "shine":
                    title = "标记点";
                    break;
                case "mgroup":
                    title = "监测组";
                    break;
                case "ungroup":
                    title = "非监测组";
                    break;
                case "tips":
                    title = "地点信息";
                    break;
                case "gps":
                    title = "GPS标记";
                    break;
                default:
                    title = "其它";
                    break;

            }
            return title;
        }

    }
    
   
}
