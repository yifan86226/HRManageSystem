/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：设备信息类
 * 日  期：2016-08-17
 * ********************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using EMCS.Types;
using AT_BC.Data;

namespace CO_IA.Data
{
    [Obsolete("该结构已经过时,将改为ActivityEquipment")]
    public class ActivityEquipmentInfo : INotifyPropertyChanged
    {
        public ActivityEquipmentInfo()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private bool isChecked;
        private string guid;
        private string orgGuid;
        private string activity_guid = string.Empty;
        private string place_guid = string.Empty;
        private string name;
        private int equCount;
        private string remark;
        private bool isStation;
        private string stationName;
        private bool isMobile;
        private double? sendFreq;
        private double? receiveFreq;
        private bool isTunAble;
        private double? sendFreqStart;
        private double? sendFreqEnd;
        private double? maxPower;
        private double? band;
        private double? channelBand;
        private double? leakage;
        private EMCS.Types.EMCModulationEnum modulateMode;

        private double? recvFreqStart;
        private double? recvFreqEnd;
        private double? et_equ_sen;
        private int? et_equ_senu;
        private double? adj_chn_rej;
        private double? snr;
        private double? co_chn_pro;
        private DateTime? running_from;
        private DateTime? running_to;
        private int origin = 0;
        private string equiModel;
        private string equNo;
        private double? assignfreq;

        private ActivityORGInfo orgInfo = new ActivityORGInfo();

        private string recvantmodel;
        private double? recvantgain;
        private double? recvantelevation;
        private double? recvantazimuth;
        private EMCS.Types.EMCPolarisationEnum recvantpolar;
        private double? recvantheight;
        private double? recvantfeedlength;
        private double? recvantfeedloss;

        private string sendantmodel;
        private double? sendantgain;
        private double? sendantelevation;
        private double? sendantazimuth;
        private EMCS.Types.EMCPolarisationEnum sendantpolar;
        private double? sendantheight;
        private double? sendantfeedlength;
        private double? sendantfeedloss;


        private string businesscode;
 
