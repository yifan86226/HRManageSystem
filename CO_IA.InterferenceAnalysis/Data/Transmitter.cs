#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：发射机数据结构
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
    public class Transmitter : IComparable<Transmitter>
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        private string equipID;

        /// <summary>
        /// 发射频率值
        /// </summary>
        private ComparableFreq freqValue;

        /// <summary>
        /// 发射机参数
        /// </summary>
        private TransmitterParams transmitterParams;

        /// <summary>
        /// 发射机所在位置坐标
        /// </summary>
        private EMCGeographyCoordinate coordinate;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipID">所属设备编号</param>
        /// <param name="freq">发射频率</param>
        /// <param name="transmitter">发射机参数</param>
        /// <param name="coordinate">地理坐标</param>
        public Transmitter(string equipID, ComparableFreq freq, TransmitterParams transmitter, EMCGeographyCoordinate coordinate)
        {
            this.equipID = equipID;
            this.freqValue = freq;
            this.transmitterParams = transmitter;
            this.coordinate = coordinate;
        }

        /// <summary>
        /// 获取发射机所属设备编号
        /// </summary>
        public string EquipID
        {
            get
            {
                return this.equipID;
            }
        }

        /// <summary>
        /// MHz为单位的频率数值
        /// </summary>
        public double Freq
        {
            get
            {
                return this.freqValue.Freq;
            }
        }

        /// <summary>
        /// 获取发射机天线增益
        /// </summary>
        public double AntGain
        {
            get
            {
                return this.transmitterParams.Antenna.Gain_dBi;
            }
        }

        /// <summary>
        /// 获取发射机功率(单位:W)
        /// </summary>
        public double TransmitPowerW
        {
            get
            {
                return this.transmitterParams.Power.WValue;
            }
        }

        /// <summary>
        /// 获取发射机馈线损耗(单位:dBi)
        /// </summary>
        public double FeedLoss
        {
            get
            {
                return this.transmitterParams.Antenna.FeedLoss.Value;
            }
        }

        /// <summary>
        /// 获取有效辐射功率(dBw)
        /// </summary>
        public double Erp
        {
            get
            {
                return this.transmitterParams.Power.DBwValue + this.AntGain - this.FeedLoss;
            }
        }

        /// <summary>
        /// 获取发射机地理坐标
        /// </summary>
        public EMCGeographyCoordinate Coordinate
        {
            get
            {
                return this.coordinate;
            }
        }

        ///// <summary>
        ///// 该发射机到参数发射机的垂直防止隔离衰减
        ///// </summary>
        ///// <param name="disturbedTransmitter">被干扰发射机</param>
        ///// <returns></returns>
        //public double GetVerticalAc(Transmitter disturbedTransmitter)
        //{
        //    double diff = Math.Abs(this.transmitterParams.Antenna.AntHeight.m_Value - disturbedTransmitter.transmitterParams.Antenna.AntHeight.m_Value);
        //    if (diff < 0.0001)
        //    {
        //        diff = 0.0001;
        //    }
        //    return 37.5 * Math.Log10(diff) + 40.3 * Math.Log10(this.Freq) - 63.87;
        //}

        //public double GetHorizontalAC(Transmitter disturbedTransmitter,EMCLengthValue distance)
        //{
        //    double diff = 0.1;
        //    if (distance!=null && distance.m_Value>0.1)
        //    {
        //        diff = distance.m_Value;
        //    }
        //    return 20 * Math.Log10(diff) + 21 * Math.Log10(this.Freq) - 23.9;
        //}

        /// <summary>
        /// 获取发射机到目标天线的隔离衰减
        /// </summary>
        /// <param name="destAnt">天线位置</param>
        /// <param name="distance">到目标天线距离</param>
        /// <returns>发射机到目标天线的隔离衰减</returns>
        public double GetAc(Antenna destAnt, EMCLengthValue distance)
        {
            if (this.transmitterParams.Antenna.Polar == EMCPolarisationEnum.V && destAnt.Polar == EMCPolarisationEnum.V)
            {
                double diff = Math.Abs(this.transmitterParams.Antenna.AntHeight.MValue - destAnt.AntHeight.MValue);
                if (diff < 0.0001)
                {
                    diff = 0.0001;
                }
                return 37.5 * Math.Log10(diff) + 40.3 * Math.Log10(this.Freq) - 63.87;
            }
            else
            {
                double diff = 0.1;
                if (distance != null && distance.MValue > 0.1)
                {
                    diff = distance.MValue;
                }
                return 20 * Math.Log10(diff) + 21 * Math.Log10(this.Freq) - 23.9;
            }
        }

        /// <summary>
        /// 获取参数指定频率是否能够由该发射机发出
        /// </summary>
        /// <param name="freq">单位为MHz的频率值</param>
        /// <returns>判定结果:true,参数指定频率可以由该发射机发出;false,参数指定频率不能由该发射机发出</returns>
        public bool TransmitableFreq(double freq)
        {
            return this.transmitterParams.TuningRangeStart.MHzValue <= freq && this.transmitterParams.TuningRangeEnd.MHzValue >= freq;
        }

        /// <summary>
        /// 发射机的发射频率
        /// </summary>
        public ComparableFreq ComparableFreq
        {
            get
            {
                return this.freqValue;
            }
        }

        /// <summary>
        /// 发射机可调频率起始(单位:MHz)
        /// </summary>
        public double TuningFreqFrom
        {
            get
            {
                return this.transmitterParams.TuningRangeStart.MHzValue;
            }
        }

        /// <summary>
        /// 发射机可调频率终止(单位:MHz)
        /// </summary>
        public double TuningRangeTo
        {
            get
            {
                return this.transmitterParams.TuningRangeEnd.MHzValue;
            }
        }

        /// <summary>
        /// 获取当前发射机到目标天线的隔离衰减
        /// </summary>
        /// <param name="antenna">目标天线</param>
        /// <param name="mDistance">距离目标天线距离</param>
        /// <returns>隔离衰减</returns>
        public double Get‎IsolationAttenuationToAntenna(Antenna antenna, double mDistance)
        {
            double attenuation = 0;
            if (this.Antenna.Polar == EMCPolarisationEnum.V && antenna.Polar == EMCPolarisationEnum.V)
            {
                //按照垂直计算AC
                attenuation = 37.5 * Math.Log10(Math.Abs(antenna.AntHeight.MValue - this.transmitterParams.Antenna.AntHeight.MValue)) + 40.3 * Math.Log10(this.Freq) - 63.87;
            }
            else
            {
                //按照水平计算AC
                attenuation = 20 * Math.Log10(mDistance) + 21 * Math.Log10(this.Freq) - 23.9;
            }
            return attenuation;
        }

        /// <summary>
        /// 获取发射机天线
        /// </summary>
        public Antenna Antenna
        {
            get
            {
                return this.transmitterParams.Antenna;
            }
        }

        #region IComparable<Transmitter> 成员

        /// <summary>
        /// 比较当前对象和同一类型的另一对象。
        /// </summary>
        /// <param name="other">与此对象进行比较的对象。</param>
        /// <returns>一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此对象小于 other 参数。零此对象等于 other。大于零此对象大于 other。</returns>
        public int CompareTo(Transmitter other)
        {
            return this.freqValue.CompareTo(other.freqValue);
        }

        #endregion
    }

}
