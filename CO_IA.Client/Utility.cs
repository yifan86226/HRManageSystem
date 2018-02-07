#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：通用静态方法封装
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion

using AT_BC.Common;
using CO_IA.Client.UIBuilderFactory;
using CO_IA.Data;
using CO_IA.Data.PlanDatabase;
using CO_IA.Data.Setting;
using CO_IA.Data.Template;
using CO_IA.Types;
 
using PT.Profile.Business;
using PT.Profile.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows.Media;

namespace CO_IA.Client
{
    /// <summary>
    /// 通用静态方法封装类
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 静态构造函数,初始化行政区域
        /// </summary>
        static Utility()
        {
            InitDistrictMapping(PT.Profile.Business.DistrictHelper.DistrictInfos);
        }

        public readonly static MediaPlayer AudioPlayer = new MediaPlayer();
        /// <summary>
        /// 行政区域字典
        /// </summary>
        internal static Dictionary<string, DistrictInfo> DistrictMapping = new Dictionary<string, DistrictInfo>();

        private static Dictionary<string, string> stationClassDic;
        /// <summary>
        /// 台站类别
        /// </summary>
        //public static Dictionary<string, string> StationClassDic
        //{
        //    get
        //    {
        //        if (stationClassDic == null)
        //        {
        //            stationClassDic = GetDicStationClass();
        //        }
        //        return stationClassDic;
        //    }
        //}

        /// <summary>
        /// 获取台站类型表
        /// </summary>
        /// <returns></returns>
        //private static Dictionary<string, string> GetDicStationClass()
        //{
        //    Dictionary<string, string> dic = new Dictionary<string, string>();
        //    List<StationClass> list = null;
        //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
        //    {
        //        list = channel.Get_StationClassList();
        //    });

        //    if (list != null && list.Count > 0)
        //    {
        //        foreach (var item in list)
        //        {
        //            dic.Add(item.NET_SVN_CODE.Trim() + "-" + item.STAT_APP_TYPE.Trim(), item.CLASS_CODE);
        //        }
        //    }
        //    return dic;
        //}
        /// <summary>
        /// 初始化行政区域
        /// </summary>
        /// <param name="districtList">从中读取行政区域信息的行政区域列表</param>
        private static void InitDistrictMapping(DistrictInfoList districtList)
        {
            foreach (var district in districtList)
            {
                DistrictMapping[district.Code.Substring(0, 4)] = district;
                if (district.DistrictLevel == DistrictLevelEnum.Nation || district.DistrictLevel == DistrictLevelEnum.Province)
                {
                    InitDistrictMapping(district.SubDistricts);
                }
            }
        }

        /// <summary>
        /// 获取参数指定区域编码的名称
        /// </summary>
        /// <param name="areaCode">要获取其名称的行政区域编码</param>
        /// <returns>参数指定行政区域编码对应的名称,如果无法获取将返回编码</returns>
        public static string GetAreaName(string areaCode)
        {
            DistrictInfo info;
            if (!string.IsNullOrWhiteSpace(areaCode))
            {
                areaCode = string.Format("{0}0000", areaCode).Substring(0, 4);
                if (DistrictMapping.TryGetValue(areaCode, out info))
                {
                    return info.Name;
                }
            }
            return areaCode;
        }
        /// <summary>
        /// 获取活动地点信息
        /// </summary>
        /// <param name="activityGuid">要获取其活动地点的活动标识</param>
        /// <returns>活动地点集合</returns>
        //public static CO_IA.Data.ActivityPlace[] GetPlaces(string activityGuid)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlace[]>(
        //        channel =>
        //        {
        //            return channel.GetPlaces(activityGuid);
        //        });
        //}

        /// <summary>
        /// 获取活动地点列表
        /// </summary>
        /// <param name="_activityGuid">活动guid</param>
        /// <returns></returns>
        public static CO_IA.Data.ActivityPlaceInfo[] GetPlacesByActivityId(string _activityGuid)
        {
            //return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivityManage.I_CO_IA_ActivityManage, CO_IA.Data.ActivityPlaceInfo[]>(
            //    channel =>
            //    {
            //        return channel.GetPlaceInfosByActivityId(_activityGuid);
            //    });


            return new  ActivityPlaceInfo[0];
        }

