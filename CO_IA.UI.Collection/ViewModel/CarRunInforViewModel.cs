using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.Collection.ViewModel
{
    /// <summary>
    /// 汽车实时运动信息
    /// </summary>
    public class CarRunInforViewModel : ViewModelBase
    {

        // "LongitudeValue", "LatitudeValue", "AltitudeValue", "CarSpeed", "SatelliteCount", 
        private double _LongitudeValue = 0.0;
        /// <summary>
        /// 经度
        /// </summary>
        public double LongitudeValue
        {
            get
            {
                return _LongitudeValue;
            }
            set
            {
                Set(() => LongitudeValue, ref _LongitudeValue, value);
            }
        }





        private double _LatitudeValue = 0.0;
        /// <summary>
        /// 纬度
        /// </summary>
        public double LatitudeValue
        {
            get
            {
                return _LatitudeValue;
            }
            set
            {
                Set(() => LatitudeValue, ref _LatitudeValue, value);
            }
        }



        private double _AltitudeValue = 0.0;
        /// <summary>
        /// 海拔高度
        /// </summary>
        public double AltitudeValue
        {
            get
            {
                return _AltitudeValue;
            }
            set
            {
                Set(() => AltitudeValue, ref _AltitudeValue, value);
            }
        }




        private double _CarSpeed = 0.0;
        /// <summary>
        /// 车速
        /// </summary>
        public double CarSpeed
        {
            get
            {
                return _CarSpeed;
            }
            set
            {
                Set(() => CarSpeed, ref _CarSpeed, value);
            }
        }

        private int _SatelliteCount = 0;
        /// <summary>
        /// 星数
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
