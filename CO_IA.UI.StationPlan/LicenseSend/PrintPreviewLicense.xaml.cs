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
using System.Windows.Shapes;
using CO_IA.Data;
using CO_IA.Data.StationPlan;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// PrintPreviewLicense.xaml 的交互逻辑
    /// </summary>
    public partial class PrintPreviewLicense : Window
    {
        public LicenseTempleteInfo _licenseTempleteInfo = null;
        private List<ActivityEquipmentInfo> _equList = null;
        public PrintPreviewLicense(List<ActivityEquipmentInfo> equList)
        {
            InitializeComponent();
            this._equList = equList;
            _licenseTempleteInfo = GetLicenseTempleteInfo();
            if (_licenseTempleteInfo == null)
            {
                MessageBox.Show("本活动还未设置许可证模板，请先设置模板", "提示", MessageBoxButton.OK);
                return;
            }
            else
            {
                InitPage();
            }
        }

        private void InitPage()
        {
            foreach (ActivityEquipmentInfo equInfo in _equList)
            {
                Canvas _canvasView = LicenseViewFactory.CreateLicenseViewCanvas(equInfo, _licenseTempleteInfo, (bool)chkIsPrintBg.IsChecked);
                Thickness thick = new Thickness(12, 12, 0, 0);
                _canvasView.Margin = thick;
                panelView.Children.Add(_canvasView);
            }
        }

        /// <summary>
        /// 获取当前活动的许可证信息
        /// </summary>
        /// <returns></returns>
        private LicenseTempleteInfo GetLicenseTempleteInfo()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.StationPlan.I_CO_IA_StationPlan, CO_IA.Data.StationPlan.LicenseTempleteInfo>(
                channel =>
                {
                    return channel.GetLicenseTempleteInfo(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
                });
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            scrolls.ScrollToHome();
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(panelView, "许可证模板");
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void chkIsPrintBg_Click(object sender, RoutedEventArgs e)
        {
            panelView.Children.Clear();
            InitPage();
        }
    }
}
