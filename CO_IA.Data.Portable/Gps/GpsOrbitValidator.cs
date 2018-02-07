using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Gps
{
    public class GpsOrbitValidator
    {
        private IGpsDataValidationParam Para;
        /// <summary>
        /// 存储点
        /// </summary>
        private List<GpsOrbit> havePoints = new List<GpsOrbit>();

        private GpsOrbitValidator(IGpsDataValidationParam _para)
        {
            Para = _para;
        }
        public static GpsOrbitValidator CreateGpsOrbitValidator(IGpsDataValidationParam para)
        {
            return new GpsOrbitValidator(para);
        }
        /// <summary>
        /// 清除已有点集
        /// </summary>
        public void Init()
        {
            havePoints.Clear();
        }
        /// <summary>
        /// 过滤GPS点
        /// </summary>
        /// <param name="gpsInfo"></param>
        /// <returns></returns>
        public bool Filter(GpsOrbit gpsInfo)
        {
            bool Re = false;
            if (gpsInfo == null)
                return false;
            if (havePoints.Count == 0)
            {
                Re = true;
            }
            else
            {
                double dis = GetDistance(gpsInfo.Longitude, gpsInfo.Latitude, havePoints[havePoints.Count - 1].Longitude, havePoints[havePoints.Count - 1].Latitude);

                if (dis >= Para.Displacement)
                    Re = true;
                else
                {
                    if (dis > Para.DisplacementReferToAzimuth && havePoints.Count > 2)
                    { //判断方位角
                        double a1 = GetAzimuthAngle(gpsInfo.Longitude, gpsInfo.Latitude, havePoints[havePoints.Count - 1].Longitude, havePoints[havePoints.Count - 1].Latitude);
                        double a2 = GetAzimuthAngle(gpsInfo.Longitude, gpsInfo.Latitude, havePoints[havePoints.Count - 2].Longitude, havePoints[havePoints.Count - 2].Latitude);
                        if (Math.Abs(a2 - a1) > Para.RefAzimuth)
                        {
                            return true;
                        }
                    }
                }
            }
            if (Re)
            {
                havePoints.Add(gpsInfo);
                if (havePoints.Count > 3)
                {
                    havePoints.RemoveAt(0);
                }
            }

            return Re;
        }
        #region 计算距离
        /// <summary>
        ///获取两点间距离 单位：米
        /// </summary>
        /// <param name="from">起点（经纬度）</param>
        /// <param name="to">终点（经纬度）</param>
        /// <returns>距离 单位：米</returns>
        public double GetDistance(double X1, double Y1, double X2, double Y2)
        {
            double SemiMajor = 6378137;
            double SemiMinor = 6356752.314245;

            double y = Y1;
            double x = X1;
            double y2 = Y2;
            double x2 = X2;
            double num1 = SemiMajor;
            double num2 = SemiMinor;
            double num3 = (SemiMajor - SemiMinor) / SemiMajor;
            double num4 = (x2 - x) * 0.0174532925;
            double num5 = Math.Atan((1.0 - num3) * Math.Tan(y * 0.0174532925));
            double num6 = Math.Atan((1.0 - num3) * Math.Tan(y2 * 0.0174532925));
            double num7 = Math.Sin(num5);
            double num8 = Math.Cos(num5);
            double num9 = Math.Sin(num6);
            double num10 = Math.Cos(num6);
            double num11 = num4;
            int num12 = 100;
            double num15;
            double num16;
            double num17;
            double num19;
            double num20;
            while (true)
            {
                double num13 = Math.Sin(num11);
                double num14 = Math.Cos(num11);
                num15 = Math.Sqrt(num10 * num13 * (num10 * num13) + (num8 * num9 - num7 * num10 * num14) * (num8 * num9 - num7 * num10 * num14));
                if (num15 == 0.0)
                {
                    break;
                }
                num16 = num7 * num9 + num8 * num10 * num14;
                num17 = Math.Atan2(num15, num16);
                double num18 = num8 * num10 * num13 / num15;
                num19 = 1.0 - num18 * num18;
                num20 = num16 - 2.0 * num7 * num9 / num19;
                if (double.IsNaN(num20))
                {
                    num20 = 0.0;
                }
                double num21 = num3 / 16.0 * num19 * (4.0 + num3 * (4.0 - 3.0 * num19));
                double num22 = num11;
                num11 = num4 + (1.0 - num21) * num3 * num18 * (num17 + num21 * num15 * (num20 + num21 * num16 * (-1.0 + 2.0 * num20 * num20)));
                if (Math.Abs(num11 - num22) <= 1E-12 || --num12 <= 0)
                {
                    goto IL_212;
                }
            }
            return 0.0;
        IL_212:
            if (num12 == 0)
            {
                return double.NaN;
            }
            double num23 = num19 * (num1 * num1 - num2 * num2) / (num2 * num2);
            double num24 = 1.0 + num23 / 16384.0 * (4096.0 + num23 * (-768.0 + num23 * (320.0 - 175.0 * num23)));
            double num25 = num23 / 1024.0 * (256.0 + num23 * (-128.0 + num23 * (74.0 - 47.0 * num23)));
            double num26 = num25 * num15 * (num20 + num25 / 4.0 * (num16 * (-1.0 + 2.0 * num20 * num20) - num25 / 6.0 * num20 * (-3.0 + 4.0 * num15 * num15) * (-3.0 + 4.0 * num20 * num20)));
            double num27 = num2 * num24 * (num17 - num26);
            return Math.Round(num27, 3);
        }
        #endregion

        #region 计算方位角
        /// <summary>
        /// 计算两点间方位角
        /// </summary>
        /// <param name="pX1">起始点经度</param>
        /// <param name="pY1">起始点纬度</param>
        /// <param name="pX2">结束点经度</param>
        /// <param name="pY2">结束点纬度</param>
        /// <returns>从起始点到终止点的方位角</returns>
        public double GetAzimuthAngle(double pX1, double pY1, double pX2, double pY2)
        {
            double dDelta = CalcuAngleCenterEarth(pX1, pY1, pX2, pY2);

            pX1 = 2 * Math.PI * pX1 / 360;
            pY1 = 2 * Math.PI * pY1 / 360;
            pX2 = 2 * Math.PI * pX2 / 360;
            pY2 = 2 * Math.PI * pY2 / 360;

            double dAzimuth1 = Math.Acos((Math.Sin(pY2) - Math.Sin(pY1) * Math.Cos(dDelta)) / (Math.Sin(dDelta) * Math.Cos(pY1)));
            if (pX1 - pX2 > 0)
            {
                dAzimuth1 = 2 * Math.PI - dAzimuth1;
            }
            return dAzimuth1 * 180 / Math.PI;
        }
        #endregion

        #region 计算地心角
        /// <summary>
        /// 计算两点地心角
        /// </summary>
        /// <param name="pX1">起始点经度</param>
        /// <param name="pY1">起始点纬度</param>
        /// <param name="pX2">结束点经度</param>
        /// <param name="pY2">结束点纬度</param>
        /// <returns>两点间地心角</returns>
        public double CalcuAngleCenterEarth(double pX1, double pY1, double pX2, double pY2)
        {
            //计算a，b点的地心角
            pX1 = 2 * Math.PI * pX1 / 360;
            pY1 = 2 * Math.PI * pY1 / 360;
            pX2 = 2 * Math.PI * pX2 / 360;
            pY2 = 2 * Math.PI * pY2 / 360;
            //两点间地心角公式
            double dDelta = Math.Acos(Math.Sin(pY1) * Math.Sin(pY2) + Math.Cos(pY1) * Math.Cos(pY2) * Math.Cos(pX1 - pX2));
            return dDelta;
        }
        #endregion
    }
}
