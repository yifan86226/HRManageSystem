using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqPlanSegment : INotifyPropertyChanged
    {
        private bool _isSelected;
        public FreqPlanSegment()
        {
            FreqValue = new Range<double>();
        }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }
        /// <summary>
        /// id
        /// </summary>
        public string FreqId { get; set; }
        /// <summary>
        /// 规划名称
        /// </summary>
        public string FreqPlanName { get; set; }
        /// <summary>
        /// 业务编号
        /// </summary>
        public string ClassCode { get; set; }
        /// <summary>
        /// 频率
        /// </summary>
        public Range<double> FreqValue { get; set; }
        /// <summary>
        /// 频率带宽
        /// </summary>
        public string FreqBand{ get; set; }
        /// <summary>
        /// 信道数量
        /// </summary>
        public string BandCount { get; set; }
        /// <summary>
        /// 起始频点
        /// </summary>
        public string FirstFreqPoint { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
