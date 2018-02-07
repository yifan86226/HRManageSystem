#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：消息接收服务承载类
 * 日 期 ：2016-09-05
 ***************************************************************#@#***************************************************************/
#endregion

using AT_BC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client
{
    /// <summary>
    /// 消息接收服务承载类
    /// </summary>
    //internal class MessageReceiveServiceHost : HttpServiceHost
    //{
    //    /// <summary>
    //    /// 客户端登记代理
    //    /// </summary>
    //    private Action<int> registerClient;

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="serviceType">服务类型</param>
    //    /// <param name="contractInterface">要发布的该服务的实现的某一接口类型</param>
    //    /// <param name="address">服务基地址</param>
    //    //public MessageReceiveServiceHost(int port,Action<int> registerClient)
    //    //    : base(typeof(MessageReceiveService), typeof(I_CO_IA.MessageCenter.I_CO_IA_MessageReceive), port)
    //    //{
    //    //    this.registerClient = registerClient;
    //    //}

    //    /// <summary>
    //    /// 服务打开方法,启动后台同步线程,负责向服务定时发送在线消息
    //    /// </summary>
    //    protected override void OnOpened()
    //    {
    //        base.OnOpened();
    //        this.registerClient.Invoke(this.ServicePort);
    //        this.syncThread = new System.Threading.Thread(SyncThreadEntry);
    //        this.syncThread.IsBackground = true;
    //        this.syncThread.Start();
    //    }

    //    /// <summary>
    //    /// 同步线程入口
    //    /// </summary>
    //    private void SyncThreadEntry()
    //    {
    //        try
    //        {
    //            //while (true)
    //            //{
    //            //    System.Threading.Thread.Sleep(53 * 1000);
    //            //    try
    //            //    {
    //            //        bool isOnline = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MessageCenter.I_CO_IA_MessageCenter, bool>
    //            //            (channel => channel.NotifyOnline(this.ServicePort));
    //            //        if (!isOnline)
    //            //        {
    //            //            this.registerClient.Invoke(this.ServicePort);
    //            //        }
    //            //    }
    //            //    catch (System.Threading.ThreadAbortException)
    //            //    {
    //            //        throw;
    //            //    }
    //            //    catch
    //            //    {
    //            //    }
    //            //}
    //        }
    //        catch (System.Threading.ThreadAbortException)
    //        {
    //        }
    //        finally
    //        {
    //            //try
    //            //{
    //            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MessageCenter.I_CO_IA_MessageCenter>
    //            //    (channel => channel.NotifyOffline(this.ServicePort));
    //            //}
    //            //catch
    //            //{

    //            //}
    //        }
    //    }

    //    /// <summary>
    //    /// 关闭方法
    //    /// </summary>
    //    /// <param name="timeout">管理超时间隔</param>
    //    protected override void OnClose(TimeSpan timeout)
    //    {
    //        base.OnClose(timeout);
    //        if (this.syncThread != null)
    //        {
    //            this.syncThread.Abort();
    //        }
    //    }

    //    /// <summary>
    //    /// 同步线程
    //    /// </summary>
    //    private System.Threading.Thread syncThread;
    //}
}
