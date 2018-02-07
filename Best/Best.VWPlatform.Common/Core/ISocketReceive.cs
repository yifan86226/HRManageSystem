using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Core
{
    /// <summary>
    /// 处理Socket接收到的数据接口
    /// </summary>
    public interface ISocketReceive
    {
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="pReceiveData">数据</param>
        void Receive(byte[] pReceiveData);
    }
}
