using Newtonsoft.Json.Linq;
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
    public class LodInfo : ILodInfo
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pJson">Json对象</param>
        public LodInfo(JObject pJson)
        {
            Level = pJson["level"].Value<int>();
            Resolution = pJson["resolution"].Value<double>();
            Scale = pJson["scale"].Value<double>();
        }

        /// <summary>
        /// 级别
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 分辨率
        /// </summary>
        public double Resolution { get; private set; }

        /// <summary>
        /// 比例
        /// </summary>
        public double Scale { get; private set; }
    }
}
