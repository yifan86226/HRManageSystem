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
    public enum EnTryModeEnum
    {
        /// <summary>
        /// 空闲
        /// </summary>
        [EnumDisplayNameAttribute("管理端")]
        管理端 = 0,
        /// <summary>
        /// 已占
        /// </summary>
        [EnumDisplayNameAttribute("采集端")]
        采集端 = 1
    }
}
