#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：频率使用情况
 * 日  期：2016-09-08
 * ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqUse
    {
        private double freq;
        /// <summary>
        /// 频率
        /// </summary>
        public double Freq
        {
            get
            {
                return freq;
            }
            set
            {
                freq = value;
            }
        }

        private Usage use;
        /// <summary>
        /// 使用情况
        /// </summary>
        public Usage Use
        {
            get
            {
                return use;
            }
            set
            {
                use = value;
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        private string tooltip;
        public string ToolTip
        {
            get
            {
                return tooltip;
            }
            set
            {
                tooltip = value;
            }
        }
    }

    /// <summary>
    /// 使用情况
    /// </summary>
    public enum Usage
    {
        //未使用
        None,
        //已经被其他设备申请
        Applied,
        //合法使用
        Lawful,
        //非法使用
        UnLawful,
        //其他
        Other
    }
}
