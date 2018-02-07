using AT_BC.Data.Helpers;
using CO_IA.Client;
using CO_IA.Client.Orgs;
using CO_IA.Data;
using CO_IA.Data.Setting;
using I_CO_IA.ActivityManage;
using I_CO_IA.MessageCenter;
using I_GS_MapBase.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CO_IA.UI.Screen
{
    public class Obj
    {
        #region 窗口

        /// <summary>
        /// loading窗口
        /// </summary>
        public static Control.Loading loading = null;
        public static VideoWindow VWindow = null;
        public static MapUtiles.ClientUtile clientUtile = MapUtiles.ClientUtile.Create(); 
        #endregion

        #region 面板
        public static CO_IA.UI.Screen.MainPage.MainPanel mainPanel = null;
        #endregion

        #region 当前活动
        private static CO_IA.Data.Activity _activity;
        /// <summary>
        /// 当前活动
        /// </summary>
        public static CO_IA.Data.Activity Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                //if (_activity != null && _activity.Guid == value.Guid)
                //    return;
                _activity = value;
                if (value != null)
                {
                    activityPlaces = Utility.GetPlacesByActivityId(_activity.Guid);
                    NewScreenMap();
                    UpdateActivity();
                }
            }
        }
        private static void UpdateActivity()
        {
            ActivityUpdateThread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Thread.Sleep(30 * 1000);
                    getActivity();
                }               

            })) { IsBackground = true };
            ActivityUpdateThread.Start();            
        }
        private static void getActivity()
        {
            Activity temp=null;
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_ActivityManage>(channel =>
            {
                temp = channel.GetActivity(Activity.Guid);
            });
            Activity.ActivityStage =  temp.ActivityStage;
            Activity.ActivityType = temp.ActivityType;
            Activity.CreateTime = temp.CreateTime;
            Activity.Creator = temp.Creator;
            Activity.DateFrom = temp.DateFrom;
            Activity.DateTo = temp.DateTo;
            Activity.Description = temp.Description;
            Activity.Icon = temp.Icon;
            Activity.Name = temp.Name;
            Activity.Organizer = temp.Organizer;
            Activity.ShortHand = temp.ShortHand;
        }
        #endregion

        #region 区域

        private static ActivityPlaceInfo[] activityPlaces = null;
        /// <summary>
        /// 当前操作活动的区域列表
        /// </summary>
        public static ActivityPlaceInfo[] ActivityPlaces
        {
            get
            {
                return activityPlaces;
            }
            set
            {
                activityPlaces = value;
            }
        }
        /// <summary>
        /// 绘制区域 图形信息
        /// </summary>
        public static Dictionary<string, ReturnDrawGraphicInfo[]> AreaGraphicInfo = new Dictionary<string,ReturnDrawGraphicInfo[]>();
        //private static ActivityPlaceInfoEx[] activityPlaces = null;
        ///// <summary>
        ///// 当前操作活动的地点列表
        ///// </summary>
        //public static ActivityPlaceInfoEx[] ActivityPlaces
        //{
        //    get
        //    {
        //        return activityPlaces;
        //    }
        //    set
        //    {
        //        activityPlaces = value;

        //    }
        //}

        #endregion

        #region 当前选中区域ID
        public static string SelectedAreaID = "";
        #endregion

        #region 是否结束线程
        public static bool StopThreadFlag = false;
        #endregion

        #region 地图对象
        public static ScreenMap screenMap;
        
        private static void NewScreenMap()
        {
            screenMap = new ScreenMap();
        }
        #endregion

        #region 轨迹回放
        public static DrawTrack drawTrack = new DrawTrack();
        #endregion

        #region 任务数据
        public static CO_IA.UI.Screen.Task.TaskData taskData = new Task.TaskData();
        #endregion

        #region GPS
        public static LoadGPSData gpsData = new LoadGPSData();
        #endregion

        #region 线程
        /// <summary>
        /// 任务
        /// </summary>
        public static Thread TaskThread;
        /// <summary>
        /// 获取组、人员GPS信息
        /// </summary>
        public static Thread groupGPSThread;
        /// <summary>
        /// 准备阶段数据线程
        /// </summary>
        public static Thread PrepareThread;
        /// <summary>
        /// 准备阶段数据线程
        /// </summary>
        public static Thread ExecThread;
        /// <summary>
        /// 区域完成百分比
        /// </summary>
        public static Thread TipsDataThread;
        /// <summary>
        /// 更新活动信息
        /// </summary>
        public static Thread ActivityUpdateThread;
        /// <summary>
        /// 终止线程
        /// </summary>
        public static void Abort()
        {
            try
            {
                if (TaskThread != null && TaskThread.IsAlive)
                    TaskThread.Abort();
                if (groupGPSThread!=null && groupGPSThread.IsAlive)
                    groupGPSThread.Abort();
                if (PrepareThread != null && PrepareThread.IsAlive)
                    PrepareThread.Abort();
                if (ExecThread != null && ExecThread.IsAlive)
                    ExecThread.Abort();
                if (TipsDataThread != null && TipsDataThread.IsAlive)
                    TipsDataThread.Abort();
                if (ActivityUpdateThread != null && ActivityUpdateThread.IsAlive)
                    ActivityUpdateThread.Abort();
            }
            catch
            { 
            
            }
        }
        #endregion

        #region 接收客户端消息
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message"></param>
        public static void SetSceneState(Data.ActivityMessage message)
        {
            if (message == null)
                return;

            if (message.MessageType == "ClientOnline")
            {
                try
                {
                    ClientInfo info = DataContractSerializeHelper.DeSerialize<ClientInfo>(message.Content);
                    Obj.SetGroupStateToMap(info, true);
                }
                catch
                {
                }
            }
            if (message.MessageType == "ClientOffline")
            {
                ClientInfo info = new ClientInfo() { GroupGuid = message.AssistantInfo };
                Obj.SetGroupStateToMap(info, false);
            }
            if (message.MessageType == "Task")
            {
                Obj.taskData.Begin();
            }
            if (message.MessageType == "TaskExecuteConclusion")
            {
                TaskExecuteConclusion info = DataContractSerializeHelper.DeSerialize<TaskExecuteConclusion>(message.Content);
                Obj.taskData.Begin();
            }
        }
        #endregion

        #region 设置小组状态
        /// <summary>
        /// 修改已绘制点的监测组状态
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="online"></param>
        public static void SetGroupStateToMap(ClientInfo clientinfo, bool online)
        {
            if (Obj.screenMap.DrawElementList.Count > 0)
            {
                var list = Obj.screenMap.DrawElementList.Where(item => item.Key == MapGroupTypes.MonitorGroup_.ToString() + clientinfo.GroupGuid).ToArray();
                if (list != null && list.Length == 1)
                {
                    OrgToMapStyle orgPoint = list[0].Value as OrgToMapStyle;
                    if (orgPoint != null)
                    {
                        orgPoint.OrgInfo.OnLine = online;
                        orgPoint.clientInfo = clientinfo;
                    }
                }
            }
            Obj.mainPanel.ChangeGroupState(clientinfo.GroupGuid, online);
        }
        /// <summary>
        /// 初始调用，设置初始状态
        /// </summary>
        /// <param name="online"></param>
        public static void SetGroupAllStateToMap(bool online)
        {
            if (Obj.screenMap.DrawElementList.Count > 0)
            {
                var list = Obj.screenMap.DrawElementList.Where(item => item.Key.StartsWith(MapGroupTypes.MonitorGroup_.ToString())).ToArray();
                if (list != null && list.Length >0)
                {
                    foreach (var item in list)
                    {
                        OrgToMapStyle orgPoint = item.Value as OrgToMapStyle;
                        if (orgPoint != null)
                        {
                            orgPoint.OrgInfo.OnLine = online;
                            orgPoint.OrgInfo.NAME = orgPoint.OrgInfo.NAME;
                        }
                    }
                }
            }
            Obj.mainPanel.ChangeGroupState(null,false);
        }
        #endregion

        
        
      
    }
}
