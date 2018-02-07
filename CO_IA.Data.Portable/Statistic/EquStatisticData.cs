using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquStatisticData : StatisticData
    {
        /// <summary>
        /// 单位类别ID
        /// </summary>
        public string ORGGuid { get; set; }

        /// <summary>
        /// 单位类别
        /// </summary>
        public string ORGName { get; set; }

        /// <summary>
        /// 设备
        /// </summary>
        public string Equipment { get; set; }

        /// <summary>
        /// 频点
        /// </summary>
        public string Freq { get; set; }
         
    }
}
