using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public enum StatModeEnum
    {
        [EnumDisplayNameAttribute("固定")]
        Fixed = 0,
        [EnumDisplayNameAttribute("移动")]
        Mobile = 1,
        [EnumDisplayNameAttribute("便携")]
        Portable = 2
    }
}
