using System;
using System.Collections.Generic;
using System.Linq;
using CO_IA.Data;
using CO_IA.Data.Monitor;
using CO_IA.Data.MonitorPlan;

namespace CO_IA.UI.MonitorPlan
{
    public class MonitorHelper
    {
        /// <summary>
        /// 获取预案中组名唯一的人员分组的集合
        /// </summary>
        /// <param name="p_monitorPlanInfo"></param>
        /// <returns></returns>
        public static List<Sealed_PP_OrgInfo> GetDistinctGroupByPlan(Sealed_PP_OrgInfo grouplocaction)
        {
            List<PP_OrgInfo> list = new List<PP_OrgInfo>();
            List<PP_OrgInfo> listOrgType = new List<PP_OrgInfo>();
            List<Sealed_PP_OrgInfo> sealed_pp_org = new List<Sealed_PP_OrgInfo>();
            string activityid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            if (activityid != "")
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
                {
                    list = channel.GetPP_OrgInfos(activityid);
                });
                if (list.Count() > 0)
                {
                    listOrgType = list.Where(p => 1 == 1).ToList();
                    foreach (PP_OrgInfo orginfo in listOrgType)
                    {
                        Sealed_PP_OrgInfo currentinfo = new Sealed_PP_OrgInfo();
                        currentinfo.Children = orginfo.Children;
                        currentinfo.GUID = orginfo.GUID;
                        currentinfo.ACTIVITY_GUID = orginfo.ACTIVITY_GUID;
                        
                        currentinfo.NAME = orginfo.NAME;
                        currentinfo.PARENT_GUID = orginfo.PARENT_GUID;
                        currentinfo.Persons = PrototypeDatas.GetOrgOfPerson(orginfo.GUID);
                        sealed_pp_org.Add(currentinfo);
                    }
                }
            }
            return sealed_pp_org;
        }
        //public static List<GroupAndLocation> GetDistinctGroupByPlan(GroupAndLocation grouplocaction)
        //{
        //    List<GroupAndLocation> getgroupList = new List<GroupAndLocation>();
        //    List<PP_OrgInfo> list = new List<PP_OrgInfo>();
        //    List<PP_OrgInfo> listOrgType = new List<PP_OrgInfo>();
        //    List<Sealed_PP_OrgInfo> sealed_pp_org = new List<Sealed_PP_OrgInfo>();
        //    string activityid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
        //    if (activityid != "")
        //    {
        //        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonPlan.I_CO_IA_PersonPlan>(channel =>
        //        {
        //            list = channel.GetPP_OrgInfos(activityid);
        //        });
        //        if (list.Count() > 0)
        //        {
        //            listOrgType = list.Where(p => p.ORG_TYPE == 1).ToList();
        //            foreach (PP_OrgInfo orginfo in listOrgType)
        //            {
        //                Sealed_PP_OrgInfo currentinfo = new Sealed_PP_OrgInfo();
        //                currentinfo.Children = orginfo.Children;
        //                currentinfo.GUID = orginfo.GUID;
        //                currentinfo.ACTIVITY_GUID = orginfo.ACTIVITY_GUID;
        //                currentinfo.DISPLAYNAME = orginfo.DISPLAYNAME;
        //                currentinfo.NAME = orginfo.NAME;
        //                currentinfo.PARENT_GUID = orginfo.PARENT_GUID;
        //                currentinfo.ORG_TYPE = orginfo.ORG_TYPE;
        //                currentinfo.Persons = PrototypeDatas.GetOrgOfPerson(orginfo.GUID);
        //                sealed_pp_org.Add(currentinfo);
        //            }

        //            var groupList = sealed_pp_org.GroupBy(p => p.NAME).ToList();//进行分组操作
        //            foreach (var group in groupList)
        //            {
        //                var personList = group.ToList();
        //                grouplocaction = new GroupAndLocation();
        //                grouplocaction.GroupGuid = personList[0].GUID;
        //                grouplocaction.GroupName = personList[0].NAME;
        //                grouplocaction.Persons = personList[0].Persons;
        //                getgroupList.Add(grouplocaction);
        //            }                  
        //        }
        //    }
        //    return getgroupList;
        //}

        public static T Clone<T>(T obj)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
            serializer.WriteObject(ms, obj);
            ms.Flush();
            ms = new System.IO.MemoryStream(ms.ToArray());
            return (T)serializer.ReadObject(ms);
        }
    }
}
