using CO_IA.Data;
using CO_IA.UI.PlanDatabase.Vehicle;
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
using System.Windows.Shapes;

namespace CO_IA.UI.PlanDatabase.Template
{
    /// <summary>
    /// VehicleSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleSettingWindow : Window
    {
        private VehicleQueryCondition queryCondition = new VehicleQueryCondition();
        private IList<string> ignoreVehicleList;
        public VehicleSettingWindow(IList<string> ignoreVehicleList)
        {
            InitializeComponent();
            this.ignoreVehicleList = ignoreVehicleList;
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                IEnumerable<VehicleInfo> vehicles=CO_IA.Client.Utility.GetVehicleInfos(queryCondition);
                if (this.ignoreVehicleList!=null)
                {
                    vehicles=from vehicle in vehicles where !this.ignoreVehicleList.Contains(vehicle.GUID) select vehicle;
                }
                this.dataGridVehicle.ItemsSource = vehicles;
            }
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
        }

        private void dataGridVehicle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    VehicleInfo vehicle = dgr.DataContext as VehicleInfo;
                    if (vehicle != null)
                    {
                        VehicleEditDialog dialog = new VehicleEditDialog(vehicle,false);
                        dialog.Title = "车辆信息";
                        dialog.ShowDialog(this);
                    }
                }
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.GetSelectedVehicle() != null)
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("没有选择车辆");
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public VehicleInfo GetSelectedVehicle()
        {
            return this.dataGridVehicle.SelectedValue as VehicleInfo;
        }
    }
}
