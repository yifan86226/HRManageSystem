using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    public enum DisturbDisposeType
    {
        [AT_BC.Types.EnumDisplayName("未知")]
        None,
        [AT_BC.Types.EnumDisplayName("关闭")]
        Close,
        [AT_BC.Types.EnumDisplayName("改频")]
        ChangeFreq,
        [AT_BC.Types.EnumDisplayName("协调")]
        Coordinate,
        [AT_BC.Types.EnumDisplayName("其他")]
        Other
    }
}
