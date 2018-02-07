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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CO_IA.Client;

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// QueryFreqPartPlan_Control1.xaml 的交互逻辑
    /// </summary>
    public partial class QueryFreqPartPlanDialog : Window
    {
        public event Action<List<FreqPlanActivity>> OnSelectList;
        public QueryFreqPartPlanDialog(string pPlaceId)
        {
            InitializeComponent();
            GetActivityFreqPlanInfoSource(pPlaceId);
        }
        private void GetActivityFreqPlanInfoSource(string pPlaceId)
        {
            //try
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.InvokeAsync<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanActivity>>(channel =>
            //    {
            //        return channel.GetFreqPlanActivitys(pPlaceId);
            //    },
            //    callback =>
            //    {
            //        if (callback.IsValid)
            //        {
            //            List<FreqPlanActivity> freqPlanActivitys = callback.Result;
            //            xQueryfreqPartPlanList.FreqPlanInfoItemsSource = new System.Collections.ObjectModel.ObservableCollection<FreqPlanActivity>(freqPlanActivitys);
            //        }
            //    });
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.GetExceptionMessage());
            //}

            List<FreqPlanActivity> freqPlanActivitys =
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, List<FreqPlanActivity>>(channel =>
            {
                return channel.GetFreqPlanActivitys(pPlaceId);
            });
            if (freqPlanActivitys != null)
                xQueryfreqPartPlanList.FreqPlanInfoItemsSource = new System.Collections.ObjectModel.ObservableCollection<FreqPlanActivity>(freqPlanActivitys);
        }
        private void xbtnOk_Click(object sender, RoutedEventArgs e)
        {
            List<FreqPlanActivity> selFreqPlans = xQueryfreqPartPlanList.FreqPlanInfoItemsSource.Where(p => p.IsSelected == true).ToList();
            if (selFreqPlans != null && OnSelectList != null)
            {
                OnSelectList(selFreqPlans);
            }
            this.Close();
        }

        private void xbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
