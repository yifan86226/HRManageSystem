using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Core.Rpc
{
    enum RpcResultCode
    {
        /// <summary>
        /// 调用失败
        /// </summary>
        Fail = 0,
        /// <summary>
        /// 调用成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 2,
        /// <summary>
        /// 确定信息
        /// </summary>
        Confirm = 3,
    }

    public class RpcResult
    {
        private int _mCode = (int)RpcResultCode.Success;
        private string _mMessage = string.Empty;
        /// <summary>
        /// 调用服务得到的数据
        /// </summary>
        public object Result
        {
            get;
            set;
        }

        public int Code
        {
            get
            {
                return _mCode;
            }
            set
            {
                _mCode = value;
            }
        }
        /// <summary>
        /// 自定义消息
        /// </summary>
        public string Message
        {
            get
            {
                return _mMessage;
            }
            set
            {
                _mMessage = value;
            }
        }
        /// <summary>
        /// 失败堆栈内容
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 校验Rpc调用是否成功
        /// </summary>
        /// <returns>true - 成功</returns>
        public bool Checking()
        {
            return _mCode == (int)RpcResultCode.Success;
        }

        /// <summary>
        /// 结果处理完毕，true - 已处理，false - 未处理
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// 设置自定义消息
        /// </summary>
        /// <param name="pCustomCode">自定义编码</param>
        /// <param name="pMessage">自定义消息</param>
        public void SetCustomMessage(int pCustomCode, string pMessage)
        {
            var v = pCustomCode == (int)RpcResultCode.Fail
                    || pCustomCode == (int)RpcResultCode.Success
                    || pCustomCode == (int)RpcResultCode.Warning
                    || pCustomCode == (int)RpcResultCode.Confirm;
            Debug.Assert(!v, "在SetCustomMessage中，自定义编码不可以用 0、1、2、3");
            if (v) return;

            _mCode = pCustomCode;
            _mMessage = pMessage;
        }

        public void SetException(Exception pException)
        {
            this.Code = (int) RpcResultCode.Fail;
            this.Message = pException.ToString();
        }
    }
}
