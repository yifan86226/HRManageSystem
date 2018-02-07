#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：接收机参数
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
    public class ReceiverParams
    {
        /// <summary>
        /// 频率可调标识 true 可调 false 不可调
        /// </summary>
        private bool freqAssignable = false;

        /// <summary>
        /// 接手机频率范围起始
        /// </summary>
        private EMCFreqValue tuningRangeStart = new EMCFreqValue();

        /// <summary>
        /// 接收机频率范围终止
        /// </summary>
        private EMCFreqValue tuningRangeEnd = new EMCFreqValue();

        /// <summary>
        /// 中频带宽
        /// </summary>
        private EMCFreqValue iFBand = new EMCFreqValue();

        /// <summary>
        /// 接收机参数编号
        /// </summary>
        private string receiverParamsID;

        /// <summary>
        /// 接收机灵敏度
        /// </summary>
        private double sensitivity = -100;

        /// <summary>
        /// 接收机灵敏度单位
        /// </summary>
        private SensitivityUnitEnum sensitivityUnit = SensitivityUnitEnum.dBm;

        /// <summary>
        /// 信噪比
        /// </summary>
        private double snRatio = 0;

        /// <summary>
        /// 邻波道抑制比
        /// </summary>
        private double adjacentChannelRejection = 0;

        /// <summary>
        /// 同道保护比
        /// </summary>
        private double coChannelProtectedRatio = 0;

        /// <summary>
        /// 接收天线
        /// </summary>
        private Antenna ant;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ReceiverParams()
        {
            receiverParamsID = Guid.NewGuid().ToString();
            ant = new Antenna();
            ant.AntennaKind = AntennaKindEnum.Receive;
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
        /// 获取或设置频率可调标识 true 可调 false 不可调
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
        /// 获取或设置中频带宽
        /// </summary>
        public EMCFreqValue IFBand
        {
            get
            {
                return this.iFBand;
            }
            set
            {
                this.iFBand = value;
            }
        }

        /// <summary>
        /// 获取或设置接收机灵敏度
        /// </summary>
        public double Sensitivity
        {
            get
            {
                return this.sensitivity;
            }
            set
            {
                this.sensitivity = value;
            }
        }

        /// <summary>
        /// 获取以dBm为单位的接收机灵敏度
        /// </summary>
        public double SensitivitydBm
        {
            get
            {
                if (SensitivityUnitEnum.dBuV == this.sensitivityUnit)
                {
                    if (this.sensitivity > 0)
                        return 20 * Math.Log10(this.sensitivity) - 113;
                    else
                        throw new Exception("接收机灵敏度单位为:dBuV时,值应该大于0!,当前值:" + sensitivity.ToString());
                }
                else
                    return this.sensitivity;
            }
        }

        /// <summary>
        /// 获取或设置信噪比
        /// </summary>
        public double SnRatio
        {
            get
            {
                return this.snRatio;
            }
            set
            {
                this.snRatio = value;
            }
        }

        /// <summary>
        /// 获取或设置邻波道抑制比
        /// </summary>
        public double AdjacentChannelRejection
        {
            get
            {
                return this.adjacentChannelRejection;
            }
            set
            {
                this.adjacentChannelRejection = value;
            }
        }

        /// <summary>
        /// 获取或设置同道保护比
        /// </summary>
        public double CoChannelProtectedRatio
        {
            get
            {
                return this.coChannelProtectedRatio;
            }
            set
            {
                this.coChannelProtectedRatio = value;
            }
        }

        /// <summary>
        /// 获取或设置接收天线信息
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
        /// 获取或设置接收机灵敏度单位
        /// </summary>
        public SensitivityUnitEnum SensitivityUnit
        {
            get
            {
                return this.sensitivityUnit;
            }
            set
            {
                this.sensitivityUnit = value;
            }
        }
        /// <summary>
        /// 获取或设置接收机参数标识
        /// </summary>
        public string ReceiverParamsID
        {
            get
            {
                return this.receiverParamsID;
            }
            set
            {
                this.receiverParamsID = value;
            }
        }

        /// <summary>
        /// 从参数指定的接收机信息加载数据,覆盖当前数据
        /// </summary>
        /// <param name="src">用于参考的接收机参数</param>
        public void CopyFrom(ReceiverParams src)
        {
            this.tuningRangeStart.CopyFrom(src.tuningRangeStart);
            this.tuningRangeEnd.CopyFrom(src.tuningRangeEnd);
            this.iFBand.CopyFrom(src.iFBand);
            this.sensitivityUnit = src.sensitivityUnit;
            this.sensitivity = src.sensitivity;
            this.adjacentChannelRejection = src.adjacentChannelRejection;
            this.snRatio = src.snRatio;
            this.coChannelProtectedRatio = src.coChannelProtectedRatio;
            this.ant.CopyFrom(src.ant);
        }

        /// <summary>
        /// 从参数指定的接收机信息加载数据,保留已存在数据
        /// </summary>
        /// <param name="src">用于参考的接收机参数</param>
        public void FillFrom(ReceiverParams src)
        {
            if (!this.tuningRangeStart.DataInitialized)
            {
                this.tuningRangeStart.CopyFrom(src.tuningRangeStart);
            }
            if (!this.tuningRangeEnd.DataInitialized)
            {
                this.tuningRangeEnd.CopyFrom(src.tuningRangeEnd);
            }
            if (!this.iFBand.DataInitialized)
            {
                this.iFBand.CopyFrom(src.iFBand);
            }
            if (this.sensitivityUnit == SensitivityUnitEnum.None)
            {
                this.sensitivityUnit = src.sensitivityUnit;
                this.sensitivity = src.sensitivity;
            }
            if (this.adjacentChannelRejection == 0)
            {
                this.adjacentChannelRejection = src.adjacentChannelRejection;
            }
            if (this.snRatio == 0)
            {
                this.snRatio = src.snRatio;
            }
            if (this.coChannelProtectedRatio == 0)
            {
                this.coChannelProtectedRatio = src.coChannelProtectedRatio;
            }
            this.ant.FillFrom(src.ant);
        }
    }

    /// <summary>
    /// 接收机灵敏度单位枚举
    /// </summary>
    [Serializable]
    public enum SensitivityUnitEnum
    {
        /// <summary>
        /// 空值
        /// </summary>
        None = 0,
        /// <summary>
        /// dBm
        /// </summary>
        dBm = 1,
        /// <summary>
        /// dBuV
        /// </summary>
        dBuV = 2
    }
}
