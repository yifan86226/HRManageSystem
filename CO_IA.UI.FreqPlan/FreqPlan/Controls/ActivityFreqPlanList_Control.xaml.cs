using AT_BC.Common;
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

namespace CO_IA.UI.FreqPlan.FreqPlan
{
    /// <summary>
    /// ActivityFreqPlanList.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityFreqPlanList_Control : UserControl
    {
        public event Action<FreqPlanActivity> OnAreaSelect;
        public event Action<FreqPlanActivity> OnShowAroundArea;
        private FreqPlanActivity _selFreqPlan = null;  //选择的频段信息
        public ActivityFreqPlanList_Control()
        {
            InitializeComponent();
        }
        public bool BtnRoundIsVisiable
        {
            get { return btnRound.Visibility == System.Windows.Visibility.Visible; }
            set 
            {
                if (value)
                    btnRound.Visibility = System.Windows.Visibility.Visible;
                else
                    btnRound.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        public ObservableCollection<FreqPlanActivity> FreqPlanInfoItemsSource
        {
            get { return xFreqPartPlanGrid.ItemsSource as ObservableCollection<FreqPlanActivity>; }
            set { xFreqPartPlanGrid.ItemsSource = value; }
        }
        public bool ShowBtnRoundStation
        {
            get { return this.btnRound.Visibility == System.Windows.Visibility.Visible; }
            set
            {
                if (value)
                    this.btnRound.Visibility = System.Windows.Visibility.Visible;
                else
                    this.btnRound.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void btnAreaSelect_Click(object sender, RoutedEventArgs e)
        {
            var dataGridRow = VisualTreeHelperExtension.GetParentObject<DataGridRow>(sender as UIElement);
            while (dataGridRow != null)
            {
                if (dataGridRow is DataGridRow && (dataGridRow.DataContext.GetType() == typeof(FreqPlanActivity)))
                {
                    _selFreqPlan = (FreqPlanActivity)dataGridRow.DataContext;
                    break;
                }
            }
            DialogExtendDistance dialog = new DialogExtendDistance();
            dialog.OnSetDistance += dialog_OnSetDistance;
            dialog.ShowDialog(this);
        }

        void dialog_OnSetDistance(double obj)
        {
            if (OnAreaSelect != null && _selFreqPlan != null)
            {
                _selFreqPlan.Distance = obj;
                OnAreaSelect(_selFreqPlan);
            }
        }

        private void xFreqPartPlanGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnShowAroundArea != null && xFreqPartPlanGrid.SelectedItem != null)
            {
                OnShowAroundArea((FreqPlanActivity)xFreqPartPlanGrid.SelectedItem);
            }
        }
        private CheckBox _isAllCheckBox;
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

        private void ckbSelectedAll_Loaded(object sender, RoutedEventArgs e)
        {
            this._isAllCheckBox = (CheckBox)sender;
            if (this.FreqPlanInfoItemsSource != null)
            {
                _isAllCheckBox.IsChecked = FreqPlanInfoItemsSource.Any(item => item.IsSelected);
            }
        }

        private void ckbSelectedAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (FreqPlanSegment item in FreqPlanInfoItemsSource)
            {
                item.IsSelected = ischecked;
            }
        }
    }
}
