using CO_IA.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// EmeClearStatisticControl.xaml 的交互逻辑
    /// </summary>
    public partial class EmeClearStatisticControl : UserControl
    {
        public EmeClearStatisticControl()
        {
            InitializeComponent();
        }
        public IList StatisticSource
        {
            set { this.xstatisticModuleControl.StatisticSource = value; }
        }
        private void xstatisticModuleControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitSource();
        }
        private void InitSource()
        {
            this.xstatisticModuleControl.TitleContent = "电磁环境清理统计图";
            this.xstatisticModuleControl.TypeSource = Enum.GetNames(typeof(EmeClearStatisticType));
            this.xstatisticModuleControl.OnStatistic<EmeClearStatisticType, EmeClearStatisticData>(GetStatisticGroups);
            this.xstatisticModuleControl.OnChangeItem += () =>
            {
                this.xstatisticModuleControl.OnStatistic<EmeClearStatisticType, EmeClearStatisticData>(GetStatisticGroups);
            };

        }

        private Func<EmeClearStatisticData, string> GetStatisticGroups(EmeClearStatisticType type)
        {
            switch (type)
            {
                case EmeClearStatisticType.活动地点:
                    {
                        return data =>
                        {
                            return data.Address;
                        };
                    };

                case EmeClearStatisticType.信号来源:
                    {
                        return data =>
                        {
                            return data.SignalSource;
                        };
                    };
                default: return null;
            }
        }
    }
}
