using CO_IA.Data;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// EquInspectionStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquInspectionStatisticControl : UserControl, IStatisticObject
    {
        public EquInspectionStatisticControl()
        {
            InitializeComponent();
            this.ShowContainer = false;
        }

        private void xstatisticModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSource();
        }

        private Func<EquInspectionStatisticData, string> GetStatisticGroups(EquInspectionSticType type)
        {
            this.xstatisticModuleControl.SelectedPlace = SelectedPlace;
            switch (type)
            {
                case EquInspectionSticType.活动地点:
                    {
                        return data =>
                        {
                            return data.AddressGuid;
                        };
                    };

                case EquInspectionSticType.检测类型:
                    {
                        return data =>
                        {
                            return data.InspectionState.ToString();
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
            this.xstatisticModuleControl.TitleContent = "设备检测统计图";
            this.xstatisticModuleControl.TypeSource = Enum.GetNames(typeof(EquInspectionSticType));
            this.xstatisticModuleControl.OnStatistic<EquInspectionSticType, EquInspectionStatisticData>(GetStatisticGroups);
            this.xstatisticModuleControl.OnChangeItem += () =>
            {
                this.xstatisticModuleControl.OnStatistic<EquInspectionSticType, EquInspectionStatisticData>(GetStatisticGroups);
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
