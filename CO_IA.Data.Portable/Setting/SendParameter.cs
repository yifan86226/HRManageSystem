using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 发射参数
    /// </summary>
    public class SendParameter
    {
        /// <summary>
        /// 发射频率
        /// </summary>
        public double? SendFreq { get; set; }

        /// <summary>
        /// 频率可调
        /// </summary>
        public bool IsTunAble { get; set; }


        public double? FreqStart
        {
            get;
            set;
        }
        public double? FreqEnd
        {
            get;
            set;
        }

        /// <summary>
        /// 最大功率
        /// </summary>
        public double? MaxPower { get; set; }

        /// <summary>
        /// 带宽
        /// </summary>
        public double? Band { get; set; }

        /// <summary>
        /// 波道间隔
        /// </summary>
        public double? ChannelBand { get; set; }

        /// <summary>
        /// 邻道泄露
        /// Adjacent
        /// </summary>
        public double? Leakage { get; set; }

        /// <summary>
        /// 调制方式
        /// </summary>
        public EMCS.Types.EMCModulationEnum? ModulateMode { get; set; }

        Antenna ant = new Antenna();
        /// <summary>
        /// 天线信息
        /// </summary>
        public Antenna Ant
        {
            get { return ant; }
            set { ant = value; }
        }
    }
}
