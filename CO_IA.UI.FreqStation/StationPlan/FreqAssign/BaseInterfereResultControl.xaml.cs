using CO_IA.Data;
using CO_IA.UI.PlanDatabase.Equipments;
using CO_IA.UI.StationManage;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// BaseInterfereResultControl.xaml 的交互逻辑
    /// </summary>
    public partial class BaseInterfereResultControl : UserControl
    {

        private List<InterfereResult> interfItemsSource = new List<InterfereResult>();

        public List<InterfereResult> InterfItemsSource
        {
            get
            {
                return interfItemsSource;
            }
            set
            {
                interfItemsSource = value;
            }
        }


        public BaseInterfereResultControl()
        {
            InitializeComponent();
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
                        ActivityEquipment equipment =
                           PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqStation.I_CO_IA_FreqStation, ActivityEquipment>(channel =>
                           {
                               return channel.GetActivityEquipment(interfobj.Guid);
                           });

                        EquipmentManageDialog equdialog = new EquipmentManageDialog();
                        equdialog.DataContext = equipment;
                        equdialog.AllowEdit = false;
                        equdialog.ShowDialog();
                    }
                    else if (interfobj.Type == InterfereObjectEnum.周围台站)
                    {
                        StationDetailDialog dialog = new StationDetailDialog(interfobj.Guid);
                        dialog.ShowDialog(this);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
