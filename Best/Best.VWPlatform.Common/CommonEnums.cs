using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common
{
    public enum MapInterfaceType
    {
        ArcGis = 0,
        SuperMap
    }

    /// <summary>
    /// 基本控制面板命令
    /// </summary>
    public enum BaseControlPanelCommandType
    {
        全屏,
        测距,
        放大,
        缩小,
        全图,
        上移,
        下移,
        左移,
        右移
    }
}
