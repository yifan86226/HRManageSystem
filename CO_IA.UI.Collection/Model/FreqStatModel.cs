using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.Model
{
    public class FreqStatModel
    {
        private double amplitudeValue;

        public double AmplitudeValue
        {
            get { return amplitudeValue; }
            set { amplitudeValue = value; }
        }

        private int appearCount;

        public int AppearCount
        {
            get { return appearCount; }
            set { appearCount = value; }
        }

        private Dictionary<int, int> dicAmplitudeCount;

        public Dictionary<int, int> DicAmplitudeCount
        {
            get { return dicAmplitudeCount; }
            set { dicAmplitudeCount = value; }
        }
    }
}
