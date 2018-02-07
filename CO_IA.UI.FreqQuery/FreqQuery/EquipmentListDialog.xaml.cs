using CO_IA.Data;
using CO_IA.UI.FreqPlan;
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

namespace CO_IA.UI.FreqQuery
{
    /// <summary>
    /// EquipmentListDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentListDialog : Window
    {
        public EquipmentListDialog(string placeid, string businesstype)
        {
            InitializeComponent();
            QueryActivityEquipments(placeid, businesstype);
        }
        public EquipmentListDialog(List<ActivityEquipmentInfo> p_equipmentList)
        {
            InitializeComponent();
            SetEquipGridItemsSource(p_equipmentList);
        }
        private void QueryActivityEquipments(string placeid, string businesstype)
        {
            dg_equiplist.ItemsSource = null;
            EquipmentQueryCondition condition = new EquipmentQueryCondition();
            condition.ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
            condition.PlaceGuid = placeid;
            condition.Businesstype = businesstype;

            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.FreqPlan.I_CO_IA_FreqPlan>(channel =>
            {
                SetEquipGridItemsSource(channel.GetEquipmentInfos(condition));
            });
        }
        private void SetEquipGridItemsSource(List<ActivityEquipmentInfo> p_equipmentList)
        {
            dg_equiplist.ItemsSource = p_equipmentList;
        }
        private void equdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dg_equiplist.SelectedItem != null)
            {
                ActivityEquipmentInfo SelectedEquipment = dg_equiplist.SelectedItem as ActivityEquipmentInfo;
                EquipmentDetailDialog dialog = new EquipmentDetailDialog(SelectedEquipment);
                dialog.IsEnabled = false;
                dialog.WindowTitle = "设备-详细信息";
                dialog.ShowDialog(this);
            }
        }
    }
}
