using AT_BC.Data;
using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MonitorPlanInfo : CheckableData<string>
    {
        public string ActivityGuid
        {
            get;
            set;
        }

        public string PlaceGuid
        {
            get;
            set;
        }

        private double mHzFreqFrom;

        public double MHzFreqFrom
        {
            get
            {
                return this.mHzFreqFrom;
            }
            set
            {
                if (this.mHzFreqFrom != value)
                {
                    this.mHzFreqFrom = value;
                    this.NotifyPropertyChanged("MHzFreqFrom");
                }
            }
        }

        private double mHzFreqTo;

        public double MHzFreqTo
        {
            get
            {
                return this.mHzFreqTo;
            }
            set
            {
                if (this.mHzFreqTo != value)
                {
                    this.mHzFreqTo = value;
                    this.NotifyPropertyChanged("MHzFreqTo");
                }
            }
        }

        public string Creator
        {
            get;
            set;
        }

        public DateTime CreateTime
        {
            get;
            set;
        }

        public DataLoggingMode LoggingMode
        {
            get;
            set;
        }

        private string comments;
        public string Comments
        {
            get
            {
                return this.comments;
            }
            set
            {
                if (value != this.comments)
                {
                    this.comments = value;
                    this.NotifyPropertyChanged("Comments");
                }
            }
        }

        private double _kHzBand;

        public double kHzBand
        {
            get
            {
                return this._kHzBand;
            }
            set
            {
                if (this._kHzBand != value)
                {
                    this._kHzBand = value;
                    this.NotifyPropertyChanged("kHzBand");
                }
            }
        }
    }
}
