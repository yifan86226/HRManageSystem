using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Gps
{
    public interface IGpsDataValidationParam
    {
        /// <summary>
        /// 位移 最大位移
        /// </summary>
        double Displacement
        {
            get;
            set;
        }
        /// <summary>
        /// 位移 第二位移 大于此位移小于最大位移则判断方位角
        /// </summary>
        double DisplacementReferToAzimuth
        {
            get;
            set;
        }
        /// <summary>
        /// 方位角偏移
        /// </summary>
        double RefAzimuth
        {
            get;
            set;
        }
    }
}
