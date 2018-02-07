using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.StationManagement
{
    public class StationDetailsModel
    {
        public StationDetailsModel()
        {
            FreqList = new List<FreqModel>();
            EquipmentList = new List<EquipmentModel>();
            AntennaList = new List<AntennaModel>();
        }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string OrgName { set; get; }

        /// <summary>
        /// 地址
        /// </summary>
        public string OrgAddress { set; get; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string OrgContactPerson { set; get; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string OrgContactPhoneNum { set; get; }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 台站名
        /// </summary>
        public string StaName { set; get; }

        /// <summary>
        /// 编号
        /// </summary>
        public string StaCode { set; get; }

        /// <summary>
        /// 上次年检时间
        /// </summary>
        public string LastYearCheckTime { set; get; }

        /// <summary>
        /// 执照有效期
        /// </summary>
        public string LicenseExpireTime { set; get; }

        /// <summary>
        /// 频率有效期
        /// </summary>
        public string FreqExpireTime { set; get; }

        /// <summary>
        /// 频率批准文号
        /// </summary>
        public string FreqAuthorizeCode { set; get; }

        /// <summary>
        /// 台站地址
        /// </summary>
        public string StaAddress { set; get; }

        /// <summary>
        /// 坐标
        /// </summary>
        public string StaCoordinate { set; get; }

        /// <summary>
        /// 海拔
        /// </summary>
        public string StaHeight { set; get; }

        //////////////////////////////////////////////////////////////////////////////////////////////

        public List<FreqModel> FreqList { private set; get; }

        public List<EquipmentModel> EquipmentList { private set; get; }

        public List<AntennaModel> AntennaList { private set; get; }

        public void AddFreq(FreqModel pFreq)
        {
            if (!FreqList.Contains(pFreq))
            {
                FreqList.Add(pFreq);
            }            
        }

        public void AddEquipment(EquipmentModel pEquipment)
        {
            if (!EquipmentList.Contains(pEquipment))
            {
                EquipmentList.Add(pEquipment);
            }
        }

        public void AddAntenna(AntennaModel pAntenna)
        {
            if (!AntennaList.Contains(pAntenna))
            {
                AntennaList.Add(pAntenna);
            }
        }
    }
}
