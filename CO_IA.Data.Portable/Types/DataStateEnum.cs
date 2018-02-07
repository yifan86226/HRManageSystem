using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    [Flags]
    public enum DataStateEnum
    {
        /// <summary>
        /// 该行尚未更改
        /// </summary>
        Unchanged = 1,

        /// <summary>
        /// 该行已被添加
        /// </summary>
        Added = 2,

        /// <summary>
        ///   该行已被删除。
        /// </summary>
        Deleted = 4,

        /// <summary>
        /// 该行已被修改
        /// </summary>
        Modified = 8,
    }
}
