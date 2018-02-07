using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.StationManagement
{
    public class EquipmentModel
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string EquModel { set; get; }

        /// <summary>
        /// 核准代码
        /// </summary>
        public string EquAuthCode { set; get; }

        /// <summary>
        /// 功率
        /// </summary>
        public string EquPower { set; get; }

        public override bool Equals(object obj)
        {
            EquipmentModel em = (EquipmentModel) obj;
            string k = EquAuthCode + EquModel + EquPower;
            string k1 = em.EquAuthCode + em.EquModel + em.EquPower;

            return k.Equals(k1);
        }
    }
}
