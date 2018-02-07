using CO_IA.Data;
using I_CO_IA.FreqPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.FreqQuery
{
    public class FreqQueryHelper
    {
        /// <summary>
        /// 创建频率规划表
        /// </summary>
        /// <returns></returns>
        public static List<BusinessType> GetActivityBusinessType(string ActivityGuid, string PlaceGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqPlan, List<BusinessType>>(channel =>
              {
                  return channel.GetActivityBusinessType(ActivityGuid, PlaceGuid);
              });
        }
    }
}
