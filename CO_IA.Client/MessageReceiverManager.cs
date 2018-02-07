#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：消息接收器管理器,负责管理消息接收器
 * 日 期 ：2016-09-02
 ***************************************************************#@#***************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client
{
    /// <summary>
    /// 消息接收器管理器,负责管理消息接收器
    /// </summary>
    internal class MessageReceiverManager
    {
        /// <summary>
        /// 同步上下文
        /// </summary>
        private System.Threading.SynchronizationContext syncContext = System.Threading.SynchronizationContext.Current;

        /// <summary>
        /// 消息接收器列表
        /// </summary>
        private List<IMessageReceiver> receiverList = new List<IMessageReceiver>();

        /// <summary>
        /// 同步锁对象
        /// </summary>
        private object syncObj = new object();

        /// <summary>
        /// 消息接收器管理器唯一实例对象
        /// </summary>
        private static readonly MessageReceiverManager Current = new MessageReceiverManager();

        /// <summary>
        /// 私有构造函数,避免管理器被以其他方式创建
        /// </summary>
        private MessageReceiverManager()
        {
        }

        /// <summary>
        /// 获取消息接收器
        /// </summary>
        /// <returns>当前消息接收器</returns>
        public static MessageReceiverManager GetMessageReceiver()
        {
            return Current;
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="message">接收到的消息</param>
        public static void ReceiveMessage(CO_IA.Data.ActivityMessage message)
        {
            Current.ReceiveActivityMessage(message);
        }

        /// <summary>
        /// 登记消息接收器
        /// </summary>
        /// <param name="receiver">消息接收器</param>
        public void RegisterMessageReceiver(IMessageReceiver receiver)
        {
            lock (syncObj)
            {
                foreach (var msgReceiver in this.receiverList)
                {
                    if (msgReceiver == receiver)
                    {
                        return;
                    }
                }
                this.receiverList.Add(receiver);
            }
        }

        /// <summary>
        /// 移除消息接收器
        /// </summary>
        /// <param name="receiver">要移除的消息接收器</param>
        public void RemoveMessageReceiver(IMessageReceiver receiver)
        {
            lock (syncObj)
            {
                foreach (var msgReceiver in this.receiverList)
                {
                    if (msgReceiver == receiver)
                    {
                        this.receiverList.Remove(msgReceiver);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 接收活动消息
        /// </summary>
        /// <param name="message">接收到的活动消息,管理器负责在接收器同步上下文上执行其接收方法</param>
        private void ReceiveActivityMessage(CO_IA.Data.ActivityMessage message)
        {
            lock (syncObj)
            {
                foreach (var msgReceiver in this.receiverList)
                {
                    this.GetReceiverSyncContext(msgReceiver).Post(stateObj =>
                    {
                        msgReceiver.Receive(message);
                    }, null);
                }
            }
        }

        /// <summary>
        /// 获取参数指定接收器的同步上下文
        /// </summary>
        /// <param name="receiver">要获取其同步上下文的接收器</param>
        /// <returns>接收器的同步上下文,如果其未定义将返回当前管理器的同步上下文</returns>
        private System.Threading.SynchronizationContext GetReceiverSyncContext(IMessageReceiver receiver)
        {
            return receiver.SyncContext ?? this.syncContext;
        }
    }
}
