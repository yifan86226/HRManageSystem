using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class FreqPlanningProgress
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

        public FreqPlanningStepState[] StepStates
        {
            get;
            set;
        }
    }
}
