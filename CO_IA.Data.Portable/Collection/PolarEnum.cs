using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT_BC.Types;

namespace CO_IA.Data.Collection
{
    /// <summary>
    /// 极化方式枚举
    /// </summary>
    public enum PolarEnum
    {
        /// <summary>
        /// 水平
        /// </summary>
        [EnumDisplayNameAttribute("水平")]
        H,
        /// <summary>
        /// 垂直
        /// </summary>
        [EnumDisplayNameAttribute("垂直")]
        V,
        /// <summary>
        /// 全向
        /// </summary>
        [EnumDisplayNameAttribute("全向")]
        M
    }
}
