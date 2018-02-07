using System;
using System.Collections.Generic;
using System.Linq;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Data.Collection;
using CO_IA.Data.TaskManage;
using CO_IA_Data;
using I_CO_IA.Collection;
using I_CO_IA.PersonSchedule;
using I_CO_IA.PlanDatabase;
using I_CO_IA.FreqStation;

namespace CO_IA.Scene
{
    public class DataOperator
    {
        //internal static List<PP_OrgInfo> GetPP_OrgInfosByActivityID(string p_activityID)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PersonSchedule, List<PP_OrgInfo>>(channel =>
        //    {
        //        return channel.GetPP_OrgInfos(p_activityID);
        //    });
        //}

        ///// <summary>
        ///// 根据用户ID获取所在组织信息
        ///// </summary>
        ///// <returns></returns>
        //internal static PP_OrgInfo GetOrgInfoByUserID()
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PersonSchedule, PP_OrgInfo>(channel =>
        //    {
        //        return channel.GetPP_OrgInfoByPersonID(RiasPortal.Current.UserSetting.UserID,RiasPortal.ModuleContainer.Activity.Guid);
        //    });
        //}

        //internal static List<ActivityEquipmentInfo> GetTaskListInfosByParam(EquipmentQueryCondition obj)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqPlan, List<ActivityEquipmentInfo>>(channel =>
        //    {
        //        return channel.GetEquipmentInfos(obj);
        //    });
        //}

        //internal static List<EmitInfo> GetEmitInfo(string p_placeGuid)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, List<EmitInfo>>(channel =>
        //    {
        //        return channel.QueryEmitInfos(p_placeGuid);
        //    });
        //}
        ///// <summary>
        ///// 根据地点查询频率详细信息列表
        ///// </summary>
        ///// <param name="p_placeGuid"></param>
        ///// <returns></returns>
        //internal static List<FreqPlanActivity> GetFreqPlanActivitysByPlaceId(string p_placeGuid)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanActivity>>(channel =>
        //       {
        //          return channel.GetFreqPlanActivitys(p_placeGuid);
        //       });
        //}
        ///// <summary>
        ///// 根据频率id查询周围台站信息列表
        ///// </summary>
        ///// <param name="p_freqId"></param>
        ///// <returns></returns>
        //internal static List<RoundStationInfo> GetRoundStationByFreqID(string p_freqId)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqPlan, List<RoundStationInfo>>(channel =>
        //    {
        //        return channel.QueryRoundStationsByFreq(p_freqId);
        //    });
        //}
        ///// <summary>
        ///// 根据频率段以及地点ID查询周围非法信号列表
        ///// </summary>
        ///// <param name="p_startFreq">起始频率，单位MHz</param>
        ///// <param name="p_endFreq">结束频率，单位MHz</param>
        ///// <param name="p_placeId"></param>
        ///// <returns></returns>
        //internal static List<AnalysisResult> GetsceneIllegalitySignalsByFreqValue(double p_startFreq, double p_endFreq, string p_placeId)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqPlan, List<AnalysisResult>>(channel =>
        //    {
        //        return channel.GetAnalysisResultList(p_startFreq, p_endFreq, p_placeId);
        //    });
        //}


        internal static ActivityOrganization[] GetORGSource(OrgQueryCondition condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityOrganization[]>(channel =>
            {
                return channel.GetActivityOrgs(condition);
            });
        }
        internal static void DeleteOrgByGuid(string p_orgGuid) 
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(org =>
                   {
                       List<string> list = new List<string>();
                       list.Add(p_orgGuid);
                       org.DeleteActivityOrg(list, RiasPortal.ModuleContainer.Activity.Guid);
                   });
        }
        internal static void SaveActivityEquipment(ActivityEquipment p_equipment)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                channel.SaveActivityEquipment(p_equipment);
            });
        }

        /// <summary>
        ///  查询设备检测
        /// </summary>
        /// <param name="loadcondition"></param>
        /// <returns></returns>
        internal static List<EquipmentInspection> GetEquipmentInspections(EquInspectionQueryCondition loadcondition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<EquipmentInspection>>(channel =>
            {
                return channel.GetEquipmentInspections(loadcondition);
            });
        }
       
        /// <summary>
        /// 查询监测设备列表
        /// </summary>
        /// <param name="p_equipmentLoadStrategy"></param>
        /// <returns></returns>
        internal static ActivityEquipment[] GetEquipments(EquipmentLoadStrategy p_equipmentLoadStrategy)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityEquipment[]>(channel =>
            {
                return channel.GetActivityEquipments(p_equipmentLoadStrategy);
            });
        }
        internal static void DeleteEquipments(string p_equipGuid)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
            {
                List<string> list = new List<string>();
                list.Add(p_equipGuid);
                channel.DeleteActivityEquipment(list);
            });
        }

        internal static ActivityOrganization[] GetActivityOrgSources(OrgQueryCondition condition)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityOrganization[]>(channel =>
            {
                return channel.GetActivityOrgs(condition);
            });
        }
        /// <summary>
        /// 查询该地点周围台站
        /// </summary>
        internal static List<ActivitySurroundStation> GetAroundStations()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<ActivitySurroundStation>>(channel =>
            {
                return channel.GetActivitySurroundStations(SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid, null);
            });
        }
        
        /// <summary>
        /// 根据设备ID查询设备详细信息
        /// </summary>
        /// <param name="p_equipmentID"></param>
        /// <returns></returns>
        internal static ActivityEquipment GetEquipmentByID(string p_equipmentID)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityEquipment>(channel =>
            {
                return channel.GetActivityEquipment(p_equipmentID);
            });  
        }
        /// <summary>
        /// 查询监测频段信息
        /// </summary>
        /// <param name="p_activityGuid"></param>
        /// <param name="p_placeGuid"></param>
        /// <returns></returns>
        internal static  MonitorPlanInfo[] GetMonitorFreqInfos(string p_activityGuid,string p_placeGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, MonitorPlanInfo[]>(channel =>
            {
                return channel.GetMonitorPlansByPlace(SystemLoginService.CurrentActivity.Guid, SystemLoginService.CurrentActivityPlace.Guid);
            });
        }
        internal static void SaveEquipFreq(List<ActivityEquipment> p_changedEquips,Action<bool> p_saveCallback)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                                       {
                                           channel.SaveAssignFreq(p_changedEquips);
                                           if(p_saveCallback!=null)
                                           {
                                               p_saveCallback(true);
                                           }
                                       });
        }
        
    }
}
