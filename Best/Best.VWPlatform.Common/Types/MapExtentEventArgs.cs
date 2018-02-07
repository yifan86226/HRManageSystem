using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 地图范围事件参数
    /// </summary>
    public class MapExtentEventArgs
    {
        public MapExtent NewExtent
        {
            get;
            set;
        }
        public MapExtent OldExtent
        {
            get;
            set;
        }
    }
}
