#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：发射机参数
 * 日  期：2016-09-07
 * ********************************************************************************/
#endregion
using EMCS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.InterferenceAnalysis
{
    public class TransmitterParams
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public TransmitterParams()
        {
            transmitterParamsID = Guid.NewGuid().ToString();
            ant = new Antenna();
            ant.AntennaKind = AntennaKindEnum.Send;
        }
        /// <summary>
        /// 发射机ID
        /// </summary>
        private string transmitterParamsID;

        /// <summary>
        /// 必要带宽
        /// </summary>
        private EMCFreqValue band = new EMCFreqValue(EMCFreqUnitEnum.KHz, 12.5);

        /// <summary>
        /// 调制方式
        /// </summary>
        private EMCModulationEnum mod = EMCModulationEnum.None;

        /// <summary>
        /// 发射功率
        /// </summary>
        private EMCPowerValue power = new EMCPowerValue();

        /// <summary>
        /// 波道间隔
        /// </summary>
        private EMCFreqValue channelSpacing = new EMCFreqValue();

        /// <summary>
        /// 频率可调范围起始
        /// </summary>
        private EMCFreqValue tuningRangeStart = new EMCFreqValue();

        /// <summary>
        /// 频率可调范围终止
        /// </summary>
        private EMCFreqValue tuningRangeEnd = new EMCFreqValue();

        /// <summary>
        /// 邻道泄露比
        /// </summary>
        private double aclRatio = 1;

        /// <summary>
        /// 发射天线信息
        /// </summary>
        private Antenna ant;

        /// <summary>
        /// 频率可调标识
        /// </summary>
        private bool freqAssignable = false;

        /// <summary>
        /// 获取或设置发射机必要带宽
        /// </summary>
        public EMCFreqValue Band
        {
            get
            {
                return this.band;
            }
            set
            {
                this.band = value;
            }
        }

        /// <summary>
        /// 获取或设置发射机调制方式
        /// </summary>
        public EMCModulationEnum Modulation
        {
            get
            {
                return this.mod;
            }
            set
            {
                this.mod = value;
            }
        }

        /// <summary>
        /// 获取或设置发射功率
        /// </summary>
        public EMCPowerValue Power
        {
            get
            {
                return this.power;
            }
            set
            {
                this.power = value;
            }
        }

        /// <summary>
        /// 获取或设置频率可调范围起始
        /// </summary>
        public EMCFreqValue TuningRangeStart
        {
            get
            {
                return this.tuningRangeStart;
            }
            set
            {
                this.tuningRangeStart = value;
            }
        }

        /// <summary>
        /// 获取或设置频率可调范围终止
        /// </summary>
        public EMCFreqValue TuningRangeEnd
        {
            get
            {
                return this.tuningRangeEnd;
            }
            set
            {
                this.tuningRangeEnd = value;
            }
        }

        /// <summary>
        /// 获取或设置频道间隔
        /// </summary>
        public EMCFreqValue ChannelSpacing
        {
            get
            {
                return this.channelSpacing;
            }
            set
            {
                this.channelSpacing = value;
            }
        }

        /// <summary>
        /// 获取或设置发射机参数ID
        /// </summary>
        public string TransmitterParamsID
        {
            get
            {
                return this.transmitterParamsID;
            }
            set
            {
                transmitterParamsID = value;
            }
        }
        /// <summary>
        /// 获取或设置邻道泄露比
        /// </summary>
        public double AclRatio
        {
            get
            {
                return this.aclRatio;
            }
            set
            {
                this.aclRatio = value;
            }
        }

        /// <summary>
        /// 获取或设置发射天线信息
        /// </summary>
        public Antenna Antenna
        {
            get
            {
                return this.ant;
            }
            set
            {
                this.ant = value;
            }
        }

        /// <summary>
        /// 获取或设置频率可调标识
        /// </summary>
        public bool FreqAssignable
        {
            get
            {
                return this.freqAssignable;
            }
            set
            {
                this.freqAssignable = value;
            }
        }

        /// <summary>
        /// 从参数指定的参考信息覆盖加载发射机参数设置
        /// </summary>
        /// <param name="src">参考发射机信息</param>
        public void CopyFrom(TransmitterParams src)
        {
            this.tuningRangeStart.CopyFrom(src.tuningRangeStart);
            this.tuningRangeEnd.CopyFrom(src.tuningRangeEnd);
            this.band.CopyFrom(src.band);
            this.channelSpacing.CopyFrom(src.channelSpacing);
            this.mod = src.mod;
            this.power.CopyFrom(src.power);
            this.ant.CopyFrom(src.ant);
        }

        /// <summary>
        /// 从参数指定的发射机设置选择加载信息,如果当前发射机对应信息以初始化就忽略,否则,从参数加载.
        /// </summary>
        /// <param name="src">参考发射机信息</param>
        public void FillFrom(TransmitterParams src)
        {
            if (!this.tuningRangeStart.DataInitialized)
            {
                this.tuningRangeStart.CopyFrom(src.tuningRangeStart);
            }
            if (!this.tuningRangeEnd.DataInitialized)
            {
                this.tuningRangeEnd.CopyFrom(src.tuningRangeEnd);
            }
            if (this.band.DataInitialized)
            {
                this.band.CopyFrom(src.band);
            }
            if (this.channelSpacing.DataInitialized)
            {
                this.channelSpacing.CopyFrom(src.channelSpacing);
            }
            if (this.mod == EMCModulationEnum.None)
            {
                this.mod = src.mod;
            }
            if (!this.power.DataInitialized)
            {
                this.power.CopyFrom(src.power);
            }
            this.ant.FillFrom(src.ant);
        }
    }
}
