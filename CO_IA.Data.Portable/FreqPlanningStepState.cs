using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqPlanningStepState : System.ComponentModel.INotifyPropertyChanged
    {
        public FreqPlanningStep Step
        {
            get;
            set;
        }

        private bool isCompleted;
        public bool IsCompleted
        {
            get
            {
                return this.isCompleted;
            }
            set
            {
                this.isCompleted = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs("IsCompleted"));
                }
            }
        }

        public int Order
        {
            get;
            set;
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
