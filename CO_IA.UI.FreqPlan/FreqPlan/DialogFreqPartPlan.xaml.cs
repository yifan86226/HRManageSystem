using CO_IA.Data;
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
using CO_IA.Client;

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// DialogFreqPartPlan.xaml 的交互逻辑
    /// </summary>
    public partial class DialogFreqPartPlan : Window
    {
        public event Action<List<FreqPlanSegment>> OnSelectList;
        public DialogFreqPartPlan()
        {
            InitializeComponent();
            this.Loaded += DialogFreqPartPlan_Loaded;
        }

        void DialogFreqPartPlan_Loaded(object sender, RoutedEventArgs e)
        {
            xfreqPartPlanList.FreqPlanInfoItemsSource = new System.Collections.ObjectModel.ObservableCollection<FreqPlanSegment>(GetFreqPartPlan());
        }
        private List<FreqPlanSegment> GetFreqPartPlan()
        {
            try
            {
                return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanSegment>>(channel =>
                {
                    return channel.GetFreqPlanInfo();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage());
                return null;
            }
        }
        private void xbtnCalcel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void xbtnOk_Click(object sender, RoutedEventArgs e)
        {
            List<FreqPlanSegment> selFreqPlans = xfreqPartPlanList.FreqPlanInfoItemsSource.Where(p=>p.IsSelected == true).ToList();
            if (selFreqPlans != null && OnSelectList != null)
            {
                OnSelectList(selFreqPlans);
            }
            Close();
        }
    }
}
