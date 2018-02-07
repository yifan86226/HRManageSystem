using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 地图服务信息接口
    /// </summary>
    public interface IMapServerInfo
    {
        /// <summary>
        /// 地图名称
        /// </summary>
        string MapName { get; }
        /// <summary>
        /// 图层列表
        /// </summary>
        IEnumerable<ILayerInfo> Layers { get; }
        /// <summary>
        /// 空间参考
        /// </summary>
        string SpatialReference { get; }
        /// <summary>
        /// 单一融合的地图缓存
        /// </summary>
        bool SingleFusedMapCache { get; }
        /// <summary>
        /// 地图切片信息
        /// </summary>
        IMapTileInfo TileInfo { get; }
        /// <summary>
        /// 坐标系单位
        /// </summary>
        MapUnits Units { get; }
        /// <summary>
        /// 获取初始范围
        /// </summary>
        MapExtent InitialExtent { get; }
        /// <summary>
        /// 获取全图
        /// </summary>
        MapExtent FullExtent { get; }
        /// <summary>
        /// 获取图层URL地址
        /// </summary>
        /// <param name="pLayerName">图层名称</param>
        /// <returns>图层地址</returns>
        string GetLayerUrl(string pLayerName);
    }
    /// <summary>
    /// 地图图层信息
    /// </summary>
    public interface ILayerInfo
    {
        /// <summary>
        /// 图层ID
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 图层名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 父图层ID
        /// </summary>
        string ParentLayerId { get; }
        /// <summary>
        /// 图层是否可见
        /// </summary>
        bool Visibility { get; }
    }
    /// <summary>
    /// 地图切片信息
    /// </summary>
    public interface IMapTileInfo
    {
        /// <summary>
        /// 宽度
        /// </summary>
        int Width { get; }
        /// <summary>
        /// 高度
        /// </summary>
        int Height { get; }
        /// <summary>
        /// DPI
        /// </summary>
        int Dpi { get; }
        /// <summary>
        /// 格式
        /// </summary>
        string Format { get; }
        /// <summary>
        /// 空间参考
        /// </summary>
        string SpatialReference { get; }
        /// <summary>
        /// 原点
        /// </summary>
        MapPointEx Origin { get; }
        /// <summary>
        /// 细节等级，切片等级集合
        /// </summary>
        IEnumerable<ILodInfo> Lods { get; }
    }

}
