using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client.UIBuilderFactory
{
    public interface IActivitySceneUIBuilder
    {
        bool SupportGPS
        {
            get;
        }

        /// <summary>
        /// 频率预案
        /// </summary>
        UIElement BuildFreqPlanning();

        /// <summary>
        /// 监测预案
        /// </summary>
        UIElement BuildMonitorPlanning();

        bool CanBuildStep(ActivityStep activityStep);
    }
}
