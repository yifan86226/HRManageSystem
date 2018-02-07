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
    /// 地图上用鼠标获取图形事件参数
    /// </summary>
    public class MapGeometryTrackedArgs
    {
        /// <summary>
        /// 图形
        /// </summary>
        public MapGeometry MapGeometry
        {
            get;
            set;
        }
    }
}
