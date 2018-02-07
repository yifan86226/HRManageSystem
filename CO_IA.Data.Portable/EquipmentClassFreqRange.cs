using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentClassFreqRange : AT_BC.Data.CheckableData<string>
    {
        private string name;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        private string equipmentClassID;

        public string EquipmentClassID
        {
            get
            {
                return this.equipmentClassID;
            }
            set
            {
                if (value != this.equipmentClassID)
                {
                    this.equipmentClassID = value;
                    this.NotifyPropertyChanged("EquipmentClassID");
                }
            }
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
                if (value != this.mHzFreqFrom)
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
                if (value != mHzFreqTo)
                {
                    this.mHzFreqTo = value;
                    this.NotifyPropertyChanged("MHzFreqTo");
                }
            }
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

        private int distanceToActivityPlace;

        public int mDistanceToActivityPlace
        {
            get
            {
                return this.distanceToActivityPlace;
            }
            set
            {
                if (value != this.distanceToActivityPlace)
                {
                    this.distanceToActivityPlace = value;
                    this.NotifyPropertyChanged("mDistanceToActivityPlace");
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
