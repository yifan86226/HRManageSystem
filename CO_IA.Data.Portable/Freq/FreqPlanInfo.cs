using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqPlanInfo : INotifyPropertyChanged
    {
        public FreqPlanInfo()
        {

        }

        /// <summary>
        /// 业务类型
        /// </summary>
        public BusinessType Businesstype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Freq_Low
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public double Freq_High
        {
            get;
            set;
        }

        /// <summary>
        /// 频率数量
        /// </summary>
        public int Freq_Count
        {
            get;
            set;
        }

        private List<double> freqs = new List<double>();
        public List<double> Freqs
        {
            get { return freqs; }
            set { freqs = value; }
        }

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
