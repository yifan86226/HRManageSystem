using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    [Flags]
    public enum TaskType
    {
        [AT_BC.Types.EnumDisplayName("未知")]
        None = 0,
        [AT_BC.Types.EnumDisplayName("一般任务")]
        Normal = 1,
        [AT_BC.Types.EnumDisplayName("干扰任务")]
        Disturb = 2,
        [AT_BC.Types.EnumDisplayName("群发任务")]
        Broadcast = 4
    }
}
