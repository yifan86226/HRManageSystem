using Best.VWPlatform.Common.Rmtp.DataFrames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.Commands
{
    /// <summary>
    /// Rmtp命令属性
    /// </summary>
    public class RmtpCommandAttribute : Attribute
    {
        private RmtpCommand _rmtpCmd;
        /// <summary>
        /// Rmtp命令属性构造函数
        /// </summary>
        /// <param name="pCmd">Rmtp命令</param>
        public RmtpCommandAttribute(RmtpCommand pCmd)
        {
            _rmtpCmd = pCmd;
        }

        public RmtpCommand RmtpCommand
        {
            get { return _rmtpCmd; }
        }
    }
    /// <summary>
    /// Rmtp命令处理接口
    /// </summary>
    public interface IRmtpCommandHandler
    {
        /// <summary>
        /// 命令处理
        /// </summary>
        /// <param name="pMessageFrame">测量请求帧</param>
        /// <param name="pParameter">参数</param>
        void CommandHandler(CommandMessageFrame pMessageFrame, object pParameter);
    }
}
