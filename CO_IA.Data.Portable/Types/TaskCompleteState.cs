using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    public enum TaskCompleteState
    {
        [AT_BC.Types.EnumDisplayName("拒绝执行")]
        Refused = 0,
        [AT_BC.Types.EnumDisplayName("完成")]
        Completed = 1,
        [AT_BC.Types.EnumDisplayName("部分完成")]
        PartCompleted = 2
    }
}
