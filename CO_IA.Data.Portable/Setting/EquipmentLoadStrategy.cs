#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：设备查询结构
 * 日  期：2016-08-12
 * ********************************************************************************/
#endregion
using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentLoadStrategy
    {
        /// <summary>
        /// 单位名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 设备名称 
        /// </summary>
        public string EquipmentName { get; set; }

        /// <summary>
        /// 频率范围
        /// </summary>
        public Range<double> FreqRange{ get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string[] EquipClasses { get; set; }

        /// <summary>
        /// 已建站
        /// </summary>
        public MultipleBoolen IsStation
        {
            get;
            set;
        }

        /// <summary>
        /// 已建站名称
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 移动设备
        /// </summary>
        public MultipleBoolen IsMobile
        {
            get;
            set;
        }

        /// <summary>
        /// 频率可调
        /// </summary>
        public MultipleBoolen IsTunable
        {
            get;
            set;
        }
    }
}
