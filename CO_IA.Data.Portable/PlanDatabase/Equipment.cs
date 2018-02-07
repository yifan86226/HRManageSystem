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
    public class Equipment : CheckableData<string>
    {
        public Equipment()
        {
            FreqRange = new Range<double?>();
            OrgInfo = new IdentifiableData<string>();
        }

        public IdentifiableData<string> OrgInfo
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

        private string seriesNumber;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string SeriesNumber
        {
            get
            {
                return this.seriesNumber;
            }
            set
            {
                this.seriesNumber = value;
                this.NotifyPropertyChanged("SeriesNumber");
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

        private string stationTDI;
        /// <summary>
        /// 已建台站名称
        /// </summary>
        public string StationTDI
        {
            get
            {
                return this.stationTDI;
            }
            set
            {
                this.stationTDI = value;
                this.NotifyPropertyChanged("StationTDI");
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


        private double? sendFreq;

        /// <summary>
        /// 移动设备
        /// true:移动设备
        /// false:固定设备
        /// </summary>
        public double? SendFreq
        {
            get
            {
                return this.sendFreq;
            }
            set
            {
                this.sendFreq = value;
                this.NotifyPropertyChanged("SendFreq");
            }
        }

        /// <summary>
        /// 备用频率
        /// </summary>
        private double? spareFreq;

        /// <summary>
        /// 获取或设置备用频率(MHz为单位)
        /// </summary>
        public double? SpareFreq
        {
            get
            {
                return this.spareFreq;
            }
            set
            {
                this.spareFreq = value;
                this.NotifyPropertyChanged("SpareFreq");
            }
        }

        private double? receiveFreq;

        public double? ReceiveFreq
        {
            get
            {
                return this.receiveFreq;
            }
            set
            {
                this.receiveFreq = value;
                this.NotifyPropertyChanged("ReceiveFreq");
            }
        }

        private bool isTunable;

        /// <summary>
        /// 频率可调
        /// </summary>
        public bool IsTunable
        {
            get
            {
                return this.isTunable;
            }
            set
            {
                this.isTunable = value;
                this.NotifyPropertyChanged("IsTunable");
            }
        }

        private Range<double?> freqRange;

        public Range<double?> FreqRange
        {
            get
            {
                return this.freqRange;
            }
            set
            {
                this.freqRange = value;
                this.NotifyPropertyChanged("FreqRange");
            }
        }

        private double? power;

        public double? Power_W
        {
            get
            {
                return this.power;
            }
            set
            {
                this.power = value;
                this.NotifyPropertyChanged("Power_W");

            }
        }

        private double? band;

        public double? Band_kHz
        {
            get
            {
                return this.band;
            }
            set
            {
                this.band = value;
                this.NotifyPropertyChanged("Band_kHz");
            }
        }


        private EMCS.Types.EMCModulationEnum modulation;

        /// <summary>
        /// 调制方式
        /// </summary>
        public EMCS.Types.EMCModulationEnum Modulation
        {
            get
            {
                return this.modulation;
            }
            set
            {
                this.modulation = value;
                this.NotifyPropertyChanged("Modulation");
            }
        }

        private string equipmentClassID;
        /// <summary>
        /// 设备类别
        /// </summary>
        public string EquipmentClassID
        {
            get
            {
                return this.equipmentClassID;
            }
            set
            {
                this.equipmentClassID = value;
                this.NotifyPropertyChanged("EquipmentClassID");

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

        private string stClassCode;
        /// <summary>
        /// 技术体制
        /// </summary>
        public string StClassCode
        {
            get
            {
                return this.stClassCode;
            }
            set
            {
                this.stClassCode = value;
            }
        }

        private double? longitude;
        public double? Longitude
        {
            get
            {
                return longitude;
            }
            set
            {
                longitude = value;
                this.NotifyPropertyChanged("Longitude");
            }
        }

        private double? latitude;
        public double? Latitude
        {
            get
            {
                return latitude;
            }
            set
            {
                latitude = value;
                this.NotifyPropertyChanged("Latitude");
            }
        }

        private string address;
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }
    }
}