        /// <summary>
        /// 查询单位信息
        /// </summary>
        /// <returns></returns>
        public static CO_IA.Data.Organization[] GetORGInfos()
        {
            //return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase, CO_IA.Data.Organization[]>(channel =>
            //{
            //    return channel.GetOrganizations();
            //});



            return new Organization[0];
        }

        /// <summary>
        /// 获取日程
        /// </summary>
        /// <returns></returns>
        public static Schedule[] getScheduleInfo()
        {
            //return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, Schedule[]>(channel =>
            //{
            //    return channel.GetScheduleInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            //});



            return new Schedule[0];
        }
        /// <summary>
        /// 从日程获取当前小组信息（位置） 根据活动的不同阶段获取不同结果。
        /// </summary>
        /// <param name="_activityGuid"></param>
        /// <param name="stageType"></param>
        /// <returns></returns>
        //public static ScheduleDetail[] getOrgGroupsBySchedule(string _activityGuid, ActivityStage stageType)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, ScheduleDetail[]>(channel =>
        //    {
        //        return channel.GetOrgsByActivityGuid(_activityGuid, stageType);
        //    });
        //}
        /// <summary>
        /// 在执行阶段从日程获取各小组信息(位置)
        /// </summary>
        /// <param name="_activityGuid"></param>
        /// <param name="AreaID"></param>
        /// <returns></returns>
        //public static ScheduleDetail[] getOrgGroupsBySchedule(string _activityGuid, string AreaID)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule, ScheduleDetail[]>(channel =>
        //    {
        //        return channel.GetOrgsByAreaGuid(_activityGuid, AreaID);
        //    });
        //}


