using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Charts;
using AT_BC.Data;
using System.ComponentModel;
using System;

namespace CO_IA.UI.Statistic
{
    /// <summary>
    /// StatisticChartControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticChartControl : UserControl, INotifyPropertyChanged
    {
        private bool legendVisibility = true;
        public bool LegendVisibility
        {
            get
            { return legendVisibility; }
            set
            {
                legendVisibility = value;
                NotifyPropertyChange("LegendVisibility");
            }
        }

        public string titleContent;

        public string TitleContent
        {
            set
            {
                chartTitle.Content = value;
            }
        }
 
        public List<SeriesNameValue<double>> StatisticItemsSource
        {
            get { return (List<SeriesNameValue<double>>)GetValue(StatisticItemsSourceProperty); }
            set { SetValue(StatisticItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty StatisticItemsSourceProperty =
            DependencyProperty.Register("StatisticItemsSource", typeof(List<SeriesNameValue<double>>), typeof(StatisticChartControl),
             new PropertyMetadata(new PropertyChangedCallback(StatisticItemsSourcePropertyChangedCallBack)));

        private static void StatisticItemsSourcePropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatisticChartControl sender = d as StatisticChartControl;
            sender.chartControl.DataSource = e.NewValue;
            sender.chartControl.Animate();
     
            foreach (BarSideBySideSeries2D series in sender.chartControl.Diagram.Series)
            {

             
                //    Type animationType = ((AnimationKind)lbSeriesAnimation.SelectedItem).Type;

                //series.
                //    series.SetSeriesAnimation(animationType != null ? (SeriesAnimationBase)Activator.CreateInstance(animationType) : null);
               

           
                series.CrosshairLabelPattern = "{S}: {V}";
            }
        }

        public StatisticChartControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
