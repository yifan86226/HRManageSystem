using AT_BC.Common;
using CO_IA.Data;
using CO_IA.UI.StationManage;
using CO_IA_Data;
using DevExpress.Xpf.Grid;
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

namespace CO_IA.UI.FreqStation.FreqPlan.SurroundStation
{
    /// <summary>
    /// SurroundStationListControl.xaml 的交互逻辑
    /// </summary>
    public partial class SurroundStationListControl : UserControl
    {
        public event Action<ActivitySurroundStation> StationSelectionChanged;
        private CheckBox checkBoxAll;

        private ActivitySurroundStation SelectedActivitySurroundStation
        {
            get { return stationdatagrid.SelectedItem as ActivitySurroundStation; }
        }

        public ObservableCollection<ActivitySurroundStation> SurroundStationItemsSource
        {
            get { return this.DataContext as ObservableCollection<ActivitySurroundStation>; }
        }

        public IEnumerable<ActivitySurroundStation> SelectedSurroundStation
        {
            get
            {
                return SurroundStationItemsSource.Where(r => r.IsChecked == true);
            }
        }

        public SurroundStationListControl()
        {
            InitializeComponent();
            this.DataContextChanged += SurroundStationListControl_DataContextChanged;
        }

        private void SurroundStationListControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // this.stationdatagrid.ItemsSource = this.DataContext as ObservableCollection<ActivitySurroundStation>;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stationdatagrid_LayoutUpdated(object sender, EventArgs e)
        {
            this.stationdatagrid.RowHeight = Double.NaN;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stationdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedActivitySurroundStation != null)
            {
                StationDetailDialog dialog = new StationDetailDialog(SelectedActivitySurroundStation.STATGUID);
                dialog.ShowDialog(this);
            }
        }

        private void stationdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedActivitySurroundStation != null)
            {
                if (StationSelectionChanged != null)
                {
                    StationSelectionChanged(SelectedActivitySurroundStation);
                }
            }
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxAll = sender as CheckBox;
        }

        public void UnChecked()
        {
            checkBoxAll.IsChecked = false;
        }
    }
}
