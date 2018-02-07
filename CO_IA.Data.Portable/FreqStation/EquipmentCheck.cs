using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentCheck : CheckableData<string>
    {
        public EquipmentCheck()
        {
            if (Guid == null)
            {
                Guid = System.Guid.NewGuid().ToString();
            }
        }
        private bool ischecked = false;
        public bool IsChecked
        {
            get
            {
                return ischecked;
            }
            set
            {
                ischecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }
        private DateTime? running_from;
        private DateTime? running_to;
        public string Guid
        {
            get;
            set;
        }

        public string EquGuid
        {
            get;
            set;
        }

        private ActivityEquipmentInfo equipment;
        public ActivityEquipmentInfo Equipment
        {
            get { return equipment; }
            set { equipment = value;
            NotifyPropertyChanged("Equipment");
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

        /// <summary>
        /// 检测频率
        /// </summary>
        public double? CheckFreq { get; set; }

        /// <summary>
        /// 检测带宽
        /// </summary>
        public double? CheckBand { get; set; }

        /// <summary>
        /// 检测功率
        /// </summary>
        public double? CheckPower { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        #region fdw新增字段（业务新增必须加字段）
        /// <summary>
        /// 申请人姓名
        /// </summary>
        //public string CHK_PERSON { get; set; }
        /// <summary>
        /// 申请人身份证
        /// </summary>
        public string CHK_PERSONCARD { get; set; }
        /// <summary>
        /// 申请单位
        /// </summary>
        //public string CHK_APPLYUNIT { get; set; }
        /// <summary>
        /// 使用单位
        /// </summary>
        public string CHK_USEUNIT { get; set; }
        /// <summary>
        /// 频率指配文件编号
        /// </summary>
        public string CHK_FREQUENCYNO { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        //public string CHK_DEVICENAME { get; set; }
        /// <summary>
        /// 设备生产商
        /// </summary>
        public string CHK_EQUIPMENTMAKERS { get; set; }
        /// <summary>
        /// 设备型号
        /// </summary>
        public string CHK_EQUIPMENTNO { get; set; }
        /// <summary>
        /// 设备运行开始时间
        /// </summary>
        public DateTime? RUNNINGFROM
        {
            get
            {
                return running_from;
            }

            set
            {
                running_from = value; NotifyPropertyChanged("RUNNINGFROM");
            }
        }
        /// <summary>
        /// 设备运行结束时间
        /// </summary>
        public DateTime? RUNNINGTO
        {
            get
            {
                return running_to;
            }

            set
            {
                running_to = value; NotifyPropertyChanged("RUNNINGTO");
            }
        }
        /// <summary>
        /// 申请区域
        /// </summary>
        public string CHK_APPLYAREA { get; set; }
        /// <summary>
        /// 设备数量
        /// </summary>
        //public string CHK_EQUIPMENTNUMBER { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string CHK_REMARKS { get; set; }

        #endregion

      
    }
}
