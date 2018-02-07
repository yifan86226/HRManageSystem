#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：发射收机查询参数
 * 日  期：2016-08-19
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class SendQueryCondition
    {
        public double? FreqStartLitte { get; set; }

        public double? FreqStartGreat { get; set; }

        public double? FreqEndLitte { get; set; }

        public double? FreqEndGreat { get; set; }

        public double? BandLitte { get; set; }

        public double? BandGreat { get; set; }

        public double? PowerLitte { get; set; }

        public double? PowerGreat { get; set; }
    }
}
