using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 测量请求帧
    /// </summary>
    /// <remarks>
    /// 请求消息格式：
    ///     - 监测指令格式如：RMTP:PARAM:frequency=97.1MHz,ifbw=120kHz,span=300kHz, squelchthreshold=40dBμV,demodmode=FM,detector=peak\n
    ///     - 非监测指令格式如：RMTP:WBCIF:CHANNEL_NAME:START: parameter\n
    /// </remarks>
    public class CommandMessageFrame : RmtpDataFrame
    {
        private readonly Dictionary<string, string> _mPropertyValue = new Dictionary<string, string>();
        private readonly List<string> _mParameterList = new List<string>();
        private readonly RmtpCommand _command;
        public CommandMessageFrame(RmtpCommand pCommand)
        {
            _command = pCommand;
        }
        /// <summary>
        /// 添加参数值
        /// </summary>
        /// <param name="pPropertyName">非空时 - 表示 name=value 的参数值；空时 - 表示 RMTP:value1:value2</param>
        /// <param name="pValue">参数值</param>
        public void AddParameter(string pPropertyName, string pValue)
        {
            if (string.IsNullOrEmpty(pPropertyName))
                _mParameterList.Add(pValue);
            else
                _mPropertyValue[pPropertyName] = pValue;
        }

        public string Message
        {
            get
            {
                string val;
                var sb = new StringBuilder();
                sb.Append(_command.ToString());
                foreach (string v in _mParameterList)
                {
                    sb.AppendFormat(":{0}", v);
                }
                if (_mPropertyValue.Count == 0)
                {
                    val = sb.ToString();
                }
                else
                {
                    sb.Append(":");
                    foreach (KeyValuePair<string, string> v in _mPropertyValue)
                    {
                        sb.AppendFormat("{0}={1},", v.Key, v.Value);
                    }
                    sb = sb.Remove(sb.Length - 1, 1);
                    val = sb.ToString();
                }

                return string.Format("RMTP:{0}\n", val);
            }
        }

        public override byte[] ToBytes()
        {
            Data = Encoding.UTF8.GetBytes(Message);
            return Data;
        }
    }
}
