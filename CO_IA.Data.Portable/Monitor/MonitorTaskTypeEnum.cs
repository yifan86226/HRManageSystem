using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CO_IA.Data.Monitor
{
    /// <summary>
    /// 监测任务类型
    /// </summary>
    public enum MonitorTaskTypeEnum
    {
        /// <summary>
        /// 预演
        /// </summary>
        [EnumDisplayNameAttribute("预演")]
        Rehearse = 1,
        /// <summary>
        /// 信号采集
        /// </summary>
        [EnumDisplayNameAttribute("信号采集")]
        SignCollect = 0,
        /// <summary>
        /// 现场保障
        /// </summary>
        [EnumDisplayNameAttribute("现场保障")]
        SiteSafeguard = 2
    }
}
