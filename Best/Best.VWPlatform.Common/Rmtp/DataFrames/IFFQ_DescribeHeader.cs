using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 中频测量描述头
    /// </summary>
    public class IffqDescribeHeader : RmtpDataFrame
    {
        public IffqDescribeHeader()
        {
            Header.DataType = RmtpDataTypes.数据描述头;
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
        /// 测量中心频率，单位Hz
        /// </summary>
        public Int64 Freq
        {
            get;
            set;
        }
        /// <summary>
        /// 测量跨距，单位Hz
        /// </summary>
        public Int64 Span
        {
            get;
            set;
        }
        /// <summary>
        /// 中频带宽，单位Hz
        /// </summary>
        public Int64 Ifbw
        {
            get;
            set;
        }

        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            int index = 0;
            IType = pDataFrame.Data[index];
            Freq = BitConverter.ToInt64(pDataFrame.Data, index += 1);
            Span = BitConverter.ToInt64(pDataFrame.Data, index += 8);
            Ifbw = BitConverter.ToInt64(pDataFrame.Data, index + 8);
        }
    }
    /// <summary>
    /// 扩展中频测量描述头
    /// </summary>
    public class IffqexDescribeHeader : IffqDescribeHeader
    {
        private Int16 _nIfqeXkind;
        public Int32 Antennafactors { get; set; }
        /// <summary>
        /// 2字节，IFQEX数据种类数
        /// </summary>
        public Int16 NIfqeXkind
        {
            get
            {
                return _nIfqeXkind;
            }
            set
            {
                if (DataTypeList == null)
                    DataTypeList = new List<DeviceData>(value);
                _nIfqeXkind = value;
            }
        }
        /// <summary>
        /// 数据种类集合
        /// </summary>
        public List<DeviceData> DataTypeList
        {
            get;
            private set;
        }
        /// <summary>
        /// 转换为扩展中频测量描述头
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <param name="pParameter"></param>
        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            int index = 0;
            IType = pDataFrame.Data[index];
            Freq = BitConverter.ToInt64(pDataFrame.Data, index += 1);
            Span = BitConverter.ToInt64(pDataFrame.Data, index += 8);
            Ifbw = BitConverter.ToInt64(pDataFrame.Data, index += 8);
            Antennafactors = BitConverter.ToInt32(pDataFrame.Data, index += 8);
            NIfqeXkind = BitConverter.ToInt16(pDataFrame.Data, index += 4);
            index += 2;
            for (Int16 i = 0; i < NIfqeXkind; i++)
            {
                DeviceData dd = DeviceData.AnalysisDataClass(pDataFrame, ref index);
                DataTypeList.Add(dd);
            }
        }
    }
}
