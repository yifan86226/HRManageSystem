using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 干扰结果信息
    /// </summary>
    public class InterfereResult
    {
        /// <summary>
        /// 干扰类型(同频、邻频、互调)
        /// </summary>
        public InterfereTypeEnum InterfType { get; set; }

        /// <summary>
        /// 干扰阶数（上邻频、下邻频、一阶、二阶、三阶）
        /// </summary>
        public InterfereOrderEnum InterfOrder { get; set; }

        /// <summary>
        /// 干扰物
        /// </summary>
        public List<InterfereObject> InterfObject { get; set; }
    }
}