        #region  
        /// <summary>
        /// 单位信息
        /// </summary>
        public ActivityORGInfo OrgInfo
        {
            get
            {
                return orgInfo;
            }

            set
            {
                orgInfo = value; NotifyPropertyChanged("OrgInfo");
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
        /// 单位Guid
        /// </summary>
        public string ORGGuid
        {
            get
            {
                return this.orgGuid;
            }
            set
            {
                this.orgGuid = value;
                this.NotifyPropertyChanged("ORGGuid");
            }
        }

        /// <summary>
        /// 活动GUID
        /// </summary>
        public string ActivityGuid
        {
            get
            {
                return activity_guid;
            }

            set
            {
                activity_guid = value; NotifyPropertyChanged("ActivityGuid");
            }
        }

        /// <summary>
        /// 地点ID
        /// </summary>
        public string PlaceGuid
        {
            get
            {
                return place_guid;
            }

            set
            {
                place_guid = value; NotifyPropertyChanged("PlaceGuid");
            }
        }

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

        /// <summary>
        /// 发射频率
        /// </summary>
        public double? SendFreq
        {
            get
            {
                return sendFreq;
            }

            set
            {
                sendFreq = value;
                this.NotifyPropertyChanged("SendFreq");
            }
        }



        /// <summary>
        /// 接收频率
        /// </summary>
        public double? ReceiveFreq
        {
            get
            {
                return receiveFreq;
            }

            set
            {
                receiveFreq = value;
                this.NotifyPropertyChanged("ReceiveFreq");
            }
        }

        /// <summary>
        /// 频率可调
        /// </summary>
        public bool IsTunAble
        {
            get
            {
                return isTunAble;
            }

            set
            {
                isTunAble = value;
                this.NotifyPropertyChanged("IsTunAble");
            }
        }

        public double? SendFreqStart
        {
            get
            {
                return sendFreqStart;
            }

            set
            {
                sendFreqStart = value;
                this.NotifyPropertyChanged("SendFreqStart");
            }
        }

        public double? SendFreqEnd
        {
            get
            {
                return sendFreqEnd;
            }

            set
            {
                sendFreqEnd = value;
                this.NotifyPropertyChanged("SendFreqEnd");
            }
        }

        /// <summary>
        /// 最大功率
        /// </summary>
        public double? MaxPower
        {
            get
            {
                return maxPower;
            }

            set
            {
                maxPower = value;
                this.NotifyPropertyChanged("MaxPower");
            }
        }


        /// <summary>
        /// 带宽
        /// </summary>
        public double? Band
        {
            get
            {
                return band;
            }

            set
            {
                band = value;
                this.NotifyPropertyChanged("Band");
            }
        }

        /// <summary>
        /// 波道间隔
        /// </summary>
        public double? ChannelBand
        {
            get
            {
                return channelBand;
            }

            set
            {
                channelBand = value;
                this.NotifyPropertyChanged("ChannelBand");
            }
        }

        /// <summary>
        /// 邻道泄露
        /// Adjacent
        /// </summary>
        public double? Leakage
        {
            get
            {
                return leakage;
            }

            set
            {
                leakage = value;
                this.NotifyPropertyChanged("Leakage");
            }
        }

        /// <summary>
        /// 调制方式
        /// </summary>
        public EMCS.Types.EMCModulationEnum ModulateMode
        {
            get
            {
                return modulateMode;
            }

            set
            {
                modulateMode = value;
                this.NotifyPropertyChanged("ModulateMode");
            }
        }

        /// <summary>
        /// 频率开始
        /// </summary>
        public double? RecvFreqStart
        {
            get
            {
                return recvFreqStart;
            }

            set
            {
                this.recvFreqStart = value;
                this.NotifyPropertyChanged("RecvFreqStart");
            }
        }

        /// <summary>
        /// 频率结束
        /// </summary>
        public double? RecvFreqEnd
        {
            get
            {
                return recvFreqEnd;
            }

            set
            {
                recvFreqEnd = value;
                this.NotifyPropertyChanged("RecvFreqEnd");
            }
        }

        /// <summary>
        /// 接收机灵敏度
        /// </summary>
        public double? Sensitivity
        {
            get
            {
                return et_equ_sen;
            }

            set
            {
                et_equ_sen = value;
                this.NotifyPropertyChanged("Sensitivity");
            }
        }

        /// <summary>
        /// 接收机灵敏度单位
        /// </summary>
        public int? SensitivityUnit
        {
            get
            {
                return et_equ_senu;
            }

            set
            {
                et_equ_senu = value;
            }
        }

        /// <summary>
        /// 邻波道抑制比
        /// </summary>
        public double? ADJChannelInh
        {
            get
            {
                return adj_chn_rej;
            }

            set
            {
                adj_chn_rej = value;
            }
        }

        /// <summary>
        /// 信噪比
        /// </summary>
        public double? SignalNoise
        {
            get
            {
                return snr;
            }

            set
            {
                snr = value;
            }
        }

        /// <summary>
        /// 同波道保护比
        /// </summary>
        public double? CoChnPro
        {
            get
            {
                return co_chn_pro;
            }

            set
            {
                co_chn_pro = value;
            }
        }

        private DateTime? runningfrom;
        /// <summary>
        /// 设备运行开始时间
        /// </summary>
        public DateTime? RunningFrom
        {
            get
            {
                return runningfrom;
            }

            set
            {
                runningfrom = value;
                NotifyPropertyChanged("RunningFrom");
            }
        }

        private DateTime? runningto;
        /// <summary>
        /// 设备运行结束时间
        /// </summary>
        public DateTime? RunningTo
        {
             get
            {
                return runningto;
            }

            set
            {
                runningto = value;
                NotifyPropertyChanged("RunningTo");
            }
        }

        /// <summary>
        /// 导入方式
        /// </summary>
        public int Origin
        {
            get
            {
                return origin;
            }

            set
            {
                origin = value; NotifyPropertyChanged("Origin");
            }
        }

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

        /// <summary>
        /// 指配频率
        /// </summary>
        public double? AssignFreq
        {
            get
            {
                return assignfreq;
            }
            set
            {
                assignfreq = value;
                NotifyPropertyChanged("AssignFreq");
            }
        }


        /// <summary>
        /// 天线型号
        /// </summary>
        public string RecvAntModel
        {
            get
            {
                return recvantmodel;
            }

            set
            {
                recvantmodel = value;
            }
        }
        /// <summary>
        /// 天线增益
        /// </summary>
        public double? RecvAntGain
        {
            get
            {
                return recvantgain;
            }

            set
            {
                recvantgain = value;
            }
        }
        /// <summary>
        /// 天线仰角
        /// </summary>
        public double? RecvAntElevation
        {
            get
            {
                return recvantelevation;
            }

            set
            {
                recvantelevation = value;
            }
        }
        /// <summary>
        /// 天线方位角
        /// </summary>
        public double? RecvAntAzimuth
        {
            get
            {
                return recvantazimuth;
            }

            set
            {
                recvantazimuth = value;
            }
        }
        /// <summary>
        /// 天线极化方式
        /// </summary>
        public EMCS.Types.EMCPolarisationEnum RecvAntPolar
        {
            get
            {
                return recvantpolar;
            }

            set
            {
                recvantpolar = value;
            }
        }
        /// <summary>
        /// 天线高度
        /// </summary>
        public double? RecvAntHeight
        {
            get
            {
                return recvantheight;
            }

            set
            {
                recvantheight = value;
            }
        }
        /// <summary>
        /// 馈线长度
        /// </summary>
        public double? RecvAntFeedLength
        {
            get
            {
                return recvantfeedlength;
            }

            set
            {
                recvantfeedlength = value;
            }
        }
        /// <summary>
        /// 馈线损耗
        /// </summary>
        public double? RecvAntFeedLoss
        {
            get
            {
                return recvantfeedloss;
            }

            set
            {
                recvantfeedloss = value;
            }
        }
        /// <summary>
        /// 天线型号
        /// </summary>
        public string SendAntModel
        {
            get
            {
                return sendantmodel;
            }

            set
            {
                sendantmodel = value;
            }
        }
        /// <summary>
        /// 天线增益
        /// </summary>
        public double? SendAntGain
        {
            get
            {
                return sendantgain;
            }

            set
            {
                sendantgain = value;
            }
        }
        /// <summary>
        /// 天线仰角
        /// </summary>
        public double? SendAntElevation
        {
            get
            {
                return sendantelevation;
            }

            set
            {
                sendantelevation = value;
            }
        }
        /// <summary>
        /// 天线方位角
        /// </summary>
        public double? SendAntAzimuth
        {
            get
            {
                return sendantazimuth;
            }

            set
            {
                sendantazimuth = value;
            }
        }
        /// <summary>
        /// 天线极化方式
        /// </summary>
        public EMCS.Types.EMCPolarisationEnum SendAntPolar
        {
            get
            {
                return sendantpolar;
            }

            set
            {
                sendantpolar = value;
            }
        }
        /// <summary>
        /// 天线高度
        /// </summary>
        public double? SendAntHeight
        {
            get
            {
                return sendantheight;
            }

            set
            {
                sendantheight = value;
            }
        }
        /// <summary>
        /// 馈线长度
        /// </summary>
        public double? SendAntFeedLength
        {
            get
            {
                return sendantfeedlength;
            }

            set
            {
                sendantfeedlength = value;
            }
        }
        /// <summary>
        /// 馈线损耗
        /// </summary>
        public double? SendAntFeedLoss
        {
            get
            {
                return sendantfeedloss;
            }

            set
            {
                sendantfeedloss = value;
            }
        }

        public string BusinessCode
        {
            get
            {
                return businesscode;
            }

            set
            {
                businesscode = value;
            }
        }



        #endregion


    }
}