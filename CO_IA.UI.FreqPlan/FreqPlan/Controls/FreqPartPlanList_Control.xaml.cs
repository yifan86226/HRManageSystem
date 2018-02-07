using CO_IA.Data;
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
using CO_IA.Client;

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// FreqPartPlanList_Control.xaml 的交互逻辑
    /// </summary>
    public partial class FreqPartPlanList_Control : UserControl
    {
        public FreqPartPlanList_Control()
        {
            InitializeComponent();
        }

        public ObservableCollection<FreqPlanSegment> FreqPlanInfoItemsSource
        {
            get { return xFreqPartPlanGrid.ItemsSource as ObservableCollection<FreqPlanSegment>; }
            set { xFreqPartPlanGrid.ItemsSource = value; }
        }

        private CheckBox _isAllCheckBox;

        private void ckbSelectedAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (FreqPlanSegment item in FreqPlanInfoItemsSource)
            {
                item.IsSelected = ischecked;
            }
        }

        private void ckbSelectedAll_Loaded(object sender, RoutedEventArgs e)
        {
            this._isAllCheckBox = (CheckBox)sender;
            if (this.FreqPlanInfoItemsSource != null)
            {
                _isAllCheckBox.IsChecked = FreqPlanInfoItemsSource.Any(item => item.IsSelected);
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = (sender as CheckBox).IsChecked;
            if (!isChecked.HasValue)
            {
                return;
            }
            bool checkedState = isChecked.Value;

            foreach (FreqPlanSegment result in FreqPlanInfoItemsSource)
            {
                if (result.IsSelected != checkedState)
                {
                    this._isAllCheckBox.IsChecked = null;
                    return;
                }
            }
            _isAllCheckBox.IsChecked = checkedState;
        }
    }
}
