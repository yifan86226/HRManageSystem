#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：消息接收器接口定义,需要接收消息的对象需要实现该接口
 * 日 期 ：2016-08-12
 ***************************************************************#@#***************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client
{
    /// <summary>
    /// 重大活动模块容器接口定义
    /// </summary>
    public interface IRiasModuleContainer
    {
        /// <summary>
        /// 获取当前操作的活动
        /// </summary>
        CO_IA.Data.Activity Activity
        {
            get;
        }

        ///// <summary>
        ///// 创建地图
        ///// </summary>
        ///// <returns>创建的地图控件</returns>
        //GS_MapBase.MapControl CreateMap();

        ExecutorLoginInfo GetExecutorLoginInfo();
    }

    public class ExecutorLoginInfo
    {
        public CO_IA.Data.ActivityPlaceInfo LoginPlace
        {
            get;
            set;
        }

        public CO_IA.Data.PP_OrgInfo LoginOrg
        {
            get;
            set;
        }
    }
}
