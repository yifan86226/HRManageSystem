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

namespace CO_IA.UI.Setting.Equipment
{
    /// <summary>
    /// EquipmentListControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentListControl : UserControl
    {

        ObservableCollection<EquipmentInfo> equipmentItemsSource;
        private CheckBox chkAll;

        public EquipmentInfo[] EquipmentItemsSource
        {
            get { return equdatagrid.ItemsSource as EquipmentInfo[]; }
            set { equdatagrid.ItemsSource = value; }
        }

        public ObservableCollection<DataGridColumn> DataGridColumns
        {
            get { return equdatagrid.Columns; }
        }

        public bool _showCompany;

        public bool ShowCompany
        {
            get
            {
                return _showCompany;
            }
            set
            {
                _showCompany = value;
                if (_showCompany)
                {
                    columnCompany.Visibility = Visibility.Visible;
                }
                else
                {
                    columnCompany.Visibility = Visibility.Collapsed;
                }
            }
        }

        public EquipmentInfo SelectedEquipment
        {
            get { return (EquipmentInfo)GetValue(SelectedEquipmentProperty); }
            set { SetValue(SelectedEquipmentProperty, value); }
        }

        public static readonly DependencyProperty SelectedEquipmentProperty =
            DependencyProperty.Register("SelectedEquipment", typeof(EquipmentInfo), typeof(EquipmentListControl),
            new PropertyMetadata(new PropertyChangedCallback(SelectedEquipmentChangedCallback)));

        private static void SelectedEquipmentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
         
        public EquipmentListControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void equdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EquipmentDetailDialog dialog = new EquipmentDetailDialog(SelectedEquipment,false);
            dialog.AllowEdite = false;
            dialog.WindowTitle = "设备-详细信息";
            dialog.ShowDialog(this);
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (EquipmentInfo item in EquipmentItemsSource)
            {
                item.IsChecked = ischecked;
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

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            bool? isChecked = (sender as CheckBox).IsChecked;
            if (!isChecked.HasValue)
            {
                return;
            }
            bool checkedState = isChecked.Value;

            foreach (EquipmentInfo result in EquipmentItemsSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }
            }
            chkAll.IsChecked = checkedState;
        }

        private void equdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.equdatagrid.SelectedItem != null)
            {
                SelectedEquipment = this.equdatagrid.SelectedItem as EquipmentInfo;
            }
        }
    }
}
