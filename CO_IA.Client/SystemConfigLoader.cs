#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：系统配置加载器,提供从命令行或者传入参数数组加载启动参数进而读取系统配置功能
 * 日 期 ：2016-08-12
 ***************************************************************#@#***************************************************************/
#endregion

using PT.Profile.Interface;
using PT.Profile.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client
{
    /// <summary>
    /// 系统配置加载器,提供从命令行或者传入参数数组加载启动参数进而读取系统配置功能
    /// </summary>
    public static class SystemConfigLoader
    {
        /// <summary>
        /// 登记系统配置信息,主要登记目录服务地址及客户端框架初始化和人员基本信息加载
        /// </summary>
        /// <param name="commands">参数数组,从中获取初始化所需参数信息,如果为空将从命令行获取这些参数</param>
        /// <returns>重新生成的启动参数的字符串格式,不包含第一个参数(启动程序名称)</returns>
        public static string RegisterSystemConfig(string[] commands=null)
        {
#if DEBUG
            commands = new string[5];
            //584035ea-8e67-45ee-86ce-8e3bb0ea323f  4.0导入管理员ID
            //73C54660-BCC3-4E6F-8144-F4369585589A  5.0初始化管理员ID
            string[] args = @"/CatalogIP=192.168.3.8 /CatalogPort=8733 /Code=2300 /UserID=73C54660-BCC3-4E6F-8144-F4369585589A".Split(' ');
            args.CopyTo(commands, 1);
#else
            if (commands == null)
            {
                commands = System.Environment.GetCommandLineArgs();
            }
            if (commands.Length < 5)
            {
                throw new Exception("非法登录,请从平台登录系统");
            }
#endif
            Dictionary<string, string> dicParam = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            int index;
            for (int i = 1; i < commands.Length; i++)
            {
                index = commands[i].IndexOf('=');
                if ((index > 0 && index < commands[i].Length - 1))
                {
                    dicParam[commands[i].Substring(0, index).TrimStart('/')] = commands[i].Substring(index + 1);
                }
            }
            string ip, port;
            if (!dicParam.TryGetValue("CatalogIP", out ip))
            {
                throw new Exception("无法分析出目录服务地址,请从平台登录系统");
            }
            if (!dicParam.TryGetValue("CatalogPort", out port))
            {
                throw new Exception("无法分析出目录服务端口,请从平台登录系统");
            }
            int intPort;
            if (!int.TryParse(port, out intPort))
            {
                throw new Exception("目录服务端口号错误,请从平台登录系统");
            }
            RiasPortal.Current.SystemConfig.CatalogIP = ip;
            RiasPortal.Current.SystemConfig.CatalogPort = intPort;
            string userID;
            if (dicParam.TryGetValue("UserID", out userID))
            {
                RiasPortal.Current.UserSetting.UserID = userID;
            }
            else
            {
                throw new Exception("无法分析出登录用户,请从平台登录系统");
            }
            string areaCode;
            if (dicParam.TryGetValue("Code", out areaCode))
            {
                RiasPortal.Current.SystemConfig.LoginArea = areaCode;
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.InitOperationInvoker(RiasPortal.Current.SystemConfig.CatalogIP, RiasPortal.Current.SystemConfig.CatalogPort, true, callback =>
                {
                    if (!callback.IsValid)
                    {
                        throw callback.Exception;
                    }
                });
            //System.Threading.Thread.Sleep(10000);
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<PT.Profile.Interface.IOrganization>(channel =>
                {
                    try
                    {
                        var userInfo = channel.GetUser(RiasPortal.Current.UserSetting.UserID);
                        if (userInfo == null)
                        {
                            throw new Exception("无效的登录用户");
                        }
                        RiasPortal.Current.UserSetting.UserName = userInfo.RealName;
                    }
                    catch
                    {
                        throw;
                    }
                });
            //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<ISubsystem>(channel =>
            //{
            //    Subsystem sys = channel.GetSubsystem(CO_IA.Public.SubSystemIDs.Rias);
            //    if (sys != null)
            //    {
            //        Utility.RiasSystemName = sys.SubsystemName;
            //    }
            //});
//#if DEBUG
//            using (System.IO.StreamReader sr =System.IO.File.OpenText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"MapServerUrl.txt")))
//            {
//                string serverUrl=sr.ReadLine();
//                if (!string.IsNullOrWhiteSpace(serverUrl))
//                {
//                    RiasPortal.Current.MapConfig.ElectricUrl=serverUrl;
//                }
//            }
//#endif
            return string.Join(" ", commands, 1, commands.Length - 1);
        }

        /// <summary>
        /// 将参数指定的字符串数组转换为数据字典
        /// </summary>
        /// <param name="srcs">要转换的字符串数组</param>
        /// <param name="seperator">转换分隔符</param>
        /// <returns>转换后的数据字典</returns>
        private static Dictionary<string, string> GetMapping(string[] srcs, params char[] seperator)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] result;
            foreach (string str in srcs)
            {
                result = str.Split(seperator);
                if (result.Length == 2)
                {
                    dic[result[0]] = result[1];
                }
            }
            return dic;
        }

        /// <summary>
        /// 登记用户姓名映射表,异步方法
        /// </summary>
        /// <param name="callback">加载映射信息后的回调方法</param>
        public static void RegisterUserIDNameMappingAsync(Action<Dictionary<string,string>> callback)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.InvokeAsync<PT.Profile.Interface.IOrganization, Dictionary<string, string>>(subSystem =>
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var result = subSystem.GetUsers();
                if (result != null)
                {
                    foreach (var user in result)
                    {
                        dic[user.UserID] = user.RealName;
                    }
                }
                return dic;
            }, result =>
            {
                if (result.IsValid)
                {
                    callback(result.Result);
                }
                else
                {
                    throw result.Exception;
                }
            });
        }
    }
}
