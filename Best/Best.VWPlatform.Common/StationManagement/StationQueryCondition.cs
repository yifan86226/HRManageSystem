using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.StationManagement
{
    [Serializable]
    public class StationQueryCondition : ICloneable
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public List<string> BusinessType { set; get; }

        /// <summary>
        /// 行业类型
        /// </summary>
        public List<string> ProType { set; get; }

        /// <summary>
        /// 设台单位（模糊查询）
        /// </summary>
        public string Unit { set; get; }

        /// <summary>
        /// 地区
        /// </summary>
        public List<string> Area { set; get; }

        /// <summary>
        /// 是否包含公共移动
        /// </summary>
        public bool IsPublic { set; get; }

        /// <summary>
        /// 频率范围
        /// </summary>
        public List<ValRange> FreqRanges { set; get; }

        /// <summary>
        /// 功率范围
        /// </summary>
        public ValRange PowerRange { set; get; }


        /// <summary>
        /// 是否包含公众移动
        /// </summary>
        public bool IsContainPublic { set; get; }

        /// <summary>
        /// 地图范围
        /// </summary>
        public MapExtent MapRange { set; get; }
        /// <summary>
        /// 地图显示比例尺级别
        /// </summary>
        public int MapLevel { set; get; }

        /// <summary>
        /// 比例尺
        /// </summary>
        public double MapScale { set; get; }

        public object Clone()
        {
            BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));

            MemoryStream stream = new MemoryStream();

            Formatter.Serialize(stream, this);

            stream.Position = 0;

            object clonedObj = Formatter.Deserialize(stream);

            stream.Close();

            return clonedObj;
        }
    }

    [Serializable]
    public class ValRange
    {
        public double? Start { get; set; }
        ///<summary>
        ///结束值
        ///</summary>
        public double? Stop { get; set; }

        public ValRange(double? pStart, double? pStop)
        {
            this.Start = pStart;
            this.Stop = pStop;
        }
    }
}
