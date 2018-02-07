using Best.VWPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 接收RMTP数据帧基类
    /// </summary>
    public class ReceiveRmtpDataFrameBase : IReceiveDataFrame
    {
        public virtual void Receive(RmtpDataFrame pDataFrame)
        {
            if (Receiving != null)
            {
                var args = new CancelEventArgs();
                Receiving(pDataFrame, args);
                if (args.Cancel)
                    return;
            }
            Received(pDataFrame);
        }
        /// <summary>
        /// 接收数据帧并处理
        /// </summary>
        /// <param name="pDataFrame"></param>
        protected virtual void Received(RmtpDataFrame pDataFrame)
        {
            // 派生类中接收数据帧并处理
        }
        /// <summary>
        /// 接收数据帧前预处理，或取消继续接收
        /// </summary>
        public event Action<RmtpDataFrame, CancelEventArgs> Receiving;
    }
}
