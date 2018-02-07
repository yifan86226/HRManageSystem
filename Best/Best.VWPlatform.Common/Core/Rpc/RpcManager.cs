using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Best.VWPlatform.Common.Utility;

namespace Best.VWPlatform.Common.Core.Rpc
{

    /// <summary>
    /// 远程过程调用回调函数委托
    /// </summary>
    /// <param name="pResult"></param>
    public delegate void RpcResultCallback(RpcResult pResult);

    /// <summary>
    /// 远程过程调用管理
    /// </summary>
    public class RpcManager
    {
        #region 变量
        private static readonly object LockObj = new object();
        private static readonly object LockOnlyOne = new object();
        private static RpcManager _mRpcManager;
        private static object _currentBatchInvokeCallbackObject;
        private static readonly Dictionary<object, BatchInvokeCallbackObject> BatchInvokeQueue = new Dictionary<object, BatchInvokeCallbackObject>();
        private static readonly Dictionary<string, int> InvokeCountDic = new Dictionary<string, int>();
        #endregion

        #region 构造函数
        private RpcManager()
        {
        }
        #endregion

        #region 公有方法
        public void Invoke(string pInterfaceName, string pFunctionName, object[] pParames, RpcResultCallback pCallback, bool pReturnOnlyOne = false)
        {
            BatchInvokeCallbackObject bInvokeCallbackObject = null;
            try
            {
                //是否为批量调用
                if (_currentBatchInvokeCallbackObject != null)
                {
                    if (BatchInvokeQueue.ContainsKey(_currentBatchInvokeCallbackObject))
                    {
                        bInvokeCallbackObject = BatchInvokeQueue[_currentBatchInvokeCallbackObject];
                        if (bInvokeCallbackObject != null)
                            bInvokeCallbackObject.AddInvokeCount();
                    }
                }
                //判断在多次调用同一个方法时，是否仅返回一次结果
                string code = pInterfaceName + pFunctionName;
                lock (LockOnlyOne)
                {
                    if (pReturnOnlyOne)
                    {
                        if (InvokeCountDic.ContainsKey(code))
                        {
                            //累加调用方法次数
                            InvokeCountDic[code]++;
                        }
                        else
                        {
                            //初次调用该方法赋值1，该方法执行返回后再减1
                            InvokeCountDic[code] = 1;
                        }
                    }
                }
                SynchronizationContext syncContext = SynchronizationContext.Current;
                string url = "";//@? Utile.GetHostUrl();
                var remoting = new Uri(string.Format("{0}/RemotingHandler.ashx", url.ToString(CultureInfo.InvariantCulture)));
                var hwRequest = (HttpWebRequest)WebRequest.Create(remoting);
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                string methodContext = RpcMethod.GetRpcMethodContext(pInterfaceName, pFunctionName, pParames);
                var rpcState = new RpcState
                {
                    WebRequest = hwRequest,
                    ResultCallback = pCallback,
                    SyncContext = syncContext,
                    MethodContext = methodContext,
                    BatchInvokeCallbackObject = bInvokeCallbackObject,
                    IsOnlyOne = pReturnOnlyOne,
                    OnlyOneCode = code
                };
                hwRequest.BeginGetRequestStream(OnGetRequestStream, rpcState);
            }
            catch (Exception ex)
            {
                if (pCallback != null)
                {
                    var result = new RpcResult();
                    result.SetException(ex);
                    pCallback(result);
                    if (bInvokeCallbackObject != null)
                    {
                        bInvokeCallbackObject.RemoveInvokeCount(result);
                    }
                }
            }
        }

        public void BatchInvoke(Action<IEnumerable<RpcResult>> pCompleteBatchInvokeFun, Action pBatchInvokeContext)
        {
            lock (LockObj)
            {
                _currentBatchInvokeCallbackObject = pCompleteBatchInvokeFun;
                BatchInvokeQueue[pCompleteBatchInvokeFun] = new BatchInvokeCallbackObject { CompletedCallback = pCompleteBatchInvokeFun };
                if (pBatchInvokeContext != null)
                    pBatchInvokeContext();
                _currentBatchInvokeCallbackObject = null;
                if (BatchInvokeQueue.ContainsKey(pCompleteBatchInvokeFun))
                {
                    if (BatchInvokeQueue[pCompleteBatchInvokeFun].InvokeCount <= 0)
                    {
                        if (pCompleteBatchInvokeFun != null)
                        {
                            pCompleteBatchInvokeFun(new List<RpcResult>());
                        }
                    }
                }
            }
        }
        #endregion

