using System;
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

namespace CO_IA.UI.FreqQuery.FreqAnalyse
{
    /// <summary>
    /// FreqAnalyseControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAnalyseControl : UserControl
    {
        public string ActivityGuid { get; set; }
 
        public string PlaceGuid
        {
            get { return (string)GetValue(PlaceGuidProperty); }
            set { SetValue(PlaceGuidProperty, value); }
        }

        public static readonly DependencyProperty PlaceGuidProperty =
          DependencyProperty.Register("PlaceGuid", typeof(string), typeof(FreqAnalyseControl), new PropertyMetadata(new PropertyChangedCallback(PlaceChanged)));

        private static void PlaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as FreqAnalyseControl).CreateFreqPartPlans();
        }

        public FreqAnalyseControl()
        {
            InitializeComponent();
            lstBusiness.ItemsSource = FreqQueryHelper.GetActivityBusinessType(ActivityGuid, PlaceGuid);
        }
 
        private void lstBusiness_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GetFreqPlan()
        {
 
        }
    }
}
