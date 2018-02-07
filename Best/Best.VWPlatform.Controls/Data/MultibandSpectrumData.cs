using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Controls.Data
{
    /// <summary>
    /// 多频段谱图数据
    /// </summary>
    public class MultibandSpectrumData
    {
        private int _count;
        private readonly bool _initializeBuffer;
        public MultibandSpectrumData(bool pInitializeBuffer = false)
        {
            _initializeBuffer = pInitializeBuffer;
            SignalDatas = new List<SpectrumSignalData>();
        }

        public bool IsInitializedBuffer
        {
            get { return _initializeBuffer; }
        }
        /// <summary>
        /// 起始频率（单位Hz）
        /// </summary>
        public double StartFreq { get; set; }
        /// <summary>
        /// 终止频率（单位Hz）
        /// </summary>
        public double StopFreq { get; set; }
        /// <summary>
        /// 步长（单位Hz）
        /// </summary>
        public double Step { get; set; }
        /// <summary>
        /// 频点总数
        /// </summary>
        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                if (_initializeBuffer)
                {
                    FreqValues = new double[_count];
                    MaxDbuvs = new short[_count];
                    MedianDbuvs = new short[_count];
                    NoiseDbuvs = new short[_count];
                }
            }
        }
        /// <summary>
        /// 频率值集合
        /// </summary>
        public double[] FreqValues { get; private set; }
        /// <summary>
        /// 最大幅度值集合
        /// </summary>
        public short[] MaxDbuvs { get; private set; }
        /// <summary>
        /// 中值幅度集合
        /// </summary>
        public short[] MedianDbuvs { get; private set; }
        /// <summary>
        /// 噪声幅度集合
        /// </summary>
        public short[] NoiseDbuvs { get; private set; }
        /// <summary>
        /// 频段内信号集合
        /// </summary>
        public List<SpectrumSignalData> SignalDatas { get; private set; }
    }
    /// <summary>
    /// 谱图信号数据
    /// </summary>
    public struct SpectrumSignalData
    {
        /// <summary>
        /// 分析得到的频率值
        /// </summary>
        public double FreqValue { get; set; }
        /// <summary>
        /// 占用带宽
        /// </summary>
        public double BandWidth { get; set; }
        /// <summary>
        /// 最大占用度
        /// </summary>
        public double OccupancyRate { get; set; }
        /// <summary>
        /// 信号性质，1-合法信号；2-非法信号；3-不明信号
        /// </summary>
        public int SignalNature { get; set; }
    }
}
