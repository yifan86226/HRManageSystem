using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 地图图形基类
    /// </summary>
    public abstract class MapGeometry
    {

    }
    /// <summary>
    /// 地图上的点
    /// </summary>
    public class GeoPoint : MapGeometry
    {
        /// <summary>
        /// 横坐标（经度）
        /// </summary>
        public double X
        {
            set;
            get;
        }
        /// <summary>
        /// 纵坐标（纬度）
        /// </summary>
        public double Y
        {
            set;
            get;
        }
        public bool Equals(GeoPoint p)
        {
            return p.X == X && p.Y == Y;
        }
    }
    /// <summary>
    /// 地图上的圆
    /// </summary>
    public class GeoCircle : MapGeometry
    {
        /// <summary>
        /// 圆心
        /// </summary>
        public GeoPoint Center
        {
            get;
            set;
        }
        /// <summary>
        /// 半径:单位km
        /// </summary>
        public double Radius
        {
            get;
            set;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public GeoCircle()
        {
            Center = new GeoPoint();
        }
    }

    /// <summary>
    /// 地图上的矩形
    /// </summary>
    public class GeoRectangle : MapGeometry
    {
        /// <summary>
        /// 矩形左上角点
        /// </summary>
        public GeoPoint LeftTop
        {
            get;
            set;
        }
        /// <summary>
        /// 地图右下角点
        /// </summary>
        public GeoPoint RightBottom
        {
            get;
            set;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public GeoRectangle()
        {
            LeftTop = new GeoPoint();
            RightBottom = new GeoPoint();
        }

        public bool Contains(double x, double y)
        {
            bool ret = false;
            if (LeftTop.X <= x && RightBottom.X >= x && LeftTop.Y >= y && RightBottom.Y <= y)
                ret = true;
            return ret;
        }
    }
    /// <summary>
    /// 地图多边形
    /// </summary>
    public class GeoPolygon : MapGeometry
    {
        /// <summary>
        /// 多边形的结点
        /// </summary>
        private List<GeoPoint> pointList;
        /// <summary>
        /// 多边形的结点
        /// </summary>
        public List<GeoPoint> PointList
        {
            get
            {
                return pointList;
            }
            set
            {
                pointList = value;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GeoPolygon()
        {
            pointList = new List<GeoPoint>();
        }
    }
}
