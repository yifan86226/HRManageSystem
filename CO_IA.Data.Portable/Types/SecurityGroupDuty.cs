using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    [Flags]
    public enum SecurityGroupDuty
    {
        None=0,
        [AT_BC.Types.EnumDisplayName("综合协调组")]
        CoordinatingGroup=1,
        [AT_BC.Types.EnumDisplayName("频率台站组")]
        FreqStationGroup=2,
        [AT_BC.Types.EnumDisplayName("监测组")]
        MonitorGroup=4,
        [AT_BC.Types.EnumDisplayName("设备检测组")]
        EquipmentInspectionGroup=8,
        [AT_BC.Types.EnumDisplayName("指挥中心")]
        CommandCenter =16
    }
}
