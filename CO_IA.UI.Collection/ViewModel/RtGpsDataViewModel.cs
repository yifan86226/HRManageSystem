using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Collection.ViewModel
{
    public class RtGpsDataViewModel : ViewModelBase
    {

        private double _Latitude = 0.0;
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude
        {
            get
            {
                return _Latitude;
            }
            set
            {
                Set(() => Latitude, ref _Latitude, value);
            }
        }



        private double _Longitude = 0.0;
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude
        {
            get
            {
                return _Longitude;
            }
            set
            {
                Set(() => Longitude, ref _Longitude, value);
            }
        }


        private double _Altitude = 0.0;
        /// <summary>
        /// 
        /// </summary>
        public double Altitude
        {
            get
            {
                return _Altitude;
            }
            set
            {
                Set(() => Altitude, ref _Altitude, value);
            }
        }





        private double _Speed = 0.0;
        /// <summary>
        /// 
        /// </summary>
        public double Speed
        {
            get
            {
                return _Speed;
            }
            set
            {
                Set(() => Speed, ref _Speed, value);
            }
        }





        private int _SatelliteCount = 0;
        /// <summary>
        /// 
        /// </summary>
        public int SatelliteCount
        {
            get
            {
                return _SatelliteCount;
            }
            set
            {
                Set(() => SatelliteCount, ref _SatelliteCount, value);
            }
        }
    }
}
