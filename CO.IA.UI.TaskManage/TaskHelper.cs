using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CO_IA.Data;
using CO_IA.Data.TaskManage;

namespace CO.IA.UI.TaskManage
{
    public  class TaskHelper
    {
        //isDigit是否是数字 
        public static bool IsNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public static T Clone<T>(T obj)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
            serializer.WriteObject(ms, obj);
            ms.Flush();
            ms = new System.IO.MemoryStream(ms.ToArray());
            return (T)serializer.ReadObject(ms);
        }

        #region 全部监测任务
        /// <summary>
        /// 获取执行小组名称
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public static string GetGroupName(string groupid)
        {
            PP_OrgInfo getOrginfo = TaskHelper.GetPP_OrgInfo(groupid);
            string personDisplay = getOrginfo.NAME;
            return personDisplay;
        }
        /// <summary>
        /// 查询，监测任务子表
        /// </summary>
        /// <returns></returns>
        //public static CO_IA.Data.TaskManage.FrequencyrangeInfo[] GetFrequencyrangeInfo(TaskListInfo CurrentTaskListInfo)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.FrequencyrangeInfo[]>(
        //         channel =>
        //         {
        //             return channel.GetFrequencyrange(CurrentTaskListInfo.CHILDGUID);
        //         });
        //}
        #endregion

        #region 一般任务
        /// <summary>
        /// 查询，一般任务
        /// </summary>
        /// <returns></returns>
        public static CO_IA.Data.TaskManage.TaskInfo[] GetTaskInfo(TaskListInfo CurrentTaskListInfo)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.TaskInfo[]>(
                 channel =>
                 {
                     return channel.GetTaskInfo(CurrentTaskListInfo.CHILDGUID);
                 });
        }

        public static CO_IA.Data.TaskManage.TaskInfo[] GetTaskInfo(string taskofchildid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.TaskInfo[]>(
                 channel =>
                 {
                     return channel.GetTaskInfo(taskofchildid);
                 });
        }
        #endregion

        #region 干扰任务
        /// <summary>
        /// 查询，干扰任务
        /// </summary>
        /// <returns></returns>
        public static CO_IA.Data.TaskManage.DisturbTaskInfo[] GetDisturbTaskInfo(TaskListInfo CurrentTaskListInfo)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.DisturbTaskInfo[]>(
                 channel =>
                 {
                     return channel.GetDisturbTaskInfo(CurrentTaskListInfo.CHILDGUID);
                 });
        }

        public static CO_IA.Data.TaskManage.DisturbTaskInfo[] GetDisturbTaskInfo(string disturbchilid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.DisturbTaskInfo[]>(
                 channel =>
                 {
                     return channel.GetDisturbTaskInfo(disturbchilid);
                 });
        }
        #endregion

        #region 监测任务
        /// <summary>
        /// 查询，监测任务
        /// </summary>
        /// <returns></returns>
        public static CO_IA.Data.TaskManage.InterimmonitorInfo[] GetInterimmonitorInfo(TaskListInfo CurrentTaskListInfo)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.InterimmonitorInfo[]>(
                 channel =>
                 {
                     return channel.GetInterimmonitorInfo(CurrentTaskListInfo.CHILDGUID);
                 });
        }
        #endregion
        #region 活动相关
        /// <summary>
        /// 根据组id，查询组信息
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static PP_OrgInfo GetPP_OrgInfo(string guid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, CO_IA.Data.PP_OrgInfo>(
               channel =>
               {
                   return channel.GetPP_OrgInfo(guid);
               });
        }


        #endregion

        #region 上传
        /// <summary>
        /// 项目所在路径
        /// </summary>
        /// <returns></returns>
        public static string GetPath()
        {
            string savePath = @"TaskManage\" + FormatNowTime(2) + @"\";
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, savePath);// proPath + @"\Output\" + "textoffice" + savePath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 格式化当前时间: 
        /// 1:yyMMddHHmmss; 2:yyyy-MM\dd\
        /// </summary>
        /// <returns></returns>
        public static string FormatNowTime(int num)
        {
            if (num == 1)
            {
                return DateTime.Now.ToString("yyMM");
            }
            else if (num == 2)
            {
                return DateTime.Now.ToString("yyyy-MM");
            }
            return "";
        }

        /// <summary>
        /// 获取附件信息
        /// </summary>
        /// <returns></returns>
        public static CO_IA.Data.TaskManage.RuleFile[] GetRulesFiilesInfo(string fileid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, CO_IA.Data.TaskManage.RuleFile[]>(
                 channel =>
                 {
                     return channel.GetRulesFile(fileid);
                 });
        }
        #endregion

    }
}
