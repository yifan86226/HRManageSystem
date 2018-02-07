using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquInspectionStatisticData : StatisticData
    {
        public InspectionStateEnum InspectionState { get; set; }
    }

    public enum InspectionStateEnum
    {
        总数 = -1,
        未检测 = 0,
        检测通过 = 1,
        检测未通过 = 2
    }
}
