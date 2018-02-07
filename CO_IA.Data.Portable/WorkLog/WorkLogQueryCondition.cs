using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class WorkLogQueryCondition : INotifyPropertyChanged
    {
        public WorkLogQueryCondition()
        {
            int date = DateTime.Today.Day;
            WorkDateFrom = DateTime.Today.Date.AddDays(1 - date);
            WorkDateTo = WorkDateFrom.AddMonths(1).AddMinutes(-1);
        }
        public string ActivityGuid
        {
            get;
            set;
        }

        private string title;

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        private DateTime workdateFrom;

        public DateTime WorkDateFrom
        {
            get
            {
                return this.workdateFrom;
            }
            set
            {
                this.workdateFrom = value;
                this.NotifyPropertyChanged("WorkDateFrom");
            }
        }

        private DateTime workdateTo;

        public DateTime WorkDateTo
        {
            get
            {
                return this.workdateTo;
            }
            set
            {
                this.workdateTo = value;
                this.NotifyPropertyChanged("WorkDateTo");
            }
        }

        private string worker;

        public string Worker
        {
            get
            {
                return this.worker;
            }
            set
            {
                this.worker = value;
                this.NotifyPropertyChanged("Worker");
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
                this.NotifyPropertyChanged("Description");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
