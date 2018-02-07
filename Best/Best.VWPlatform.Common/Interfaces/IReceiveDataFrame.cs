using Best.VWPlatform.Common.Rmtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Interfaces
{
    /// <summary>
    /// 数据帧接收接口
    /// </summary>
    public interface IReceiveDataFrame
    {
        /// <summary>
        /// 接收
        /// </summary>
        /// <param name="pDataFrame">Rmtp数据帧</param>
        void Receive(RmtpDataFrame pDataFrame);
    }
}
