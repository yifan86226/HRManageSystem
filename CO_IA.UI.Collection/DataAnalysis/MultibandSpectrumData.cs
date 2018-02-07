using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.DataAnalysis
{
    /// <summary>
    /// 多频段谱图数据（循环调用服务获取数据后添加到本类实例）
    /// </summary>
    public class MultibandSpectrumData
    {
        /// <summary>
        /// 当前服务获取的频点个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 频率值集合
        /// </summary>
        public List<double> FreqValues { get; private set; }

        /// <summary>
        /// 最大幅度值集合
        /// </summary>
        public List<short> MaxDbuvs { get; private set; }

        /// <summary>
        /// 中值幅度集合
        /// </summary>
        public List<short> MedianDbuvs { get; private set; }

        /// <summary>
        /// 噪声幅度集合
        /// </summary>
        public List<short> NoiseDbuvs { get; private set; }

        /// <summary>
        /// 频段内信号集合
        /// </summary>
        public List<ElecEnvFlexGridData> SignalList { get; private set; }

        /// <summary>
        /// 起始频率（单位Hz）
        /// </summary>
        public double StartFreq { get; set; }

        /// <summary>
        /// 步长（单位Hz）
        /// </summary>
        public double Step { get; set; }

        /// <summary>
        /// 终止频率（单位Hz）
        /// </summary>
        public double StopFreq { get; set; }

        public MultibandSpectrumData()
        {
            SignalList = new List<ElecEnvFlexGridData>();
            FreqValues = new List<double>();
            MaxDbuvs = new List<short>();
            MedianDbuvs = new List<short>();
            NoiseDbuvs = new List<short>();
        }
    }
}
