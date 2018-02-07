using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 互调干扰结果类
    /// </summary>
    public class CrossFreqResult
    {
        /// <summary>
        /// 互调公式
        /// </summary>
        public string Formula{ get; set; }
        /// <summary>
        /// 互调频率
        /// </summary>
        public double CrossFreq { get; set; }
        /// <summary>
        /// 阶数
        /// </summary>
        public double Order { get; set; }
        /// <summary>
        /// 干扰频率
        /// </summary>
        public double InterferenceFreq { get; set; }
        /// <summary>
        /// 距离拟建站距离
        /// </summary>
        public double Distance { get; set; }
        /// <summary>
        /// 干扰站名称
        /// </summary>
        public string  InterferenceStatName{ get; set; }
        /// <summary>
        /// 干扰站经度
        /// </summary>
        public double InterferenceStatLng { get; set; }
        /// <summary>
        /// 干扰站纬度
        /// </summary>
        public double InterferenceStatLat { get; set; }
    }
}
