using System.Windows.Media.Media3D;
using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Rmtp.Commands;
using Best.VWPlatform.Common.Rmtp.DataFrames;
using Best.VWPlatform.Common.User;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Best.VWPlatform.Common.Rmtp.MeasureHandler;
using System.Collections.Concurrent;

namespace Best.VWPlatform.Common.Rmtp
{
    public delegate void DataFrameCachesHandler();
    /// <summary>
    /// Rmtp数据帧缓存处理
    /// </summary>
    internal class RmtpDataFrameCache : ISocketReceive, IDisposable
    {
        private byte[] _buffer;

        private readonly Queue<byte[]> _bytesCache = new Queue<byte[]>();
        private readonly object _lockObj = new object();

        /// <summary>
        /// 缓冲区
        /// </summary>
        private RmtpDataFrameCacheValue _dataFrameCaches;

        /// <summary>
        /// Rmtp指令
        /// </summary>
        private RmtpCommand _rmtpcmd;

        /// <summary>
        /// 停止标识
        /// </summary>
        private bool _isStop = false;

        /// <summary>
        /// Cache的构造函数
        /// </summary>
        /// <param name="pCmd">Rmtp指令</param>
        public RmtpDataFrameCache(RmtpCommand pCmd)
        {
            _rmtpcmd = pCmd;

            _dataFrameCaches = new RmtpDataFrameCacheValue();

            ThreadPool.QueueUserWorkItem(TryDequeueFromCache);
        }

        private void TryDequeueFromCache(object state)
        {
            while (!_isStop)
            {
                try
                {
                    var data = _dataFrameCaches.DequeueFrameData();
                    if (data != null)
                    {
                        OnReceiveDataFrame(data);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RMTPDataFrameCache {0}", ex.Message);
                }
            }
        }

        public void StopDequeueFromCache()
        {
            _isStop = true;
        }

        public void Receive(byte[] pReceiveData)
        {
            if (pReceiveData == null)
                return;
            lock (_lockObj)
            {
                var buffer = new byte[pReceiveData.Length];
                Buffer.BlockCopy(pReceiveData, 0, buffer, 0, pReceiveData.Length);
                _bytesCache.Enqueue(buffer);

                AnalyzingPackage();
            }
        }

        private void AnalyzingPackage()
        {
        Start_Analyzing:
            try
            {
                byte[] buffer = GetBuffer();
                if (buffer == null)
                {
                    return;
                }

                byte[] surplusBuffer;
                RmtpDataFrame data = RmtpDataFrame.Load(buffer, out surplusBuffer);
                if (data == null)
                {
                    string strcmd = ByteToString(buffer);
                    if (FindString(strcmd, "VERSION"))
                    {
                        string strvertion = splitCmdString(strcmd, ":");
                        if (strvertion == "VERSION")//版本信息
                        {
                            //登陆认证指令
                            string username = ((UserInfo)Application.Current.Properties["UserInfo"]).UserName;
                            string password = ((UserInfo)Application.Current.Properties["UserInfo"]).Password;
                            var userverify = new UserVerifyHandler("01", username, password);
                            RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.VERIF, new object[] { userverify.ToList() });
                        }
                    }
                    else if (FindString(strcmd, "VERIF"))
                    {
                        string strresult = splitCmdString(strcmd, ":");
                        if (strresult == "+OK")//登陆认证结果成功，发送预执行指令
                        {
                            var param = new Tuple<string, string>(
                                                "88888888@gnosis",
                                                "88888888@gnosis");
                            RmtpManager.Rmtp.ExecuteCommand(_rmtpcmd, param);//预执行指令
                        }
                    }
                    else
                    {
                        BufferBytes(buffer);
                    }
                }
                else
                {
                    BufferBytes(surplusBuffer);
                   
                    switch (data.Header.DataType)
                    {
                        case RmtpDataTypes.执行结果信息:
                            _dataFrameCaches.ExecuteResult.Enqueue(data);
                            break;
                        case RmtpDataTypes.音频数据:
                            _dataFrameCaches.Audio.Enqueue(data);
                            break;
                        default:
                            _dataFrameCaches.Other.Enqueue(data);
                            break;
                    }

                    goto Start_Analyzing;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AnalyzingPackage {0}", ex.Message);
            }
        }

        public event Action<RmtpDataFrame> ReceiveDataFrame;

        #region 内部方法

        private void OnReceiveDataFrame(RmtpDataFrame pFrame)
        {
            if (ReceiveDataFrame != null && pFrame != null)
                ReceiveDataFrame(pFrame);
        }

        private void BufferBytes(byte[] pBuffer)
        {
            if (pBuffer == null)
                return;

            if (_buffer == null)
            {
                _buffer = new byte[pBuffer.Length];
                Buffer.BlockCopy(pBuffer, 0, _buffer, 0, pBuffer.Length);
            }
        }

        private byte[] GetBuffer()
        {
            try
            {
                byte[] buffer = null;
                int bufferLen = 0;
                if (_bytesCache.Count > 0)
                {
                    buffer = _bytesCache.Dequeue();
                    bufferLen = buffer.Length;
                }

                byte[] surplusBuffer = null;
                int cacheBufferLen = (_buffer != null ? _buffer.Length : 0);
                if (bufferLen > 0 || cacheBufferLen > 0)
                    surplusBuffer = new byte[bufferLen + cacheBufferLen];

                if (cacheBufferLen > 0 && _buffer != null)
                    Buffer.BlockCopy(_buffer, 0, surplusBuffer, 0, _buffer.Length);
                if (bufferLen > 0 && buffer != null)
                    Buffer.BlockCopy(buffer, 0, surplusBuffer, (_buffer != null ? _buffer.Length : 0), buffer.Length);
                _buffer = null;
                return surplusBuffer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 字节数据转换成字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string ByteToString(byte[] buffer)
        {
            string strbuffer = Encoding.UTF8.GetString(buffer);
            return strbuffer;
        }
        /// <summary>
        /// 分离字符串，用于得到版本信息和验证结果信息
        /// </summary>
        /// <param name="strcmd"></param>
        /// <param name="strsplit"></param>
        /// <returns></returns>
        private string splitCmdString(string strcmd, string strsplit)
        {
            string[] sArray;
            string[] sArray1;
            if (strcmd.IndexOf(strsplit) == -1)
            {
                return null;
            }
            else
            {
                sArray = Regex.Split(strcmd, strsplit, RegexOptions.IgnoreCase);
                sArray1 = Regex.Split(sArray[2], "\n", RegexOptions.IgnoreCase);
            }

            if (sArray1[0] == "+OK" || sArray1[0] == "+REFUSE")
            {
                return sArray1[0];
            }

            return sArray[1];
        }
        /// <summary>
        /// 判断是否是版本或登陆认证返回结果
        /// </summary>
        /// <param name="strcmd"></param>
        /// <param name="strsplit"></param>
        /// <returns></returns>
        private bool FindString(string strcmd, string strfind)
        {
            if (strcmd.IndexOf(strfind) == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region IDisposable
        private bool _disposed;
        ~RmtpDataFrameCache()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool pDisposing)
        {
            if (!_disposed)
            {
                if (pDisposing)
                {
                    _isStop = true;
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
