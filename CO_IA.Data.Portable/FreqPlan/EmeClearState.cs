using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EmeClearState : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; this.NotifyPropertyChanged("Name"); }
        }
        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; this.NotifyPropertyChanged("Value"); }
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
