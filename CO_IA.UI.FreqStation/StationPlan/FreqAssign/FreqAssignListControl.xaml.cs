using AT_BC.Common;
using CO_IA.Data;
using CO_IA.UI.PlanDatabase.Equipments;
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
using System.Linq;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// FreqAssignListControl.xaml 的交互逻辑
    /// </summary>
    public partial class FreqAssignListControl : UserControl
    {
        CheckBox checkBoxAll;

        public ActivityEquipment[] ActivityEquipmentItemsSource
        {
            get
            {
                return xFreqAssignGrid.ItemsSource as ActivityEquipment[];
            }
        }

        private ActivityEquipment SelectedActivityEquipment
        {
            get { return xFreqAssignGrid.SelectedItem as ActivityEquipment; }
        }

        public FreqAssignListControl()
        {
            InitializeComponent();
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxAll = sender as CheckBox;
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }

        private void xFreqAssignGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedActivityEquipment != null)
            {
                EquipmentManageDialog equdialog = new EquipmentManageDialog();
                equdialog.DataContext = SelectedActivityEquipment;
                equdialog.AllowEdit = false;
                equdialog.ShowDialog();
            }
        }

        public void UnSelectAll()
        {
            checkBoxAll.IsChecked = false;
            if (ActivityEquipmentItemsSource != null)
            {
                ActivityEquipmentItemsSource.ToList().ForEach(r => r.IsChecked = false);
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string value = e.Text;
            double d;
            if (!double.TryParse(value,out d))
            {
                e.Handled = true;
            }
        }
    }
}
