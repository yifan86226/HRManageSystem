using CO_IA.Data;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// FreqPartPlanStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPartPlanStatisticControl : UserControl, IStatisticObject
    {
        public FreqPartPlanStatisticControl()
        {
            InitializeComponent();
            this.ShowContainer = false;
        }

        private void xstatisticModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSource();
        }

        private Func<FreqPartPlanStatisticData, string> GetStatisticGroups(FreqPartPlanStatisticType type)
        {
            this.xstatisticModuleControl.SelectedPlace = SelectedPlace;
            switch (type)
            {
                case FreqPartPlanStatisticType.活动地点:
                    {
                        return data =>
                        {
                            return data.AddressGuid;
                        };
                    };

                case FreqPartPlanStatisticType.业务类型:
                    {
                        return data =>
                        {
                            return data.STClass;
                        };
                    };

                default: return null;
            }
        }

        #region IStatisticObject接口

        public string SelectedPlace
        {
            set;
            get;
        }

        public IList StatisticSource
        {
            set { this.xstatisticModuleControl.StatisticSource = value; }
        }

        public bool ShowContainer
        {
            get
            {
                return this.xstatisticModuleControl.ShowContainer;
            }
            set
            {
                this.xstatisticModuleControl.ShowContainer = value;
            }
        }

        public bool ShowStatisticList
        {
            get
            {
                return this.xstatisticModuleControl.ShowStatisticList;
            }
            set
            {
                this.xstatisticModuleControl.ShowStatisticList = value;
            }
        }

        public void InitSource()
        {
            this.xstatisticModuleControl.TitleContent = "频率保障方案统计图";
            this.xstatisticModuleControl.TypeSource = Enum.GetNames(typeof(FreqPartPlanStatisticType));
            this.xstatisticModuleControl.OnStatistic<FreqPartPlanStatisticType, FreqPartPlanStatisticData>(GetStatisticGroups);
            this.xstatisticModuleControl.OnChangeItem += () =>
            {
                this.xstatisticModuleControl.OnStatistic<FreqPartPlanStatisticType, FreqPartPlanStatisticData>(GetStatisticGroups);
            };
        }


        public bool ShowLegend
        {
            get { return xstatisticModuleControl.ShowLegend; }
            set { xstatisticModuleControl.ShowLegend = value; }
        }

        #endregion
    }
}
