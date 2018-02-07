using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class PlaceFreqPlan : EquipmentClassFreqRange
    {
        public string PlaceGuid
        {
            get;
            set;
        }

        public string ActivityGuid
        {
            get;
            set;
        }

        private Range<double> rangeLongitude;
        public Range<double> LongitudeRange
        {
            get
            {
                return this.rangeLongitude;
            }
            set
            {
                if (value != this.rangeLongitude)
                {
                    this.rangeLongitude = value;
                    this.NotifyPropertyChanged("LongitudeRange");
                }
            }
        }

        private Range<double> rangeLatitude;
        public Range<double> LatitudeRange
        {
            get
            {
                return this.rangeLatitude;
            }
            set
            {
                if (value != this.rangeLatitude)
                {
                    this.rangeLatitude = value;
                    this.NotifyPropertyChanged("LatitudeRange");
                }
            }
        }

        public List<GeoPoint> RangePointList
        {
            get;
            set;
        }

        public void CopyFrom(EquipmentClassFreqRange freqRange)
        {
            this.Key = freqRange.Key;
            this.Name = freqRange.Name;
            this.MHzFreqFrom = freqRange.MHzFreqFrom;
            this.MHzFreqTo = freqRange.MHzFreqTo;
            this.EquipmentClassID = freqRange.EquipmentClassID;
            this.mDistanceToActivityPlace = freqRange.mDistanceToActivityPlace;
            this.kHzBand = freqRange.kHzBand;
        }
    }
}
