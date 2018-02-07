using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client.UIBuilderFactory
{
    public interface IActivityUIBuilderFactory
    {
        IActivityUIBuilder GetUIBuilder(string activityType);

        System.Windows.Media.ImageSource GetImageSource(string activityType);

        IActivityDutyCodeQuerier DutyCodeQuerier
        {
            get;
        }
        //ActivityStep GetActivityStep();
    }

    public interface IActivityDutyCodeQuerier
    {
        /// <summary>
        /// 获取领导职责便秘
        /// </summary>
        string Lead
        {
            get;
        }

        /// <summary>
        /// 获取指挥职责便秘
        /// </summary>
        string Command
        {
            get;
        }

        /// <summary>
        /// 获取协调职责编码
        /// </summary>
        string Coordinate
        {
            get;
        }

        /// <summary>
        /// 获取频率台站职责编码
        /// </summary>
        string FreqStation
        {
            get;
        }

        /// <summary>
        /// 获取频率监测职责编码
        /// </summary>
        string FreqMonitor
        {
            get;
        }

        /// <summary>
        /// 获取设备检测职责编码
        /// </summary>
        string EquipmentInspection
        {
            get;
        }

        /// <summary>
        /// 获取行政执法职责编码
        /// </summary>
        string Administrator
        {
            get;
        }

        /// <summary>
        /// 获取后勤保障职责编码
        /// </summary>
        string EnsureSupplies
        {
            get;
        }

        /// <summary>
        /// 获取宣传职责编码
        /// </summary>
        string Propaganda
        {
            get;
        }
    }
}
