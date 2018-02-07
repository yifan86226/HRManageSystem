using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    public enum DisturbType
    {
        [EnumDisplayNameAttribute("其他")]
        Other = 0,
        [EnumDisplayNameAttribute("语音")]
        Voice = 1,
        [EnumDisplayNameAttribute("噪音")]
        Noise = 2,
        [EnumDisplayNameAttribute("数据")]
        Data = 3
    }
}
