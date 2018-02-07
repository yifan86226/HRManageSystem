using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MonitorStationInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string LG { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string LA { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string Type { get; set; }
    }
}
