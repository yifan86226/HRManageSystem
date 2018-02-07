using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Best.VWPlatform.Common.Core.Rpc;
using Newtonsoft.Json;

namespace Best.VWPlatform.Common.Rpc
{
    /// <summary>
    /// 远程过程调用对象
    /// </summary>
    public sealed class RpcObject
    {
        private static RpcObject _rpcObject;
        private static object LockObj = new object();

        public static RpcObject Current
        {
            get
            {
                if (_rpcObject == null)
                {
                    lock (LockObj)
                    {
                        if (_rpcObject == null)
                        {
                            _rpcObject = new RpcObject();
                        }
                    }
                }
                return _rpcObject;
            }
        }
        /// <summary>
        /// 远程过程调用
        /// </summary>
        /// <param name="pInterfaceName">接口名称</param>
        /// <param name="pFunctionName">方法名称</param>
        /// <param name="pParames">参数集合</param>
        /// <param name="pCallback">回调函数，方法调用结束</param>
        /// <param name="pReturnOnlyOne">true - 确保相同的方法多次调用后，只返回最后一次调用的结果。默认为 false</param>
        public static void Invoke(string pInterfaceName, string pFunctionName, object[] pParames, RpcResultCallback pCallback, bool pReturnOnlyOne = false)
        {
            //@?
            //var paramList = new List<object>
            //                    {
            //                        WMonitorConfig.Current.CatalogServiceIp,
            //                        WMonitorConfig.Current.CatalogServicePort,
            //                        WMonitorConfig.Current.RemotingToken
            //                    };
            //if (pParames != null)
            //{
            //    paramList.Add(JsonConvert.SerializeObject(pParames));
            //}
            //RpcManager.Current.Invoke(pInterfaceName, pFunctionName, paramList.ToArray(), delegate(RpcResult pResult)
            //{
            //    try
            //    {
            //        if (pResult.Code == RpcResultCode.TokenInvalid
            //            || pResult.Code == RpcResultCode.SocketError)
            //        {
            //            WaitIndicatorEx.WaitMessage(pResult.Message);
            //        }
            //        else
            //        {
            //            switch (pResult.Code)
            //            {
            //                case RpcResultCode.Fail:
            //                    {
            //                        Utile.ShowUnhandledException(pResult.Message);
            //                        try
            //                        {
            //                            if (pCallback != null)
            //                            {
            //                                pCallback(pResult);
            //                            }
            //                        }
            //                        catch
            //                        {
            //                        }
            //                    }
            //                    break;
            //                default:
            //                    {
            //                        if (pCallback != null)
            //                        {
            //                            pCallback(pResult);
            //                        }
            //                    }
            //                    break;
            //            }
            //            if (Current.RpcInvokeCompleted != null)
            //                Current.RpcInvokeCompleted(pResult);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(string.Format("接口：{0}，方法：{1}，回调函数内发生异常", pInterfaceName, pFunctionName), ex);
            //    }
            //}, pReturnOnlyOne);
        }
        /// <summary>
        /// 批量远程过程调用
        /// </summary>
        /// <param name="pCompleteBatchInvokeFun">
        /// pBatchInvokeContext内执行的所有批量调用，全部返回结果后执行的回调方法
        /// 回调参数 IEnumerable《RpcReuslt》 表示批量调用完成后，某些调用失败的结果集合
        /// </param>
        /// <param name="pBatchInvokeContext">可以包含N个远程过程调用 RpcObject.Invoke </param>
        /// <remarks>
        /// 不支持 pBatchInvokeContext中包含的多个RpcObject.Invoke 回调中再次异步 RpcObject.Invoke 调用
        /// </remarks>
        public static void BatchInvoke(Action<IEnumerable<RpcResult>> pCompleteBatchInvokeFun, Action pBatchInvokeContext)
        {
            RpcManager.Current.BatchInvoke(pCompleteBatchInvokeFun, pBatchInvokeContext);
        }

        public event Action<RpcResult> RpcInvokeCompleted;
    }
}
