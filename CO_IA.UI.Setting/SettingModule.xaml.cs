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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CO_IA.UI.Setting
{
    /// <summary>
    /// SettingManage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingModule : UserControl
    {
        //EquipmentManageModule equipment;
        public SettingModule()
        {
            InitializeComponent();
            listBoxMenu.SelectedIndex = 0;
        }

        private void listBoxMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mainGrid.Children.Clear();
            ListBoxItem item = listBoxMenu.SelectedItem as ListBoxItem;
            switch (item.Name)
            {
                //保障类别
                case "itemSecurity":
                    SecurityClassManageModule security = new SecurityClassManageModule();
                    _mainGrid.Children.Add(security);
                    break;

                //监测设备类别
                case "itemmonstationtype":
                    MonitorEQUTypeModule type = new MonitorEQUTypeModule();
                    _mainGrid.Children.Add(type);
                    break;
            }
        }

        private void itemMonitorEquType_Selected(object sender, RoutedEventArgs e)
        {
            _mainGrid.Children.Clear();
            MonitorEQUTypeModule type = new MonitorEQUTypeModule();
            _mainGrid.Children.Add(type);
        }

        private void itemSecurityClass_Selected(object sender, RoutedEventArgs e)
        {
            _mainGrid.Children.Clear();
            SecurityClassManageModule security = new SecurityClassManageModule();
            _mainGrid.Children.Add(security);
        }

        private void itemEquipmentClassFreqPlanning_Selected(object sender, RoutedEventArgs e)
        {
            _mainGrid.Children.Clear();
            EquipmentClassFreqPlanningControl control = new EquipmentClassFreqPlanningControl();
            control.DataContext = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting,EquipmentClassFreqRange[]>(channel =>
                {
                    return channel.GetEquipmentClassFreqRanges();
                });
            _mainGrid.Children.Add(control);
        }

        private void itemGroupDuty_Selected(object sender, RoutedEventArgs e)
        {
            _mainGrid.Children.Clear();
            DutyControl control = new DutyControl();
            control.DataContext = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting, PP_DutyInfo[]>(channel =>
            {
                return channel.GetDutyInfos();
            });
            _mainGrid.Children.Add(control);
        }
    }
}
