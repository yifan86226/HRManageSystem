using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Types
{
    public enum FreqPlanningStep
    {
        [EnumDisplayNameAttribute("频率保障方案")]
        BusinessPlanning,
        [EnumDisplayNameAttribute("参保单位管理")]
        StationPlanning,
        [EnumDisplayNameAttribute("周围台站分析")]
        SurroundStationAnalyse,
        [EnumDisplayNameAttribute("监测数据分析")]
        EMEAnalyse,
        [EnumDisplayNameAttribute("监测信号管控")]
        EMEClear,
        [EnumDisplayNameAttribute("频率指配")]
        FreqAssign,
        [EnumDisplayNameAttribute("设备检测")]
        EquipmentInspection,
        [EnumDisplayNameAttribute("监测预案")]
        MonitorPlanning

    }
}
