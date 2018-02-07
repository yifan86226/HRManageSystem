using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 聚合圆对象
    /// </summary>
    public class ClustererItem : StationInfo
    {
        public ClustererItem(StationInfo pItem)
        {
            this.DicProperties.Add("RSBT_STATION_CACHE.STAT_LG", pItem.DicProperties["RSBT_STATION_CACHE.STAT_LG"]);
            this.DicProperties.Add("RSBT_STATION_CACHE.STAT_LA", pItem.DicProperties["RSBT_STATION_CACHE.STAT_LA"]);
            this.DicProperties.Add("RSBT_STATION_CACHE.COUNT", pItem.DicProperties["RSBT_STATION_CACHE.COUNT"]);
        }

        /// <summary>
        /// 圆半径
        /// </summary>
        public double Radius { get; set; }
    }
}
