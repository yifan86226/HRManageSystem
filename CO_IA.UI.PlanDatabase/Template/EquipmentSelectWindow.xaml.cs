using CO_IA.Data;
using CO_IA.UI.PlanDatabase.MonitorEquipment;
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
    /// MonitorEquipmentSelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentSelectWindow : Window
    {
        private IList<string> ignoreEquipmentGuidList;
        public EquipmentSelectWindow(IList<string> ignoreEquipmentGuidList)
        {
            InitializeComponent();
            this.ignoreEquipmentGuidList = ignoreEquipmentGuidList;
            this.equipmentListControl.DoubleClick += equipmentListControl_DoubleClick;
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                IEnumerable<Equipment> equipments=
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_PlanDatabase,Equipment[]>(channel =>
                {
                    return channel.GetEquipments(new Data.EquipmentLoadStrategy());
                });
                if (ignoreEquipmentGuidList != null)
                {
                    var result = from data in equipments where !this.ignoreEquipmentGuidList.Contains(data.Key) select data;
                    equipments = new System.Collections.ObjectModel.ObservableCollection<Equipment>(result);
                }
                this.equipmentListControl.DataContext = equipments;
            }
        }

        private void equipmentListControl_DoubleClick(Equipment obj)
        {
            Window wnd = new Window();
            var content = new CO_IA.UI.PlanDatabase.Equipments.EquipmentEditControl();
            wnd.Content = content;
            wnd.DataContext = obj;
            wnd.Loaded += (sender, arg) =>
                {
                    content.IsReadOnly = true;
                };
            wnd.ShowDialog();
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //var selectedEquipment = this.monitorEquipmentListControl.EquipmentMultiSelected;
            //if (selectedEquipment != null && selectedEquipment.Count>0)
            //{
            //    this.DialogResult = true;
            //}
            //else
            //{
            //    MessageBox.Show("没有选择设备");
            //}
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public List<Equipment> GetSelectedEquipmentList()
        {
            var listEquipment = this.equipmentListControl.DataContext as IList<Equipment>;
            if (listEquipment != null)
            {
                var checkedEquipments = from equipment in listEquipment where equipment.IsChecked select equipment;
                return checkedEquipments.ToList();
            }
            return new List<Equipment>();
        }
    }
}
