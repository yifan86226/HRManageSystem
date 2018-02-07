using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.Commands
{
    [RmtpCommand(RmtpCommand.RSTOP)]
    public class RmtpCmdStop : IRmtpCommandHandler
    {
        public void CommandHandler(DataFrames.CommandMessageFrame pMessageFrame, object pParameter)
        {
            object[] ps = (object[])pParameter;
            //优先级代码
            pMessageFrame.AddParameter(string.Empty, ps[1].ToString());
        }
    }
}
