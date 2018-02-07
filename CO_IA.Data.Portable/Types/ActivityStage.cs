#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：活动阶段定义
 * 日 期 ：2016-08-11
 ***************************************************************#@#***************************************************************/
#endregion

using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    /// <summary>
    /// 活动阶段定义
    /// </summary>
    [Flags]
    public enum ActivityStage
    {
        /// <summary>
        /// 未知,活动被保存前标记阶段,保存后会更改为准备阶段
        /// </summary>
        [EnumDisplayNameAttribute("未知")]
        None,
        /// <summary>
        /// 准备
        /// </summary>
        [EnumDisplayNameAttribute("准备")]
        Prepare=1,
        /// <summary>
        /// 执行
        /// </summary>
        [EnumDisplayNameAttribute("执行")]
        Execute=2,
        /// <summary>
        /// 总结
        /// </summary>
        [EnumDisplayNameAttribute("总结")]
        Summary=4
    }
}
