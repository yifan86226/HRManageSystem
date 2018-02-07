using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client.UIBuilderFactory.Standard
{
    public abstract class ContingencyActivityUIBuilder : IActivityUIBuilder
    {
        internal protected ContingencyActivityUIBuilder()
        {
            
        }

       
        public virtual UIElement BuildStaffPlanning()
        {
            return new CO_IA.UI.PersonSchedule.PersonPlanModule();
        }
 
 
        public virtual bool CanBuildStep(ActivityStep activityStep)
        {
            switch (activityStep)
            {
                case ActivityStep.FreqPlanning:
                case ActivityStep.StationPlanning:
                    return false;
                default:
                    return true;
            }
        }

        public virtual UIElement BuildSchedule()
        {
            return new CO_IA.UI.PersonSchedule.ScheduleModule();
        }

        public UIElement BuildStatistic()
        {
            return new CO_IA.UI.Statistic.StatisticModule();
        }


        public UIElement BuildStatistic2()
        {
            return new CO_IA.UI.Statistic.StatisticModule(2);
        }



        public UIElement BuildPlanDatabase()
        {
            return new CO_IA.UI.PlanDatabase.TemplateModule();
        }

    }
}
