using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 频段扫描描述头
    /// </summary>
    public class Fscan_DescribeHeader : RmtpDataFrame
    {
        public Fscan_DescribeHeader()
        {
            Header.DataType = RmtpDataTypes.数据描述头;
            Freqs = new List<long>();
            ScanTypes = new List<Tuple<string, long, string, int>>();
        }

        /// <summary>
        /// 数据值类型，0为场强，否则为电平
        /// </summary>
        public byte IType
        {
            get;
            set;
        }

        /// <summary>
        /// 保留为频段扫描数段数，目前固定为1
        /// </summary>
        public short FscanSegment { get; set; }

        /// <summary>
        /// 8字节，64位整数，单位Hz
        /// </summary>
        public Int64 StartFrequency { get; set; }

        /// <summary>
        /// 8字节，64位整数，单位Hz
        /// </summary>
        public Int64 EndFrequency { get; set; }

        /// <summary>
        /// 8字节， 64位整数，单位Hz
        /// </summary>
        public Int64 Step { get; set; }

        /// <summary>
        /// 频段扫描的扫描点数，4字节，32位整数
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// 频率点 单位kHz
        /// </summary>
        public List<Int64> Freqs { get; set; }

        /// <summary>
        /// 天线因子
        /// </summary>
        public Int64 Antennafactors { get; set; }

        /// <summary>
        /// fscan扫描各类数
        /// </summary>
        public short FscanTypeCount { get; set; }

        /// <summary>
        /// 固定16字节字符，第1个 MSCAN数据种类英文名称
        /// 该数据种类的缺省最小值，4字节
        /// 单位, 固定10字节字符
        /// 序号
        /// </summary>
        public List<Tuple<string, long, string, int>> ScanTypes { get; set; }

        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            var index = 0;
            IType = pDataFrame.Data[index];
            index += 1;

            FscanSegment = BitConverter.ToInt16(pDataFrame.Data, index);
            index += sizeof(short);

            StartFrequency = BitConverter.ToInt64(pDataFrame.Data, index);
            index += sizeof(Int64);

            EndFrequency = BitConverter.ToInt64(pDataFrame.Data, index);
            index += sizeof(Int64);

            Step = BitConverter.ToInt64(pDataFrame.Data, index);
            index += sizeof(Int64);

            Points = BitConverter.ToInt32(pDataFrame.Data, index);
            index += sizeof(int);

            Antennafactors = BitConverter.ToInt32(pDataFrame.Data, index);
            index += sizeof(int);

            FscanTypeCount = BitConverter.ToInt16(pDataFrame.Data, index);
            index += sizeof(short);

            for (var i = 0; ; i++)
            {
                var freq = (StartFrequency + i * Step) / 1000; // 转成kHz
                if (freq > (EndFrequency / 1000))
                    break;
                if (!Freqs.Contains(freq))
                    Freqs.Add(freq);
            }

            for (var i = 0; i < FscanTypeCount; i++)
            {
                var name = Encoding.UTF8.GetString(pDataFrame.Data, index, 16).TrimEnd('$').TrimEnd('\0');
                index += 16;

                var value = BitConverter.ToSingle(pDataFrame.Data, index);
                index += sizeof(float);

                var unit = Encoding.UTF8.GetString(pDataFrame.Data, index, 10).TrimEnd('$').TrimEnd('\0');
                index += 10;

                ScanTypes.Add(new Tuple<string, long, string, int>(name, (long)value, unit, i));
            }
        }
    }
}
