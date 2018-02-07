using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class StatisticData
    {
        /// <summary>
        /// 地点ID
        /// </summary>
        public string AddressGuid { get; set; }
        /// <summary>
        /// 地点
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double Count { get; set; }
    }
}
