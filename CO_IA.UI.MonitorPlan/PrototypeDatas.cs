using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CO_IA.Data;
using CO_IA.Data.Monitor;
using CO_IA.Data.MonitorPlan;

namespace CO_IA.UI.MonitorPlan
{
    public class PrototypeDatas
    {
        //public static MonitorPlanInfo GetMonitorPlan()
        //{
        //    var monitorPlan = new MonitorPlanInfo();
        //    //monitorPlan.Tasks = GetMonitorTasks();
        //    return monitorPlan;
        //}
        
        //private static List<FreqRange> GetFreqList(string p_areaStr)
        //{
        //    List<FreqRange> freqranges = new List<FreqRange>();
        //    List<FreqPlanInfo> freqlist = CreateFreqPartPlans();
        //    //var list = freqlist.Where(p => p.ActivityArea == p_areaStr);
        //    //foreach (FreqPlanInfo freq in list)
        //    //{
        //    //    freqranges.Add(new FreqRange() { FreqFrom = freq.SendPara.FreqStart, FreqTo = freq.SendPara.FreqEnd });
        //    //}
        //    return freqranges;
        //}


      

        #region 查询
        /// <summary>
        /// 创建频率规划表
        /// </summary>
        /// <returns></returns>
        public static List<FreqPlanInfo> CreateFreqPartPlans(string p_placeid)
        {
            List<FreqPlanActivity> originalFreqPlan = new List<FreqPlanActivity>();
            originalFreqPlan = GetActivityInfo(p_placeid);
            List<FreqPlanInfo> freqPlanInfos = new List<FreqPlanInfo>();
            foreach (FreqPlanActivity originalFreq in originalFreqPlan)
            {
                FreqPlanInfo currentFreq = new FreqPlanInfo();
                currentFreq.FreqIdForMonitor = originalFreq.Guid;
                currentFreq.FreqPlanName = originalFreq.FreqPlanName;
                currentFreq.FreqValue = originalFreq.FreqValue;
                currentFreq.BandCount = originalFreq.BandCount;
                currentFreq.CreateState =0;
                freqPlanInfos.Add(currentFreq);
            }     
            return freqPlanInfos;
        }
        /// <summary>
        /// 根据监测地点id，查询频率预案
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public static List<FreqPlanActivity> GetActivityInfo(string placeid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<CO_IA.Data.FreqPlanActivity>>(
                 channel =>
                 {
                     return channel.GetFreqPlanActivitys(placeid);
                 });
        }

        //public static void GetOrgOfPerson(string parentGuid)
        //{
        //    List<PP_PersonInfo> list = new List<PP_PersonInfo>();
        //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonPlan.I_CO_IA_PersonPlan>(channel =>
        //    {
        //        list = channel.GetPP_PersonInfos(parentGuid);
        //    });
        //}
        /// <summary>
        /// 根据人员组织id获取组织下人员信息
        /// </summary>
        /// <param name="parentGuid"></param>
        /// <returns></returns>
        public static List<PP_PersonInfo> GetOrgOfPerson(string parentGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, List<CO_IA.Data.PP_PersonInfo>>(
                 channel =>
                 {
                     return channel.GetPP_PersonInfos(parentGuid);
                 });
        }

