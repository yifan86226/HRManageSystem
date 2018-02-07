using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class AssignFreq : INotifyPropertyChanged
    {
        public double EmitFreq { get; set; }
        public double ReciveFreq { get; set; }

        /// <summary>
        /// 备用频率
        /// </summary>
        public string BackUpFreq { get; set; }

        private double confirmfreq;
        /// <summary>
        /// 确认频率
        /// </summary>
        public double ConfirmFreq
        {
            get
            {
                return confirmfreq;
            }
            set
            {
                confirmfreq = value;
                NotifyPropertyChanged("ConfirmFreq");
            }
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
