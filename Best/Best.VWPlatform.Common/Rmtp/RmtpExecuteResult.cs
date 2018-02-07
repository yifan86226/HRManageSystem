using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp
{
    /// <summary>
    /// 在执行请求消息后的应答消息
    /// </summary>
    public class RmtpExecuteResult : RmtpDataFrame
    {
        private static readonly Dictionary<ExecuteResultErrorCode, string> ErrorMessageDic;
        static RmtpExecuteResult()
        {
            ErrorMessageDic = new Dictionary<ExecuteResultErrorCode, string>()
                                   {
                                       {ExecuteResultErrorCode.未知错误, "未知错误"},
                                       {ExecuteResultErrorCode.已占用, "该业务功能已被优先级更高的请求占用"},
                                       {ExecuteResultErrorCode.未收到指令, "未收到预执行指令"},
                                       {ExecuteResultErrorCode.参数错误, "测量参数错误"},
                                       {ExecuteResultErrorCode.已停止, "设备已经停止"},
                                       {ExecuteResultErrorCode.已抢占, "该业务功能被优先级更高的请求抢占"},
                                       {ExecuteResultErrorCode.超时, "该业务功能执行时间超时"},
                                       {ExecuteResultErrorCode.转发服务器离线, "转发服务器离线"}
                                   };
        }
        /// <summary>
        /// 执行结果状态
        /// </summary>
        public ExecuteResult Result
        {
            get;
            internal set;
        }
        /// <summary>
        /// 执行结果返回的错误编码
        /// </summary>
        public ExecuteResultErrorCode ErrorCode
        {
            get;
            private set;
        }
        /// <summary>
        /// 执行结果返回的错误消息
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }
        /// <summary>
        /// 转换为执行结果类型
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <param name="pParameter"> </param>
        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            SetPropertyValue(this, System.Text.Encoding.UTF8.GetString(pDataFrame.Data, 0, pDataFrame.Data.Length));
            //Result = (ExecuteResult)Enum.Parse(typeof(ExecuteResult), this["RESULT"], true);
            ExecuteResult er;
            if (Enum.TryParse(this["RESULT"], out er))
            {
                Result = er;
            }
            string v = this["INFO"];
            string[] rs = v.Split(',');
            int code;
            if (int.TryParse(rs[0], out code))
            {
                ErrorCode = (ExecuteResultErrorCode)code;
                ErrorMessage = ErrorMessageDic[ErrorCode];
            }
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                switch (Result)
                {
                    case ExecuteResult.REFUSE:
                        ErrorMessage = ExecuteResultMessage.REFUSE;
                        break;
                    case ExecuteResult.FAILURE:
                        ErrorMessage = ExecuteResultMessage.FAILURE;
                        break;
                    case ExecuteResult.WARNING:
                        ErrorMessage = ExecuteResultMessage.WARNING;
                        break;
                }
            }
        }

    }
}
