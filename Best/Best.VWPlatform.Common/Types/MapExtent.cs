using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 地图范围
    /// </summary>
    [Serializable]
    public class MapExtent
    {
        public MapExtent(MapPointEx pXY1, MapPointEx pXY2)
        {
            XY1 = new MapPointEx(pXY1.X, pXY1.Y);
            XY2 = new MapPointEx(pXY2.X, pXY2.Y);
        }

        public MapPointEx XY1 { get; set; }
        public MapPointEx XY2 { get; set; }

        public override string ToString()
        {
            if (XY1 == null || XY2 == null)
                return string.Empty;
            return string.Format("{0},{1},{2},{3}", XY1.X, XY1.Y, XY2.X, XY2.Y);
        }
        /// <summary>
        /// 从字符串获取地图范围对象
        /// </summary>
        /// <param name="pMapExtent">地图范围字符串表达式</param>
        /// <returns>MapExtent</returns>
        /// <remarks>
        /// 例如：
        /// x1, y1, x2, y2
        /// 106.12, 26.38, 107.38, 24.38
        /// </remarks>
        public static MapExtent GetMapExtent(string pMapExtent)
        {
            string[] xys = pMapExtent.Split(',');
            if (xys.Length != 4)
                return null;
            return new MapExtent(
                new MapPointEx(double.Parse(xys[0]), double.Parse(xys[1])), new MapPointEx(double.Parse(xys[2]), double.Parse(xys[3])));
        }
    }
}