        ///// <summary>
        ///// 查询单位信息
        ///// </summary>
        ///// <returns></returns>
        //public static CO_IA.Data.ORGInfo[] GetORGInfos()
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, CO_IA.Data.ORGInfo[]>(channel =>
        //     {
        //         return channel.GetORGInfos();
        //     });
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="queryCondition"></param>
        ///// <returns></returns>
        //public static CO_IA.Data.EquipmentInfo[] GetEquipmentInfos(CO_IA.Data.EquipmentQueryCondition queryCondition)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, CO_IA.Data.EquipmentInfo[]>(
        //        channel =>
        //        {
        //            return channel.GetEquipmentInfos(queryCondition);
        //        });
        //}

        ///// <summary>
        ///// 获取设备库中指定单位下的所有设备
        ///// </summary>
        ///// <param name="activityGuid">要获取其设备的单位标识</param>
        ///// <returns>查询结果:参数指定标识的单位的所有设备</returns>
        //public static CO_IA.Data.EquipmentInfo[] GetEquipmentForOrg(string orgGuid)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, CO_IA.Data.EquipmentInfo[]>(
        //        channel =>
        //        {
        //            return channel.GetEquipmentForOrg(orgGuid);
        //        });
        //}

        /// <summary>
        /// 获取安全保障类别
        /// </summary>
        /// <returns>系统支持的所有安全保障类别</returns>
        //public static CO_IA.Data.SecurityClass[] GetSecurityClasses()
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, CO_IA.Data.SecurityClass[]>(
        //        channel =>
        //        {
        //            return channel.GetSecurityClasses();
        //        });
        //}

        /// <summary>
        /// 获取考点信息
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// LLQ 2017-05-16
        /// </remarks>
        //public static CO_IA.Data.Setting.ExamPlace[] GetExamPlace(ExamCondition queryCondition)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase, CO_IA.Data.Setting.ExamPlace[]>(
        //        channel =>
        //        {
        //            return channel.GetExamPlace(queryCondition);
        //        });
        //}

        //public static CO_IA.Data.VehicleInfo[] GetVehicleInfos(VehicleQueryCondition queryCondition)
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase,
        //        CO_IA.Data.VehicleInfo[]>(channel =>
        //        {
        //            return channel.GetVehicleInfos(queryCondition);
        //        });
        //}

        /// <summary>
        /// 通信系统
        /// </summary>
        private static CO_IA.Data.BusinessType[] businessTypes;

        /// <summary>
        /// 业务类型
        /// </summary>
        /// <returns></returns>
        //public static BusinessType[] BusinessTypes
        //{
        //    get
        //    {
        //        if (businessTypes == null)
        //        {
        //            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
        //             {
        //                 businessTypes = channel.GetBusinessType().ToArray();
        //             });
        //        }
        //        return businessTypes;
        //    }
        //}

        /// <summary>
        /// 通信系统
        /// </summary>
        private static CO_IA.Data.CommunicationSystem[] communicationSystems;

        /// <summary>
        /// 获取通信系统-从台站标准库导入
        /// </summary>
        //public static CO_IA.Data.CommunicationSystem[] CommunicationSystems
        //{
        //    get
        //    {
        //        if (communicationSystems == null)
        //        {
        //            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(
        //            channel =>
        //            {
        //                communicationSystems = channel.GetCommuniationSystems();
        //            });
        //        }
        //        return communicationSystems;
        //    }
        //}

        /// <summary>
        /// 生成新的Guid,该方法提供36位Guid统一生成入口
        /// </summary>
        /// <returns>32位Guid</returns>
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// 登记消息接收器
        /// </summary>
        /// <param name="receiver">要登记的消息接收器</param>
        public static void RegisterMessageReceiver(IMessageReceiver receiver)
        {
            MessageReceiverManager.GetMessageReceiver().RegisterMessageReceiver(receiver);
        }

        /// <summary>
        /// 移除消息接收器,移除以后该接收器将无法接收消息
        /// </summary>
        /// <param name="receiver">要移除的消息接收器</param>
        public static void RemoveMessageReceiver(IMessageReceiver receiver)
        {
            MessageReceiverManager.GetMessageReceiver().RemoveMessageReceiver(receiver);
        }

        /// <summary>
        /// 启动消息接收服务,如果当前以后运行的服务,不会启动新的服务
        /// </summary>
        /// <returns>消息接受服务使用的端口号</returns>
        //public static int StartMessageService(Action<int> registerClientAction)
        //{
        //    if (messageReceiveServiceHost == null)
        //    {
        //        int servicePort;
        //        if (!AT_BC.Common.PortDetector.TryGetFirstAvailablePort(8799, out servicePort))
        //        {
        //            throw new Exception("未找到可用端口,无法启动消息接收服务");
        //        }
        //        messageReceiveServiceHost = new MessageReceiveServiceHost(servicePort, registerClientAction);
        //        messageReceiveServiceHost.Open();
        //    }
        //    return messageReceiveServiceHost.ServicePort;
        //}

        /// <summary>
        /// 启动消息接收服务,如果当前以后运行的服务,不会启动新的服务
        /// </summary>
        /// <returns>消息接受服务使用的端口号</returns>
        public static void StopMessageService()
        {
            if (messageReceiveServiceHost != null)
            {
                if (messageReceiveServiceHost.State == CommunicationState.Opened)
                {
                    messageReceiveServiceHost.Close();
                }
                messageReceiveServiceHost = null;
            }
        }

        /// <summary>
        /// 消息接收服务发布类引用
        /// </summary>
        private static HttpServiceHost messageReceiveServiceHost;

        public static string RiasSystemName = "油料股大数据管理系统";

        private static IActivityUIBuilderFactory uiFactory;
        public static IActivityUIBuilderFactory GetUIFactory()
        {
            if (uiFactory == null)
            {
                //var systemParams = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.ISubsystem, SysParamList>(channel =>
                //    {
                //        return channel.GetSysParamsByClassName(CO_IA.Public.SubSystemIDs.Rias, "UIFactory");
                //    });


                SysParamList systemParams = new SysParamList();
                AT_BC.Data.LoadableType loadableType = new AT_BC.Data.LoadableType { Assembly = "CO_IA.Client.UIBuilderFactory.Standard.dll", TypeName = "ActivityUIBuilderFactory" }; ;
                if (systemParams != null && systemParams.Count >= 2)
                {
                    string assemblyName = systemParams.GetParamValueByParamName("Assembly", true);
                    string clsName = systemParams.GetParamValueByParamName("Factory", true);
                    if (!string.IsNullOrWhiteSpace(assemblyName) && !string.IsNullOrWhiteSpace(clsName))
                    {
                        loadableType = new AT_BC.Data.LoadableType { Assembly = assemblyName, TypeName = clsName };
                    }
                }
                System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, loadableType.Assembly));
                if (assembly != null)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.Name == loadableType.TypeName)
                        {
                            uiFactory = Activator.CreateInstance(type) as IActivityUIBuilderFactory;
                            break;
                        }
                    }
                }
            }
            return uiFactory;
        }

        private static ActivityTypeDef[] supportActivityTypes;

        //public static ActivityTypeDef[] SupportActivityTypes
        //{
        //    get
        //    {
        //        var factory = Utility.GetUIFactory();
        //        if (supportActivityTypes == null)
        //        {
        //            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
        //            {
        //                supportActivityTypes = channel.GetActivityTypes();
        //                foreach (var activityType in supportActivityTypes)
        //                {
        //                    ImageSource imageSource = factory.GetImageSource(activityType.ID); //this.TryFindResource(string.Format("ActivityType.{0}", activityType)) as ImageSource;
        //                    ActivityIconDictionary.RegisterIcon(activityType.ID, imageSource);
        //                }
        //            });
        //        }
        //        return supportActivityTypes;
        //    }
        //}

        private static EquipmentClass[] equipmentClasses = null;

        //public static EquipmentClass[] EquipmentClasses
        //{
        //    get
        //    {
        //        if (equipmentClasses == null)
        //        {
        //            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
        //                {
        //                    equipmentClasses = channel.GetEquipmentClasses();
        //                });
        //        }
        //        return equipmentClasses;
        //    }
        //}

        //public static string GetActivityTypeName(string activityTypeID)
        //{
        //    foreach (var activityType in SupportActivityTypes)
        //    {
        //        if (activityType.ID == activityTypeID)
        //        {
        //            return activityType.Name;
        //        }
        //    }
        //    return activityTypeID;
        //}

        public static List<string> GetAllSubAreas()
        {
            List<string> areaCodeList = new List<string>();
            if (!string.IsNullOrWhiteSpace(RiasPortal.Current.SystemConfig.LoginArea))
            {
                string refAreaCode = RiasPortal.Current.SystemConfig.LoginArea.PadRight(6, '0');
                foreach (DistrictInfo district in PT.Profile.Business.DistrictHelper.DistrictInfos)
                {
                    if (district.Code == refAreaCode)
                    {
                        foreach (var subDistrict in district.SubDistricts)
                        {
                            areaCodeList.Add(subDistrict.Code.Substring(0, 4));
                        }
                    }
                }
            }
            return areaCodeList;
        }

        /// <summary>
        /// key:code
        /// value:name
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAllAreas()
        {
            Dictionary<string, string> dicArea = new Dictionary<string, string>();
            List<string> codes = Utility.GetAllSubAreas();
            string name;
            foreach (string code in codes)
            {
                name = Utility.GetAreaName(code);
                dicArea.Add(code, name);
            }
            return dicArea;
        }

        /// <summary>
        /// 带有省级编码的区域编码
        /// </summary>
        public static Dictionary<string, string> GetProvinceAreaCode()
        {
            Dictionary<string, string> dicArea = new Dictionary<string, string>();
            string loginAreaCode = RiasPortal.Current.SystemConfig.LoginArea;
            string loginAreaName = Utility.GetAreaName(loginAreaCode);
            dicArea.Add(loginAreaCode, loginAreaName);
            if (loginAreaCode.Substring(2, 2) == "00")  //省级区域编码
            {
                List<string> codes = Utility.GetAllSubAreas();
                string name;
                foreach (string code in codes)
                {
                    name = Utility.GetAreaName(code);
                    dicArea.Add(code, name);
                }
            }
            return dicArea;
        }

        //public static ActivityTemplate[] GetActivityTemplates()
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Template, ActivityTemplate[]>(
        //        channel =>
        //        {
        //            return channel.GetActivities();
        //        });
        //}

        //public static EquipmentClassFreqRange[] GetEquipmentClassFreqRanges()
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, EquipmentClassFreqRange[]>(channel =>
        //        {
        //            return channel.GetEquipmentClassFreqRanges();
        //        });
        //}

        /// <summary>
        /// 控件打印
        /// </summary>
        /// <param name="element"></param>
        public static void PrintElement(System.Windows.FrameworkElement element)
        {
            CustomPrint.PrintHelper print = new CustomPrint.PrintHelper();
            print.print(element);
        }


        public const string DevThemeName = "DevThemeName";

        public const string DateFormatString = "yyyy-MM-dd";

        public static string OrgRootName = "重大活动安全保障办公室";

        private static Stopwatch stopwatch = new Stopwatch();

        private static DateTime loginTime = DateTime.MinValue;

        public static DateTime LoginTime
        {
            get
            {
                return loginTime;
            }
        }

        public static void RegisterLoginTime(DateTime time)
        {
            if (loginTime != DateTime.MinValue)
            {
                throw new Exception("登录时间已经设置,不能重新设置");
            }
            loginTime = time;
            stopwatch.Restart();
        }

        public static DateTime GetServerTime()
        {
            return loginTime.Add(stopwatch.Elapsed);
        }

        public static void SendMessageServicePort(ICommunicationObject communicationObj)
        {
            if (communicationObj is IClientChannel)
            {
                OperationContext.Current = new OperationContext(communicationObj as IClientChannel);
                MessageHeader header = MessageHeader.CreateHeader("ServicePort", "http://www.bestitu.com", messageReceiveServiceHost.ServicePort);
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
            }
        }

        public static bool HasDuty(string dutyCode)
        {
            if (string.IsNullOrWhiteSpace(dutyCode))
            {
                return true;
            }
            else if (HasActivityManageRight())
            {
                return true;
            }
            else
            {
                return RiasPortal.HasDuty(dutyCode);
            }
        }

        private const string ActivityManageRight = "重大活动管理";

        public static bool HasActivityManageRight()
        {
            //return RiasPortal.Current.UserSetting.UserRights.Contains(ActivityManageRight);

            return true;
        }

        private readonly static Encoding GB2312Encoding = Encoding.GetEncoding(936);

        public static int GetVarcharLength(string str)
        {
            return GB2312Encoding.GetByteCount(str);
        }

        public static bool IsSupported(ActivityStep[] requiredSteps, string[] requiredDuties)
        {
            bool bVisible = true;
            if (requiredSteps != null && requiredSteps.Length > 0)
            {
                bVisible = false;
                var uiBuilder = Utility.GetUIFactory().GetUIBuilder(RiasPortal.ModuleContainer.Activity.ActivityType);
                foreach (ActivityStep step in requiredSteps)
                {
                    if (uiBuilder.CanBuildStep(step))
                    {
                        bVisible = true;
                        break;
                    }
                }
            }
            if (bVisible)
            {
                if (requiredDuties != null && requiredDuties.Length > 0)
                {
                    bVisible = false;
                    foreach (string dutyCode in requiredDuties)
                    {
                        if (HasDuty(dutyCode))
                        {
                            bVisible = true;
                            break;
                        }
                    }
                }
            }
            return bVisible;
            //return System.Windows.Visibility.Visible;
        }

        //public static List<string> GetMonitorEQUType()
        //{
        //    return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Setting, List<string>>(channel =>
        //    {
        //        return channel.GetMonitorEQUType();
        //    });
        //}

        public static void UpdateUserDuties()
        {

        }
    }
}
