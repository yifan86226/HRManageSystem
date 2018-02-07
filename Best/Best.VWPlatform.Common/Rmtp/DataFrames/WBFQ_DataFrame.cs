using Best.VWPlatform.Common.Rmtp.MeasureHandler;
using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 宽频测量数据帧
    /// </summary>
    /// <remarks>
    /// 1、频率序号对应描述头返回的频率顺序，编号从0开始。频率序号与频率数量相加得到的值不大于描述头中的信号点数
    /// 2、场强单位为dBμV/m电平值单位为dBμV，用整数表示，固定后两位表示小数
    /// </remarks>
    public class WbfqDataFrame : RmtpDataFrame
    {
        /// <summary>
        /// 主通道标识，固定Channel_ID=100
        /// </summary>
        public byte ChannelId
        {
            get;
            set;
        }
        /// <summary>
        /// 数据值，实际场强或电平*100
        /// </summary>
        public Int16[] 数据值
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// 扩展宽频测量数据帧
    /// </summary>
    public class WbfqexDataFrame : WbfqDataFrame
    {
        private Int32 _xhCount;
        private UInt32 _freqCount;
        private List<SignalStatisticsItem> _signalStatisticsItems;

        /// <summary>
        /// 测量数据类型，2字节整数 (0：实时，1：最大，2：最小，3：平均，4：电磁背景)
        /// </summary>
        public MeasureDataType 频谱数据类型
        {
            get;
            internal set;
        }
        /// <summary>
        /// 4字节整数（信号数量为零时表示无信号数据）
        /// </summary>
        public Int32 信号数量
        {
            get
            {
                return _xhCount;
            }
            internal set
            {
                _xhCount = value;
                if (_xhCount == 0)
                    return;
                _signalStatisticsItems = new List<SignalStatisticsItem>(_xhCount);
            }
        }
        public double 起始频率 { get; internal set; }

        /// <summary>
        /// Step
        /// </summary>
        public double 步长 { get; internal set; }

        public UInt32 频谱点数
        {
            get { return _freqCount; }
            internal set
            {
                _freqCount = value;
                数据值 = new Int16[_freqCount];
            }
        }

        public UInt32 频率序号 { get; internal set; }
        ///// <summary>
        ///// Int64 - 频率Freq，Single[] - 数据种类对应的值
        ///// </summary>
        public IEnumerable<SignalStatisticsItem> SignalStatisticsItems
        {
            get { return _signalStatisticsItems; }
        }

        /// <summary>
        /// 转换为扩展宽频测量结果
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <param name="pParameter"></param>
        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            if (pParameter == null)
                return;
            double milliseconds;
            long lastTime;
            try
            {
                int index = 0;
                ChannelId = pDataFrame.Data[index];
                频谱数据类型 = (MeasureDataType)BitConverter.ToInt16(pDataFrame.Data, index += 1);
                信号数量 = BitConverter.ToInt32(pDataFrame.Data, index += 2);
                起始频率 = BitConverter.ToDouble(pDataFrame.Data, index += 4);
                步长 = BitConverter.ToDouble(pDataFrame.Data, index += 8);
                频谱点数 = BitConverter.ToUInt32(pDataFrame.Data, index += 8);
                频率序号 = BitConverter.ToUInt32(pDataFrame.Data, index += 4);

                index += 4;
                for (var i = 0; i < 频谱点数; i++)
                {
                    if (index >= pDataFrame.Data.Length)
                    {
                        //数据桢大小有问题
                        return;
                    }
                    数据值[i] = (Int16)(BitConverter.ToInt16(pDataFrame.Data, index) / 100);
                    index += 2;
                }

                for (int i = 0; i < 信号数量; i++)
                {
                    var item = new SignalStatisticsItem();
                    item.SignalId = BitConverter.ToInt32(pDataFrame.Data, index);
                    item.Frequency = BitConverter.ToDouble(pDataFrame.Data, index += 4);
                    item.CurrentFrequency = BitConverter.ToDouble(pDataFrame.Data, index += 8);
                    item.CurrentBandwidth = BitConverter.ToDouble(pDataFrame.Data, index += 8);
                    item.Bandwidth = BitConverter.ToDouble(pDataFrame.Data, index += 8);
                    item.InterceptedNumber = BitConverter.ToInt64(pDataFrame.Data, index += 8);
                    item.Duration = BitConverter.ToInt64(pDataFrame.Data, index += 8);
                    lastTime = BitConverter.ToInt64(pDataFrame.Data, index += 8);
                    milliseconds = WMonitorUtile.DateTime1970Milliseconds + lastTime;
                    var ts = TimeSpan.FromMilliseconds(milliseconds);
                    item.LastTime = new DateTime(ts.Ticks);
                    item.FirstTime = item.LastTime;
                    item.OccupancyRate = BitConverter.ToSingle(pDataFrame.Data, index += 8);
                    item.Density = BitConverter.ToSingle(pDataFrame.Data, index += 4);
                    item.CustomValue = BitConverter.ToSingle(pDataFrame.Data, index += 4);
                    item.Level = BitConverter.ToInt16(pDataFrame.Data, index += 4);
                    item.FieldStrength = BitConverter.ToInt16(pDataFrame.Data, index += 2);
                    _signalStatisticsItems.Add(item);
                    index += 2;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("WBFQ_DataFrame.cs {0}",ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 获取WBFQ_DataFrame数据帧中的信号总数
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <returns></returns>
        public static Int32 GetSignalCount(RmtpDataFrame pDataFrame)
        {
            const int index = 3;
            return BitConverter.ToInt32(pDataFrame.Data, index);
        }
        /// <summary>
        /// 获取数据帧中的序号
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <returns></returns>
        public static uint GetSerialNumber(RmtpDataFrame pDataFrame)
        {
            const int index = 27;
            return BitConverter.ToUInt32(pDataFrame.Data, index);
        }
        /// <summary>
        /// 获取频谱数据类型
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <returns></returns>
        public static MeasureDataType GetDataType(RmtpDataFrame pDataFrame)
        {
            return (MeasureDataType)BitConverter.ToInt16(pDataFrame.Data, 1);
        }
    }
}
