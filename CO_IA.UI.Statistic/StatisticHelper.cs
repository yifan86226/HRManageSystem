using CO_IA.Client;
using CO_IA.Data;
 

using PT_BS_Service.Client.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CO_IA.UI.Statistic
{
    public class StatisticHelper
    {

        public static Dictionary<string, string> dicActivityPlace;

        public static Dictionary<string, string> InitActivityPlace()
        {
            dicActivityPlace = new Dictionary<string, string>();
            ActivityPlaceInfo[] actPlace = Utility.GetPlacesByActivityId(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            dicActivityPlace.Add("all", "全部");
            for (int i = 0; i < actPlace.Length; i++)
            {
                dicActivityPlace.Add(actPlace[i].Guid, actPlace[i].Name);
            }
            return dicActivityPlace;
        }

        /// <summary>
        /// 统计频率规划方案
        /// </summary>
        /// <param name="activityguid"></param>
        /// <returns></returns>
        //public static IList StatisticFreqPartPlan(string activityguid)
        //{
        //    return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
        //        channel =>
        //        {
        //            return channel.StatisticFreqPartPlan(activityguid);
        //        });
        //}

        /// <summary>
        /// 统计设备
        /// </summary>
        /// <param name="activityguid"></param>
        /// <returns></returns>
        //public static IList StatisticEquipment(string activityguid)
        //{
        //    return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
        //        channel =>
        //        {
        //            return channel.StatisticEquipment(activityguid);
        //        });
        //}

        /// <summary>
        /// 统计周围台站
        /// </summary>
        /// <param name="activityguid"></param>
        /// <returns></returns>
        //public static IList StatisticSurroundStation(string activityguid)
        //{
        //    return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
        //        channel =>
        //        {
        //            return channel.StatisticSurroundStation(activityguid);
        //        });
        //}

        /// <summary>
        /// 统计频率指配
        /// </summary>
        /// <returns></returns>
        //public static IList StatisticFreqAssign(string activityguid)
        //{
        //    return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
        //        channel =>
        //        {
        //            return channel.StatisticFreqAssign(activityguid);
        //        });
        //}

        /// <summary>
        /// 统计设备检测
        /// </summary>
        /// <param name="activityguid"></param>
        /// <returns></returns>
        //public static IList StatisticEquInspection(string activityguid)
        //{
        //    return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
        //        channel =>
        //        {
        //            return channel.GetEquInspectionStats(activityguid);
        //        });
        //}

        /// <summary>
        /// 统计人员预案
        /// </summary>
        /// <param name="activityguid"></param>
        /// <returns></returns>
        public static IList StatisticPersonPlan(string activityguid)
        {
            //return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
            //    channel =>
            //    {
            //        return channel.GetPersonPlanStats(activityguid);
            //    });

            DataManager.Public.StatisticModel model = new DataManager.Public.StatisticModel();

            return model.GetPersonPlanStats2017(activityguid).ToList<PersonPlanStatisticData>();

        }





        /// <summary>
        /// 统计人员预案
        /// </summary>
        /// <param name="activityguid"></param>
        /// <returns></returns>
        public static IList StatisticPersonOut()
        {
            //return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
            //    channel =>
            //    {
            //        return channel.GetPersonPlanStats(activityguid);
            //    });


            DataManager.Public.StatisticModel model = new DataManager.Public.StatisticModel();

            return model.GetPersonOutStatsByDate(DateTime.Now.Year.ToString() + "-01-01", DateTime.Now.Year.ToString() + "-12-31").ToList<PersonPlanStatisticData>();

        }




        public static IList StatisticPersonRP()
        {
            //return BeOperationInvoker.Invoke<I_CO_IA.Statistic.I_CO_IA_Statistic, IList>(
            //    channel =>
            //    {
            //        return channel.GetPersonPlanStats(activityguid);
            //    });

            DataManager.Public.StatisticModel model = new DataManager.Public.StatisticModel();

            return model.GetPersonRPStatByDate(DateTime.Now.Year.ToString()+"-01-01", DateTime.Now.Year.ToString() + "-12-31").ToList<PersonPlanStatisticData>();

        }



        /// <summary>
        /// 查询设备
        /// </summary>
        /// <param name="condition"></param>
        public static ActivityEquipment[] QueryActivityEquipments(EquipmentLoadStrategy condition)
        {
            //return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityEquipment[]>(channel =>
            //   {
            //       return channel.GetActivityEquipments(condition);
            //   });


            return null;
        }
 

        /// <summary>
        ///  查询设备检测
        /// </summary>
        /// <param name="loadcondition"></param>
        /// <returns></returns>
        public static List<EquipmentInspection> QueryEquipmentInspections(EquInspectionQueryCondition loadcondition)
        {
            //return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, List<EquipmentInspection>>(channel =>
            //{
            //    return channel.GetEquipmentInspections(loadcondition);
            //});

            return null;
        }
    }
}
