using System;
using System.Windows;
using System.Windows.Controls;
using CO_IA.Data;
using System.Collections;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// 单位设备统计
    /// </summary>
    public partial class ORGAndEQUStatisticControl : UserControl, IStatisticObject
    {
        public ORGAndEQUStatisticControl()
        {
            InitializeComponent();
            this.ShowContainer = false;
        }

        private void xstatisticModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSource();
        }


        private Func<EquStatisticData, string> GetStatisticGroups(ORGAndEQUStatisticType type)
        {
            this.xstatisticModuleControl.SelectedPlace = SelectedPlace;
            switch (type)
            {
                case ORGAndEQUStatisticType.活动地点:
                    {
                        return data =>
                        {
                            return data.AddressGuid;
                        };
                    };

                case ORGAndEQUStatisticType.单位类别:
                    {
                        return data =>
                        {
                            return data.ORGName;
                        };
                    };
                //case StatisticType.频点类别:
                //    {
                //        return data =>
                //        {
                //            return data.Freq;
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
                return xstatisticModuleControl.ShowStatisticList;
            }
            set
            {
                xstatisticModuleControl.ShowStatisticList = value;
            }
        }

        public void InitSource()
        {
            this.xstatisticModuleControl.TitleContent = "参保单位计图";
            this.xstatisticModuleControl.TypeSource = Enum.GetNames(typeof(ORGAndEQUStatisticType));
            this.xstatisticModuleControl.OnStatistic<ORGAndEQUStatisticType, EquStatisticData>(GetStatisticGroups);
            this.xstatisticModuleControl.OnChangeItem += () =>
            {
                this.xstatisticModuleControl.OnStatistic<ORGAndEQUStatisticType, EquStatisticData>(GetStatisticGroups);
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
