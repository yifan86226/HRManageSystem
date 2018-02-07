using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class SecurityGrade : IdentifiableData<string>
    {
        /// <summary>
        /// 获取或设置保证类别说明信息
        /// </summary>
        public string Comments
        {
            get;
            set;
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Orders
        {
            get;
            set;
        }
    }
}
