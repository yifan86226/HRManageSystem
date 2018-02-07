using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 设备信息
    /// </summary>
    [Obsolete("请使用Equipment")]
    public class EquipmentInfo : INotifyPropertyChanged
    {
        public EquipmentInfo()
        {
            if (string.IsNullOrEmpty(GUID))
            {
                GUID = System.Guid.NewGuid().ToString();
            }
        }

        private bool isChecked;
        /// <summary>
        ///  选择
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }

        private string guid;
        public string GUID
        {
            get
            {
                return this.guid;
            }
            set
            {
                this.guid = value;
                this.NotifyPropertyChanged("GUID");
            }
        }

        /// <summary>
        /// 单位信息
        /// </summary>
        public IdentifiableData<string> ORG
        {
            get;
            set;
        }

        private string name;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        private string equiModel;
        /// <summary>
        /// 设备型号
        /// </summary>
        public string EquModel
        {
            get
            {
                return this.equiModel;
            }
            set
            {
                this.equiModel = value;
                this.NotifyPropertyChanged("EquModel");
            }
        }

        private string equNo;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string EquNo
        {
            get
            {
                return this.equNo;
            }
            set
            {
                this.equNo = value;
                this.NotifyPropertyChanged("EquNo");
            }
        }

        private string businesscode;

        public string BusinessCode
        {
            get { return businesscode; }
            set
            {
                businesscode = value;
                this.NotifyPropertyChanged("BusinessCode");
            }
        }

        private int equCount;
        /// <summary>
        /// 设备数量
        /// </summary>
        public int EQUCount
        {
            get
            {
                return this.equCount;
            }
            set
            {
                this.equCount = value;
                this.NotifyPropertyChanged("EQUCount");
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
                return this.remark;
            }
            set
            {
                this.remark = value;
                this.NotifyPropertyChanged("Remark");
            }
        }


        private bool isStation;
        /// <summary>
        /// 是否是已建站
        /// </summary>
        public bool IsStation
        {
            get
            {
                return this.isStation;
            }
            set
            {
                this.isStation = value;
                this.NotifyPropertyChanged("IsStation");
            }
        }

        private string stationName;
        /// <summary>
        /// 已建台站名称
        /// </summary>
        public string StationName
        {
            get
            {
                return this.stationName;
            }
            set
            {
                this.stationName = value;
                this.NotifyPropertyChanged("StationName");
            }
        }

        private bool isMobile;
        /// <summary>
        /// 移动设备
        /// true:移动设备
        /// false:固定设备
        /// </summary>
        public bool IsMobile
        {
            get
            {
                return this.isMobile;
            }
            set
            {
                this.isMobile = value;
                this.NotifyPropertyChanged("IsMobile");
            }
        }

        private string stclasscode;
        /// <summary>
        /// 技术体制
        /// </summary>
        public string StClassCode
        {
            get
            {
                return this.stclasscode;
            }
            set
            {
                this.stclasscode = value;
            }
        }

        SendParameter sendpara = new SendParameter();

        /// 发射参数
        /// </summary>
        public SendParameter SendPara
        {
            get 
            { 
                return sendpara;
            }
            set
            {
                sendpara = value;
                this.NotifyPropertyChanged("SendPara");
            }
        }

        ReceiveParameter recivepara = new ReceiveParameter();
        /// <summary>
        /// 接收参数
        /// </summary>
        public ReceiveParameter RecivePara
        {
            get 
            { 
                return recivepara;
            }
            set
            { 
                recivepara = value;
                this.NotifyPropertyChanged("RecivePara");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
