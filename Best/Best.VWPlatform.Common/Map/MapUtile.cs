using Best.VWPlatform.Common.Types;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using Gds;
//using Gds;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Best.VWPlatform.Common.Map
{
    public static class MapUtile
    {
        //地球半径
        private const double EarthRadius = 6378.137;
        private const double Len = 20037508.34;

        /// <summary>
        /// 经纬度十进制度转换为六十进制度分秒表示
        /// </summary>
        /// <param name="pDecimalDegree">十进制度</param>
        /// <returns>[0] - 度, [1] - 分, [2] - 秒</returns>
        public static double[] DecimalDegreeToDegree(double pDecimalDegree)
        {
            var empty = new[] { 0.0, 0, 0 };
            if (double.IsNaN(pDecimalDegree) || double.IsNegativeInfinity(pDecimalDegree) || double.IsPositiveInfinity(pDecimalDegree) || pDecimalDegree == 0)
                return empty;
            try
            {
                pDecimalDegree = MathNoRound(pDecimalDegree, 6);
                string strCoordinate = pDecimalDegree.ToString(CultureInfo.InvariantCulture);
                string[] cs = strCoordinate.Split('.');
                int degree = int.Parse(cs[0]);
                double dMin = double.Parse(string.Format("0.{0}", cs[1])) * 60;
                var min = (int)MathNoRound(dMin, 0);
                double sec = 0;
                cs = dMin.ToString(CultureInfo.InvariantCulture).Split('.');
                if (cs.Length > 1)
                {
                    double dSec = double.Parse(string.Format("0.{0}", cs[1])) * 60;
                    sec = Math.Round(dSec, 1);
                }
                return new[] { degree, min, sec };
            }
            catch
            {
                return empty;
            }
        }

        /// <summary>
        /// 六十进制度转换为十进制度
        /// </summary>
        /// <param name="pDegree">六十进制度</param>
        /// <returns>浮点坐标</returns>
        public static double DegreeToDecimalDegree(string pDegree)
        {
            double min = double.Parse(pDegree.Substring(pDegree.IndexOf('°') + 1, pDegree.IndexOf('′') - pDegree.IndexOf('°') - 1));
            double sec = double.Parse(pDegree.Substring(pDegree.IndexOf('′') + 1, pDegree.IndexOf('″') - pDegree.IndexOf('′') - 1));
            return double.Parse(pDegree.Substring(0, pDegree.IndexOf('°'))) + (min / 60 + sec / 3600);
        }

        /// <summary>
        /// 双精度浮点数截取
        /// </summary>
        /// <param name="pValue">数值</param>
        /// <param name="pDigits">位数</param>
        /// <returns>截取后数值</returns>
        private static double MathNoRound(double pValue, uint pDigits)
        {
            double num = Math.Pow(10.0, pDigits);
            if (pDigits == 0u)
            {
                if (pValue <= 0.0)
                {
                    return Math.Ceiling(pValue);
                }
                return Math.Floor(pValue);
            }
            if (pValue <= 0.0)
            {
                return Math.Ceiling(pValue * num) / num;
            }
            return Math.Floor(pValue * num) / num;
        }

        /// <summary>
        /// 地图位置修正
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double[] OffSetPosition(double lon, double lat)
        {
            CoordOffset coordOffset = CoordOffset.getInstance(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/" + "packfile.dat", false);
            double[] outLL = new double[2];
            coordOffset.offsetCoord(lon, lat, outLL);

            return outLL;
        }
        private static bool IsMercator(double pXy)
        {
            string strXY = pXy.ToString(CultureInfo.InvariantCulture);
            string[] sp = strXY.Split('.');
            return sp[0].Length > 3;
        }

        private static bool IsLonLat(double pXy)
        {
            string strXY = pXy.ToString(CultureInfo.InvariantCulture);
            string[] sp = strXY.Split('.');
            return sp[0].Length <= 3;
        }
        /// <summary>
        /// 经度转墨卡托值
        /// </summary>
        /// <param name="pLon">经度</param>
        /// <returns></returns>
        public static double LonToMercator(double pLon)
        {
            if (Portal.Map.MapServerInfo != null && (IsLonLat(pLon) && Portal.Map.MapServerInfo.Units == MapUnits.Meters))
            {
                return pLon * Len / 180;
            }
            return pLon;
        }
        /// <summary>
        /// 纬度转墨卡托值
        /// </summary>
        /// <param name="pLat">纬度</param>
        /// <returns></returns>
        public static double LatToMercator(double pLat)
        {
            if (Portal.Map.MapServerInfo != null && (IsLonLat(pLat) && Portal.Map.MapServerInfo.Units == MapUnits.Meters))
            {
                double y = Math.Log(Math.Tan((90 + pLat) * Math.PI / 360)) / (Math.PI / 180);
                y = y * Len / 180;
                return y;
            }
            return pLat;
        }

        public static double MercatorToLon(double pLon)
        {
            if (Portal.Map.MapServerInfo != null && (IsMercator(pLon) && Portal.Map.MapServerInfo.Units == MapUnits.Meters))
            {
                return pLon / Len * 180;
            }
            return pLon;
        }

        public static double MercatorToLat(double pLat)
        {
            if (Portal.Map.MapServerInfo != null && (IsMercator(pLat) && Portal.Map.MapServerInfo.Units == MapUnits.Meters))
            {
                double y = pLat / Len * 180;
                y = 180 / Math.PI * (2 * Math.Atan(Math.Exp(y * Math.PI / 180)) - Math.PI / 2);
                return y;
            }
            return pLat;
        }

        /// <summary>
        /// 判断指定位置是否在指定范围内
        /// </summary>
        /// <param name="pExtent">范围</param>
        /// <param name="pPoint">位置</param>
        /// <returns>true - 在该范围</returns>
        public static bool ContainsPoint(MapExtent pExtent, MapPointEx pPoint)
        {
            return pPoint.X >= pExtent.XY1.X
                && pPoint.Y <= pExtent.XY1.Y
                && pPoint.X <= pExtent.XY2.X
                && pPoint.Y >= pExtent.XY2.Y;
        }
    }
}
