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
using CO_IA.Data;
using PT_BS_Service.Client.Framework;
using I_CO_IA.PlanDatabase;
using CO_IA.Scene.FreqPlan;
using CO_IA.UI.PlanDatabase.Equipments;
using I_CO_IA.FreqStation;
using Microsoft.Win32;
using System.Data;
using CO_IA.UI.PlanDatabase;
using AT_BC.Data;
using CO_IA.UI.FreqStation.FreqPlan;

namespace CO_IA.Scene.CommonCtr
{
    /// <summary>
    /// EquipmentsManageCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentsManageCtrl : UserControl
    {
        public CO_IA.Data.Equipment[] Equipments { get; set; }
        
        //public Visibility ToolBarDisplay 
        //{ 
        //    set
        //    {
        //        _toolBar.Visibility = value;
        //    }
        //}
        public EquipmentsManageCtrl()
        {
            InitializeComponent();
            xDataGrid.ItemsSource = DataOperator.GetORGSource(new OrgQueryCondition() { ActivityGuid = SystemLoginService.CurrentActivity.Guid });
        }
        private bool _isReadOnly = false;
        public bool IsReadOnly 
        { 
            get
            {
                return _isReadOnly;
            }
            set
            {
                this._isReadOnly = value;
                if(value == false)
                {
                    _toolBar.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    _toolBar.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void AddOrg_Click(object sender, RoutedEventArgs e)
        {
            OrgInputDialog orgmanage = new OrgInputDialog();
            orgmanage.AfterSave += (newOrg) =>
            {
                ReLoadOrgGridSource(newOrg);
            };
            orgmanage.ShowDialog();
        }
       
        private void DelOrg_Click(object sender, RoutedEventArgs e)
        {
            ActivityOrganization org = xDataGrid.SelectedItem as ActivityOrganization;
            if(org == null)
            {
                MessageBox.Show("请先选中要删除的单位信息，然后再次点击删除按钮","操作提示");
                return;
            }
            else if(Equipments.Length >0)
            {
                MessageBox.Show(string.Format("当前活动下,{0}已经包含设备,请先删除设备后再执行删除该单位的操作。", org.Name), "操作提示");
                return;
            }
            try
            {
                DataOperator.DeleteOrgByGuid(org.Guid);
                Organization[] orgs = xDataGrid.ItemsSource as Organization[];
                
                xDataGrid.ItemsSource = null;
                xDataGrid.ItemsSource = orgs.Where(p => p.Guid != org.Guid).ToArray();  
                MessageBox.Show("删除成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage(), "删除失败");
            }
        }

        private void AddEquip_Click(object sender, RoutedEventArgs e)
        {
            DataGrid equipDataGrid = (sender as Button).Tag as DataGrid;
            //equipDataGrid.ItemsSource = null;
            //操作xTestDataGrid就行。
            ActivityOrganization org = xDataGrid.SelectedItem as ActivityOrganization;
            ActivityEquipment newequ = new ActivityEquipment();
            newequ.OrgInfo.Guid = org.Guid;
            newequ.OrgInfo.Name = org.Name;
            newequ.Key = System.Guid.NewGuid().ToString();
            newequ.EQUCount = 1;
            ShowEquipDetail(newequ);

        }
      
        private void DelEquip_Click(object sender, RoutedEventArgs e)
        {
            ActivityEquipment equip = (sender as Button).Tag as ActivityEquipment;
            if (equip == null)
                return;
            try
            {
                DataOperator.DeleteEquipments(equip.Key);
                LoadEquipmentsForOrg();
                MessageBox.Show("删除成功");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"删除失败");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as DataGrid).SelectedItem;
            if (item == null)
                return;
            LoadEquipmentsForOrg();

            if(item is ActivityOrganization)
            {
                EquipmentLoadStrategy strategy = new EquipmentLoadStrategy();
                strategy.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
                strategy.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid;
                strategy.OrgName = (item as ActivityOrganization).Name;
                if (DataOperator.GetEquipments(strategy).Length > 0)
                {
                    xDeleteButton.IsEnabled = false;
                }
                else
                {
                    xDeleteButton.IsEnabled = true;
                }
            }
           
        }
        private void LoadEquipmentsForOrg()
        {
            var item = xDataGrid.SelectedItem;  
            Organization organization = item as Organization;
            //EquipmentLoadStrategy eququerycondition = new EquipmentLoadStrategy();
            //eququerycondition.OrgName = organization.Name;
            //eququerycondition.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
            //eququerycondition.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid;
            //Equipments = DataOperator.GetActivityEquipments(eququerycondition);

            EquInspectionQueryCondition condition = new EquInspectionQueryCondition();
            condition.CheckState = (new CheckStateEnum[] { CheckStateEnum.Qualified }).ToList();
            condition.ORGName = organization.Name;
            condition.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
            condition.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid;
            Equipments = DataOperator.GetEquipmentInspections(condition).Select(p => p.ActivityEquipment).ToArray();
            xTestDataGrid.ItemsSource = Equipments;
            this.UpdateLayout();  
        }

        private void xEquipDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as DataGrid).Width = xDataGrid.ActualWidth - 25;
        }

        private void xEquipDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }
        /// <summary>
        /// 设备库导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EquipLibraryImportBtn_Click(object sender, RoutedEventArgs e)
        {
            EquipmentSelectorDialog equselectordialog = new EquipmentSelectorDialog();
            equselectordialog.OnConfirmEvent += ImportFromRiasEvent;
            equselectordialog.ShowDialog();
        }
        private void ImportFromRiasEvent(Organization arg1, List<Equipment> arg2)
        {
            ActivityOrganization importorg = new ActivityOrganization();
            importorg.CopyFrom(arg1);
            importorg.ActivityGuid = SystemLoginService.CurrentActivity.Guid;

            if (arg2 != null && arg2.Count > 0)
            {
                List<ActivityEquipment> actequs = new List<ActivityEquipment>();
                foreach (Equipment item in arg2)
                {
                    ActivityEquipment equ = new ActivityEquipment();
                    equ.CopyFrom(item);
                    equ.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
                    equ.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid;
                    equ.OrgInfo.Guid = importorg.Guid;
                    actequs.Add(equ);
                }

                this.ImportActivityEquipmentEvent(importorg, actequs);
            }
            else
            {
                MessageBox.Show("请选择要导入的设备");
            }
        }

        private void ImportActivityEquipmentEvent(ActivityOrganization orginfo, List<ActivityEquipment> lstequ)
        {
            try
            {
                BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(channel =>
                {
                    channel.ImportActivityEquipment(orginfo, lstequ);
                    ReLoadOrgGridSource(orginfo);
                    MessageBox.Show("导入成功！", "提示", MessageBoxButton.OK);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetExceptionMessage(), "导入失败");
            }
        }
        /// <summary>
        /// 重新加载单位Grid的数据源
        /// </summary>
        /// <param name="p_newOrganization"></param>
        private void ReLoadOrgGridSource(Organization p_newOrganization)
        {
            Organization[] orgs = xDataGrid.ItemsSource as Organization[];
            Organization[] neworgs = orgs.Concat(new Organization[] { p_newOrganization }).ToArray();
            xDataGrid.ItemsSource = neworgs;
        }

        private void ExcelImportBtn_Click(object sender, RoutedEventArgs e)
        {
            ORGAndEquipmentManage ctrl = new ORGAndEquipmentManage(SystemLoginService.CurrentActivityPlace);
            ctrl.Import();

            xDataGrid.ItemsSource = DataOperator.GetORGSource(new OrgQueryCondition() { ActivityGuid = SystemLoginService.CurrentActivity.Guid });
            //ExcelOperator.Import(ImportActivityEquipmentEvent); 
        }

        private void EquipmentGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
               DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    ActivityEquipment equipment = dgr.DataContext as ActivityEquipment;
                    ShowEquipDetail(equipment);
                }
            }
        }
        /// <summary>
        /// 打开设备详细信息
        /// </summary>
        /// <param name="equipment"></param>
        private void ShowEquipDetail(ActivityEquipment equipment)
        {
            if (equipment == null)
                return;
            EquipmentManageDialog addequdialog = new EquipmentManageDialog();
            addequdialog.AllowEdit = !IsReadOnly;
            addequdialog.DataContext = equipment;
            addequdialog.OnSaveEvent += (equ) =>
            {
                ActivityEquipment actequ = new ActivityEquipment();
                actequ.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
                actequ.PlaceGuid = SystemLoginService.CurrentActivityPlace.Guid;
                actequ.CopyFrom(equ); 
                return SaveEquipment(actequ);
            };
            addequdialog.ShowDialog();
        }
        /// <summary>
        /// 保存设备信息
        /// </summary>
        /// <param name="p_equipment"></param>
        /// <returns></returns>
        private bool SaveEquipment(ActivityEquipment p_equipment)
        {
            try
            {
                DataOperator.SaveActivityEquipment(p_equipment);
                LoadEquipmentsForOrg();
                MessageBox.Show("保存成功");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "保存失败！");
                return false;
            }
        }

    }
}
