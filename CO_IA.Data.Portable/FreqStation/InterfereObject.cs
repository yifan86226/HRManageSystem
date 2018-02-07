using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 干扰设备、或者干扰台站
    /// </summary>
    public class InterfereObject
    {
        /// <summary>
        /// 干扰物Guid
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 干扰物名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 干扰物类别
        /// </summary>
        public InterfereObjectEnum Type { get; set; }

        /// <summary>
        /// 干扰物频率
        /// </summary>
        public double? Freq { get; set; }

        /// <summary>
        /// 备用频率
        /// </summary>
        public double? SpareFreq { get; set; }

        /// <summary>
        /// 干扰物带宽
        /// </summary>
        public double? Band { get; set; }

        /// <summary>
        /// 干扰频率
        /// </summary>
        public string InterfFreq
        {
            get;
            set;
        }
    }
}
