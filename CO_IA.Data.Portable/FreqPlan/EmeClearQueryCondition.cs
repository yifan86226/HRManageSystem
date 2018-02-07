using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EmeClearQueryCondition
    {

        ///// <summary>
        ///// 活动Guid
        ///// </summary>
        //public string ActivityGuid { get; set; }

        /// <summary>
        /// 地点Guid
        /// </summary>
        public string PlaceGuid { get; set; }

        ///// <summary>
        ///// 单位Guid 
        ///// </summary>
        //public string ORGguid { get; set; }



        /// <summary>
        /// 单位名称
        /// </summary>
        public string ORGName { get; set; }

        /// <summary>
        /// 台站名称
        /// </summary>
        public string StatName { get; set; }


        /// <summary>
        /// 台站地址
        /// </summary>
        public string StatAddress { get; set; }

        /// <summary>
        /// 单位联系人
        /// </summary>
        public string ORGLinkPerson { get; set; }


        /// <summary>
        /// 单位联系人电话
        /// </summary>
        public string ORGPhone { get; set; }



        ///// <summary>
        ///// 设备名称
        ///// </summary>
        //public string EuqName { get; set; }

        /// <summary>
        /// 发射频率开始
        /// </summary>
        public double? SendFreqLittle { get; set; }

        /// <summary>
        /// 频率结束
        /// </summary>
        public double? SendFreqGreat { get; set; }


        /// <summary>
        /// 接收频率开始
        /// </summary>
        public double? RecvFreqLittle { get; set; }

        /// <summary>
        /// 接收频率结束
        /// </summary>
        public double? RecvFreqGreat { get; set; }

        /// <summary>
        /// 带宽开始
        /// </summary>
        public double? BandLittle { get; set; }

        /// <summary>
        /// 带宽结束
        /// </summary>
        public double? BandGreat { get; set; }

        /// <summary>
        /// 是否需要清理
        /// </summary>
        public string NeedClear { get; set; }     
        
        /// <summary>
        /// 清理结果
        /// </summary>
        public string ClearResult { get; set; }
    }
}
