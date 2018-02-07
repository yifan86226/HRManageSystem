using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Screen
{
    public class DataHelper
    {
        #region 获取活动地点列表
        public static ActivityPlaceInfo[] GetPlacesByActivityId(string activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo[]>(
            channel =>
            {
                return channel.GetPlaceInfosByActivityId(activityGuid);
            });
        }

        /// <summary>
        /// 获取活动地点列表
        /// </summary>
        /// <param name="_activityGuid">活动guid</param>
        /// <returns></returns>
        public static ActivityPlaceInfoEx[] GetPlacesExByActivityId(string activityGuid)
        {
            ActivityPlaceInfo[] activityPlaces = GetPlacesByActivityId(activityGuid);
            ActivityPlaceInfoEx[] ActivityPlaces = new ActivityPlaceInfoEx[activityPlaces.Length];
            if (activityPlaces != null && activityPlaces.Length > 0)
            {
                for (int i = 0; i < activityPlaces.Length; i++)
                {
                    ActivityPlaces[i] = new ActivityPlaceInfoEx();
                    ActivityPlaces[i].Guid = activityPlaces[i].ActivityGuid;

                    ActivityPlaces[i]._activityPlaceInfo = activityPlaces[i];
                    ActivityPlaces[i].Tips = new ActivityPlaceTips()
                    {
                        FinishValue = 0,
                        PlaceID = activityPlaces[i].ActivityGuid,
                        PlaceName = activityPlaces[i].Name,
                        DsList = new List<KeyValuePair<string, double>>() {
                                    new KeyValuePair<string,double>("已知信号",0),
                                    new KeyValuePair<string,double>("未知信号",0),
                                    new KeyValuePair<string,double>("处理任务",0),
                                    new KeyValuePair<string,double>("干扰数量",0),
                                }
                    };
                }
            }
            else
                ActivityPlaces = null;

            return ActivityPlaces;
        }
        #endregion
    }
}
