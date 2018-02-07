using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Public
{
    /// <summary>
    /// 地图服务器参数
    /// </summary>
    public class MapServerParamNames
    {
        /// <summary>
        /// 电子地图地址
        /// </summary>
        public const string ElectricUrl = "ElectricUrl";

        /// <summary>
        /// 卫星地图地址
        /// </summary>
        public const string SatelliteUrl = "SatelliteUrl";
        /// <summary>
        /// 几何服务地址
        /// </summary>
        public const string GeometryServerUrl = "GeometryUrl";
        /// <summary>
        /// 地图默认显示区域,格式:(经度,纬度)-(经度,纬度)
        /// </summary>
        public const string DefaultArea = "DefaultArea";
    }
}
