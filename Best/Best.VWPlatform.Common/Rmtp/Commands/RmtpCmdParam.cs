using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.Commands
{
    [RmtpCommand(RmtpCommand.PARAM)]
    public class RmtpCmdParam : IRmtpCommandHandler
    {
        public void CommandHandler(DataFrames.CommandMessageFrame pMessageFrame, object pParameter)
        {
            var ps = (object[])pParameter;
            var paramArray = (object[])ps[0];

            var paramList = (List<Tuple<string, string>>)paramArray[0];
            foreach (Tuple<string, string> param in paramList)
            {
                pMessageFrame.AddParameter(param.Item1, param.Item2);
            }
        }
    }
}
