using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentCheckQueryCondition : EquipmentQueryCondition
    {
        /// <summary>
        /// 带宽开始
        /// </summary>
        public double? BandLittle { get; set; }

        /// <summary>
        /// 带宽结束
        /// </summary>
        public double? BandGreat { get; set; }

        private CheckStateEnum checkstate = CheckStateEnum.UnCheck;
        /// <summary>
        /// 检测状态
        /// </summary>
        public CheckStateEnum CheckState
        {
            get 
            {
                return checkstate; 
            }
            set
            {
                checkstate = value;
            }
        }

        private SendLicenseEnum sendlicense = SendLicenseEnum.UnSendLicense;
        /// <summary>
        /// 许可证发放状态
        /// </summary>
        public SendLicenseEnum SendLicense
        {
            get
            {
                return sendlicense; 
            }
            set
            {
                sendlicense = value; 
            }
        }
    }
}
