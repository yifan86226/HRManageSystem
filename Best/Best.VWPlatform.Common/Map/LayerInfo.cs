using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{

    /// <summary>
    /// 图层信息类
    /// </summary>
    public class LayerInfo : ILayerInfo
    {
        public LayerInfo(JObject pJson)
        {
            Id = pJson["id"].Value<string>();
            Name = pJson["name"].Value<string>();
            ParentLayerId = pJson["parentLayerId"].Value<string>();
            Visibility = pJson["defaultVisibility"].Value<bool>();
        }

        /// <summary>
        /// 图层ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 图层名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 父图层ID
        /// </summary>
        public string ParentLayerId { get; private set; }

        /// <summary>
        /// 可见性
        /// </summary>
        public bool Visibility { get; private set; }
    }
}
