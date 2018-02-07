#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：设备查询结构
 * 日  期：2016-08-12
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentQueryCondition
    {
        /// <summary>
        /// 活动Guid
        /// </summary>
        public string ActivityGuid { get; set; }

        /// <summary>
        /// 单位Guid 
        /// </summary>
        public string ORGguid { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string ORGName { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string Businesstype { get; set; }

        /// <summary>
        /// 地点GUID
        /// </summary>
        public string PlaceGuid { get; set; }


        /// <summary>
        /// 设备名称
        /// </summary>
        public string EuqName { get; set; }

        /// <summary>
        /// 发射频率开始
        /// </summary>
        public double? SendFreqLittle { get; set; }

        /// <summary>
        /// 发射频率结束
        /// </summary>
        public double? SendFreqGreat { get; set; }

        /// <summary>
        /// 发射频率开始
        /// </summary>
        public double? ReciveFreqLittle { get; set; }

        /// <summary>
        /// 发射频率结束
        /// </summary>
        public double? ReciveFreqGreat { get; set; }

        private bool isStation = true;
        /// <summary>
        /// 已建站
        /// </summary>
        public bool IsStation
        {
            get { return isStation; }
            set { isStation = value; }
        }

        /// <summary>
        /// 已建站名称
        /// </summary>
        public string StationName { get; set; }

        private bool isMobile;
        /// <summary>
        /// 已建站
        /// </summary>
        public bool IsMobile
        {
            get { return isMobile; }
            set { isMobile = value; }
        }

        private bool isTunAble;
        /// <summary>
        /// 频率可调
        /// </summary>
        public bool IsTunAble
        {
            get { return isTunAble; }
            set { isTunAble = value; }
        }

        private SendQueryCondition sendCondition = new SendQueryCondition();
        /// <summary>
        /// 发射查询参数
        /// </summary>
        public SendQueryCondition SendCondition
        {
            get { return sendCondition; }
            set { sendCondition = value; }
        }

        /// <summary>
        /// 接收查询参数
        /// </summary>
        private ReceiveQueryCondition receiveCondition = new ReceiveQueryCondition();
        public ReceiveQueryCondition ReceiveCondition
        {
            get { return receiveCondition; }
            set { receiveCondition = value; }
        }
    }
}
