using System.Collections;

namespace CO_IA.UI.Statistic
{
    public interface IStatisticObject
    {
        /// <summary>
        /// 统计数据源
        /// </summary>
        IList StatisticSource
        {
            set;
        }

        /// <summary>
        /// 选择地点
        /// </summary>
        string SelectedPlace
        {
            set;
            get;
        }
        /// <summary>
        /// 显示内容
        /// </summary>
        bool ShowContainer
        {
            get;
            set;
        }

        /// <summary>
        /// 显示统计列表
        /// </summary>
        bool ShowStatisticList
        {
            get;
            set;
        }

        bool ShowLegend
        {
            get;
            set;
        }

        void InitSource();
    }
}
