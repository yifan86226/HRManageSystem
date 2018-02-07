using CO_IA.Data;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// RoundStatAnalyseStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class RoundStatStatisticControl : UserControl, IStatisticObject
    {
        public RoundStatStatisticControl()
        {
            InitializeComponent();
            this.ShowContainer = false;
        }

        private void xstatisticModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSource();
        }

        private Func<SurroundStatStatisticData, string> GetStatisticGroups(SurroundStatStatisticType type)
        {
            this.xstatisticModuleControl.SelectedPlace = SelectedPlace;
            switch (type)
            {
                case SurroundStatStatisticType.活动地点:
                    {
                        return data =>
                        {
                            return data.AddressGuid;
                        };
                    };

                case SurroundStatStatisticType.资料表类型:
                    {
                        return data =>
                        {
                            return data.AppType;
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
                return xstatisticModuleControl.ShowStatisticList;
            }
            set
            {
                xstatisticModuleControl.ShowStatisticList = value;
            }
        }

        public void InitSource()
        {
            this.xstatisticModuleControl.TitleContent = "周围台站统计图";
            this.xstatisticModuleControl.TypeSource = Enum.GetNames(typeof(SurroundStatStatisticType));
            this.xstatisticModuleControl.OnStatistic<SurroundStatStatisticType, SurroundStatStatisticData>(GetStatisticGroups);
            this.xstatisticModuleControl.OnChangeItem += () =>
            {
                this.xstatisticModuleControl.OnStatistic<SurroundStatStatisticType, SurroundStatStatisticData>(GetStatisticGroups);
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
