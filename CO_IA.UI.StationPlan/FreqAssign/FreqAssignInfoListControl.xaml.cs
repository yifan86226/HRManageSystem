using CO_IA.Data;
using CO_IA.UI.FreqPlan;
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

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// FreqAssignList_Control.xaml 的交互逻辑 EquipmentAssignFreq
    /// </summary>
    public partial class FreqAssignInfoListControl : UserControl
    {
        private CheckBox chkAll;

        public FreqAssignInfoListControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public List<ActivityEquipmentInfo> EquipmentItemsSource
        {
            get { return (List<ActivityEquipmentInfo>)GetValue(EquipmentItemsSourceProperty); }
            set { SetValue(EquipmentItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty EquipmentItemsSourceProperty =
            DependencyProperty.Register("EquipmentItemsSource", typeof(List<ActivityEquipmentInfo>), typeof(FreqAssignInfoListControl),
            new PropertyMetadata(new PropertyChangedCallback(propertyChanged))
          );

        private static void propertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
         

        public ActivityEquipmentInfo SelectedEquipment
        {
            get
            {
                return xFreqAssignGrid.SelectedItem as ActivityEquipmentInfo;
            }
        }

        public List<ActivityEquipmentInfo> CheckedEquipment
        {
            get
            {
                return EquipmentItemsSource.Where(r => r.IsChecked == true).ToList();
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.EquipmentItemsSource != null)
            {
                chkAll.IsChecked = EquipmentItemsSource.Any(item => item.IsChecked);
            }
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;

            bool ischecked = chk.IsChecked == null ? false : chk.IsChecked.Value;

            foreach (ActivityEquipmentInfo item in EquipmentItemsSource)
            {
                item.IsChecked = ischecked;
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

            foreach (ActivityEquipmentInfo result in EquipmentItemsSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }
            }
            chkAll.IsChecked = checkedState;
        }

        public void CancelChKAll()
        {
            if (chkAll != null)
            {
                chkAll.IsChecked = false;
            }
        }

        private void xFreqAssignGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EquipmentDetailDialog dialog = new EquipmentDetailDialog(SelectedEquipment);
            dialog.IsEnabled = false;
            dialog.WindowTitle = "设备-详细信息";
            dialog.ShowDialog(this);
        }
    }
}
