using CO.IA.UI.TaskManage.Rules;
using CO_IA.UI.FreqPlan.FreqPlan;
using System.Windows;
using System.Windows.Controls;
using CO_IA.Client;
using CO_IA.UI.FreqPlan;

namespace CO_IA.UI.Scene
{
    /// <summary>
    /// 频率预案
    /// </summary>
    public partial class FreqPlanView : UserControl
    {
        public FreqPlanView()
        {
            InitializeComponent();
            FreqPlanModule freqPlanModule = new FreqPlanModule(LoginService.CurrentActivity, LoginService.CurrentActivityPlace);
            LayOutRoot.Children.Add(freqPlanModule);
           // LoginService.ActivityPlaceChanged += LoginService_ActivityPlaceChanged;
           
            //this.xBtnAroundStation.Click += OnAroundStation_Click;
            //this.xBtnEmcClear.Click += xBtnEmcClear_Click;
            //this.xBtnEquipmentCheck.Click += xBtnEquipmentCheck_Click;
            //this.xBtnLicenceGrant.Click += xBtnLicenceGrant_Click;
            //this.xBtnFreqPaln.Click += xBtnFreqPaln_Click;
        }

        void LoginService_ActivityPlaceChanged(Data.ActivityPlace obj)
        {
            FreqPlanModule freqPlanModule = new FreqPlanModule(LoginService.CurrentActivity,LoginService.CurrentActivityPlace);
            LayOutRoot.Children.Add(freqPlanModule);
        }

        ///// <summary>
        ///// 许可证发放
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void xBtnLicenceGrant_Click(object sender, RoutedEventArgs e)
        //{
        //    LicenseOfSend licesend = new LicenseOfSend();
        //    licesend.ShowDialog(this);
        //}

        ///// <summary>
        ///// 周围台站
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void OnAroundStation_Click(object sender, RoutedEventArgs e)
        //{
        //    Window dialog = new Window();
        //    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        //    dialog.Content = new RoundStatAnalyse_Control();
        //    dialog.ShowDialog(this);
        //}

        ///// <summary>
        ///// 电磁环境清理方案
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void xBtnEmcClear_Click(object sender, RoutedEventArgs e)
        //{
        //    Window dialog = new Window();
        //    dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        //    dialog.Content = new EmeClearHandle_Control();
        //    dialog.ShowDialog(this);
        //}

        ///// <summary>
        ///// 设备检测
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void xBtnEquipmentCheck_Click(object sender, RoutedEventArgs e)
        //{
        //    Window window = new Window();
        //    window.Content = new EquCheck_Control();
        //    window.ShowDialog(this);
        //}

        //private void xBtnFreqPaln_Click(object sender, RoutedEventArgs e)
        //{
        //    FreqPlan_Control freqplandialog = new FreqPlan_Control();
        //    freqplandialog.ShowDialog(this);
        //}
    }
}
