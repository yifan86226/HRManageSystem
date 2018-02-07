using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Core.Rpc
{
    internal class RpcState
    {
        public const int BufferSize = 1024;

        public RpcState()
        {
            BufferRead = new byte[BufferSize];
            RequestData = new StringBuilder();
        }

        public HttpWebRequest WebRequest
        {
            get;
            internal set;
        }

        public HttpWebResponse WebResponse
        {
            get;
            internal set;
        }

        public Stream ResponseStream
        {
            get;
            internal set;
        }

        public byte[] BufferRead
        {
            get;
            internal set;
        }

        public StringBuilder RequestData
        {
            get;
            private set;
        }

        public RpcResultCallback ResultCallback
        {
            get;
            internal set;
        }

        public string MethodContext
        {
            get;
            internal set;
        }

        public SynchronizationContext SyncContext
        {
            get;
            internal set;
        }

        public RpcResult RpcResult
        {
            get;
            internal set;
        }

        public BatchInvokeCallbackObject BatchInvokeCallbackObject { get; internal set; }

        public bool IsOnlyOne { get; internal set; }
        public string OnlyOneCode { get; internal set; }
    }
}
