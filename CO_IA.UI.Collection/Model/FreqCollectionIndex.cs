using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.Model
{
    public class FreqCollectionIndex
    {
        private double _startFreq;

        public double StartFreq
        {
            get { return _startFreq; }
            set { _startFreq = value; }
        }

        private double _endFreq;

        public double EndFreq
        {
            get { return _endFreq; }
            set { _endFreq = value; }
        }

        private double _freqStep;

        public double FreqStep
        {
            get { return _freqStep; }
            set { _freqStep = value; }
        }

        private string _fileAddr;

        public string FileAddr
        {
            get { return _fileAddr; }
            set { _fileAddr = value; }
        }

        private string _measureID;

        public string MeasureID
        {
            get { return _measureID; }
            set { _measureID = value; }
        }

        private string _displayMem;

        public string DisplayMem
        {
            get { return _displayMem; }
            set { _displayMem = value; }
        }

        private string _currentActivityGuid;

        public string CurrentActivityGuid
        {
            get { return _currentActivityGuid; }
            set { _currentActivityGuid = value; }
        }

        private string _currentActivityName;

        public string CurrentActivityName
        {
            get { return _currentActivityName; }
            set { _currentActivityName = value; }
        }

        private string _currentActivityPlaceGuid;

        public string CurrentActivityPlaceGuid
        {
            get { return _currentActivityPlaceGuid; }
            set { _currentActivityPlaceGuid = value; }
        }

        private string _currentActivityPlaceName;

        public string CurrentActivityPlaceName
        {
            get { return _currentActivityPlaceName; }
            set { _currentActivityPlaceName = value; }
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
