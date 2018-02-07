using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.helper
{
    public class InternetHelper
    {
        /// <summary>
        /// 判断是否联网
        /// </summary>
        /// <param name="p_linkIp">待连接的IP地址或网址</param>
        /// <returns></returns>
        public static bool IsConn(string p_linkIp)
        {
            System.Net.NetworkInformation.Ping ping;
            System.Net.NetworkInformation.PingReply res;

            ping = new System.Net.NetworkInformation.Ping();
            try
            {
                res = ping.Send(p_linkIp);
                if (res.Status != System.Net.NetworkInformation.IPStatus.Success)
                    return false;
                else
                    return true;
            }
            catch (Exception er)
            {
                return false;
            }
        }
    }
}
