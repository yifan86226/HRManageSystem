using CO_IA.Client;
using CO_IA.Data;
using CO_IA.UI.PlanDatabase.MonitorEquipment;
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

namespace CO_IA.UI.Screen.Dialog
{
    /// <summary>
    /// 固定监测站详细信息
    /// </summary>
    public partial class FixedStationDetailDialog : Window
    {
        /// <summary>
        /// 监测站信息
        /// </summary>
        private ObservableCollection<FixedStationInfo> monitorStationItemSource;

        /// <summary>
        /// 设备信息
        /// </summary>
        private ObservableCollection<MonitorStationEquInfo> equipmentItemSource;

        /// <summary>
        /// 天线信息
        /// </summary>
        private ObservableCollection<MonitorStationAntInfo> antennaItemSource;

        private FixedStationInfo SelectedMonitorStation;

        public FixedStationDetailDialog(FixedStationInfo stationInfo)
        {
            InitializeComponent();
            if (stationInfo != null)
            {
                monitorStationItemSource = new ObservableCollection<FixedStationInfo>() { stationInfo };
            }
            else
            {
                FixedStationQueryCondition queryCondition = new FixedStationQueryCondition();
                monitorStationItemSource = MonitorStationHelper.QueryMonitorStationItemSource(queryCondition);
            }
            monitorstationdatagrid.ItemsSource = monitorStationItemSource;
            if (monitorStationItemSource.Count>0)
                this.monitorstationdatagrid.SelectedIndex = 0;

            
            this.equipmentListControl.DoubleClickEvent += equipmentListControl_DoubleClickEvent;
            this.antennaListControl.DoubleClickEvent += antennaListControl_DoubleClickEvent;
            monitorStationManageControl.IsReadOnly = true;
            InitData();
        }

        private void InitData()
        {
            
            equipmentItemSource = MonitorStationHelper.QueryEquipmentItemSource(StatModeEnum.Fixed);
            antennaItemSource = MonitorStationHelper.QueryAntennaItemSource(StatModeEnum.Fixed);

            monitorStationManageControl_MonitorStationSelectionChanged(SelectedMonitorStation);
        }

        /// <summary>
        ///监测站切换行 
        /// </summary>
        /// <param name="obj"></param>
        private void monitorStationManageControl_MonitorStationSelectionChanged(FixedStationInfo obj)
        {
            if (obj != null)
            {
                if (equipmentItemSource != null)
                {
                    ObservableCollection<MonitorStationEquInfo> items = new ObservableCollection<MonitorStationEquInfo>(equipmentItemSource.Where(r => r.MonitorStationID == obj.Guid).ToList()) { };
                    equipmentListControl.EquipmentItemsSource = items;
                }
                else
                {
                    equipmentListControl.EquipmentItemsSource = null;
                }
                if (antennaItemSource != null)
                {
                    ObservableCollection<MonitorStationAntInfo> antitems = new ObservableCollection<MonitorStationAntInfo>(antennaItemSource.Where(r => r.MonitorStationID == obj.Guid).ToList()) { };
                    antennaListControl.AntennaItemsSource = antitems;
                }
                else
                {
                    antennaListControl.AntennaItemsSource = null;
                }
            }
            else
            {
                equipmentListControl.EquipmentItemsSource = null;
                antennaListControl.AntennaItemsSource = null;
            }
        }

        #region 设备相关操作

        
        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="obj"></param>
        private void equipmentListControl_DoubleClickEvent(MonitorStationEquInfo obj)
        {
            if (SelectedMonitorStation == null)
            {
                MessageBox.Show("请选择要查看的设备监测站信息!");
                return;
            }
            if (equipmentListControl.EquipmentSelected == null)
            {
                MessageBox.Show("请选择要查看的设备信息!");
                return;
            }
            MonitorStationEquDialog dialog = new MonitorStationEquDialog(equipmentListControl.EquipmentSelected);
            dialog.Title = "查看设备信息";
            dialog.IsShowDetail = true;
            
            dialog.ShowDialog(this);
        }
           
        #endregion

        #region 天线相关操作

       
        /// <summary>
        /// 修改天线信息
        /// </summary>
        /// <param name="obj"></param>
        private void antennaListControl_DoubleClickEvent(MonitorStationAntInfo obj)
        {
            if (SelectedMonitorStation == null)
            {
                MessageBox.Show("请选择要查看天线的监测站信息!");
                return;
            }
            if (antennaListControl.AntennaSelected == null)
            {
                MessageBox.Show("请选择要查看的天线信息!");
                return;
            }
            MonitorStationAntDialog dialog = new MonitorStationAntDialog(antennaListControl.AntennaSelected);
            dialog.Title = "查看天线信息";
            dialog.IsShowDetail = true;
            
            dialog.ShowDialog(this);
        }

       
        #endregion

        private void monitorstationdatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedMonitorStation = monitorstationdatagrid.SelectedItem as FixedStationInfo;
            this.monitorStationManageControl.DataContext = SelectedMonitorStation;
            monitorStationManageControl_MonitorStationSelectionChanged(SelectedMonitorStation);
        }

        private void monitorstationdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FixedStationInfo stationInfo = monitorstationdatagrid.SelectedItem as FixedStationInfo;
            if (stationInfo != null)
            {
                if (Obj.screenMap.CheckCoordinate(new double[] { stationInfo.LONG.Value, stationInfo.LAT.Value }))
                {
                    Obj.screenMap.setExtent(new AT_BC.Data.GeoPoint() { Longitude = stationInfo.LONG.Value, Latitude = stationInfo.LAT.Value });
                }
            }
        }

    }
}
