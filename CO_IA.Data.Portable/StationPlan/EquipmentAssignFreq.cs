using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentAssignFreq : INotifyPropertyChanged
    {
        public EquipmentAssignFreq()
        {
            Equ = new EquipmentInfo();
            Freq = new AssignFreq();
        }

        private bool ischecked = false;
        public bool IsChecked
        {
            get
            {
                return ischecked;
            }
            set
            {
                ischecked = value;
                NotifyPropertyChanged("IsChecked");
            }
        }

        private EquipmentInfo equ;
        public EquipmentInfo Equ
        {
            get
            { 
                return equ;
            }
            set 
            { 
                equ = value; 
            }
        }

        
        private AssignFreq freq = new AssignFreq();
        public AssignFreq Freq
        {
            get
            {
                return freq;
            }
            set
            {
                freq = value;
                NotifyPropertyChanged("Freq");
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
