using CO_IA.Data;
using I_CO_IA.PlanDatabase;
using PT_BS_Service.Client.Framework;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Linq;

namespace CO_IA.UI.PlanDatabase.Equipments
{
    /// <summary>
    ///设备选择窗体
    /// </summary>
    public partial class EquipmentSelectorDialog : Window
    {
        public event Action<Organization,List<Equipment>> OnConfirmEvent;

        public EquipmentSelectorDialog()
        {
            InitializeComponent();
            GetORGItemsSource();
            orgListControl.SelectionChanged += orgListControl_SelectionChanged;
        }

        private void GetORGItemsSource()
        {
            Organization[] source = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase, Organization[]>(channel =>
            {
                return channel.GetOrgByCondition(new OrgQueryCondition());
            });

            orgListControl.DataContext = source;
            if (source == null || source.Length == 0)
            {
                equListControl.DataContext = null;
            }
        }

        private void orgListControl_SelectionChanged(Data.Organization obj)
        {
            if (obj != null)
            {
                EquipmentLoadStrategy eququerycondition = new EquipmentLoadStrategy();
                eququerycondition.OrgName = obj.Name;

                BeOperationInvoker.Invoke<I_CO_IA_PlanDatabase>(channel =>
                {
                    equListControl.DataContext = channel.GetEquipmentsForOrg(obj.Guid);
                });
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            if (OnConfirmEvent != null)
            {
                List<Equipment> equs = equListControl.EquipmentItemsSource.Where(r => r.IsChecked == true).ToList();
                OnConfirmEvent(orgListControl.SelectedORG,equs);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
