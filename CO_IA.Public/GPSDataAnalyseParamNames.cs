using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Public
{
    public class GPSDataAnalyseParamNames
    {
        /// <summary>
        /// 位移,当当前点与上一点位移大于该参数指定距离,视为该点有效
        /// </summary>
        public const string Displacement = "Displacement";

        /// <summary>
        /// 位移,当与上一点之间距离小于Displacement指定距离大于该距离时,分析距离上一点的偏移角度
        /// </summary>
        public const string DisplacementReferToAzimuth = "DisplacementReferToAzimuth";

        /// <summary>
        /// 方位角,当当前点相对于上一点方位角大于该参数指定角度,位移参考DisplacementReferToAzimuth进行分析
        /// </summary>
        public const string RefAzimuth = "RefAzimuth";

        /// <summary>
        /// 以秒为单位的超时时间,当超过该参数指定时间没有返回数据,认为GPS断线
        /// </summary>
        public const string SecondsTimeout = "SecondsTimeout";
    }
}
