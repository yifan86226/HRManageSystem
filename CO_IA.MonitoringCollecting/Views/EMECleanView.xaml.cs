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

namespace CO_IA.MonitoringCollecting.Views
{
    /// <summary>
    /// EMECleanView.xaml 的交互逻辑
    /// </summary>
    public partial class EMECleanView : UserControl
    {
        public EMECleanView()
        {
            InitializeComponent();
            LoadEMECleanView();
        }

        private void LoadEMECleanView()
        {
            //EmeMonitorAnalyse_Control emeControl = new EmeMonitorAnalyse_Control();
            //LayOutRoot.Children.Clear();
            //LayOutRoot.Children.Add(emeControl);
        }
    }
}
