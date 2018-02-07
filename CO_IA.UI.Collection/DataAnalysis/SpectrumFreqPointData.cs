using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.DataAnalysis
{
    /// <summary>
    /// 谱图频点包含信号数据
    /// </summary>
    public class SpectrumFreqPointData
    {
        /// <summary>
        /// 幅度值、幅度中值
        /// </summary>
        public short Dbuv { get; internal set; }

        /// <summary>
        /// 频率值
        /// </summary>
        public double Freq { get; internal set; }

        /// <summary>
        /// 幅度最大值
        /// </summary>
        public short? MaxDbuv { get; internal set; }

        /// <summary>
        /// 噪声
        /// </summary>
        public short? Noise { get; internal set; }

        /// <summary>
        /// 频点在当前频段的索引
        /// </summary>
        public int Index { get; set; }
    }
}
