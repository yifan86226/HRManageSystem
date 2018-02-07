using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.StationManagement
{
    public class AntennaModel
    {
        /// <summary>
        /// 天线类型
        /// </summary>
        public string AntType { set; get; }

        /// <summary>
        /// 极化方式
        /// </summary>
        public string AntPole { set; get; }

        /// <summary>
        /// 天线增益
        /// </summary>
        public string AntGain { set; get; }


        public override bool Equals(object obj)
        {
            AntennaModel p = (AntennaModel) obj;
            string k = AntType + AntPole + AntGain;
            string k1 = p.AntType + p.AntPole + p.AntGain;

            return k.Equals(k1);
        }
    }
}
