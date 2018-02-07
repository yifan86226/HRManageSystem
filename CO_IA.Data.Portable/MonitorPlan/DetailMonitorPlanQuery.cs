using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.MonitorPlan
{
    [Obsolete("DetailMonitorPlanQuery已经过时")]
   public class DetailMonitorPlanQuery
    {
       /// <summary>
       /// 唯一标识
       /// </summary>
       public string GUID { get; set; }
       /// <summary>
       /// 监测组id
       /// </summary>
       public string SENDGROUPIDS { get; set; }
       /// <summary>
       /// 监测地点id
       /// </summary>
       public string WORKPLACEID { get; set; }
       /// <summary>
       /// 重点频段
       /// </summary>
       public string IMPORTFREQUENCYRANGE { get; set; }
       /// <summary>
       /// 活动id
       /// </summary>
       public string ACTIVITY_GUID { get; set; }
    }
}
