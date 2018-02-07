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

namespace CO_IA.Scene.Modules
{
    /// <summary>
    /// MonitorPlanModule.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorPlanModule : UserControl
    {
        public MonitorPlanModule()
        {
            InitializeComponent();
        }
        CO_IA.Scene.Controls.MonitorTaskRunControl taskRunControl;
        private void TaskRun_Click(object sender, RoutedEventArgs e)
        {
            if (taskRunControl == null)
            {
                taskRunControl = new Controls.MonitorTaskRunControl();
                this.gridMonitorRunningContainer.Children.Clear();
                this.gridMonitorRunningContainer.Children.Add(taskRunControl);
                taskRunControl.StopMonitored += bFinished =>
                {
                    if (bFinished)
                    {
                        taskRunControl = null;
                        this.gridMonitorRunningContainer.Children.Clear();
                    }
                    this.gridMonitorRunningContainer.Visibility = System.Windows.Visibility.Collapsed;
                };
            }
            this.gridMonitorRunningContainer.Visibility = System.Windows.Visibility.Visible;
        }

        private void dataFreqRange_DoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
