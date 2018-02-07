using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqAssignStatisticData : StatisticData
    {
        /// <summary>
        /// 业务代码
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 单位名称ID
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string CompanyName { get; set; }

        ///// <summary>
        ///// 频段
        ///// </summary>
        //public string FreqPart { get; set; }
      
        ///// <summary>
        ///// 业务代码别名
        ///// </summary>
        //public string STClass { get; set; }
    }
}
