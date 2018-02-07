using CO_IA.Data;
using System;
using System.Data;
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
using System.Collections.ObjectModel;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// CheckEquListControl.xaml 的交互逻辑
    /// </summary>
    public partial class CheckEquListControl : UserControl
    {
        private CheckBox chkAll;

        private bool showcheck = true;
        public bool ShowCheck
        {
            set
            {
                showcheck = value;
                columncheck.Visibility = showcheck == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ObservableCollection<DataGridColumn> DataGridColumns
        {
            get { return equchkdatagrid.Columns; }
        }

        public EquipmentCheck SelectedEquipmentCheck
        {
            get { return (EquipmentCheck)GetValue(SelectedEquipmentCheckProperty); }
            set { SetValue(SelectedEquipmentCheckProperty, value); }
        }

        public static readonly DependencyProperty SelectedEquipmentCheckProperty =
            DependencyProperty.Register("SelectedEquipmentCheck", typeof(EquipmentCheck), typeof(CheckEquListControl),
            new PropertyMetadata(new PropertyChangedCallback(SelectedPropertyChanged)));


        public ObservableCollection<EquipmentCheck> EquipmentCheckItemsSource
        {
            get { return (ObservableCollection<EquipmentCheck>)GetValue(EquipmentCheckItemsSourceProperty); }
            set { SetValue(EquipmentCheckItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty EquipmentCheckItemsSourceProperty =
            DependencyProperty.Register("EquipmentCheckItemsSource", typeof(ObservableCollection<EquipmentCheck>), typeof(CheckEquListControl),
            new PropertyMetadata(new PropertyChangedCallback(ItemsSourceChangedCallback)));

        private static void ItemsSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void SelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public CheckEquListControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void equchkdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (equchkdatagrid.SelectedItem != null)
            {
                SelectedEquipmentCheck = equchkdatagrid.SelectedItem as EquipmentCheck;
            }
        }

        private void equchkdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //CO_IA.UI.Setting.Equipment.EquipmentDetailDialog equipmentDetail = new CO_IA.UI.Setting.Equipment.EquipmentDetailDialog(SelectedEquipmentCheck.Equipment, false);
            //equipmentDetail.IsEnabled = false;
            //equipmentDetail.ShowDialog();

            //CO_IA.UI.FreqPlan.EquipmentDetailDialog equipmentDetail = new CO_IA.UI.FreqPlan.EquipmentDetailDialog(SelectedEquipmentCheck.Equipment);
            //equipmentDetail.IsEnabled = false;
            //equipmentDetail.ShowDialog(this);
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.EquipmentCheckItemsSource != null)
            {
                chkAll.IsChecked = EquipmentCheckItemsSource.Any(item => item.IsChecked);
            }
        }

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;

            bool ischecked = chk.IsChecked == null ? false : chk.IsChecked.Value;

            foreach (EquipmentCheck item in EquipmentCheckItemsSource)
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

            foreach (EquipmentCheck result in EquipmentCheckItemsSource)
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
        /// <summary>
        /// 设置指配频率可编辑列的显示状态
        /// </summary>
        /// <param name="p_visible"></param>
        public void SetEditFreqColumnVisible(bool p_isVisible)
        {
            if (p_isVisible)
            {
                _editFreqColumn.Visibility = System.Windows.Visibility.Visible;
                _readOnlyFreqColumn.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                _editFreqColumn.Visibility = System.Windows.Visibility.Collapsed;
                _readOnlyFreqColumn.Visibility = System.Windows.Visibility.Visible;
            }
        }
        string _oldValue;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            double d;
            TextBox tbox = sender as TextBox;
            if (double.TryParse(tbox.Text, out d))
            {
                _oldValue = tbox.Text;
            }
            else
            {
                tbox.Text = _oldValue.ToString();
            }
        }

    }
}
