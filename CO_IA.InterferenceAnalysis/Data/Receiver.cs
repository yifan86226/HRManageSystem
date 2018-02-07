#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：接收机数据结构
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
    public class Receiver : IComparable<Receiver>
    {
        /// <summary>
        /// 接收机频率
        /// </summary>
        private ComparableFreq freqValue;

        /// <summary>
        /// 接收机参数
        /// </summary>
        private ReceiverParams receiverParams;

        /// <summary>
        /// 接收机地理坐标
        /// </summary>
        private EMCGeographyCoordinate coordinate;

        /// <summary>
        /// 所属设备编号
        /// </summary>
        private string equipID;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="equipID">设备编号</param>
        /// <param name="freqValue">接收机频率</param>
        /// <param name="receiverParams">接收机参数</param>
        /// <param name="coordinate">接收机坐标</param>
        public Receiver(string equipID, ComparableFreq freqValue, ReceiverParams receiverParams, EMCGeographyCoordinate coordinate)
        {
            this.equipID = equipID;
            this.receiverParams = receiverParams;
            this.freqValue = freqValue;
            this.coordinate = coordinate;
        }

        /// <summary>
        /// 获取接收机所属设备编号
        /// </summary>
        public string EquipID
        {
            get
            {
                return this.equipID;
            }
        }
        /// <summary>
        /// 获取以MHz为单位的接收机接收频率
        /// </summary>
        public double Freq
        {
            get
            {
                return this.freqValue.Freq;
            }
        }

        /// <summary>
        /// 获取以dBi为单位的接收机增益
        /// </summary>
        public double AntGaindBi
        {
            get
            {
                return this.receiverParams.Antenna.Gain_dBi;
            }
        }

        /// <summary>
        /// 获取馈线损耗
        /// </summary>
        public double FeedLoss
        {
            get
            {
                return this.receiverParams.Antenna.FeedLoss.Value;
            }
        }

        /// <summary>
        /// 判别参数指定功率信号是否为有效信号
        /// </summary>
        /// <param name="powerIn">接收机输入功率</param>
        /// <returns>判定结果:true,参数指定功率信号为有效信号;false,参数指定功率信号为无效信号</returns>
        public bool IsValidatingSignal(double powerIn)
        {
            return this.receiverParams.SensitivitydBm - powerIn < this.receiverParams.CoChannelProtectedRatio;
        }

        /// <summary>
        /// 获取接收机地理坐标
        /// </summary>
        public EMCGeographyCoordinate Coordinate
        {
            get
            {
                return this.coordinate;
            }
        }

        /// <summary>
        /// 判定参数指定频率是否为接收机有效接收频率
        /// </summary>
        /// <param name="freq">输入频率</param>
        /// <returns>判定结果:true,参数指定频率在接收机接收频率范围内,是有效频率;false,参数指定频率位在接收机接收频率范围内,不是有效频率</returns>
        public bool ReceivableFreq(EMCFreqValue freq)
        {
            double freqMHz = freq.MHzValue;
            return freqMHz >= this.receiverParams.TuningRangeStart.MHzValue && freqMHz <= this.receiverParams.TuningRangeEnd.MHzValue;
        }

        /// <summary>
        /// 判定参数指定频率是否可由接收机接收
        /// </summary>
        /// <param name="freq">以MHz为单位的频率值</param>
        /// <returns>判定结果:true:参数指定频率可以进入接收机;false,参数指定频率无法进入接收机</returns>
        public bool ReceivableFreq(double freq)
        {
            return freq >= this.receiverParams.TuningRangeStart.MHzValue && freq <= this.receiverParams.TuningRangeEnd.MHzValue;
        }

        /// <summary>
        /// 判定参数指定频率是否和当前接收机接收频率匹配
        /// </summary>
        /// <param name="freq">单位为MHz的频率值</param>
        /// <returns>判定结果:true,参数指定频率和当前接收期望频率匹配;false,参数指定频率和当前接收机期望频率不匹配</returns>
        public bool IsValidatingFreq(double freq)
        {
            double halfBand = this.receiverParams.IFBand.MHzValue / 2;
            return freq >= this.Freq - halfBand && freq <= this.Freq + halfBand;
        }

        /// <summary>
        /// 获取接收机参数
        /// </summary>
        internal ReceiverParams ReceiverParams
        {
            get
            {
                return this.receiverParams;
            }
        }

        /// <summary>
        /// 获取可比较频率,该接收机关键字,外部程序通过该频率能够获取使用该接收机的对象
        /// </summary>
        public ComparableFreq ComparableFreq
        {
            get
            {
                return this.freqValue;
            }
        }

        /// <summary>
        /// 接收机可接收频率起始
        /// </summary>
        public double TuningFreqFrom
        {
            get
            {
                return this.receiverParams.TuningRangeStart.MHzValue;
            }
        }

        /// <summary>
        /// 接收机可接受频率终止
        /// </summary>
        public double TuningRangeTo
        {
            get
            {
                return this.receiverParams.TuningRangeEnd.MHzValue;
            }
        }

        #region IComparable<Receiver> 成员

        /// <summary>
        /// 比较当前对象和同一类型的另一对象。
        /// </summary>
        /// <param name="other">与此对象进行比较的对象。</param>
        /// <returns>一个值，指示要比较的对象的相对顺序。返回值的含义如下：值含义小于零此对象小于 other 参数。零此对象等于 other。大于零此对象大于 other。</returns>
        public int CompareTo(Receiver other)
        {
            return this.freqValue.CompareTo(other.freqValue);
        }

        #endregion
    }
}
