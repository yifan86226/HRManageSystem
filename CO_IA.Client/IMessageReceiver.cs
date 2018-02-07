#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：消息接收器接口定义,需要接收消息的对象需要实现该接口
 * 日 期 ：2016-09-03
 ***************************************************************#@#***************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client
{
    /// <summary>
    /// 消息接收器接口定义,需要接收消息的对象需要实现该接口
    /// </summary>
    public interface IMessageReceiver
    {
        /// <summary>
        /// 接收消息方法定义
        /// </summary>
        /// <param name="message">接收到的消息</param>
        void Receive(CO_IA.Data.ActivityMessage message);

        /// <summary>
        /// 要在其上执行接收消息的同步上下文,通常该对象未消息接收器实现对象的创建者(UI)同步上下文
        /// </summary>
        System.Threading.SynchronizationContext SyncContext
        {
            get;
        }
    }
}
