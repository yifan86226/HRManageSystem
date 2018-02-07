using AT_BC.Data;
using DevExpress.Xpf.Charts;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// StatisticChartPieControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticChartPieControl : UserControl
    {
        public List<SeriesNameValue<double>> StatisticItemsSource
        {
            get { return (List<SeriesNameValue<double>>)GetValue(StatisticItemsSourceProperty); }
            set { SetValue(StatisticItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty StatisticItemsSourceProperty =
            DependencyProperty.Register("StatisticItemsSource", typeof(List<SeriesNameValue<double>>), typeof(StatisticChartPieControl),
             new PropertyMetadata(new PropertyChangedCallback(StatisticItemsSourcePropertyChangedCallBack)));

        private static void StatisticItemsSourcePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatisticChartPieControl sender = d as StatisticChartPieControl;
            sender.chartControl.DataSource = e.NewValue;
            sender.chartControl.Animate();

            //foreach (PieSeries2D series in sender.chartControl.Diagram.Series)
            //{
            //    series.ToolTipPointPattern = "{S}: {V}";
            //}
        }

        public StatisticChartPieControl()
        {
            InitializeComponent();
        }
    }
}
