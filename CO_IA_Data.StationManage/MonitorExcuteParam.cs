using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA_Data.StationManage
{
    /// <summary>
    /// 监测参数  -- add by wrx 20170708 用于提供给监测设备端
    /// </summary>
    public class MonitorExcuteParam
    {
         private List<MonitorPlanInfo> _monitorPlanInfos = new List<MonitorPlanInfo>();
         private List<ActivityEquipment> _activityEquipments = new List<ActivityEquipment>();
         private List<ActivitySurroundStation> _activitySurroundStations = new List<ActivitySurroundStation>();

        /// <summary>
        /// 监测频段信息
        /// </summary>
         public List<MonitorPlanInfo> MonitorPlanInfos
         {
             get 
             { 
                 return _monitorPlanInfos; 
             }
             set 
             { 
                 _monitorPlanInfos = value; 
             }
         } 
        /// <summary>
        /// 监测设备信息
        /// </summary>
         public List<ActivityEquipment> ActivityEquipments
         {
             get 
             { 
                 return _activityEquipments; 
             }
             set 
             { 
                 _activityEquipments = value; 
             }
         }
        /// <summary>
        /// 监测周围台站信息
        /// </summary>
         public List<ActivitySurroundStation> ActivitySurroundStations
         {
             get 
             { 
                 return _activitySurroundStations; 
             }
             set 
             { 
                 _activitySurroundStations = value; 
             }
         }
    }
}
