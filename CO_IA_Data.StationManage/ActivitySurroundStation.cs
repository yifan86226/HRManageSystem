using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA_Data
{
    public class ActivitySurroundStation : SurroundStationInfo
    {
        public ActivitySurroundStation()
        {
            FreqPlan = new PlaceFreqPlan();
        }
        /// <summary>
        /// 活动id
        /// </summary>
        public string ActivityId { get; set; }

        public PlaceFreqPlan FreqPlan
        {
            get;
            set;
        }

        /// <summary>
        /// 地点id
        /// </summary>
        public string PlaceId { get; set; }

        /// <summary>
        /// 周围台站
        /// </summary>
        //public List<SurroundStationInfo> SurroundStations
        //{
        //    get;
        //    set;
        //}
    }
}
