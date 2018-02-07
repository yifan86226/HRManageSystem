using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT_BC.Types;

namespace CO_IA.Data.Collection
{
    /// <summary>
    /// 信号类别枚举
    /// </summary>
    public enum SignalTypeEnum
    {
        /// <summary>
        /// 空闲
        /// </summary>
        [EnumDisplayNameAttribute("空闲")]
        空闲 = 0,
        /// <summary>
        /// 已占
        /// </summary>
        [EnumDisplayNameAttribute("已占")]
        已占 = 1,
        /// <summary>
        /// 未知
        /// </summary>
        [EnumDisplayNameAttribute("清理")]
        清理 = 2
    }
}
