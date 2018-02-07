using CO_IA.Data;
using CO_IA.UI.FreqPlan;
using CO_IA.UI.StationManage;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// BaseInterfereResultControl.xaml 的交互逻辑
    /// </summary>
    public partial class BaseInterfereResultControl : UserControl
    {
        public BaseInterfereResultControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public List<InterfereResult> InterfItemsSource
        {
            get { return (List<InterfereResult>)GetValue(InterfItemsSourceProperty); }
            set { SetValue(InterfItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty InterfItemsSourceProperty =
            DependencyProperty.Register("InterfItemsSource", typeof(List<InterfereResult>), typeof(BaseInterfereResultControl), null);

        private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                DataGrid datagrid = sender as DataGrid;
                InterfereObject interfobj = datagrid.SelectedItem as InterfereObject;
                if (interfobj != null)
                {
                    if (interfobj.Type == InterfereObjectEnum.设备)
                    {
                        ActivityEquipmentInfo equ = new ActivityEquipmentInfo();
                        equ.GUID = interfobj.Guid;
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                        {
                            equ = channel.GetEquipmentInfo(equ);
                            if (equ != null)
                            {
                                EquipmentDetailDialog dialog = new EquipmentDetailDialog(equ);
                                dialog.IsEnabled = false;
                                dialog.WindowTitle = "设备-详细信息";
                                dialog.ShowDialog(this);
                            }
                        });
                    }
                    else if (interfobj.Type == InterfereObjectEnum.周围台站)
                    {
                        StationDetailDialog dialog = new StationDetailDialog(interfobj.Guid);
                        dialog.ShowDialog(this);
                    }
                }
            }
        }

        private void InterferedResult_LayoutUpdated(object sender, System.EventArgs e)
        {
            InterferedResult.RowHeight = double.NaN;
        }

        private void interfObjectdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                InterfereObject interfobj = (sender as DataGridRow).Item as InterfereObject;
                if (interfobj != null)
                {
                    if (interfobj.Type == InterfereObjectEnum.设备)
                    {
                        ActivityEquipmentInfo equ = new ActivityEquipmentInfo();
                        equ.GUID = interfobj.Guid;
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
                        {
                            equ = channel.GetEquipmentInfo(equ);
                            if (equ != null)
                            {
                                EquipmentDetailDialog dialog = new EquipmentDetailDialog(equ);
                                dialog.IsEnabled = false;
                                dialog.WindowTitle = "设备-详细信息";
                                dialog.ShowDialog(this);
                            }
                        });
                    }
                    else if (interfobj.Type == InterfereObjectEnum.周围台站)
                    {
                        StationDetailDialog dialog = new StationDetailDialog(interfobj.Guid);
                        dialog.ShowDialog(this);
                    }
                }
            }
        }
    }
}
