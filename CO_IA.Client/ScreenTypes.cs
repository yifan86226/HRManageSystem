#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：地图绘制标识
 * 日 期 ：2017-06-29
 ***************************************************************#@#***************************************************************/
#endregion
using AT_BC.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client
{
    public enum MapGroupTypes
    {
        //MapGroupTypes.AreaRange_.ToString();
        //MapGroupTypes t = MapGroupTypes.AreaRange_; t.GetDisplayNameFromEnumDisplayNameAttribute().ToString();
        [EnumDisplayName("指挥中心")]
        CommandCenter_ = 0,
        [EnumDisplayName("活动区域")]
        AreaRange_ = 1,
        [EnumDisplayName("已设台站")]
        HaveStationPoint_ = 2,
        [EnumDisplayName("固定监测设备")]
        FixMonitorPoint_ = 4,
        [EnumDisplayName("便携式监测设备")]
        MobileMonitorPoint_ = 5,
        [EnumDisplayName("监测小组")]
        MonitorGroup_ = 6,
        [EnumDisplayName("非监测组")]
        UnMonitorGroup_ = 7,
        [EnumDisplayName("地点信息")]
        SiteTipsPoint_ = 8,
        [EnumDisplayName("GPS点")]
        GPSPoint_ = 9,
        [EnumDisplayName("闪烁点")]
        ShinePoint_ = 10,
        [EnumDisplayName("其它")]
        other_ = 11,
        [EnumDisplayName("地点")]
        Location_ = 12,
        [EnumDisplayName("参保设备")]
        OrgEqu_ = 13,
        [EnumDisplayName("人")]
        Person_ = 14,


    }
    public class GraphicInfo
    {
        /// <summary>
        /// 边框颜色
        /// </summary>
        public string BorderColor;
        /// <summary>
        /// 边框宽度
        /// </summary>
        public double BorderWidth;
    }




}
