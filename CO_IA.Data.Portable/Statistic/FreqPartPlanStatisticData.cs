using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqPartPlanStatisticData:StatisticData
    {        
        /// <summary>
        /// 类别
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 指配频率数量
        /// </summary>
        public int AssignFreqCount { get; set; }

        /// <summary>
        /// 不明频率数量
        /// </summary>
        public int UnknowFreqCount { get; set; }

        /// <summary>
        /// 划分频率数量
        /// </summary>
        public int BandCount { get; set; }

        /// <summary>
        /// 空闲频率数量
        /// </summary>
        public int FreeFreqCount 
        {
            get
            {
                return BandCount - AssignFreqCount - UnknowFreqCount < 0 ? 0 : BandCount - AssignFreqCount - UnknowFreqCount;
            }
        }

        /// <summary>
        /// 业务代码
        /// </summary>
        public string STClassCode { get; set; }

        /// <summary>
        /// 业务代码别名
        /// </summary>
        public string STClass { get; set; }
    }
}
