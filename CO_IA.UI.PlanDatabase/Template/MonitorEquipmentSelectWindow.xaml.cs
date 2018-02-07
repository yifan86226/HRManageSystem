using CO_IA.Data;
using CO_IA.UI.PlanDatabase.MonitorEquipment;
using I_CO_IA.Setting;
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
using System.Windows.Shapes;

namespace CO_IA.UI.PlanDatabase.Template
{
    /// <summary>
    /// MonitorEquipmentSelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorEquipmentSelectWindow : Window
    {
        private IList<string> ignoreEquipmentGuidList;
        private MobileStationQueryCondition queryCondition;
        private ObservableCollection<MonitorStationEquInfo> DataSource;
        public MonitorEquipmentSelectWindow(IList<string> ignoreEquipmentGuidList)
        {
            InitializeComponent();
            IniCondition();
            this.ignoreEquipmentGuidList = ignoreEquipmentGuidList;
            this.monitorEquipmentListControl.DoubleClickEvent += monitorEquipmentListControl_DoubleClickEvent;
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                var datas = MonitorStationHelper.QueryEquipmentItemSource(Data.StatModeEnum.Portable);
                if (ignoreEquipmentGuidList != null)
                {
                    var result = from data in datas where !this.ignoreEquipmentGuidList.Contains(data.ID) select data;
                    datas = new ObservableCollection<MonitorStationEquInfo>(result);
                }
                DataSource = datas;
                this.monitorEquipmentListControl.EquipmentItemsSource = datas;
            }
        }
        
        private void monitorEquipmentListControl_DoubleClickEvent(Data.MonitorStationEquInfo obj)
        {
            if (obj == null)
            {
                return;
            }
            
            MonitorStationEquDialog dialog = new MonitorStationEquDialog(obj);
            dialog.Title = "设备信息";
            dialog.IsShowDetail = true;
            dialog.ShowDialog(this);
        }
       

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            var selectedEquipment = this.monitorEquipmentListControl.EquipmentMultiSelected;
            if (selectedEquipment != null && selectedEquipment.Count>0)
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("没有选择设备");
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public List<MonitorStationEquInfo> GetSelectedEquipmentList()
        {
            return this.monitorEquipmentListControl.EquipmentMultiSelected;
        }

        
        #region 筛选
        private void IniCondition()
        {
            cbarea.ItemsSource = CO_IA.Client.Utility.GetProvinceAreaCode();
            cbtype.ItemsSource = GetEquType();
            queryCondition = new MobileStationQueryCondition();
        }
        private List<string> GetEquType()
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Setting, List<string>>(channel =>
            {
                return channel.GetMonitorEQUType();
            });
        }
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            queryCondition.Name = txtName.Text.Trim();
            if (cbarea.SelectedItems != null && cbarea.SelectedItems.Count > 0)
            {
                List<string> newItem = new List<string>();
                foreach (var item in cbarea.SelectedItems)
                {
                    KeyValuePair<string, string> values = (KeyValuePair<string, string>)item;
                    newItem.Add(values.Key);
                }
                queryCondition.AreaCodes = newItem;
            }


            if (cbtype.SelectedItems != null && cbtype.SelectedItems.Count > 0)
            {
                List<string> newItem = new List<string>();
                foreach (var item in cbtype.SelectedItems)
                {
                    newItem.Add(item.ToString());
                }
                queryCondition.Types = newItem;
            }

            queryCondition_PropertyChanged(null, null);
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = "";
            cbarea.SelectedItems.Clear();
            cbtype.SelectedItems.Clear();
            queryCondition.Name = "";
            queryCondition.AreaCodes = null;
            queryCondition.Types = null;
            queryCondition.Address = "";
            queryCondition_PropertyChanged(null, null);
        }
        void queryCondition_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (DataSource != null && DataSource.Count > 0)
            {
                var result = DataSource.Where(item =>
                {
                    if (!string.IsNullOrEmpty(queryCondition.Name) && item.Name.IndexOf(queryCondition.Name) == -1)
                        return false;
                    if (queryCondition.AreaCodes != null && queryCondition.AreaCodes.Count > 0 && !queryCondition.AreaCodes.Contains(item.AreaCode))
                        return false;
                    if (queryCondition.Types != null && queryCondition.Types.Count > 0 && !queryCondition.Types.Contains(item.Type))
                        return false;
                    return true;
                }).ToList();
                ObservableCollection<MonitorStationEquInfo> source = new ObservableCollection<MonitorStationEquInfo>(result);
                monitorEquipmentListControl.EquipmentItemsSource = source;
            }
        }
        #endregion

    }
   
}
