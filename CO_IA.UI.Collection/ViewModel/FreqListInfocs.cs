using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.ViewModel
{
    public class FreqListInfocs
    {
        private string signalFreq;

        public string SignalFreq
        {
            get { return signalFreq; }
            set { signalFreq = value; }
        }

        private string bandWidth;

        public string BandWidth
        {
            get { return bandWidth; }
            set { bandWidth = value; }
        }

        private string rangeMedian;

        public string RangeMedian
        {
            get { return rangeMedian; }
            set { rangeMedian = value; }
        }

        private string rangeMax;

        public string RangeMax
        {
            get { return rangeMax; }
            set { rangeMax = value; }
        }

        private string occupyDegree;

        public string OccupyDegree
        {
            get { return occupyDegree; }
            set { occupyDegree = value; }
        }

        private string stationName;

        public string StationName
        {
            get { return stationName; }
            set { stationName = value; }
        }

        private bool isCheck;

        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; }
        }

        private string freqType;

        public string FreqType
        {
            get { return freqType; }
            set { freqType = value; }
        }
    }
}
