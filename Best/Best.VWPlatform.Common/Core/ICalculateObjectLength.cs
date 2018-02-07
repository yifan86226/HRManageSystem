using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Core
{
    /// <summary>
    /// 计算对象长度
    /// </summary>
    public interface ICalculateObjectLength
    {
        /// <summary>
        /// 获取实体对象的大小，字节单位
        /// </summary>
        /// <returns>字节数</returns>
        int GetLength();
    }
}
