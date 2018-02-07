using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqPlanActivity : FreqPlanSegment
    {
        public FreqPlanActivity()
        {
            Points = new CustomPoint[0];
            StartPoint = new CustomPoint();
            EndPoint = new CustomPoint();
            //QBehavior = QueryBehavior.Polygon;
            ApplyEquipments = new List<ActivityEquipmentInfo>();
            Guid = System.Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 活动地点频率id
        /// </summary>
        public string Guid { get; set; }
        /// <summary>
        /// 申请频点数量
        /// </summary>
        public int ApplyFreqPointNum { get; set; }
        /// <summary>
        /// 申请频点的设备集合
        /// </summary>
        public List<ActivityEquipmentInfo> ApplyEquipments { get; set; }
        /// <summary>
        /// 地点id
        /// </summary>
        public string PlaceId{get;set;}
        /// <summary>
        /// 活动id
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// 活动地点向外扩展距离
        /// </summary>
        public double Distance { get; set; }
        /// <summary>
        /// 地图绘制样式
        /// </summary>
        //public QueryBehavior QBehavior { get; set; }
        /// <summary>
        /// 扩展坐标点集
        /// </summary>
        public CustomPoint[] Points { get; set; }
        /// <summary>
        /// 区域的最小点(左上角)
        /// </summary>
        public CustomPoint StartPoint { get; set; }
        /// <summary>
        /// 区域的最大点(右下角)
        /// </summary>
        public CustomPoint EndPoint { get; set; }
    }
}
