#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：车辆选择弹出窗口
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using CO_IA.Data;
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

namespace CO_IA.UI.PersonSchedule
{

    /// <summary>
    /// VehicleSelectDialog.xaml 的交互逻辑
    /// </summary>
    public partial class VehicleSelectDialog : Window
    {

        private CheckBox chkAll;
        List<string> exitVehicle;
        public List<VehicleInfo> VehicleItemsSource
        {
            get { return cardatagrid.ItemsSource as List<VehicleInfo>; }
            set { cardatagrid.ItemsSource = value; }
        }

        public VehicleInfo VehicleSelected
        {
            get
            {
                if (cardatagrid.SelectedItem == null)
                {
                    return null;
                }
                else
                {
                    return cardatagrid.SelectedItem as VehicleInfo;
                }
            }
            set
            {
                cardatagrid.SelectedItem = value;
            }
        }


        public VehicleSelectDialog(List<string> exitvehicle)
        {
            InitializeComponent();
            exitVehicle = exitvehicle;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.DataContext = this;
            this.GetVehicleInfos();

        }



        private void cardatagrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item != null)
            {
                VehicleInfo item = e.Row.Item as VehicleInfo;
                //if (item.VehicleUse.Value == "监测车" || item.VehicleUse.Value == "监测")
                if (item.IsMonitor)
                {
                    BrushConverter brushConverter = new BrushConverter();
                    e.Row.Background = (Brush)brushConverter.ConvertFromString("#FFC7F9E5");
                }
            }
        }

        private void cardatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //VehicleEditDialog dialog = new VehicleEditDialog(VehicleSelected);
            //dialog.Title = "车辆-详细信息";
            //dialog.IsEnabled = false;
            //dialog.ShowDialog();
            this.DialogResult = true;
            this.Close();

        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void GetVehicleInfos()
        {
            VehicleInfo[] source = CO_IA.Client.Utility.GetVehicleInfos(new VehicleQueryCondition());
            if (exitVehicle != null && exitVehicle.Count > 0)
            {
                source = source.Where(item =>
                    {
                        if (string.IsNullOrEmpty(item.VehicleNo))
                            return true;
                        foreach (string s in exitVehicle)
                        {
                            if (s == item.VehicleNo)
                                return false;
                        }
                            return true;
                    }).ToArray();
            }
            List<VehicleInfo> monitorsource = source.Where(r => r.IsMonitor).ToList();
            this.VehicleItemsSource = new List<VehicleInfo>();
            VehicleItemsSource.AddRange(monitorsource);
            VehicleItemsSource.AddRange(source.Except(monitorsource));
        }

        #region 全选

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            bool ischecked = chk.IsChecked.Value;

            foreach (VehicleInfo item in VehicleItemsSource)
            {
                item.IsChecked = ischecked;
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.VehicleItemsSource != null)
            {
                chkAll.IsChecked = VehicleItemsSource.Any(item => item.IsChecked);
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

            foreach (VehicleInfo result in VehicleItemsSource)
            {
                if (result.IsChecked != checkedState)
                {
                    this.chkAll.IsChecked = null;
                    return;
                }
            }
            chkAll.IsChecked = checkedState;
        }

        #endregion


    }
}
