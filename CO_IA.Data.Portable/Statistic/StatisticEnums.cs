using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 统计类型
    /// </summary>
    public enum StatisticTypes
    {
        /// <summary>
        /// 单位设备
        /// </summary>
        ORGAndEQUStatisticType,

        /// <summary>
        /// 频率保障方案
        /// </summary>
        FreqPartPlanStatisticType,

        /// <summary>
        /// 周围台站
        /// </summary>
        SurroundStatStatisticType,

        /// <summary>
        /// 频率指配
        /// </summary>
        FreqAssignStatisticType,

        /// <summary>
        /// 设备检测
        /// </summary>
        EquInspectionSticType,

        /// <summary>
        /// 人员预案
        /// </summary>
        PersonPlanStatisticType,

        /// <summary>
        /// 人员奖惩
        /// </summary>
        PersonRPStatisticType,

        /// <summary>
        /// 人员外出
        /// </summary>
        PersonOutStatisticType


    }

    /// <summary>
    /// 单位设备
    /// </summary>
    public enum ORGAndEQUStatisticType
    {
        活动地点,
        单位类别,
        //频点类别,
        //频点数量
        //设备数量,
    }

    /// <summary>
    /// 频率规划
    /// </summary>
    public enum FreqPartPlanStatisticType
    {
        活动地点,
        业务类型
    }

    /// <summary>
    /// 周围台站
    /// </summary>
    public enum SurroundStatStatisticType
    {
        活动地点,
        资料表类型
    }

    /// <summary>
    /// 频率指配
    /// </summary>
    public enum FreqAssignStatisticType
    {
        活动地点,
        频率类型
    }

    /// <summary>
    /// 设备检测
    /// </summary>
    public enum EquInspectionSticType
    {
        活动地点,
        检测类型
    }

    /// <summary>
    /// 人员预案
    /// </summary>
    public enum PersonPlanSticType
    {
        监测小组,
        统计类型
    }

    /// <summary>
    /// 频率清理
    /// </summary>
    public enum EmeClearStatisticType
    {
        活动地点,
        信号来源
    }


}
