using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Collection
{
    /// <summary>
    /// 分析结果
    /// </summary>
    public class AnalysisResult
    {
        private string _id;
        private double _frequency;
        private double _bandWidth;
        private double _analysisBandWidth;
        private int _amplitudeMidValue;
        private int _amplitudeMaxValue;
        private int _occupy;
        private string _stationName;
        private string _stationguid;
        private SignalTypeEnum _freqType;
        private string _measureId;
        private double _startFreq;
        private double _endFreq;
        private string _placeGuid;
        private string _stClassCode;
        private string _freqGuid;

        private bool isCheck;
        private bool isSend;
        private string sendStatusPic;
        private string _equimentName;

        private DateTime _measureStartTime;
        private DateTime _measureEndTime;



        private int _needClear;
        private int _clearResult;

        /// <summary>
        /// 
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 信号频率
        /// </summary>
        public double Frequency
        {
            get { return _frequency; }
            set { _frequency = value; }
        }
        /// <summary>
        /// 带宽
        /// </summary>
        public double BandWidth
        {
            get { return _bandWidth; }
            set { _bandWidth = value; }
        }
        /// <summary>
        /// 幅度中值
        /// </summary>
        public int AmplitudeMidValue
        {
            get { return _amplitudeMidValue; }
            set { _amplitudeMidValue = value; }
        }
        /// <summary>
        /// 幅度最大值
        /// </summary>
        public int AmplitudeMaxValue
        {
            get { return _amplitudeMaxValue; }
            set { _amplitudeMaxValue = value; }
        }
        /// <summary>
        /// 占用度
        /// </summary>
        public int Occupy
        {
            get { return _occupy; }
            set { _occupy = value; }
        }
        /// <summary>
        /// 台站名称
        /// </summary>
        public string StationName
        {
            get { return _stationName; }
            set { _stationName = value; }
        }
        /// <summary>
        /// 台站guid
        /// </summary>
        public string StationGuid
        {
            get { return _stationguid; }
            set { _stationguid = value; }
        }
        /// <summary>
        /// 信号类型
        /// </summary>
        public SignalTypeEnum FreqType
        {
            get { return _freqType; }
            set { _freqType = value; }
        }
        /// <summary>
        /// 测量ID
        /// </summary>
        public string MeasureId
        {
            get { return _measureId; }
            set { _measureId = value; }
        }
        /// <summary>
        /// 起始频率
        /// </summary>
        public double StartFreq
        {
            get { return _startFreq; }
            set { _startFreq = value; }
        }
        /// <summary>
        /// 终止频率
        /// </summary>
        public double EndFreq
        {
            get { return _endFreq; }
            set { _endFreq = value; }
        }

        /// <summary>
        /// 地点GUID
        /// </summary>
        public string PlaceGuid
        {
            get { return _placeGuid; }
            set { _placeGuid = value; }
        }

        /// <summary>
        /// 业务编号
        /// </summary>
        public string StClassCode
        {
            get { return _stClassCode; }
            set { _stClassCode = value; }
        }
        /// <summary>
        /// 频率GUID (发射设备GUID)
        /// </summary>
        public string FreqGuid
        {
            get { return _freqGuid; }
            set { _freqGuid = value; }
        }

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; }
        }

        /// <summary>
        /// 是否发射
        /// </summary>
        public bool IsSend
        {
            get { return isSend; }
            set { isSend = value; }
        }

        /// <summary>
        /// 发射状态图片
        /// </summary>
        public string SendStatusPic
        {
            get { return sendStatusPic; }
            set { sendStatusPic = value; }
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquimentName
        {
            get { return _equimentName; }
            set { _equimentName = value; }
        }

        /// <summary>
        /// 是否需要清理 0:不需要 1:需要清理
        /// </summary>
        public int NeedClear
        {
            get { return _needClear; }
            set { _needClear = value; }
        }

        /// <summary>
        /// 清理结果 0:未作处理 1:清理成功 2:清理失败
        /// </summary>
        public int ClearResult
        {
            get { return _clearResult; }
            set { _clearResult = value; }
        }

        public double AnalysisBandWidth
        {
            get { return _analysisBandWidth; }
            set { _analysisBandWidth = value; }
        }
        /// <summary>
        /// 测量开始时间，用于计算测量时长
        /// </summary>
        public DateTime MeasureStartTime
        {
            get { return _measureStartTime; }
            set { _measureStartTime = value; }
        }
        /// <summary>
        /// 测量结束时间，用于计算测量时长
        /// </summary>
        public DateTime MeasureEndTime
        {
            get { return _measureEndTime; }
            set { _measureEndTime = value; }
        }
    }
}
