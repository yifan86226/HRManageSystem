using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    public enum TaskFormat
    {
        [AT_BC.Types.EnumDisplayName("文本")]
        Text,

        [AT_BC.Types.EnumDisplayName("语音")]
        Phonetic
    }
}
