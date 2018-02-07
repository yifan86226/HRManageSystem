#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：接收机查询参数
 * 日  期：2016-08-19
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ReceiveQueryCondition
    {
        public double? FreqStartLitte { get; set; }

        public double? FreqStartGreat { get; set; }

        public double? FreqEndLitte { get; set; }

        public double? FreqEndGreat { get; set; }
    }
}
