using System;
using System.Collections.Generic;
using System.Linq;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Data.TaskManage;
using I_CO_IA.PersonPlan;
using I_CO_IA.TaskManage;

namespace CO_IA.UI.Scene
{
    public class DataOperator
    {
        /// <summary>
        /// 根据用户ID获取所在组织信息
        /// </summary>
        /// <returns></returns>
        public static PP_OrgInfo GetOrgInfoByUserID()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PersonPlan, PP_OrgInfo>(channel =>
            {
                return channel.GetPP_OrgInfoByPersonID(RiasPortal.Current.UserSetting.UserID);
            });
        }
        /// <summary>
        /// 保存任务回执消息
        /// </summary>
        /// <param name="p_taskGuid"></param>
        /// <param name="p_receiptMsg"></param>
        /// <returns></returns>
        public static bool SaveTaskReceiptMsg(string p_taskGuid,string p_receiptMsg)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Task, bool>(channel =>
            {
                return channel.SaveTaskReceipt(p_taskGuid, p_receiptMsg);
            });
        }

        public static TaskListInfo[] GetTaskListInfosByParam(TaskQueryList obj)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.TaskManage.I_CO_IA_Task, TaskListInfo[]>(channel =>
            {
               return channel.GetTaskListParam(obj);
            });
        }
    }
}
