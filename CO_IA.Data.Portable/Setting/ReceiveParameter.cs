using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 接收信号
    /// </summary>
    public class ReceiveParameter
    {
        /// <summary>
        /// 接收频率
        /// </summary>
        public double? ReceiveFreq { get; set; }

        /// <summary>
        /// 频率开始
        /// </summary>
        public double? FreqStart
        {
            get;
            set;
        }

        /// <summary>
        /// 频率结束
        /// </summary>
        public double? FreqEnd
        {
            get;
            set;
        }

        /// <summary>
        /// 接收机灵敏度
        /// </summary>
        public string Sensitivity { get; set; }

        /// <summary>
        /// 接收机灵敏度单位
        /// </summary>
        public string SensitivityUnit { get; set; }

        /// <summary>
        /// 领道抑制
        /// </summary>
        public double? ADJChannelInh { get; set; }

        /// <summary>
        /// 信噪比
        /// </summary>
        public double? SignalNoise { get; set; }

        /// <summary>
        /// 同道保护
        /// </summary>
        public double? CoChnPro { get; set; }

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
