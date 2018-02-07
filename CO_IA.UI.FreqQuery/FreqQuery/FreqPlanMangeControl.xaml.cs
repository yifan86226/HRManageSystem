using CO_IA.Data;
using I_CO_IA.FreqPlan;
using CO_IA.UI.StationPlan;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CO_IA.UI.FreqQuery
{
    /// <summary>
    /// FreqPlanMangeControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPlanMangeControl : UserControl
    {
        private string ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;

        public List<string> BusinessCodes
        {
            get;
            set;
        }

        public string PlaceGuid
        {
            get { return (string)GetValue(PlaceGuidProperty); }
            set { SetValue(PlaceGuidProperty, value); }
        }

        public static readonly DependencyProperty PlaceGuidProperty =
          DependencyProperty.Register("PlaceGuid", typeof(string), typeof(FreqPlanMangeControl), new PropertyMetadata(new PropertyChangedCallback(PlaceChanged)));

        private static void PlaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FreqPlanMangeControl).CreateFreqPartPlans();
        }

        public FreqPlanMangeControl()
        {
            InitializeComponent();
            //CreateFreqPartPlans();
        }

        /// <summary>
        /// 创建频率规划表
        /// </summary>
        /// <returns></returns>
        private void CreateFreqPartPlans()
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqPlan>(channel =>
            {
                List<FreqPlanInfo> results = channel.GetActivityFreqPlan(ActivityGuid, PlaceGuid, BusinessCodes);
                xFreqPartPlanGrid.ItemsSource = results;
            });
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            FreqPlanQueryDialog query = new FreqPlanQueryDialog(this.ActivityGuid, this.PlaceGuid);
            query.OnQueryEvent += (business) =>
            {
                BusinessCodes = business;
                CreateFreqPartPlans();
            };
            query.ShowDialog(this);
        }

        private void FreqCount_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hy = sender as Hyperlink;
            if (hy.DataContext != null)
            {
                FreqPlanInfo freqplan = hy.DataContext as FreqPlanInfo;
                if (freqplan.Freq_Count > 0)
                {
                    EquipmentListDialog equlistdialog = new EquipmentListDialog(this.PlaceGuid, freqplan.Businesstype.Guid);
                    equlistdialog.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show("当前业务类型下没有设备", "提示", MessageBoxButton.OK);
                }
            }
        }

        private void BusinessType_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hy = sender as Hyperlink;
            if (hy.DataContext != null)
            {
                BusinessType businesstype = hy.DataContext as BusinessType;
                FreqAssignDialog assigndialog = new FreqAssignDialog(this.PlaceGuid, businesstype.Guid);
                if (assigndialog.isInitOk)
                {
                    assigndialog.ShowDialog(this);
                }
                //MessageBox.Show(string.Format("打开'{0}'业务类型的频率占用表", businesstype.Value));
            }
        }

        private void xFreqPartPlanGrid_LayoutUpdated(object sender, System.EventArgs e)
        {
            this.xFreqPartPlanGrid.RowHeight = double.NaN;
        }
    }
}
