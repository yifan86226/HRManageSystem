namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 移动方向
    /// </summary>
    public enum MoveOrientation
    {
        Left,
        Top,
        Right,
        Bottom
    }
    /// <summary>
    /// 地图操作
    /// </summary>
    public enum MapOperate
    {
        /// <summary>
        /// 测距操作
        /// </summary>
        MeasureDistance
    }
    /// <summary>
    /// 地图绘制模式
    /// </summary>
    public enum MapDrawMode
    {
        None = 0,
        Point = 1,
        Polyline = 2,
        Polygon = 3,
        Rectangle = 4,
        Freehand = 5,
        Arrow = 6,
        Triangle = 7,
        Ellipse = 8,
        Circle = 9
    }
    /// <summary>
    /// 地图单位
    /// </summary>
    public enum MapUnits
    {
        /// <summary>
        /// 米
        /// </summary>
        Meters,
        /// <summary>
        /// 经纬度
        /// </summary>
        Degrees
    }
}
