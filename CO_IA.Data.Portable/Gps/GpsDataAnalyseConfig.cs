using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Gps
{
    public class GpsDataAnalyseConfig : IGpsDataValidationParam
    {
        /// <summary>
        /// 位移,当当前点与上一点位移大于该参数指定距离,视为该点有效
        /// </summary>
        private double displacement = 10d;

        /// <summary>
        /// 位移,当与上一点之间距离小于Displacement指定距离大于该距离时,分析距离上一点的偏移角度
        /// </summary>
        private double displacementReferToAzimuth = 5d;

        /// <summary>
        /// 方位角,当当前点相对于上一点方位角大于该参数指定角度,位移参考DisplacementReferToAzimuth进行分析
        /// </summary>
        private double refAzimuth = 45d;

        /// <summary>
        /// 以秒为单位的超时时间,当超过该参数指定时间没有返回数据,认为GPS断线
        /// </summary>
        private int secondsTimeout = 10;

        /// <summary>
        /// 获取或设置位移,当当前点与上一点位移大于该参数指定距离,视为该点有效
        /// </summary>
        public double Displacement
        {
            get
            {
                return this.displacement;
            }
            set
            {
                this.displacement = value;
            }
        }

        /// <summary>
        /// 获取或设置位移,当与上一点之间距离小于Displacement指定距离大于该距离时,分析距离上一点的偏移角度
        /// </summary>
        public double DisplacementReferToAzimuth
        {
            get
            {
                return this.displacementReferToAzimuth;
            }
            set
            {
                this.displacementReferToAzimuth = value;
            }
        }

        /// <summary>
        /// 获取或设置方位角,当当前点相对于上一点方位角大于该参数指定角度,位移参考DisplacementReferToAzimuth进行分析
        /// </summary>
        public double RefAzimuth
        {
            get
            {
                return this.refAzimuth;
            }
            set
            {
                this.refAzimuth = value;
            }
        }

        /// <summary>
        /// 获取或设置以秒为单位的超时时间,当超过该参数指定时间没有返回数据,认为GPS断线
        /// </summary>
        public int SecondsTimeout
        {
            get
            {
                return this.secondsTimeout;
            }
            set
            {
                this.secondsTimeout = value;
            }
        }
    }
}
