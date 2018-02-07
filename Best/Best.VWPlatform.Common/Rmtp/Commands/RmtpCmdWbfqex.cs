using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.Commands
{
    /// <summary>
    /// 预执行指令
    /// </summary>
    [RmtpCommand(RmtpCommand.WBQEX)]
    public class RmtpCmdWbfqex : IRmtpCommandHandler
    {
        public void CommandHandler(DataFrames.CommandMessageFrame pMessageFrame, object pParameter)
        {
            var ps = (object[])pParameter;
            var paramValue = (Tuple<string, string>)ps[0];
            //设备ID，88888888@gnosis
            pMessageFrame.AddParameter(string.Empty, paramValue.Item1);
            //用户ID，88888888@gnosis
            pMessageFrame.AddParameter(string.Empty, paramValue.Item2);
            //优先级代码，1
            pMessageFrame.AddParameter(string.Empty, ps[1].ToString());
        }
    }
}
