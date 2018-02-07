using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Setting
{
    public class JHCondition
    {
        /// <summary>
        /// 屏幕范围,长度4  x1, y1, x2, y2
        /// </summary>
        public double[] Range;
        /// <summary>
        /// 经度步长
        /// </summary>
        public double xInterval;
        /// <summary>
        /// 纬度步长
        /// </summary>
        public double yInterval;
        /// <summary>
        /// 偏移格子数
        /// </summary>
        public double CC;
        /// <summary>
        /// 经度偏移量
        /// </summary>
        public double LotdOffset;
        public string SqlWhere;
    }
    public class JHStation
    {
        public string GUID;
        public string NAME;
        public string STAT_STATUS;
        public string NET_SVN;
        public string ST_TYPE;
        public string STAT_LG;
        public string STAT_LA;
        public string XMin;
        public string XMax;
        public string YMin;
        public string YMax;
        public string CO;
    }
}
