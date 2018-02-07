using CO_IA.UI.PlanDatabase;
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

namespace CO_IA.UI.PlanDatabase
{
    /// <summary>
    /// SettingManage.xaml 的交互逻辑
    /// </summary>
    public partial class PlanDatabaseModule : UserControl
    {
        //EquipmentManageModule equipment;
        public PlanDatabaseModule()
        {
            InitializeComponent();
            listBoxMenu.SelectedIndex = 0;
        }

        private void listBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_mainGrid.Children.Clear();
            //ListBoxItem item = listBoxMenu.SelectedItem as ListBoxItem;
            //switch (item.Name)
            //{
            //    //单位类别
            //    case "itemORG":
            //        EquipmentManageModule equipment = new EquipmentManageModule();
            //        _mainGrid.Children.Add(equipment);
            //        break;
            //    //监测设施
            //    case "itemMonitor":
            //        CO_IA.UI.PlanDatabase.MonitorEquipment.MonitorStationManageModule monitor = new CO_IA.UI.PlanDatabase.MonitorEquipment.MonitorStationManageModule();
            //        _mainGrid.Children.Add(monitor);
            //        break;
            //    case "itemVehicle":
            //        CO_IA.UI.PlanDatabase.Vehicle.VehicleManageModule vehicle = new CO_IA.UI.PlanDatabase.Vehicle.VehicleManageModule();
            //        _mainGrid.Children.Add(vehicle);
            //        break;
            //    //考点管理
            //    case "itemExamPlace":
            //        CO_IA.UI.PlanDatabase.Location.ExamSiteManageModule examSite = new CO_IA.UI.PlanDatabase.Location.ExamSiteManageModule();
            //        _mainGrid.Children.Add(examSite);
            //        break;
            //}
        }
    }
}
