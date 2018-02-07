using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MobileStationInfo //: MonitorStation
    {
        private VehicleInfo _vehicle =new VehicleInfo();
        /// <summary>
        /// 车辆信息
        /// </summary>
        public VehicleInfo Vehicle 
        { 
            get
            {
                return _vehicle;
            }
            set
            {
                _vehicle = value;
            }
        }
        public string Name { get; set; }
        public string PlateNumber { get; set; }

    }
}
