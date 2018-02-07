using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.Commands
{
    /// <summary>
    /// 登陆认证指令"RMTP:VERIF:METHOD=01,USER=rxtest,PASSWD=rxtest\n"
    /// </summary>
    [RmtpCommand(RmtpCommand.VERIF)]
    public class RmtpCmdVerify : IRmtpCommandHandler
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
