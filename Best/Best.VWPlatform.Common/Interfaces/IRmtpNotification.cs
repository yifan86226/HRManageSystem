using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Interfaces
{
    /// <summary>
    /// Rmtp消息通知接口
    /// </summary>
    public interface IRmtpNotification
    {
        /// <summary>
        /// Rmtp缓存刷新完成通知
        /// </summary>
        void OnCacheRefreshCompleted();
    }
}
