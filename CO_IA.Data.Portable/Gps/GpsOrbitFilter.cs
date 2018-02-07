using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Gps
{
    public class GpsOrbitFilter
    {
        public GpsOrbitFilter()
        {
            RunTimeRange = new Range<double>();
            RunTimeRange.Little = 0;
            RunTimeRange.Great = 235959.999;
        }
        public string ActivityId { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime RunDate { get; set; }
        /// <summary>
        /// 时间格式为hhmmss.sss
        /// </summary>
        public Range<double> RunTimeRange { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string PlateNumber { get; set; }
    }
}
