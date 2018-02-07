using System;
using System.Windows;
using System.Windows.Controls;
using Best.VWPlatform.Common.Types;
using System.Collections.Generic;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 地图入口，ArcGis或SuperMap
    /// </summary>
    public interface IMap
    {
        #region 方法
        /// <summary>
        /// 初始化地图控件
        /// </summary>
        void Initialize();
        /// <summary>
        /// 设置地图操作绑定
        /// </summary>
        /// <param name="pBindingOperateList">操作列表</param>
        void SetBindingAction(Dictionary<MapOperate, DependencyObject> pBindingOperateList);
        /// <summary>
        /// 地图缩放
        /// </summary>
        /// <param name="pFactor">因素</param>
        void Zoom(double pFactor);
        /// <summary>
        /// 地图平移
        /// </summary>
        /// <param name="pOrientation">方向</param>
        void PanTo(MoveOrientation pOrientation);
        /// <summary>
        /// 地图平移到指定的坐标
        /// </summary>
        /// <param name="pPoint">坐标</param>
        void PanTo(MapPointEx pPoint);
        /// <summary>
        /// 地图平移到矩形范围外指定的点上
        /// </summary>
        /// <param name="pPoint">矩形范围外的点</param>
        /// <param name="pOutside">矩形范围</param>
        void PanToRectOutside(MapPointEx pPoint, Rect pOutside);
        /// <summary>
        /// 设置地图可视范围
        /// </summary>
        /// <param name="pExtent">范围</param>
        void SetMapViewBounds(MapExtent pExtent);
        /// <summary>
        /// 地图坐标转屏幕坐标
        /// </summary>
        /// <param name="pMapPoint"></param>
        /// <returns></returns>
        Point MapToScreen(MapPointEx pMapPoint);
        /// <summary>
        /// 屏幕坐标转地图坐标
        /// </summary>
        /// <param name="pScreenPoint"></param>
        /// <returns></returns>
        MapPointEx ScreenToMap(Point pScreenPoint);
        /// <summary>
        /// 设置测试距离操作
        /// </summary>
        void SetMeasureDistanceAction();
        /// <summary>
        /// 旋转角度
        /// </summary>
        double RotationAngle
        {
            get;
            set;
        }
        /// <summary>
        /// 设置地图为灰色
        /// </summary>
        void SetGary(bool pBGary);
        #endregion

        #region 属性
        /// <summary>
        /// 获取地图服务信息
        /// </summary>
        IMapServerInfo MapServerInfo
        {
            get;
        }
        /// <summary>
        /// 获取地图控件
        /// </summary>
        Control Map
        {
            get;
        }
        /// <summary>
        /// 获取缩略图控件
        /// </summary>
        Control OverviewMap
        {
            get;
        }
        /// <summary>
        /// 比例尺缩放控件
        /// </summary>
        Control Navigation
        {
            get;
        }
        /// <summary>
        /// 比例尺
        /// </summary>
        Control ScaleLine
        {
            get;
        }
        /// <summary>
        /// 获取地图当前范围
        /// </summary>
        /// <returns>地图范围</returns>
        MapExtent CurrentMapExtent
        {
            get;
        }
        /// <summary>
        /// 获取或设置地图是否可以平移
        /// </summary>
        bool PanEnabled
        {
            get;
            set;
        }
        /// <summary>
        /// 获取地图当前缩放比例
        /// </summary>
        double CurrentScale
        {
            get;
        }
        /// <summary>
        /// 获取地图当前分辨率
        /// </summary>
        double CurrentResolution
        {
            get;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 地图初始化完毕事件
        /// </summary>
        event Action<bool> Initialized;
        /// <summary>
        /// 地图区域范围已发生变化后事件
        /// </summary>
        event MapExtentChangeHandler MapExtentChanged;
        /// <summary>
        /// 地图区域范围发生变化时事件
        /// </summary>
        event MapExtentChangeHandler MapExtentChanging;
        /// <summary>
        /// 在地图上鼠标移动事件
        /// </summary>
        event MapMouseEventHandler MapMouseMove;
        event Action<double> ScaleChanged;
        #endregion
    }

    public interface IMapInitialize
    {
        /// <summary>
        /// 初始化地图站点接口
        /// </summary>
        /// <param name="pMap">基本地图接口</param>
        void Initialize(IMap pMap);
    }
}
