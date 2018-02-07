using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Gps
{
    public class GpsOrbit
    {
        private double _runTime;
        public GpsOrbit()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        public string Guid {get;set;}
        /// <summary>
        /// 活动id
        /// </summary>
        public string ActivityId { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string PlateNumber{get;set;}
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime  RunDate{get;set;}
        /// <summary>
        /// 字符串类型时间
        /// </summary>
        public string StrRunTime{get;set;}
        /// <summary>
        /// 接收到的时间BTC时间，时间格式为hhmmss.sss
        /// </summary>
        public double RunTime
        {
            get { return _runTime; }
            set 
            {
                _runTime = value;
                StrRunTime = TimeConvert(value);
            }
        }
        /// <summary>
        /// 定位状态，A=有效定位，V=无效定位
        /// </summary>
        public string  LocationState{get;set;}
        /// <summary>
        /// 纬度
        /// </summary>
        public double  Latitude{get;set;}
        /// <summary>
        /// 经度
        /// </summary>
        public double  Longitude{get;set;}
        /// <summary>
        /// 纬度半球
        /// </summary>
        public string  LatitudeHemisphere{get;set;}
        /// <summary>
        /// 经度半球
        /// </summary>
        public string  LongitudeHemisphere{get;set;}
        /// <summary>
        /// 高度
        /// </summary>
        public double  Height{get;set;}
        /// <summary>
        /// 速度
        /// </summary>
        public double  Speed{get;set;}
        /// <summary>
        /// 使用的星数
        /// </summary>
        public int UsingStartNum { get; set; }
        private string TimeConvert(double pTime)
        {
            string str = pTime.ToString();
            int n = str.IndexOf('.');
            if (n>0)
                str = str.Substring(0, n);
            if (str.Length >4)
            {
                str = str.Insert(str.Length - 4, ":");
                str = str.Insert(str.Length - 2, ":");
            }
            return str;
        }
    }
}
