using Best.VWPlatform.Common.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    public class MapTileInfo : IMapTileInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pJson">Json对象</param>
        public MapTileInfo(JObject pJson)
        {
            Width = pJson["rows"].Value<int>();
            Height = pJson["cols"].Value<int>();
            Dpi = pJson["dpi"].Value<int>();
            Format = pJson["format"].Value<string>();
            JObject origin = pJson["origin"].Value<JObject>();
            Origin = new MapPointEx(origin["x"].Value<double>(), origin["y"].Value<double>());
            JToken reference = pJson["spatialReference"].Value<JObject>()["wkid"];
            if (reference == null)
            {
                reference = pJson["spatialReference"].Value<JObject>()["wkt"];
            }
            SpatialReference = reference.Value<string>();
            if (string.IsNullOrWhiteSpace(SpatialReference))
                throw new Exception("地图服务配置错误，缺少“空间参考”数据。");

            JArray lods = pJson["lods"].Value<JArray>();
            List<LodInfo> lodList = (from JObject lod in lods select new LodInfo(lod)).ToList();
            Lods = lodList;
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// DPI
        /// </summary>
        public int Dpi { get; private set; }

        /// <summary>
        /// 格式
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// 空间参考
        /// </summary>
        public string SpatialReference { get; private set; }

        /// <summary>
        /// 原点
        /// </summary>
        public MapPointEx Origin { get; private set; }

        /// <summary>
        /// 细节等级，切片等级集合
        /// </summary>
        public IEnumerable<ILodInfo> Lods { get; private set; }
    }
}
