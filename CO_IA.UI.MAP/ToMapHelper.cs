using CO_IA.Client;
using CO_IA.Client.Orgs;
using CO_IA.Data;
using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CO_IA.UI.MAP
{
    public class ToMapHelper
    {
        /// <summary>
        /// 绘制组，获取当前活动时的小组信息
        /// </summary>
        /// <param name="map"></param>
        /// <param name="_activityGuid"></param>
        /// <param name="stageType"></param>
        public static void DrawOrgsToMap(MapGIS map, string _activityGuid, ActivityStage stageType,ContextMenu menu=null)
        {
            map.RemoveElementByFlag(MapGroupTypes.MonitorGroup_.ToString());
            ScheduleDetail[] scheduleDetails = Utility.getOrgGroupsBySchedule(_activityGuid, stageType);
            if (scheduleDetails != null && scheduleDetails.Length > 0)
            {
                foreach (ScheduleDetail detail in scheduleDetails)
                {
                    PP_OrgInfo orginfo = detail.ScheduleOrgs[0].OrgInfo;
                    orginfo.OnLine = null;
                    OrgToMapStyle group = new OrgToMapStyle(orginfo);
                    if (menu != null)
                    {
                        group.ContextMenu = menu; 
                    }
                    map.AddElement(group, map.GetMapPointEx(detail.ScheduleOrgs[0].LONGITUDE, detail.ScheduleOrgs[0].LATITUDE));
                }
            }
        }

    }
}
