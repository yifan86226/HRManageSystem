using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Map
{
    public class Navigator
    {
        public string Time { set; get; }

        public double Pos_Long { set; get; }

        public double Pos_Lat { set; get; }

        /// <summary>
        /// 所在区域
        /// </summary>
        public string LocArea { set; get; }

        public float YAW { set; get; }

        public float ROLL { set; get; }

        public float PITCH { set; get; }
    }
}
