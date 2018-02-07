using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ExecutorCompleteState : AT_BC.Data.NotifyPropertyChangedObject
    {
        private string executor;

        public string Executor
        {
            get
            {
                return this.executor;
            }
            set
            {
                this.executor = value;
                this.NotifyPropertyChanged("Executor");
            }
        }

        private bool executed;

        public bool Executed
        {
            get
            {
                return this.executed;
            }
            set
            {
                this.executed = value;
                this.NotifyPropertyChanged("Executed");
            }
        }
    }
}
