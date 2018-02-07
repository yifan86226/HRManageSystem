using Best.VWPlatform.Common.Rmtp.MeasureHandler;
using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 频段扫描数据帧
    /// </summary>
    public class Fscan_DataFrame : RmtpDataFrame
    {
        private Int32 _xhCount;
        private List<SignalStatisticsItem> _signalStatisticsItems;

        public Fscan_DataFrame()
        {
            Header.DataType = RmtpDataTypes.业务数据;
            ScanValues = new List<Tuple<int, short>>();
            FreqValues = new Dictionary<long, List<Tuple<short, float>>>();
            Values = new List<short>();
        }

        /// <summary>
        /// 扫描数据个数
        /// </summary>
        public Int64 ScanCount { get; set; }

        /// <summary>
        /// ITU测量结果种类数
        /// </summary>
        public short ItuResultTypeCount { get; set; }

        /// <summary>
        /// ITU测量结果个数
        /// </summary>
        public long ItuResultCount { get; set; }

        /// <summary>
        /// 序号、值
        /// </summary>
        public List<Tuple<int, short>> ScanValues { get; set; }
        public List<short> Values { get; set; }

        /// <summary>
        /// 频率序号
        /// FSCAN数据种类序号、FSCAN数据
        /// </summary>
        public Dictionary<long, List<Tuple<short, float>>> FreqValues { get; set; }

        /// <summary>
        /// 频谱类型，2字节整数 (0：实时，1：最大，2：最小，3：平均，4：电磁背景)
        /// </summary>
        public MeasureDataType SpectrumType
        {
            get;
            internal set;
        }
        /// <summary>
        /// 4字节整数（信号数量为零时表示无信号数据）
        /// </summary>
        public Int32 SignalCount
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

        /// <summary>
        /// 信号列表信息
        /// </summary>
        public IEnumerable<SignalStatisticsItem> SignalStatisticsItems
        {
            get { return _signalStatisticsItems; }
        }
        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            try
            {
                var index = 0;
                double milliseconds;
                long lastTime;

                ScanCount = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                if (ConfigurationManager.AppSettings["FscanSignalShold"].Equals("1"))
                {
                    SignalCount = BitConverter.ToInt32(pDataFrame.Data, index);
                    index += sizeof(int);
                }

                ItuResultTypeCount = BitConverter.ToInt16(pDataFrame.Data, index);
                index += sizeof(short);

                ItuResultCount = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                if (ConfigurationManager.AppSettings["FscanSignalShold"].Equals("1"))
                {
                    SpectrumType = (MeasureDataType)BitConverter.ToInt16(pDataFrame.Data, index);
                    index += 2;
                }

                if (ScanCount > 0)
                {
                    var sequency = BitConverter.ToInt32(pDataFrame.Data, index);
                    index += sizeof(int);
                    for (var i = 0; i < ScanCount; i++)
                    {
                        var value = BitConverter.ToInt16(pDataFrame.Data, index);
                        index += sizeof(short);

                        ScanValues.Add(new Tuple<int, short>(sequency += 1, (short)(value / 100)));
                        Values.Add((short)(value / 100));
                    }
                }

                for (var i = 0; i < ItuResultCount; i++)
                {
                    var freqNo = BitConverter.ToInt32(pDataFrame.Data, index);
                    index += sizeof(int);
                    var lst = new List<Tuple<short, float>>();
                    for (var j = 0; j < ItuResultTypeCount; j++)
                    {
                        var typeNo = BitConverter.ToInt16(pDataFrame.Data, index);
                        index += sizeof(short);

                        var value = BitConverter.ToSingle(pDataFrame.Data, index);
                        index += sizeof(float);

                        lst.Add(new Tuple<short, float>(typeNo, value));
                    }

                    FreqValues.Add(freqNo, lst);
                }

                if (ConfigurationManager.AppSettings["FscanSignalShold"].Equals("1"))
                {
                    for (int i = 0; i < SignalCount; i++)
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
            }
            catch(Exception ex)
            {
                return;
            }
            
        }
    }
}
