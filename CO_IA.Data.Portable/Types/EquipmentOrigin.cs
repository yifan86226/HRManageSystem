#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：设备来源定义
 * 日 期 ：2016-08-16
 ***************************************************************#@#***************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    /// <summary>
    /// 设备来源定义
    /// </summary>
    public enum EquipmentOrigin
    {
        /// <summary>
        /// 未知
        /// </summary>
        None,
        /// <summary>
        /// 手工录入
        /// </summary>
        ManualEntry,

        /// <summary>
        /// Excel导入
        /// </summary>
        ExcelLoad,

        /// <summary>
        /// 设备库
        /// </summary>
        EquipmentDatabase,

        /// <summary>
        /// 标准台站库
        /// </summary>
        StandardStationDatabase
    }
}
