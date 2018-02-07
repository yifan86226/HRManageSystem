using Best.VWPlatform.Common.Map;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 地图坐标
    /// </summary>
    public class MapPointEx : IEquatable<MapPointEx>
    {
        private double _x;
        private double _y;

        [JsonConstructor]
        public MapPointEx()
        {

        }

        public MapPointEx(double x, double y)
        {
            _x = MapUtile.MercatorToLon(x);
            _y = MapUtile.MercatorToLat(y);
        }
        /// <summary>
        /// 获取或设置X坐标（墨卡托值或经纬度值）
        /// </summary>
        public double X
        {
            get { return _x; }
            set
            {
                _x = MapUtile.MercatorToLon(value);
            }
        }
        /// <summary>
        /// 获取或设置Y坐标（墨卡托值或经纬度值）
        /// </summary>
        public double Y
        {
            get { return _y; }
            set
            {
                _y = MapUtile.MercatorToLat(value);
            }
        }

        /// <summary>
        /// 获取X坐标（根据当前地图的空间引用类型，返回墨卡托值或经纬度值）
        /// </summary>
        public double MapX
        {
            get { return MapUtile.LonToMercator(_x); }
        }
        /// <summary>
        /// 获取Y坐标（根据当前地图的空间引用类型，返回墨卡托值或经纬度值）
        /// </summary>
        public double MapY
        {
            get { return MapUtile.LatToMercator(_y); }
        }

        public MapPointEx Clone()
        {
            return (MapPointEx)MemberwiseClone();
        }

        public bool Equals(MapPointEx other)
        {
            return _x.Equals(other._x) && _y.Equals(other._y);
        }
    }
}
