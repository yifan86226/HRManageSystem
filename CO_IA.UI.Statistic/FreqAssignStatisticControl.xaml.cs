using CO_IA.Data;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// FreqAssignStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAssignStatisticControl : UserControl, IStatisticObject
    {
        public FreqAssignStatisticControl()
        {
            InitializeComponent();
            this.ShowContainer = false;
        }

        private void xstatisticModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSource();
        }

        private Func<FreqAssignStatisticData, string> GetStatisticGroups(FreqAssignStatisticType type)
        {
            this.xstatisticModuleControl.SelectedPlace = SelectedPlace;
            switch (type)
            {
                case FreqAssignStatisticType.活动地点:
                    {
                        return data =>
                        {
                            return data.AddressGuid;
                        };
                    };
                case FreqAssignStatisticType.频率类型:
                    {
                        return data => { return data.Type; };
                    };

                //case FreqAssignStatisticType.单位名称:
                //    {
                //        return data =>
                //        {
                //            return data.CompanyName;
                //        };
                //    };
                //case FreqAssignStatisticType.频段:
                //    {
                //        return data =>
                //        {
                //            return data.Type;
                //        };
                //    };
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
            this.xstatisticModuleControl.TitleContent = "频率指配统计图";
            this.xstatisticModuleControl.TypeSource = Enum.GetNames(typeof(FreqAssignStatisticType));
            this.xstatisticModuleControl.OnStatistic<FreqAssignStatisticType, FreqAssignStatisticData>(GetStatisticGroups);
            this.xstatisticModuleControl.OnChangeItem += () =>
            {
                this.xstatisticModuleControl.OnStatistic<FreqAssignStatisticType, FreqAssignStatisticData>(GetStatisticGroups);
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
