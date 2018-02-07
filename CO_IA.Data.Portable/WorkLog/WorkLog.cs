using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class WorkLog : CheckableData<string>
    {
        public WorkLog()
        {
            if (string.IsNullOrEmpty(this.Key))
            {
                this.Key = System.Guid.NewGuid().ToString();
            }
            if (StuffList == null)
            {
                StuffList = new List<WorkLogStuff>();
            }

            WorkDateFrom = DateTime.Today.Date;
            WorkDateTo = DateTime.Today.Date.AddDays(1).AddSeconds(-1);
            SubmitTime = DateTime.Today.Date;
        }

        public string ActivityGuid
        {
            get;
            set;
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

        private string workType;

        public string WorkType
        {
            get
            {
                return this.workType;
            }
            set
            {
                this.workType = value;
                this.NotifyPropertyChanged("WorkType");
            }
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

        private string submitter;

        public string Submitter
        {
            get
            {
                return this.submitter;
            }
            set
            {
                this.submitter = value;
                this.NotifyPropertyChanged("Submitter");
            }
        }

        private DateTime submitTime;

        public DateTime SubmitTime
        {
            get
            {
                return this.submitTime;
            }
            set
            {
                this.submitTime = value;
                this.NotifyPropertyChanged("SubmitTime");
            }
        }

        private List<WorkLogStuff> stuffList;

        public List<WorkLogStuff> StuffList
        {
            get
            {
                return this.stuffList;
            }
            set
            {
                this.stuffList = value;
                this.NotifyPropertyChanged("StuffList");
            }
        }
    }
}
