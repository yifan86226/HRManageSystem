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
    /// 地图鼠标事件参数
    /// </summary>
    public class MapMouseEventArgs : EventArgs
    {
        /// <summary>
        /// 地理坐标
        /// </summary>
        public MapPointEx MapPoint { get; set; }
        /// <summary>
        /// 屏幕坐标
        /// </summary>
        public Point ScreenPoint { get; set; }

        public object OriginalSource { get; set; }
    }
}
