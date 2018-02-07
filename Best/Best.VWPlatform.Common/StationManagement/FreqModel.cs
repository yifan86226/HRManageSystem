using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.StationManagement
{
    public class FreqModel
    {
        public string FreqEmission { set; get; }

        public string FreqReceive { set; get; }

        /// <summary>
        /// 调制特性
        /// </summary>
        public string FreqMod { set; get; }


        public override bool Equals(object obj)
        {
            FreqModel fm = (FreqModel) obj;
            string k = FreqEmission + FreqMod + FreqReceive;
            string k1 = fm.FreqEmission + fm.FreqMod + fm.FreqReceive;
            return k.Equals(k1);
        }
    }
}
