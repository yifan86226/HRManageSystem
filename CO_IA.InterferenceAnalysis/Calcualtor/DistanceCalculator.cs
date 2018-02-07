#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：距离计算
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using EMCS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{ /// <summary>
    /// 距离计算器
    /// </summary>
    internal static class DistanceCalculator
    {
        /// <summary>
        /// 圆周率
        /// </summary>
        private const double PI = 3.1415926;

        /// <summary>
        /// 地球半径
        /// </summary>
        private const double earthRadius = 6378d;

        /// <summary>
        /// 两点间最小距离
        /// </summary>
        private const double minDistance = 0.00001;

        /// <summary>
        /// 计算两点之间距离
        /// </summary>
        /// <param name="fromCoordinate">起始点坐标</param>
        /// <param name="toCoordinate">终止点坐标</param>
        /// <returns>两点之间距离(单位:km)</returns>
        public static double GetKmDistance(EMCGeographyCoordinate fromCoordinate, EMCGeographyCoordinate toCoordinate)
        {
            double dalg = 0.0;
            double dala = 0.0;
            double dblg = 0.0;
            double dbla = 0.0;
            if (fromCoordinate != null)
            {
                dalg = 2 * PI * fromCoordinate.Longitude.Value / 360;
                dala = 2 * PI * fromCoordinate.Latitude.Value / 360;
            }
            if (toCoordinate != null)
            {
                dblg = 2 * PI * toCoordinate.Longitude.Value / 360;
                dbla = 2 * PI * toCoordinate.Latitude.Value / 360;
            }

            double result = earthRadius * Math.Sqrt((dbla - dala) * (dbla - dala) + (dblg - dalg) * (dblg - dalg) * Math.Cos(dbla) * Math.Cos(dbla));
            if (result < minDistance)
                result = minDistance;
            return result;
        }
    }
}
