using CO_IA.Client;
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

namespace CO_IA.UI.FreqPlan.StationPlan
{
    /// <summary>
    /// QueryStationRiasDialog.xaml 的交互逻辑
    /// </summary>
    public partial class QueryStationRiasDialog : Window
    {
        public QueryStationRiasDialog()
        {
            InitializeComponent();
            comboxORG.ItemsSource = Utility.GetORGInfos();
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
