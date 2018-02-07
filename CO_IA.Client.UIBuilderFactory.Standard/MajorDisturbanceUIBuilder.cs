using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client.UIBuilderFactory.Standard
{
    /// <summary>
    /// 重大干扰
    /// </summary>
    public class MajorDisturbanceUIBuilder : ContingencyActivityUIBuilder
    {
        internal protected MajorDisturbanceUIBuilder()
        {

        }
     

        public override UIElement BuildSchedule()
        {
            return new CO_IA.UI.PersonSchedule.ScheduleModule();
        }
    }
}