        /// <summary>
        /// 根据活动id,获取活动地点实体列表
        /// </summary>
        /// <param name="_activityGuid">活动guid</param>
        /// <returns></returns>
        public static CO_IA.Data.ActivityPlaceInfo[] GetPlacesByActivityId(string _activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo[]>(
                channel =>
                {
                    return channel.GetPlaceInfosByActivityId(_activityGuid);
                });
        }
        /// <summary>
        /// 根据活动id 获取纯信息列表
        /// </summary>
        /// <param name="activityGuid"></param>
        /// <returns></returns>
        public static CO_IA.Data.ActivityPlace[] GetPlaces(string activityGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlace[]>(
               channel =>
               {
                   return channel.GetPlaces(activityGuid);
               });
        }
        /// <summary>
        /// 查询，监测预案详表(只跟监测地点id)
        /// </summary>
        /// <param name="_placeGuid"></param>
        /// <returns></returns>
        public static CO_IA.Data.MonitorPlan.DetailMonitorPlan[] GetDetailMonitorPlan(string _placeGuid,string activityid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorPlan.I_CO_IA_MonitorPlan, CO_IA.Data.MonitorPlan.DetailMonitorPlan[]>(
                channel =>
                {
                    return channel.GetSigleMonitorPlan(_placeGuid, "", activityid);
                });
        }
        /// <summary>
        /// 带参数查询
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DetailMonitorPlan[] GetDetailMonitorByParam(DetailMonitorPlanQuery obj)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorPlan.I_CO_IA_MonitorPlan, DetailMonitorPlan[]>(channel =>
            {
                return channel.GetDetailMonitorList(obj);
            });
        }
        /// <summary>
        /// 获取全部，监测预案信息
        /// </summary>
        /// <returns></returns>
        public static CO_IA.Data.MonitorPlan.DetailMonitorPlan[] GetAllDetailMonitorPlan()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorPlan.I_CO_IA_MonitorPlan, CO_IA.Data.MonitorPlan.DetailMonitorPlan[]>(
               channel =>
               {
                   return channel.GetAllDetailMonitorPlan();
               });
        }
       
        /// <summary>
        /// 根据监测组id，查询监测组信息
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
        /// <summary>
        /// 查询组织下所有人员
        /// </summary>
        /// <param name="orgguid"></param>
        /// <returns></returns>
        public static List<PP_PersonInfo> GetPersonList(string orgguid)
        {
            List<PP_PersonInfo> personlist = new List<PP_PersonInfo>();
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
               personlist= channel.GetPP_PersonInfos(orgguid);
            });
           return personlist;
        }
        /// <summary>
        /// 根据监测地点id查位置信息
        /// </summary>
        /// <param name="activityPlaceGuid"></param>
        /// <returns></returns>
        public static CO_IA.Data.ActivityPlaceLocation[] GetLocations(string activityPlaceGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceLocation[]>(
               channel =>
               {
                   return channel.GetLocations(activityPlaceGuid);
               });
        }
        /// <summary>
        /// 根据地点id，查询监测地点信息
        /// </summary>
        /// <param name="placeGuid"></param>
        /// <returns></returns>
        public static CO_IA.Data.ActivityPlaceInfo GetPlaceInfo(string placeGuid)
        {
          return  PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo>(channel =>
            {
               return channel.GetPlaceInfo(placeGuid);
            });
        }
        /// <summary>
        /// 根据位置id，查询位置信息
        /// </summary>
        /// <param name="locationGuid"></param>
        /// <returns></returns>
        public static CO_IA.Data.ActivityPlaceLocation GetLocationByGuid(string locationGuid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceLocation>(channel =>
            {
                return channel.GetLocationByGuid(locationGuid);
            });
        }
        #endregion

        #region 判断某个日期是否在某段日期范围内
        /// <summary> 
        /// 判断某个日期是否在某段日期范围内，返回布尔值
        /// </summary> 
        /// <param name="dt">要判断的日期</param> 
        /// <param name="dt1">开始日期</param> 
        /// <param name="dt2">结束日期</param> 
        /// <returns></returns>  
        public static bool IsInDate(DateTime dt, DateTime dt1, DateTime dt2)
        {
            return dt.CompareTo(dt1) >= 0 && dt.CompareTo(dt2) <= 0;
        }
        #endregion

        #region 保存
        /// <summary>
        /// 手动，生成任务
        /// </summary>
        /// <param name="p_detailMonitor"></param>
        public static void SaveMonitorInfo(DetailMonitorPlan p_detailMonitor)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorPlan.I_CO_IA_MonitorPlan>
                     (channel =>
                     {
                         channel.SaveDetailMonitorPlan(p_detailMonitor);
                     });
        }
        #endregion
    }

    //fdw创建继承
    public class FreqPlanInfo : FreqPlanActivity, INotifyPropertyChanged
    {
        /// <summary>
        /// 频率预案id
        /// </summary>
        public string FreqIdForMonitor { get; set; } 
        /// <summary>
        /// 生成状态 0表示未生成，1表示已生成
        /// </summary>
        public int CreateState { get; set; }

        private bool isChecked;
        /// <summary>
        ///  选择
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }

        private List<DetailMonitorPlan> _detailPlan = new List<DetailMonitorPlan>();
        /// <summary>
        /// 监测预案生成列表
        /// </summary>
        public List<DetailMonitorPlan> ProtectDetailPlan
        {
            get
            {
                return _detailPlan;
            }
            set
            {
                _detailPlan = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
