using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.Data.Portable.Collection
{
    public class FreqLineDataItem
    {
        /// <summary>
        /// 总序号
        /// </summary>
        public int DataIndex { get; set; }

        /// <summary>
        /// 测试任务的开始频点 MHz
        /// </summary>
        public double TestFreqStart { get; set; }
        /// <summary>
        /// 测试任务的结束频点 MHz
        /// </summary>
        public double TestFreqEnd { get; set; }

        public int FreqMeasureId { get; set; }
        public int FreqMeasurePakageId { get; set; }
        public double startFrequency { get; set; }
        public double frequencyStep { get; set; }
        public float[] byteArray { get; set; }
    }
}
