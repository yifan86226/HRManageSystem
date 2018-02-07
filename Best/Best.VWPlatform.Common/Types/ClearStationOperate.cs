using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 清除站点操作
    /// </summary>
    public enum ClearStationOperate
    {
        /// <summary>
        /// 清除所有站点
        /// </summary>
        All,
        /// <summary>
        /// 清除当前可见范围以外的所有站点
        /// </summary>
        Overstep
    }
}
