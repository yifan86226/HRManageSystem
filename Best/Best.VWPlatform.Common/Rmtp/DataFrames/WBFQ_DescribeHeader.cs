using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// 宽频测量描述头
    /// </summary>
    public class WbfqDescribeHeader : RmtpDataFrame, INotifyPropertyChanged
    {
        public WbfqDescribeHeader()
        {
            Header.DataType = RmtpDataTypes.数据描述头;
        }
        /// <summary>
        /// 主数据类型，等于0为场强，否则为电平
        /// </summary>
        public byte IType
        {
            get;
            set;
        }
        /// <summary>
        /// 主通道标识，固定Channel_ID=100
        /// </summary>
        public byte ChannelId
        {
            get;
            set;
        }
        /// <summary>
        /// 第一个信号频率值，8字节，64位整数，单位Hz
        /// </summary>
        public Double Startfreq
        {
            get;
            set;
        }
        /// <summary>
        /// 信号之间间隔，8字节，64位整数，单位Hz
        /// </summary>
        public Double Step
        {
            get;
            set;
        }
        /// <summary>
        /// 最后一段的最后一个频率值，8字节，64位浮点数，单位Hz
        /// </summary>
        public Double Stopfreq
        {
            get;
            set;
        }
        /// <summary>
        /// 频率点数，4字节，无符号32位整数
        /// </summary>
        public Int32 Points
        {
            get;
            set;
        }

        protected void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// 扩展宽频测量描述头
    /// </summary>
    public class WbfqexDescribeHeader : WbfqDescribeHeader
    {
        /// <summary>
        /// 天线因子数量
        /// </summary>
        public Int32 AntennaFactors
        {
            get;
            set;
        }

        private Int16 _neXkind;
        /// <summary>
        /// 2字节，EX 数据种类数
        /// </summary>
        public Int16 NeXkind
        {
            get
            {
                return _neXkind;
            }
            set
            {
                DataTypeList = null;
                DataTypeList = new List<DeviceData>(value);
                _neXkind = value;
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
        /// 转换为扩展宽频测量描述头
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <param name="pParameter"></param>
        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            IType = pDataFrame.Data[0];
            ChannelId = pDataFrame.Data[1];
            Startfreq = BitConverter.ToDouble(pDataFrame.Data, 2);
            Step = BitConverter.ToDouble(pDataFrame.Data, 10);
            Stopfreq = BitConverter.ToDouble(pDataFrame.Data, 18);
            Points = BitConverter.ToInt32(pDataFrame.Data, 26);
            
            AntennaFactors = BitConverter.ToInt32(pDataFrame.Data, 30);
            
            NeXkind = BitConverter.ToInt16(pDataFrame.Data, 34);
            //int index = 36;
            //for (Int16 i = 0; i < NeXkind; i++)
            //{
            //    DeviceData dd = DeviceData.AnalysisDataClass(pDataFrame, ref index);
            //    if (dd != null)
            //    {
            //        DataTypeList.Add(dd);
            //    }
            //}
        }
    }
}
