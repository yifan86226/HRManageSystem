using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Core
{
    public interface ISocketSend
    {
        /// <summary>
        /// 获取向服务端发送的字节数组
        /// </summary>
        /// <returns>字节数组</returns>
        byte[] ToBytes();
    }
}
