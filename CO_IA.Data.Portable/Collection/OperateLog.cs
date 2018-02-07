using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Collection
{
    public class OperateLog
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string  Guid { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Operater { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperateDate { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public OperateTypeEnum OperateType { get; set; }
        /// <summary>
        /// 操作表名
        /// </summary>
        public string OperateTables { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    public enum OperateTypeEnum
    { 
        UpLoad,
        DownLoad,
        Initialize,
        Other
    }
}
