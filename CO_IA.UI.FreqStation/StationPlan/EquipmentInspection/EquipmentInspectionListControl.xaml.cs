using AT_BC.Common;
using CO_IA.Data;
using CO_IA.UI.PlanDatabase.Equipments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// EquCheckListControl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentInspectionListControl : UserControl, INotifyPropertyChanged
    {
        CheckBox checkBoxAll;
        public event Action<EquipmentInspection> EquInspectionSelectionChanged;

        private EquipmentInspection equipmentinspection;
        public EquipmentInspection EquipmentInspectionSelected
        {
            get
            {
                return equipmentinspection;
            }
            set
            {
                equipmentinspection = value;
                NotifyPropertyChanged("EquipmentInspectionSelected");
            }
        }

        public List<EquipmentInspection> EquipmentInspectionItemsSource
        {
            get { return this.DataContext as List<EquipmentInspection>; }

        }

        public EquipmentInspectionListControl()
        {
            InitializeComponent();
            this.DataContextChanged += EquipmentInspectionListControl_DataContextChanged;
        }

        private void EquipmentInspectionListControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            List<EquipmentInspection> sources = sender as List<EquipmentInspection>;
            if (sources != null && sources.Count > 0)
            {
                this.equInspectiondatagrid.SelectedIndex = 0;
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxAll = sender as CheckBox;
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }

        private void equInspectiondatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EquipmentInspectionSelected != null && EquipmentInspectionSelected.ActivityEquipment != null)
            {
                EquipmentManageDialog equdialog = new EquipmentManageDialog();
                equdialog.DataContext = EquipmentInspectionSelected.ActivityEquipment;
                equdialog.AllowEdit = false;
                equdialog.ShowDialog();
            }
        }

        private void equInspectiondatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EquipmentInspectionSelected = equInspectiondatagrid.SelectedItem as EquipmentInspection;
            if (EquInspectionSelectionChanged != null)
            {
                EquInspectionSelectionChanged(EquipmentInspectionSelected);
            }
        }

        public void UnSelectedAll()
        {
            this.checkBoxAll.IsChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
