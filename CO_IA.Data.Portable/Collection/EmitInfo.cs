using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AT_BC.Types;

namespace CO_IA.Data.Collection
{
    /// <summary>
    /// 发射信息
    /// </summary>
    public class EmitInfo
    {
        private string _guid;
        private string _stationGuid;
        private string _placeGuid;
        private double _antHeight;
        private double _equPower;
        private double _freqEc;
        private double _freqRc;
        private double _freqBand;
        private double _antEgain;
        private double _feedLose;
        private string _freqMod;
        private PolarEnum _antPole;
        private double _statAt;
        private double _rcvAntHeight;
        private double _rcvAntEgain;
        private double _rcvFeedLose;
        private int _needClear;
        private int _clearResult;
        
        /// <summary>
        /// 
        /// </summary>
        public string Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }
        /// <summary>
        /// 台站GUID
        /// </summary>
        public string StationGuid
        {
            get { return _stationGuid; }
            set { _stationGuid = value; }
        }
        /// <summary>
        /// 活动地点ID
        /// </summary>
        public string PlaceGuid
        {
            get { return _placeGuid; }
            set { _placeGuid = value; }
        }
        //(7,3)
        /// <summary>
        /// 天线高度 单位:m
        /// </summary>
        public double AntHeight
        {
            get { return _antHeight; }
            set { _antHeight = value; }
        }
        /// <summary>
        /// 设备功率 单位:dBW
        /// </summary>
        public double EquPower
        {
            get { return _equPower; }
            set { _equPower = value; }
        }
        /// <summary>
        /// 发射频率 单位：MHz
        /// </summary>
        public double FreqEc
        {
            get { return _freqEc; }
            set { _freqEc = value; }
        }
        /// <summary>
        /// 接收频率 单位：MHz
        /// </summary>
        public double FreqRc
        {
            get { return _freqRc; }
            set { _freqRc = value; }
        }
        /// <summary>
        /// 频率带宽 数据库定义UML图中单位标注的是MHz
        /// </summary>
        public double FreqBand
        {
            get { return _freqBand; }
            set { _freqBand = value; }
        }
        /// <summary>
        /// 发射增益 单位：dBi
        /// </summary>
        public double AntEgain
        {
            get { return _antEgain; }
            set { _antEgain = value; }
        }
        /// <summary>
        /// 馈线损耗 单位：dB
        /// </summary>
        public double FeedLose
        {
            get { return _feedLose; }
            set { _feedLose = value; }
        }
        /// <summary>
        /// 调整方式
        /// </summary>
        public string FreqMod
        {
            get { return _freqMod; }
            set { _freqMod = value; }
        }
        /// <summary>
        /// 极化方式
        /// </summary>
        public PolarEnum AntPole
        {
            get { return _antPole; }
            set { _antPole = value; }
        }
        /// <summary>
        /// 海拔高度 单位：m
        /// </summary>
        public double StatAt
        {
            get { return _statAt; }
            set { _statAt = value; }
        }
        /// <summary>
        /// 接收天线高度 单位：m
        /// </summary>
        public double RcvAntHeight
        {
            get { return _rcvAntHeight; }
            set { _rcvAntHeight = value; }
        }
        /// <summary>
        /// 接收天线增益 单位：dBi
        /// </summary>
        public double RcvAntEgain
        {
            get { return _rcvAntEgain; }
            set { _rcvAntEgain = value; }
        }
        /// <summary>
        /// 接收天线馈线损耗 dB
        /// </summary>
        public double RcvFeedLose
        {
            get { return _rcvFeedLose; }
            set { _rcvFeedLose = value; }
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
    }
}
