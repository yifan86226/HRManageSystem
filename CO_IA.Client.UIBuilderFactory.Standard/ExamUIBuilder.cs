using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client.UIBuilderFactory.Standard
{
    public class ExamUIBuilder : StandardActivityUIBuilder
    {
        internal protected ExamUIBuilder()
        {
            
        }
       
      

        public override bool CanBuildStep(Types.ActivityStep activityStep)
        {
            switch (activityStep)
            {
                case Types.ActivityStep.FreqPlanning:
                case Types.ActivityStep.StationPlanning:
                    return false;
                default:
                    return base.CanBuildStep(activityStep);
            }
        }
    }
}
