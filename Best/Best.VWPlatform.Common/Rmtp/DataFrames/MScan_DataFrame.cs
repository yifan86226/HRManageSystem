using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 离散扫描数据帧
    /// </summary>
    public class MScan_DataFrame : RmtpDataFrame
    {
        /// <summary>
        /// 描述个数
        /// </summary>
        public int ScanCount { get; set; }

        /// <summary>
        /// ITU测量结果种类数
        /// </summary>
        public short ItuResultTypeCount { get; set; }

        /// <summary>
        /// ITU测量结果数
        /// </summary>
        public int ItuResultCount { get; set; }

        /// <summary>
        /// 扫描的频率序号、数据值
        /// </summary>
        public Dictionary<int, short> ScanValues { get; set; }
        public List<short> Values { get; set; }

        /// <summary>
        /// 频率序号 4字节
        /// item1 mscan数据种类序号、item2 mscan数据
        /// </summary>
        public Dictionary<long, List<Tuple<short, double>>> FreqValues { get; set; }

        public MScan_DataFrame()
        {
            Header.DataType = RmtpDataTypes.业务数据;
            ScanValues = new Dictionary<int, short>();
            Values = new List<short>();
            FreqValues = new Dictionary<long, List<Tuple<short, double>>>();
        }

        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            var index = 0;
            ScanCount = BitConverter.ToInt32(pDataFrame.Data, index);
            index += sizeof(int);

            ItuResultTypeCount = BitConverter.ToInt16(pDataFrame.Data, index);
            index += sizeof(short);

            ItuResultCount = BitConverter.ToInt32(pDataFrame.Data, index);
            index += sizeof(int);

            Values.Clear();
            ScanValues.Clear();
            for (var i = 0; i < ScanCount; i++)
            {
                if (pDataFrame.Data.Length <= index + 4) break;
                var key = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                if (pDataFrame.Data.Length < index + 2) break;
                var value = BitConverter.ToInt16(pDataFrame.Data, index);
                index += sizeof(short);

                Values.Add(value);
                ScanValues[key] = (short)(value / 100);
            }

            for (var i = 0; i < ItuResultCount; i++)
            {
                var freqNo = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                var lst = new List<Tuple<short, double>>();
                for (var j = 0; j < ItuResultTypeCount; j++)
                {
                    var typeNo = BitConverter.ToInt16(pDataFrame.Data, index);
                    index += sizeof(short);

                    var dataValue = BitConverter.ToSingle(pDataFrame.Data, index);
                    index += sizeof(float);
                    lst.Add(new Tuple<short, double>(typeNo, dataValue));
                }
                FreqValues[freqNo] = lst;
            }
        }
    }
}
