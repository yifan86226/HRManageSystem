using Best.VWPlatform.Common.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 地图配置
    /// </summary>
    public class MapConfig
    {
        /// <summary>
        /// 获取或设置地图类型
        /// </summary>
        [JsonIgnore]
        public MapInterfaceType MapInterfaceType
        {
            get
            {
                return (MapInterfaceType)Enum.Parse(typeof(MapInterfaceType), VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.地图类型)), true);
            }
        }
        /// <summary>
        /// 获取地图默认范围
        /// </summary>
        [JsonIgnore]
        public MapExtent MapDefaultExtent
        {
            get
            {
                string extent = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.默认地图范围));
                if (string.IsNullOrEmpty(extent))
                    return null;
                string[] es = extent.Split(',');
                if (es.Length != 4)
                    return null;
                return MapExtent.GetMapExtent(extent);
            }
        }

        /// <summary>
        /// 获取地图默认地址
        /// </summary>
        [JsonIgnore]
        public string MapDefaultUrl
        {
            get
            {
                return VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.地图地址));
            }
        }
        /// <summary>
        /// 地图是否支持旋转功能
        /// </summary>
        [JsonIgnore]
        public bool MapRotation
        {
            get
            {
                string rotation = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.地图旋转));
                if (string.IsNullOrWhiteSpace(rotation))
                    return false;
                return rotation == "1";
            }
        }
        /// <summary>
        /// 地图Geometry服务地址
        /// </summary>
        [JsonIgnore]
        public string MapGeometryServiceUrl
        {
            get
            {
                string url;
                url = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.Geometry服务地址));
                return url;
            }
        }
        /// <summary>
        /// 多点最小凸包多边形GP服务地址
        /// </summary>
        [JsonIgnore]
        public string MinBoundaryGPServiceUrl
        {
            get
            {
                string url;
                url = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.Sensor区域GP服务地址));
                return url;
            }
        }
        /// <summary>
        /// 台站(信号)密度图GP服务地址
        /// </summary>
        [JsonIgnore]
        public string DensityGPServiceUrl
        {
            get
            {
                string url;
                url = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.密度图GP服务地址));
                return url;
            }
        }
        /// <summary>
        /// 覆盖图GP服务地址
        /// </summary>
        [JsonIgnore]
        public string CoverageGPServiceUrl
        {
            get
            {
                string url;
                url = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.覆盖图GP服务地址));
                return url;
            }
        }
        /// <summary>
        /// 台站(信号)密度图栅格大小
        /// </summary>
        [JsonIgnore]
        public double DensityCellSize
        {
            get
            {
                string size;
                size = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.密度图栅格大小));
                return double.Parse(size);
            }
        }
        /// <summary>
        /// 台站(信号)密度图栅格大小
        /// </summary>
        [JsonIgnore]
        public double CoverCellSize
        {
            get
            {
                string size;
                size = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.覆盖图栅格大小));
                return double.Parse(size);
            }
        }
        /// <summary>
        /// 获取地图名称
        /// </summary>
        [JsonIgnore]
        public string MapName
        {
            get
            {
                return VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.地图名称));
            }
        }
        /// <summary>
        /// 获取地图坐标显示格式，false - 经纬度浮点值， true - 度分秒值，默认 false
        /// </summary>
        [JsonIgnore]
        public bool DegreeMinSec
        {
            get
            {
                string degree = VWPlatformConfig.Current.QueryParamValue(string.Format("{0}\\{1}", PlatformParameterConstant.系统设置, PlatformParameterConstant.度分秒));
                if (string.IsNullOrEmpty(degree))
                    return false;

                return degree == "1";
            }
        }
    }
}
