using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 切片等级信息
    /// </summary>
    public interface ILodInfo
    {
        /// <summary>
        /// 级别ID
        /// </summary>
        int Level { get; }
        /// <summary>
        /// 分辨率
        /// </summary>
        double Resolution { get; }
        /// <summary>
        /// 比例
        /// </summary>
        double Scale { get; }
    }
}
