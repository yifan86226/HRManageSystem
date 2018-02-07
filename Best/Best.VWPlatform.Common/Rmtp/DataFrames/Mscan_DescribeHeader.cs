using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 离散扫描描述头
    /// </summary>
    public class Mscan_DescribeHeader : RmtpDataFrame
    {
        public Mscan_DescribeHeader()
        {
            Header.DataType = RmtpDataTypes.数据描述头;
            Freqs = new List<double>();
            ScanTypes = new List<Tuple<string, long, string, int>>();
        }

        /// <summary>
        /// 天线因子
        /// </summary>
        public Int64 Antennafactors { get; set; }

        /// <summary>
        /// 数据值类型，0为场强，否则为电平
        /// </summary>
        public byte IType
        {
            get;
            set;
        }

        /// <summary>
        /// MSCAN数据种类数
        /// </summary>
        public short MscanTypeCount { get; set; }

        /// <summary>
        /// 2字节整数，离散扫描数点数
        /// </summary>
        public short MscanPoint { get; set; }

        /// <summary>
        /// 单位MHz
        /// </summary>
        public List<double> Freqs { get; set; }

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

            Antennafactors = BitConverter.ToInt32(pDataFrame.Data, index);
            index += sizeof(int);

            MscanTypeCount = BitConverter.ToInt16(pDataFrame.Data, index);
            index += sizeof(short);

            MscanPoint = BitConverter.ToInt16(pDataFrame.Data, index);
            index += sizeof(short);

            Freqs.Clear();

            for (var i = 0; i < MscanPoint; i++)
            {
                Freqs.Add(BitConverter.ToInt64(pDataFrame.Data, index) / 1000000d); // 转成 MHz
                index += sizeof(Int64);
            }

            for (var i = 0; i < MscanTypeCount; i++)
            {
                var name = Encoding.UTF8.GetString(pDataFrame.Data, index, 16).TrimEnd('$').TrimEnd('\0');
                index += 16;

                var value = BitConverter.ToSingle(pDataFrame.Data, index);
                index += 4;

                var unit = Encoding.UTF8.GetString(pDataFrame.Data, index, 10).TrimEnd('$').TrimEnd('\0');
                index += 10;

                ScanTypes.Add(new Tuple<string, long, string, int>(name, (long)value, unit, i));
            }
        }
    }
}
