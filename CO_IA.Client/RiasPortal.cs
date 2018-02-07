#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：重大活动入口,提供系统信息记录及通用数据获取功能
 * 日 期 ：2016-08-12
 ***************************************************************#@#***************************************************************/
#endregion

using AT_BC.Data;
using CO_IA.Data;
 
using System;
using System.Collections.Generic;
using System.Linq;
namespace CO_IA.Client
{
    /// <summary>
    /// 重大活动入口,提供系统信息记录及通用数据获取功能
    /// </summary>
    public sealed class RiasPortal : AT_BC.SystemPortal.Portal<IRiasModuleContainer>
    {
        //static RiasPortal()
        //{
        //    Current.OnModuleContainerChanged += OnModuleContainerChanged;
        //}

        /// <summary>
        /// 获取当前活动地点信息
        /// </summary>
        /// <returns>当前活动地点信息</returns>
        //public static CO_IA.Data.ActivityPlace[] GetCurrentActivityPlaces()
        //{
        //    if (RiasPortal.ModuleContainer.Activity==null)
        //    {
        //        throw new Exception("获取活动区域异常,没有打开任何活动");
        //    }
        //    return Utility.GetPlaces(RiasPortal.ModuleContainer.Activity.Guid);
        //}

        /// <summary>
        /// 获取地图默认显示区域
        /// </summary>
        /// <returns>地图默认显示区域</returns>
        public static Range<GeoPoint> GetMapDefaultArea()
        {
            return GetSessionParam<Range<GeoPoint>>(MapDefaultArea);
        }

        /// <summary>
        /// 获取地图图形服务地址
        /// </summary>
        /// <returns>地图图形服务地址</returns>
        public static string GetMapGeometryServerUrl()
        {
            return GetSessionParam<string>(MapGeometryServerUrl);
        }

        /// <summary>
        /// 地图默认区域键值定义
        /// </summary>
        public const string MapDefaultArea = "MapDefaultArea";

        /// <summary>
        /// 地图图形服务地址键值定义
        /// </summary>
        public const string MapGeometryServerUrl = "GeometryServerUrl";

        private static readonly List<string> dutyList=new List<string>();

        private static List<string> DutyList
        {
            get
            {
                return dutyList;
            }
        }

        private static List<string> QueryRightList = new List<string>();

        private static List<string> ManageRightList = new List<string>();

        public static RightState VerifyRight(string rightName)
        {
            if (ManageRightList.Contains(rightName))
            {
                return RightState.Manage;
            }
            else if (QueryRightList.Contains(rightName))
            {
                return RightState.Query;
            }
            else
            {
                return RightState.None;
            }
        }

        public static void UpdateUserRights(PP_OrgInfo orgInfo=null)
        {
            //var activity = ModuleContainer.Activity;
            //if (activity != null)
            //{
            //    DutyList.Clear();
            //    PP_OrgInfo userInfo = orgInfo;
            //    if (userInfo == null)
            //    {
            //        userInfo = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PersonSchedule, PP_OrgInfo>(channel =>
            //        {
            //            return channel.GetPP_OrgInfoByPersonID(RiasPortal.Current.UserSetting.UserID, activity.Guid);
            //        });
            //    }
            //    if (userInfo != null)
            //    {
            //        if (!string.IsNullOrWhiteSpace(userInfo.DUTY))
            //        {
            //            DutyList.AddRange(userInfo.DUTY.Split(','));

            //            var duties = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, PP_DutyInfo[]>(channel =>
            //            {
            //                return channel.GetDutyInfos();
            //            });
            //            if (duties != null && duties.Length > 0)
            //            {
            //                List<string> queryRightList = new List<string>();
            //                List<string> manageRightList = new List<string>();
            //                foreach (string dutyCode in DutyList)
            //                {
            //                    PP_DutyInfo dutyInfo = null;
            //                    foreach (var duty in duties)
            //                    {
            //                        if (duty.Key == dutyCode)
            //                        {
            //                            dutyInfo = duty;
            //                            break;
            //                        }
            //                    }
            //                    if (dutyInfo != null)
            //                    {
            //                        if (dutyInfo.QueryRights != null && dutyInfo.QueryRights.Length > 0)
            //                        {
            //                            queryRightList.AddRange(dutyInfo.QueryRights);
            //                        }
            //                        if (dutyInfo.ManageRights != null && dutyInfo.ManageRights.Length > 0)
            //                        {
            //                            manageRightList.AddRange(dutyInfo.ManageRights);
            //                        }
            //                    }
            //                }
            //                ManageRightList = (from right in manageRightList select right).Distinct().ToList();
            //                QueryRightList = (from right in queryRightList select right).Distinct().ToList();
            //            }
            //        }
            //    }
            //}
        }

        public static bool HasDuty(string dutyCode)
        {
            return DutyList.Contains(dutyCode);
        }
    }


    public enum RightState
    {
        None=0,
        Query=1,
        Manage=3
    }
}
