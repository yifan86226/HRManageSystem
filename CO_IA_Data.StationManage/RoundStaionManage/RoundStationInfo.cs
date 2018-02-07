using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO_IA_Data
{
    [KnownType(typeof(FreqPlanActivity))]
    public class RoundStationInfo : StationInfo
    {
        public RoundStationInfo()
        {
            FreqPart = new FreqPlanSegment();
            EmitInfos = new List<FreqEmitInfo>();
        }
        /// <summary>
        /// 活动地点频率id
        /// </summary>
        public string FreqActivityGuid { get; set; }
        /// <summary>
        /// 地点id
        /// </summary>
        public string PlaceId { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// 频率规划信息
        /// </summary>
        public FreqPlanSegment FreqPart { get; set; }


        private List<FreqEmitInfo> emitinfos;
        /// <summary>
        /// 发射信息
        /// </summary>
        public List<FreqEmitInfo> EmitInfos 
        {
            get
            {
                return emitinfos;
            }
            set
            {
                emitinfos = value;
                NotifyPropertyChanged("EmitInfos");
            }
        }
    }
}
