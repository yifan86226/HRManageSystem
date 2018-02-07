using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client.UIBuilderFactory
{
    public interface IActivityUIBuilder
    {
        
 

        /// <summary>
        /// 统计
        /// </summary>
        /// <returns></returns>
        UIElement BuildStatistic();

  
        UIElement BuildStatistic2();


        UIElement BuildPlanDatabase();



        bool CanBuildStep(ActivityStep activityStep);
    }
}
