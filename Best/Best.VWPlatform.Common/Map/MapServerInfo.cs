using Best.VWPlatform.Common.Types;
using ESRI.ArcGIS.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    /// <summary>
    /// 地图服务信息类
    /// </summary>
    public class MapServerInfo : IMapServerInfo
    {
        public MapServerInfo(JObject pJson)
        {
            MapName = pJson["mapName"].Value<string>();
            JArray layers = pJson["layers"].Value<JArray>();
            List<ILayerInfo> layersList = (from JObject layer in layers select new LayerInfo(layer)).Cast<ILayerInfo>().ToList();
            Layers = layersList;
            JToken reference = pJson["spatialReference"].Value<JObject>()["wkid"];
            if (reference == null)
            {
                reference = pJson["spatialReference"].Value<JObject>()["wkt"];
            }
            SpatialReference = reference.Value<string>();
            if (string.IsNullOrWhiteSpace(SpatialReference))
                throw new Exception("地图服务配置错误，缺少“空间参考”数据。");
            SingleFusedMapCache = pJson["singleFusedMapCache"].Value<bool>();
            string units = pJson["units"].Value<string>();
            switch (units.ToLower())
            {
                case "esrimeters":
                    Units = MapUnits.Meters;
                    break;
                case "esridecimaldegrees":
                    Units = MapUnits.Degrees;
                    break;
            }
            if (SingleFusedMapCache)
                TileInfo = new MapTileInfo(pJson["tileInfo"].Value<JObject>());
            JObject initialExtent = pJson["initialExtent"].Value<JObject>();
            InitialExtent = new MapExtent(
                new MapPointEx(initialExtent["xmin"].Value<double>(), initialExtent["ymax"].Value<double>()),
                new MapPointEx(initialExtent["xmax"].Value<double>(), initialExtent["ymin"].Value<double>()));
            JObject fullExtent = pJson["fullExtent"].Value<JObject>();
            FullExtent = new MapExtent(
                new MapPointEx(fullExtent["xmin"].Value<double>(), fullExtent["ymax"].Value<double>()),
                new MapPointEx(fullExtent["xmax"].Value<double>(), fullExtent["ymin"].Value<double>()));
        }

        public string MapName { get; private set; }

        public IEnumerable<ILayerInfo> Layers { get; private set; }

        public string SpatialReference { get; private set; }

        public bool SingleFusedMapCache { get; private set; }

        public IMapTileInfo TileInfo { get; private set; }

        /// <summary>
        /// 地图单位，Meters，Degrees
        /// </summary>
        public MapUnits Units { get; private set; }

        public MapExtent InitialExtent { get; private set; }

        public MapExtent FullExtent { get; private set; }

        public string GetLayerUrl(string pLayerName)
        {
            var foundLayer = from i in Layers
                             where i.Name == pLayerName
                             select i;
            if (!foundLayer.Any())
                return string.Empty;
            return string.Format("{0}/{1}", VWPlatformConfig.Current.MapConfig.MapDefaultUrl, foundLayer.First().Id);
        }
    }
}
