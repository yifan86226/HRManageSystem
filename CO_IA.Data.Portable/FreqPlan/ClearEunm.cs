using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public enum NeedClearEunm
    {
        [EnumDisplayNameAttribute("不需要清理")]
        NotNeedClear = 0,
        [EnumDisplayNameAttribute("需要清理")]
        NeedClear = 1
    }

    public enum ClearResultEnum
    {
        [EnumDisplayNameAttribute("未作处理")]
        NotClear = 0,
        [EnumDisplayNameAttribute("清理成功")]
        ClearSucceed = 1,
        [EnumDisplayNameAttribute("清理失败")]
        ClearFail = 2
    }
}
