using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CO_IA.UI.Collection.DataAnalysis
{
    /// <summary>
    /// 电磁环境分析:信号列表数据源定义
    /// </summary>
    public class ElecEnvFlexGridData : INotifyPropertyChanged
    {
        /// <summary>
        ///  true 信号性质发生变化 false 信号性质未发生变化
        /// </summary>
        public bool ElecEnvPropertyChanged;

        private string _bandImage = "/CO_IA.UI.Collection;component/Images/closelist.png";

        //private FreqDetailElecEnv _freband = new FreqDetailElecEnv();

        private bool _isBandOpen;

        private bool _isServiceOpen;

        private string _monitorId = "-999";

        private string _monitorName = "空";

        private string _recordId = "-999";

        //private ServiceElecEnv _service = new ServiceElecEnv();

        /// <summary>
        /// 展开折叠按钮图片路径-业务
        /// </summary>
        private string _serviceImage = "/CO_IA.UI.Collection;component/Images/closelist.png";

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 行政区代码
        /// </summary>
        public string AreaCode { get; set; }

        //<--频段属性-->
        /// <summary>
        /// 频段编号
        /// </summary>
        public string BandId { get; set; }

        /// <summary>
        /// 展开折叠按钮图片路径-子频段
        /// </summary>
        public string BandImage
        {
            get { return _bandImage; }
        }

        /// <summary>
        /// 是否勾选
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 是否可以勾选
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 终止频率 MHz
        /// </summary>
        public double EndFreq { get; set; }

        /// <summary>
        /// 每个子频段第一条数据
        /// </summary>
        public bool FirstItem { get; set; }

        //public FreqDetailElecEnv FreBand
        //{
        //    get { return _freband; }
        //    set { _freband = value; }
        //}

        /// <summary>
        /// 频段名称
        /// </summary>
        public string FreqName { get; set; }

        /// <summary>
        /// 子频段非法信号比例
        /// </summary>
        public double FrqBandIllLegal { get; set; }

        /// <summary>
        /// 子频段合法信号比例
        /// </summary>
        public double FrqBandLegal { get; set; }

        /// <summary>
        /// 子频段占用度
        /// </summary>
        public double FrqBandOccupy { get; set; }

        /// <summary>
        /// 子频段未知信号比例
        /// </summary>
        public double FrqBandUnknown { get; set; }

        public bool IsBandOpen
        {
            get { return _isBandOpen; }
            set
            {
                _isBandOpen = value;
                _bandImage = _isBandOpen ? "/CO_IA.UI.Collection;component/Images/openlist.png" : "/CO_IA.UI.Collection;component/Images/closelist.png";
                OnPropertyChanged("BandImage");
            }
        }

        public bool IsServiceOpen
        {
            get { return _isServiceOpen; }
            set
            {
                _isServiceOpen = value;
                _serviceImage = _isServiceOpen ? "/CO_IA.UI.Collection;component/Images/openlist.png" : "/CO_IA.UI.Collection;component/Images/closelist.png";
                OnPropertyChanged("ServiceImage");
            }
        }

        /// <summary>
        /// 已加载条数
        /// </summary>
        public double LoadedCount { get; set; }

        /// <summary>
        /// 监测站ID
        /// </summary>
        public string MonitorId
        {
            get { return _monitorId; }
            set { _monitorId = value; }
        }

        /// <summary>
        /// 监测站名称
        /// </summary>
        public string MonitorName
        {
            get { return _monitorName; }
            set { _monitorName = value; }
        }

        /// <summary>
        /// 行数据唯一编号
        /// </summary>
        public string RecordId
        {
            get { return _recordId; }
            set { _recordId = value; }
        }

        /// <summary>
        /// 样本ID
        /// </summary>
        public string SampleId { get; set; }

        //public ServiceElecEnv Service
        //{
        //    get { return _service; }
        //    set { _service = value; }
        //}

        //<--业务属性-->
        /// <summary>
        /// 业务编号
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 业务非法信号比例
        /// </summary>
        public double ServiceIllegalPercentage { get; set; }

        public string ServiceImage
        {
            get { return _serviceImage; }
            set { _serviceImage = value; }
        }

        /// <summary>
        /// 业务类别
        /// </summary>
        public string ServiceKind { get; set; }

        /// <summary>
        /// 业务合法信号比例
        /// </summary>
        public double ServiceLegalPercentage { get; set; }

        /// <summary>
        /// 业务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 业务占用度
        /// </summary>
        public double ServiceOccupy { get; set; }

        /// <summary>
        /// 业务未知信号比例
        /// </summary>
        public double ServiceUnknownPercentage { get; set; }

        /// <summary>
        /// 样本带宽 kHz
        /// </summary>
        public double SignalBandWidth { get; set; }

        /// <summary>
        /// 样本中心频率 MHz
        /// </summary>
        public double SignalCenterFreq { get; set; }

        /// <summary>
        /// 信号幅度最大值
        /// </summary>
        public double SignalMaxAmplitude { get; set; }

        /// <summary>
        /// 信号幅度中值
        /// </summary>
        public double SignalMidAmplitude { get; set; }

        //<--信号属性-->
        /// <summary>
        /// 信号时间占用度
        /// </summary>
        public double SignalOccupy { get; set; }

        /// <summary>
        /// 信号性质
        /// </summary>
        public SPropertyEnum SignalProperty { get; set; }

        /// <summary>
        /// 台站名称
        /// </summary>
        public string SignalStation { get; set; }

        /// <summary>
        /// 起始频率 MHz
        /// </summary>
        public double StartFreq { get; set; }

        /// <summary>
        /// 数据总条数
        /// </summary>
        public double SumCount { get; set; }

        private void OnPropertyChanged(string pPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pPropertyName));
        }
    }
}
