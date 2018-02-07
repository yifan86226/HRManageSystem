using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.Commands
{
    [RmtpCommand(RmtpCommand.AUDIO)]
    public class RmtpCmdAudio : IRmtpCommandHandler
    {
        public void CommandHandler(DataFrames.CommandMessageFrame pMessageFrame, object pParameter)
        {
            object[] ps = (object[])pParameter;
            string paramValue = (string)ps[0];
            pMessageFrame.AddParameter(string.Empty, paramValue);
        }
    }
}
