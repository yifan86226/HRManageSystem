using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CO_IA.UI.Collection.Chart
{
    public class MinuteQuoteViewModel : INotifyPropertyChanged
    {
        private double lastPx = double.NaN;
        public double LastPx
        {
            get { return this.lastPx; }
            set { if (this.lastPx != value) { this.lastPx = value; this.OnPropertyChanged("LastPx"); } }
        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
