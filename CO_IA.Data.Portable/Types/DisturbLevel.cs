using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    public enum DisturbLevel
    {
        [EnumDisplayNameAttribute("轻微干扰")]
        Slight = 0,
        [EnumDisplayNameAttribute("一般干扰")]
        Normal = 1,
        [EnumDisplayNameAttribute("严重干扰")]
        Greatly = 2
    }
}
