using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    /// <summary>
    /// 活动步骤
    /// </summary>
    [Flags]
    public enum ActivityStep
    {
        None=0,

        /// <summary>
        /// 地点建立
        /// </summary>
        LocationBuild=0x01,

        /// <summary>
        /// 人员预案
        /// </summary>
        StaffPlanning=0x02,

        /// <summary>
        /// 日程安排
        /// </summary>
        Schedule=0x04,

        /// <summary>
        /// 频率预案
        /// </summary>
        FreqPlanning=0x08,

        /// <summary>
        /// 台站预案
        /// </summary>
        StationPlanning=0x10,

        /// <summary>
        /// 监测预案
        /// </summary>
        MonitorPlanning=0x20,

        /// <summary>
        /// 规章制度发布
        /// </summary>
        RulePublish=0x40,

        /// <summary>
        /// 文件管理
        /// </summary>
        FileManage=0x80,

        /// <summary>
        /// 活动总结
        /// </summary>
        Summary = 0x100
    }

    public static class ActivityStepHelper
    {
        public static ActivityStep GetBitOr()
        {
            var steps = Enum.GetValues(typeof(ActivityStep)) as ActivityStep[];
            ActivityStep result = ActivityStep.None;
            foreach (var step in steps)
            {
                result |= step;
            }
            return result;
        }
    }
}
