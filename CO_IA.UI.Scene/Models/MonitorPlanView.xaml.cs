using CO_IA.UI.MonitorPlan;
using System.Windows;
using System.Windows.Controls;
//using CO_IA.UI.Collection;

namespace CO_IA.UI.Scene
{
    /// <summary>
    /// MonitorPlan.xaml 的交互逻辑 MonitorPlanCreate monitorPlan = new MonitorPlanCreate();
    /// </summary>
    public partial class MonitorPlanView : UserControl
    {
        public MonitorPlanView()
        {
            InitializeComponent();
            MonitorPlanCreate monitorPlan = new MonitorPlanCreate();
            LayOutRoot.Children.Add(monitorPlan);
            this.xBtnManagementConnect.Click += xBtnManagementConnect_Click;
            this.xBtnCollect.Click += OnCollect_Click;
        }

        private void xBtnManagementConnect_Click(object sender, RoutedEventArgs e)
        {
            EquipmentManage dlg = new EquipmentManage();
            dlg.ShowDialog();
        }

        /// <summary>
        /// 采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCollect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //LayOutRoot.Children.Clear();
            //RealTimeMonitor rtm = new RealTimeMonitor();
            //LayOutRoot.Children.Add(rtm);
            //xRealTimeMonitoring.Visibility = Visibility.Visible;
            //xGridMain.Visibility = Visibility.Collapsed;
            //xRealTimeMonitoring.GoBack += () =>
            //{
            //    xRealTimeMonitoring.Visibility = Visibility.Collapsed;
            //    xGridMain.Visibility = Visibility.Visible;
            //};
        }
    }
}
