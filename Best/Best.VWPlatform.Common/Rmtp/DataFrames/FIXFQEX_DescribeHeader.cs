using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    public class FIXFQEX_DescribeHeader : RmtpDataFrame
    {
        private Int16 _nFIXFQkind;
        /// <summary>
        /// 1字节，主数据类型，等于0，表示包含场强数据，否则表示无场强数据，有电平数据
        /// </summary>
        public byte iType
        {
            get;
            private set;
        }
        /// <summary>
        /// 8字节，64位整数，测量中心频率，单位Hz
        /// </summary>
        public Int64 freq
        {
            get;
            private set;
        }
        /// <summary>
        /// 2字节，FIXFQ数据种类数
        /// </summary>
        public Int16 nFIXFQkind
        {
            get
            {
                return _nFIXFQkind;
            }
            private set
            {
                _nFIXFQkind = value;
                this.DataTypeList = new List<DeviceData>(_nFIXFQkind);
            }
        }
        public List<DeviceData> DataTypeList
        {
            get;
            private set;
        }

        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            iType = pDataFrame.Data[0];
            freq = BitConverter.ToInt64(pDataFrame.Data, 1);
            nFIXFQkind = BitConverter.ToInt16(pDataFrame.Data, 9);

            int index = 11;
            for (int i = 0; i < nFIXFQkind; i++)
            {
                DeviceData dd = DeviceData.AnalysisDataClass(pDataFrame, ref index);
                if (dd == null)
                    continue;
                DataTypeList.Add(dd);
            }
        }
    }
}
