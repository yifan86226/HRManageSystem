using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    public enum DataLoggingMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        [AT_BC.Types.EnumDisplayName("未知")]
        None,
        /// <summary>
        /// 手工录入
        /// </summary>
        [AT_BC.Types.EnumDisplayName("手工录入")]
        ManualEntry,

        /// <summary>
        /// 生成
        /// </summary>
        [AT_BC.Types.EnumDisplayName("生成")]
        Created,

        /// <summary>
        /// 导入
        /// </summary>
        [AT_BC.Types.EnumDisplayName("导入")]
        Loaded
    }
}