        #region 内部
        private void OnGetRequestStream(IAsyncResult pAsyncResult)
        {
            var rpcState = (RpcState)pAsyncResult.AsyncState;
            var webRequest = rpcState.WebRequest;
            try
            {
                var stream = webRequest.EndGetRequestStream(pAsyncResult);
                byte[] buffer = Encoding.UTF8.GetBytes(rpcState.MethodContext);
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(buffer, 0, buffer.Length);
                    writer.Flush();
                }
                webRequest.BeginGetResponse(OnGetResponse, rpcState);
            }
            catch (Exception ex)
            {
                ExceptionHandler(rpcState, new Exception(webRequest.RequestUri.ToString(), ex));
            }
        }
        private void OnGetResponse(IAsyncResult pAsyncResult)
        {
            try
            {
                var rpcState = (RpcState)pAsyncResult.AsyncState;
                if (rpcState == null)
                    return;
                HttpWebRequest webRequest = rpcState.WebRequest;
                if (webRequest == null)
                    return;

                rpcState.WebResponse = (HttpWebResponse)webRequest.EndGetResponse(pAsyncResult);
                using (Stream responseStream = rpcState.WebResponse.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream);
                    var readValues = reader.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(readValues))
                    {
                        var result = Utile.Deserialize<RpcResult>(readValues);
                        rpcState.RpcResult = result;
                    }
                }
                rpcState.WebResponse.Close();
                if (rpcState.SyncContext == null)
                    return;
                rpcState.SyncContext.Send(delegate(object pState)
                {
                    var rs = pState as RpcState;

                    if (rs != null
                        && rs.RpcResult != null
                        && rs.ResultCallback != null
                        && (rs.ResultCallback.Target != null || rs.ResultCallback.Method.IsStatic))
                    {
                        if (!string.IsNullOrWhiteSpace(rs.OnlyOneCode) && rs.IsOnlyOne && _currentBatchInvokeCallbackObject == null)
                        {
                            lock (LockOnlyOne)
                            {
                                if (InvokeCountDic.ContainsKey(rs.OnlyOneCode))
                                {
                                    InvokeCountDic[rs.OnlyOneCode]--;
                                    if (InvokeCountDic[rs.OnlyOneCode] <= 0)
                                    {
                                        //单个调用完成回调
                                        rs.ResultCallback(rs.RpcResult);
                                        InvokeCountDic.Remove(rs.OnlyOneCode);
                                    }
                                }
                                else
                                {
                                    //单个调用完成回调
                                    rs.ResultCallback(rs.RpcResult);
                                }
                            }
                        }
                        else
                        {
                            //单个调用完成回调
                            rs.ResultCallback(rs.RpcResult);
                        }
                        //批量调用完成回调入口
                        BatchInvokeCallbackObject batchInvokeCallbackObject = rs.BatchInvokeCallbackObject;
                        if (batchInvokeCallbackObject != null)
                        {
                            batchInvokeCallbackObject.RemoveInvokeCount(rs.RpcResult.Checking() ? null : rs.RpcResult);
                        }
                    }
                }, rpcState);
            }
            catch (Exception ex)
            {
                //XGZ：发生异常时如何处理？
                //if (webRequest != null && webRequest.RequestUri != null)
                //{
                //    ExceptionHandler(rpcState, new Exception(webRequest.RequestUri.ToString(), ex));
                //}
                //else
                //{
                //    ExceptionHandler(rpcState, ex);
                //}
            }
        }
        private static void ExceptionHandler(RpcState rpcState, Exception ex)
        {
            try
            {
                if (rpcState.SyncContext == null)
                    return;
                rpcState.SyncContext.Send(delegate(object pState)
                {
                    var rs = pState as RpcState;
                    var result = new RpcResult();
                    result.SetException(ex);
                    if (rs != null && rs.ResultCallback != null)
                    {
                        rs.ResultCallback(result);
                        BatchInvokeCallbackObject batchInvokeCallbackObject = rs.BatchInvokeCallbackObject;
                        if (batchInvokeCallbackObject != null)
                        {
                            batchInvokeCallbackObject.RemoveInvokeCount(null);
                        }
                    }
                }, rpcState);
            }
            catch (ObjectDisposedException) { }
        }


        #endregion

        public static RpcManager Current
        {
            get
            {
                if (_mRpcManager == null)
                {
                    lock (LockObj)
                    {
                        if (_mRpcManager == null)
                        {
                            _mRpcManager = new RpcManager();
                        }
                    }
                }
                return _mRpcManager;
            }
        }
    }

    /// <summary>
    /// 批次调用回调对象
    /// </summary>
    internal class BatchInvokeCallbackObject
    {
        private readonly List<RpcResult> _errorResult = new List<RpcResult>();
        /// <summary>
        /// 添加调用计数
        /// </summary>
        public void AddInvokeCount()
        {
            InvokeCount++;
        }
        /// <summary>
        /// 删除调用计数
        /// </summary>
        public void RemoveInvokeCount(RpcResult pErrResult)
        {
            InvokeCount--;
            if (pErrResult != null)
                _errorResult.Add(pErrResult);
            if (InvokeCount <= 0 && CompletedCallback != null)
            {
                CompletedCallback(_errorResult);
                _errorResult.Clear();
                InvokeCount = 0;
            }
        }

        public Action<IEnumerable<RpcResult>> CompletedCallback;
        internal int InvokeCount;
    }
}
