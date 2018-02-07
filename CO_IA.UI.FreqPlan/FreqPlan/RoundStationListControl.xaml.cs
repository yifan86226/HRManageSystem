using CO_IA.Data;
using CO_IA.UI.StationManage;
using CO_IA_Data;
using I_CO_IA.StationManage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// ExtractFreqsUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class RoundStationListControl : UserControl
    {
        public event Action<FreqPlanActivity> OnShowAroundArea;
        public RoundStationListControl()
        {
            InitializeComponent();
        }

        public List<RoundStationInfo> RoundStationItemsSource
        {
            get { return (List<RoundStationInfo>)GetValue(RoundStationItemsSourceProperty); }
            set { SetValue(RoundStationItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoundStationItemsSourceProperty =
            DependencyProperty.Register("RoundStationItemsSource", typeof(List<RoundStationInfo>), typeof(RoundStationListControl),
            new PropertyMetadata(new PropertyChangedCallback(RoundStationItemsSourceChanged)));

        private static void RoundStationItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Stationdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RoundStationInfo selectstation = this.stationdatagrid.SelectedItem as RoundStationInfo;
            if (selectstation != null)
            {
                StationDetailDialog dialog = new StationDetailDialog(selectstation.STATGUID);
                dialog.ShowDialog(this);
            }
        }

        private void stationdatagrid_LayoutUpdated(object sender, EventArgs e)
        {
            stationdatagrid.RowHeight = double.NaN;
        }

        private void stationdatagrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (stationdatagrid.SelectedItem == null) return;
            FreqPlanActivity freqPlanActivity =
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan, FreqPlanActivity>(channel =>
                {
                    return channel.GetSingFreqPlanActivity(((RoundStationInfo)stationdatagrid.SelectedItem).FreqActivityGuid);
                });
            if (OnShowAroundArea != null && freqPlanActivity != null)
            {
                OnShowAroundArea(freqPlanActivity);
            }

        }

        #region  全选/全取消事件  取消该功能
        //private void chkAll_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox chk = sender as CheckBox;
        //    bool ischecked = chk.IsChecked.Value;

        //    foreach (StationInfo item in StationItemsSource)
        //    {
        //        item.IsChecked = ischecked;
        //    }
        //}

        //private void chkAll_Loaded(object sender, RoutedEventArgs e)
        //{
        //    CheckBox chkAll = sender as CheckBox;
        //    if (this.StationItemsSource != null)
        //    {
        //        chkAll.IsChecked = StationItemsSource.Any(item => item.IsChecked);
        //    }
        //}

        //private void chkCell_Click(object sender, RoutedEventArgs e)
        //{

        //    //bool? isChecked = (sender as CheckBox).IsChecked;
        //    //if (!isChecked.HasValue)
        //    //{
        //    //    return;
        //    //}
        //    //bool checkedState = isChecked.Value;

        //}

        #endregion

    }
}
