using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.DataFrames
{
    /// <summary>
    /// TDOA定位结果
    /// </summary>
    public class TdoaDataFrame : RmtpDataFrame
    {
        private readonly List<TdoaData> _tdoaList = new List<TdoaData>();

        /// <summary>
        /// 位置个数
        /// </summary>
        public Int16 Count
        {
            get;
            internal set;
        }

        /// <summary>
        /// TDOA数据
        /// </summary>
        public List<TdoaData> Tdoa
        {
            get
            {
                return _tdoaList;
            }
        }

        internal void Add(TdoaData pData)
        {
            _tdoaList.Add(pData);
        }

        public double Frequency
        {
            get;
            internal set;
        }

        private List<string> _list = new List<string>();
        public List<string> MonitorIdList
        {
            get
            {
                return _list;
            }
        }

        /// <summary>
        /// 转换为TDOA定位结果
        /// </summary>
        /// <param name="pDataFrame"></param>
        /// <param name="pParameter"></param>
        protected override void InternalConverter(RmtpDataFrame pDataFrame, object pParameter)
        {
            var index = 0;

            Frequency = BitConverter.ToInt64(pDataFrame.Data, index);
            index += sizeof(Int64);

            Count = BitConverter.ToInt16(pDataFrame.Data, index);
            index += sizeof(UInt16);

            for (var i = 0; i < Count; i++)
            {
                var tdoa = new TdoaData();

                tdoa.IsValid = pDataFrame.Data[index];
                index += 1;

                tdoa.Sequency = BitConverter.ToUInt16(pDataFrame.Data, index);
                index += sizeof(UInt16);

                tdoa.Long = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.Lat = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.Height = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.ConfidenceRadius = BitConverter.ToDouble(pDataFrame.Data, index);
                index += sizeof(double);

                tdoa.TdoaQuality = BitConverter.ToSingle(pDataFrame.Data, index);
                index += sizeof(float);

                tdoa.LeftTopLong = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.LeftTopLat = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.RightBottomLong = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.RightBottomLat = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.DateType = pDataFrame.Data[index];
                index += sizeof(byte);

                tdoa.FileExtension = Encoding.UTF8.GetString(pDataFrame.Data, index, 5);
                index += 5;

                tdoa.AreaWidth = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.AreaHeight = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                tdoa.ResultLength = BitConverter.ToInt32(pDataFrame.Data, index);
                index += sizeof(int);

                for (var j = 0; j < tdoa.ResultLength; j++)
                {
                    switch (tdoa.DateType)
                    {
                        case 0:
                            tdoa.LocationResults.Add(pDataFrame.Data[index]);
                            index += 1;
                            break;
                        case 1:
                            tdoa.LocationResults.Add(BitConverter.ToInt16(pDataFrame.Data, index));
                            index += sizeof(short);
                            break;
                        case 3:
                            tdoa.LocationResults.Add(BitConverter.ToInt32(pDataFrame.Data, index));
                            index += sizeof(int);
                            break;
                    }
                }

                tdoa.DeviceCount = pDataFrame.Data[index];
                index += sizeof(byte);

                for (var j = 0; j < tdoa.DeviceCount; j++)
                {
                    var dpd = new DeviceSpectrumData();
                    dpd.Sequency = pDataFrame.Data[index];
                    index += sizeof(byte);

                    dpd.IdLength = BitConverter.ToUInt16(pDataFrame.Data, index);
                    index += sizeof(UInt16);

                    dpd.Id = Encoding.UTF8.GetString(pDataFrame.Data, index, dpd.IdLength);
                    index += dpd.IdLength;

                    MonitorIdList.Add(dpd.Id);

                    // 时域数据
                    dpd.TimeDomainDataType = pDataFrame.Data[index];
                    index += sizeof(byte);

                    dpd.TimeDomainDataCount = BitConverter.ToInt32(pDataFrame.Data, index);
                    index += sizeof(int);

                    dpd.XAxisUnit = Encoding.UTF8.GetString(pDataFrame.Data, index, 8);
                    index += 8;

                    dpd.YAxisUnit = Encoding.UTF8.GetString(pDataFrame.Data, index, 8);
                    index += 8;

                    dpd.SampleFrequency = BitConverter.ToDouble(pDataFrame.Data, index);
                    index += sizeof(double);

                    for (var k = 0; k < dpd.TimeDomainDataCount; k++)
                    {
                        if (dpd.TimeDomainDataType == 0)
                        {
                            dpd.TimeDomainDatas.Add(BitConverter.ToInt16(pDataFrame.Data, index));
                            index += sizeof(Int16);
                        }
                        else
                        {
                            dpd.TimeDomainDatas.Add(BitConverter.ToSingle(pDataFrame.Data, index));
                            index += sizeof(float);
                        }
                    }
                    // 频谱数据
                    dpd.SpectrumDataType = pDataFrame.Data[index];
                    index += sizeof(byte);

                    dpd.SpectrumDataCount = BitConverter.ToInt32(pDataFrame.Data, index);
                    index += sizeof(int);

                    for (var k = 0; k < dpd.SpectrumDataCount; k++)
                    {
                        if (dpd.SpectrumDataType == 0)
                        {
                            dpd.SpectrumDatas.Add(BitConverter.ToInt16(pDataFrame.Data, index));
                            index += sizeof(Int16);
                        }
                        else
                        {
                            dpd.SpectrumDatas.Add(BitConverter.ToSingle(pDataFrame.Data, index));
                            index += sizeof(float);
                        }
                    }

                    tdoa.DeviceDatas.Add(dpd);
                }
                // 相关数据
                tdoa.CorelationCount = pDataFrame.Data[index];
                index += sizeof(byte);

                for (var j = 0; j < tdoa.CorelationCount; j++)
                {
                    var csd = new CorelationData();
                    csd.LeftDeviceSequency = pDataFrame.Data[index];
                    index += sizeof(byte);

                    csd.RightDeviceSequency = pDataFrame.Data[index];
                    index += sizeof(byte);

                    csd.CorelationTdoa = BitConverter.ToDouble(pDataFrame.Data, index);
                    index += sizeof(double);

                    csd.CorelationQuality = BitConverter.ToDouble(pDataFrame.Data, index);
                    index += sizeof(double);

                    csd.XAxisUnit = Encoding.UTF8.GetString(pDataFrame.Data, index, 8);
                    index += 8;

                    csd.YAxisUnit = Encoding.UTF8.GetString(pDataFrame.Data, index, 8);
                    index += 8;

                    csd.StartTime = BitConverter.ToDouble(pDataFrame.Data, index);
                    index += sizeof(double);

                    csd.TimeStep = BitConverter.ToDouble(pDataFrame.Data, index);
                    index += sizeof(double);

                    csd.CorelationDataType = pDataFrame.Data[index];
                    index += sizeof(byte);

                    csd.CorelationDataCount = BitConverter.ToInt32(pDataFrame.Data, index);
                    index += sizeof(int);

                    for (var k = 0; k < csd.CorelationDataCount; k++)
                    {
                        if (csd.CorelationDataType == 0)
                        {
                            csd.CorelationDatas.Add(BitConverter.ToInt16(pDataFrame.Data, index));
                            index += sizeof(UInt16);
                        }
                        else
                        {
                            csd.CorelationDatas.Add(BitConverter.ToSingle(pDataFrame.Data, index));
                            index += sizeof(float);
                        }
                    }

                    tdoa.CorelationDatas.Add(csd);
                }

                Add(tdoa);

            }
            if (pDataFrame.Data.Length - index > 0)
                SetPropertyValue(this, System.Text.Encoding.UTF8.GetString(pDataFrame.Data, index, pDataFrame.Data.Length - index));
        }
    }

    public class TdoaData
    {
        private List<int> _locationResults = new List<int>();
        private List<DeviceSpectrumData> _deviceDatas = new List<DeviceSpectrumData>();
        private List<CorelationData> _corelationDatas = new List<CorelationData>();

        /// <summary>
        /// 是否有效 0无效 1有效
        /// </summary>
        public byte IsValid { get; internal set; }

        /// <summary>
        /// 序号
        /// </summary>
        public ushort Sequency
        {
            get;
            internal set;
        }

        /// <summary>
        /// 经度
        /// </summary>
        public int Long
        {
            get;
            internal set;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public int Lat
        {
            get;
            internal set;
        }
        /// <summary>
        /// 海拔高度
        /// </summary>
        public long Height
        {
            get;
            internal set;
        }

        /// <summary>
        /// 置信半径
        /// </summary>
        public double ConfidenceRadius
        {
            get;
            internal set;
        }

        /// <summary>
        /// 定位质量
        /// </summary>
        public float TdoaQuality { get; internal set; }

        /// <summary>
        /// 左上角经度
        /// </summary>
        public long LeftTopLong
        {
            get;
            internal set;
        }

        /// <summary>
        /// 左上角纬度
        /// </summary>
        public long LeftTopLat
        {
            get;
            internal set;
        }

        /// <summary>
        /// 右下角经度
        /// </summary>
        public long RightBottomLong { get; internal set; }

        /// <summary>
        /// 右下角纬度
        /// </summary>
        public long RightBottomLat { get; internal set; }

        /// <summary>
        /// 定位结果数据类型 byte型 0:short  2个字节 1:int  4个字节 3:long 4个字节
        /// </summary>
        public byte DateType { get; internal set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExtension { get; internal set; }

        /// <summary>
        /// 定位结果长度
        /// </summary>
        public int ResultLength { get; internal set; }

        /// <summary>
        /// 定位结果区域宽度
        /// </summary>
        public long AreaWidth { get; internal set; }

        /// <summary>
        /// 定位结果区域高度
        /// </summary>
        public long AreaHeight { get; internal set; }

        /// <summary>
        /// 定位结果 w*h*单位字节数
        /// </summary>
        public List<int> LocationResults
        {
            get { return _locationResults; }
            internal set { _locationResults = value; }
        }

        /// <summary>
        /// 参与定位的设备数量
        /// </summary>
        public byte DeviceCount { get; internal set; }

        /// <summary>
        /// 谱数据
        /// </summary>
        public List<DeviceSpectrumData> DeviceDatas
        {
            get { return _deviceDatas; }
            internal set { _deviceDatas = value; }
        }

        /// <summary>
        /// 相关谱个数
        /// </summary>
        public byte CorelationCount { get; internal set; }

        public List<CorelationData> CorelationDatas
        {
            get { return _corelationDatas; }
            internal set { _corelationDatas = value; }
        }
    }

    public class DeviceSpectrumData
    {
        private List<float> _spectrumDatas = new List<float>();
        private List<float> _timeDomainDatas = new List<float>();

        /// <summary>
        /// 序号
        /// </summary>
        public byte Sequency { get; internal set; }

        /// <summary>
        /// 设备ID长度
        /// </summary>
        public ushort IdLength { get; internal set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// 频谱数据库 byte型 0:short  2个字节   1:int  4个字节
        /// </summary>
        public byte SpectrumDataType { get; internal set; }

        /// <summary>
        /// 频谱数据点数
        /// </summary>
        public long SpectrumDataCount { get; internal set; }

        /// <summary>
        /// X 轴单位
        /// </summary>
        public string XAxisUnit { get; internal set; }

        /// <summary>
        /// Y 轴单位
        /// </summary>
        public string YAxisUnit { get; internal set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public double SampleFrequency { get; internal set; }

        /// <summary>
        /// 频谱数据
        /// </summary>
        public List<float> SpectrumDatas
        {
            get { return _spectrumDatas; }
            internal set { _spectrumDatas = value; }
        }

        /// <summary>
        /// 时域波形数据类型 byte型 0:short  2个字节1:int  4个字节
        /// </summary>
        public byte TimeDomainDataType { get; internal set; }

        /// <summary>
        /// 时域波形数据点数
        /// </summary>
        public long TimeDomainDataCount { get; internal set; }

        /// <summary>
        /// 时域波形数据
        /// </summary>
        public List<float> TimeDomainDatas
        {
            get { return _timeDomainDatas; }
            internal set { _timeDomainDatas = value; }
        }
    }

    public class CorelationData
    {
        private List<float> _corelationDatas = new List<float>();

        /// <summary>
        /// 左部设备序号
        /// </summary>
        public byte LeftDeviceSequency { get; internal set; }

        /// <summary>
        /// 右部设备序号
        /// </summary>
        public byte RightDeviceSequency { get; internal set; }

        /// <summary>
        /// 相关时差
        /// </summary>
        public double CorelationTdoa { get; internal set; }

        /// <summary>
        /// 相关质量
        /// </summary>
        public double CorelationQuality { get; internal set; }

        /// <summary>
        /// 相关谱数据类型    byte型 0:short  2个字节   1:int  4个字节
        /// </summary>
        public byte CorelationDataType { get; internal set; }

        /// <summary>
        /// 相关谱数据点数
        /// </summary>
        public long CorelationDataCount { get; internal set; }

        /// <summary>
        /// X 轴单位
        /// </summary>
        public string XAxisUnit { get; internal set; }

        /// <summary>
        /// Y 轴单位
        /// </summary>
        public string YAxisUnit { get; internal set; }

        /// <summary>
        /// 开始点时间
        /// </summary>
        public double StartTime { get; internal set; }

        /// <summary>
        /// 时间步进
        /// </summary>
        public double TimeStep { get; internal set; }

        /// <summary>
        /// 相关谱数据
        /// </summary>
        public List<float> CorelationDatas
        {
            get { return _corelationDatas; }
            internal set { _corelationDatas = value; }
        }
    }
}
