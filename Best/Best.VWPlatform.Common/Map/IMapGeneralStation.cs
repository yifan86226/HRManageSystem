using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 台站相关地图操作接口
    /// </summary>
    public interface IMapGeneralStation : IMapStation
    {
        /// <summary>
        /// 添加聚合点
        /// </summary>
        /// <param name="pCluster">聚合属性</param>
        void AddClustererPoint(ClustererItem pCluster);
    }
}
