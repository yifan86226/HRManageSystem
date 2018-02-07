using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client.UIBuilderFactory.Standard
{
    public class StandardActivityUIBuilder : IActivityUIBuilder
    {
        internal protected StandardActivityUIBuilder()
        {

        }

    
        // 
        public virtual UIElement BuildStaffPlanning()
        {
            return new CO_IA.UI.PersonSchedule.PersonPlanModule();
        }
 

        public virtual bool CanBuildStep(ActivityStep activityStep)
        {
            return activityStep != ActivityStep.None;
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
