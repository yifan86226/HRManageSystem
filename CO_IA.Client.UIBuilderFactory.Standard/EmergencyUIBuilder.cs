using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client.UIBuilderFactory.Standard
{
    /// <summary>
    /// 突发事件
    /// </summary>
    public class EmergencyUIBuilder : ContingencyActivityUIBuilder
    {
        internal protected EmergencyUIBuilder()
        {

        }

 

        public override bool CanBuildStep(ActivityStep activityStep)
        {
            switch (activityStep)
            {
                case ActivityStep.StationPlanning:
                    return false;
                default:
                    return true;
            }
        }
    }
}
