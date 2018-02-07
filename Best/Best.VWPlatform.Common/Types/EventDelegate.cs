using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 地图范围发生变化事件委托类型
    /// </summary>
    /// <param name="p_args"></param>
    public delegate void MapExtentChangeHandler(MapExtentEventArgs p_args);
    /// <summary>
    /// 地图上鼠标事件委托类型
    /// </summary>
    /// <param name="p_args"></param>
    public delegate void MapMouseEventHandler(MapMouseEventArgs p_args);
    /// <summary>
    /// 地图上站点鼠标事件委托类型
    /// </summary>
    /// <param name="p_args"></param>
    public delegate void MapStationMouseEventHandler(MapStationMouseEventArgs p_args);
    /// <summary>
    /// 地图上用鼠标获取图形事件委托类型
    /// </summary>
    /// <param name="p_args">鼠标获取图形事件委托类型参数</param>
    public delegate void MapGeometryTrackedEventHandler(MapGeometryTrackedArgs p_args);
}
