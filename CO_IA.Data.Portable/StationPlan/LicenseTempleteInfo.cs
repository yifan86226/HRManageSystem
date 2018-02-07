#region 文件描述
/*********************************************************************************************************************************
 * 创建人：wangxin
 * 摘 要 ：许可证模板实体类
 * 日 期 ：2016-09-07
 *********************************************************************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.StationPlan
{
    public class LicenseTempleteInfo
    {

        /// <summary>
        /// 模板标识
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// 活动标识
        /// </summary>
        public string ActivityGuid { get; set; }

        /// <summary>
        /// 模板XML
        /// </summary>
        public string TempXML { get; set; }

        /// <summary>
        /// 背景图片
        /// </summary>
        public byte[] BGImage { get; set; }

    }
}
