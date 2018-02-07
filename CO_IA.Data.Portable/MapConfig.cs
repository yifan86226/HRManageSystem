using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MapConfig
    {
        public string SatelliteUrl
        {
            get;
            set;
        }

        public string ElectricUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 几何服务地址
        /// </summary>
        public string GeometryServerUrl
        {
            get;
            set;
        }
        public Range<GeoPoint> DefaultGeoRange
        {
            get;
            set;
        }
    }
}
