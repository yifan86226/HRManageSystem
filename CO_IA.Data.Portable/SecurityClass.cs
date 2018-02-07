#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：保障类别定义
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion

using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 保障类别定义
    /// </summary>
    public class SecurityClass : IdentifiableData<string>
    {
        /// <summary>
        /// 获取或设置保证类别说明信息
        /// </summary>
        public string Comments
        {
            get;
            set;
        }
    }
}
