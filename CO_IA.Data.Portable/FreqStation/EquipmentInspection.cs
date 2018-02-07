using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentInspection : CheckableData<string>
    {
        public EquipmentInspection()
        {
            if (Guid == null)
            {
                Guid = System.Guid.NewGuid().ToString();
            }
            ActivityEquipment = new ActivityEquipment();
        }

        public string Guid
        {
            get;
            set;
        }
        private string activityGuid;

        public string ActivityGuid
        {
            get
            {
                return this.activityGuid;
            }
            set
            {
                if (value != this.activityGuid)
                {
                    this.activityGuid = value;
                    this.NotifyPropertyChanged("ActivityGuid");
                }
            }
        }

        private string placeGuid;

        public string PlaceGuid
        {
            get
            {
                return this.placeGuid;
            }
            set
            {
                if (value != this.placeGuid)
                {
                    this.placeGuid = value;
                    this.NotifyPropertyChanged("PlaceGuid");
                }
            }
        }

        private ActivityEquipment activityequipment;

        public ActivityEquipment ActivityEquipment
        {
            get { return activityequipment; }
            set
            {
                activityequipment = value;
            }
        }


        private string applyPerson;
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplyPerson
        {
            get
            {
                return applyPerson;
            }
            set
            {
                applyPerson = value;
                NotifyPropertyChanged("ApplyPerson");
            }
        }

        private string applyPersonNo;
        /// <summary>
        /// 申请人身份证
        /// </summary>
        public string ApplyPersonNo
        {
            get
            {
                return applyPersonNo;
            }
            set
            {
                applyPersonNo = value;
                NotifyPropertyChanged("ApplyPersonNo");
            }
        }

        private string useOrg;
        public string UseORG
        {
            get
            {
                return useOrg;
            }
            set
            {
                useOrg = value;
                NotifyPropertyChanged("UseORG");
            }
        }

        private string applyOrg;
        /// <summary>
        /// 使用单位
        /// </summary>
        public string ApplyORG
        {
            get
            {
                return applyOrg;
            }
            set
            {
                applyOrg = value;
                NotifyPropertyChanged("ApplyORG");
            }
        }

        private string equManufacturer;
        /// <summary>
        /// 
        /// </summary>
        public string EquManufacturer
        {
            get
            {
                return equManufacturer;
            }
            set
            {
                equManufacturer = value;
                NotifyPropertyChanged("EquManufacturer");
            }
        }

        private string freqUENCYNO;
        /// <summary>
        /// 频率指配文件编号
        /// </summary>
        public string FreqUENCYNO
        {
            get
            {
                return freqUENCYNO;
            }
            set
            {
                freqUENCYNO = value;
                NotifyPropertyChanged("FreqUENCYNO");
            }
        }

        private DateTime? runningFrom;

        /// <summary>
        /// 设备运行开始时间
        /// </summary>
        public DateTime? RunningFrom
        {
            get
            {
                return runningFrom;
            }
            set
            {
                runningFrom = value;
                NotifyPropertyChanged("RunningFrom");
            }
        }

        private DateTime? runningTo;

        /// <summary>
        /// 设备运行结束时间
        /// </summary>
        public DateTime? RunningTo
        {
            get
            {
                return runningTo;
            }
            set
            {
                runningTo = value;
                NotifyPropertyChanged("RunningTo");
            }
        }

        /// <summary>
        /// 申请区域
        /// </summary>
        private string applyArea;
        public string ApplyArea
        {
            get
            {
                return applyArea;
            }
            set
            {
                applyArea = value;
                NotifyPropertyChanged("ApplyArea");
            }
        }

        private string remark;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
                NotifyPropertyChanged("Remark");
            }
        }

        private string inspectionDescription;
        /// <summary>
        /// 检测描述
        /// </summary>
        public string InspectionDescription
        {
            get
            {
                return inspectionDescription;
            }
            set
            {
                inspectionDescription = value;
                NotifyPropertyChanged("InspectionDescription");
            }
        }


        private CheckStateEnum checkState = CheckStateEnum.UnCheck;
        public CheckStateEnum CheckState
        {
            get
            {
                return checkState;
            }
            set
            {
                checkState = value;
                NotifyPropertyChanged("CheckState");
            }
        }

        private SendLicenseEnum sendlicense = SendLicenseEnum.UnSendLicense;
        public SendLicenseEnum SendLicense
        {
            get
            {
                return sendlicense;
            }
            set
            {
                sendlicense = value;
                NotifyPropertyChanged("SendLicense");
            }
        }
    }
}
