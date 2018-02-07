using AT_BC.Common;
using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Data.Template;
using CO_IA.UI.PlanDatabase.MonitorEquipment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace CO_IA.UI.PlanDatabase.Template
{
    /// <summary>
    /// OrganizationTemplate.xaml 的交互逻辑
    /// </summary>
    public partial class OrganizationTemplateControl : UserControl
    {
        public OrganizationTemplateControl()
        {
            InitializeComponent();

            //List<PP_Duty> list = new List<PP_Duty>();
            //for (int i = 0; i < 30; i++)
            //{
            //    list.Add(new PP_Duty { Key = i.ToString(), Name = string.Format("第{0}个职责", i) });
            //}
            //CheckableData<string>.CheckItems(list, new string[] { "0", "1", "3", "10" });
            //listBoxDuty.ItemsSource = list;
            //this.Loaded += OrganizationTemplate_Loaded;
            this.DataContextChanged += OrganizationTemplate_DataContextChanged;
            this.organizationEditControl.GetExistOrganizationTemplateList = () =>
                {
                    List<OrganizationTemplate> list = new List<OrganizationTemplate>();
                    var treeNodeList = this.treeViewOrg.ItemsSource as IList<TreeNode<OrganizationTemplate>>;
                    foreach (TreeNode<OrganizationTemplate> treeNode in treeNodeList)
                    {
                        list.AddRange(treeNode.GetNodeDataList());
                    }
                    return list;
                };
        }

        private void OrganizationTemplate_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //if (!this.isLoaded)
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
            //    {
            //        //更新当前节点
            //        listBoxDuty.ItemsSource = channel.Get_DutyList();
            //    });
            //    this.isLoaded = true;
            //}
            this.Reload();
            //加载人员和监测设备
        }

        private bool isLoaded;
        void OrganizationTemplate_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private bool isChanged = false;

        private void NotifyOrganizationChanged(object sender, PropertyChangedEventArgs e)
        {
            this.isChanged = true;
        }

        private void buttonPersonAdd_Click(object sender, RoutedEventArgs e)
        {
            var orgTemplate = this.gridOrgTemplate.DataContext as OrganizationTemplate;
            if (orgTemplate==null)
            {
                return;
            }
            StaffInfo[] existStaffs = null; 
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                {
                    existStaffs=channel.GetStaffInfosByTemplateGuid(orgTemplate.TemplateGuid);
                });
            IList<string> listExistStaffGuid = null;
            if (existStaffs != null)
            {
                listExistStaffGuid = (from staff in existStaffs select staff.Key).ToList();
            }
            UserSettingWindow wnd = new UserSettingWindow(listExistStaffGuid);
            wnd.ShowDialog(this);
            if (wnd.DialogResult == true)
            {
                IList<StaffInfo> listSelectedStaff=wnd.GetSelectedStaffList();
                if (listSelectedStaff.Count > 0)
                {
                    StaffInfo[] staffInfos = new StaffInfo[listSelectedStaff.Count];
                    for (int i = 0; i < staffInfos.Length;i++ )
                    {
                        staffInfos[i] = listSelectedStaff[i];
                        staffInfos[i].OrgGuid = orgTemplate.Guid;
                    }
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            channel.SaveStaffInfos(orgTemplate.Guid, staffInfos);
                        });
                    var staffCollection=this.dataGridPersons.ItemsSource as ObservableCollection<StaffInfo>;
                    foreach (var staff in staffInfos)
                    {
                        staffCollection.Add(staff);
                    }
                }
            }
        }

        private void buttonPersonDelete_Click(object sender, RoutedEventArgs e)
        {
            var orgTemplate = this.gridOrgTemplate.DataContext as OrganizationTemplate;
            if (orgTemplate==null)
            {
                return;
            }
            StaffInfo selectedStaff = this.dataGridPersons.SelectedValue as StaffInfo;
            if (selectedStaff == null)
            {
                return;
            }
            if (MessageBox.Show("确实要删除选中的人员吗?", "删除提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObservableCollection<StaffInfo> staffInfos=this.dataGridPersons.ItemsSource as ObservableCollection<StaffInfo>;
                
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                    {
                        channel.DeleteStaff(orgTemplate.Guid, new string[] { selectedStaff.Key});
                    });
                staffInfos.Remove(selectedStaff);
            }
        }

        private void buttonMonitorEquipmentAdd_Click(object sender, RoutedEventArgs e)
        {
            var orgTemplate = this.gridOrgTemplate.DataContext as OrganizationTemplate;
            if (orgTemplate==null)
            {
                return;
            }
            MonitorStationEquInfo[] existOrgEquipments = null; 
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(
                channel =>
                {
                    existOrgEquipments= channel.GetMonitorEquipmentsByTemplateGuid(orgTemplate.TemplateGuid);
                });
            IList<string> listExistEquipmentGuid = null;
            if (existOrgEquipments != null)
            {
                listExistEquipmentGuid = (from equipment in existOrgEquipments select equipment.ID).ToList();
            }
            MonitorEquipmentSelectWindow wnd = new MonitorEquipmentSelectWindow(listExistEquipmentGuid);
            wnd.ShowDialog(this);
            if (wnd.DialogResult == true)
            {
                var list=wnd.GetSelectedEquipmentList();
                if (list != null && list.Count > 0)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            channel.SaveMonitorEquipments(orgTemplate.Guid, list.ToArray());
                        });
                    var monitorCollection = this.dataGridMonitorEquipment.ItemsSource as ObservableCollection<MonitorStationEquInfo>;
                    if (monitorCollection != null)
                    {
                        foreach (var monitorEquipment in list)
                        {
                            monitorCollection.Add(monitorEquipment);
                        }
                    }
                }
            }
        }

        private void buttonMonitorEquipmentDelete_Click(object sender, RoutedEventArgs e)
        {
            var orgTemplate = this.gridOrgTemplate.DataContext as OrganizationTemplate;
            if (orgTemplate == null)
            {
                return;
            }
            MonitorStationEquInfo selectedEuipement = this.dataGridMonitorEquipment.SelectedValue as MonitorStationEquInfo;
            if (selectedEuipement == null)
            {
                return;
            }
            if (MessageBox.Show("确实要删除选中的设备吗?", "删除提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObservableCollection<MonitorStationEquInfo> orgEquipmentInfos = this.dataGridMonitorEquipment.ItemsSource as ObservableCollection<MonitorStationEquInfo>;

                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                {
                    channel.DeleteMonitorEquipment(orgTemplate.Guid, new string[] { selectedEuipement.ID });
                });
                orgEquipmentInfos.Remove(selectedEuipement);
            }
        }

        private void dataGridMonitorEquipment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    var monitorEquipnent=dgr.DataContext as MonitorStationEquInfo;
                    if (monitorEquipnent != null)
                    {
                        MonitorStationEquDialog dialog = new MonitorStationEquDialog(monitorEquipnent);
                        dialog.Title = "便携式设备信息";
                        dialog.IsShowDetail = true;
                        dialog.ShowDialog(this);
                    }
                }
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            this.SaveOrganization();
        }

        private void SaveOrganization()
        {
            if (organizationEditControl.NeedSave)
            {
                organizationEditControl.Save();
            }
            //var orgTemplate = this.gridOrgTemplate.DataContext as OrganizationTemplate;
            //if (orgTemplate != null)
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
            //    {
            //        channel.SaveOrganizationTempate(orgTemplate);
            //    });
            //    this.isChanged = false;
            //    orgTemplate.IsNew = false;
            //}

        }

        private void imageVehicle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<string> existVehicleGuid = new List<string>();
            var treeNodeList = this.treeViewOrg.ItemsSource as IList<TreeNode<OrganizationTemplate>>;
            foreach (TreeNode<OrganizationTemplate> treeNode in treeNodeList)
            {
                foreach (var template in treeNode.GetNodeDataList())
                {
                    if (template.Vehicle!=null)
                    {
                        existVehicleGuid.Add(template.Vehicle.GUID);
                    }
                }
            }
            VehicleSettingWindow settingWindow = new VehicleSettingWindow(existVehicleGuid);
            settingWindow.ShowDialog(this);
            if (settingWindow.DialogResult == true)
            {
                var template = this.gridOrgTemplate.DataContext as OrganizationTemplate;
                if (template != null)
                {
                    template.Vehicle=settingWindow.GetSelectedVehicle();
                }
            }
        }

        internal void Reload()
        {
            ActivityTemplate template = this.DataContext as ActivityTemplate;
            if (template != null)
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                    {
                        var organizations = channel.GetOrganizations(template.Guid);
                        var orgTreeView=TreeNode<OrganizationTemplate>.CreateTreeNodes(organizations, org => string.IsNullOrWhiteSpace(org.ParentGuid), (org, node) =>
                            {
                                return org.ParentGuid == node.Value.Guid;
                            });
                        this.treeViewOrg.ItemsSource = orgTreeView;
                        if (orgTreeView.Count > 0)
                        {
                            orgTreeView[0].IsSelected = true;
                        }
                    });
            }
        }

        private void TreeViewOrgItem_Selected(object sender, RoutedEventArgs e)
        {
            var treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem != null)
            {
                var a = treeViewItem.ContainerFromElement(treeViewItem);
            }
        }

        //private void listBoxDuty_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.NewValue is OrganizationTemplate)
        //    {
        //        (this.listBoxDuty.ItemsSource as IList<PP_Duty>).CheckItems((e.NewValue as OrganizationTemplate).DutyList);
        //    }
        //    //CheckableData<string>.CheckItems(this.listBoxDuty.ItemsSource as IList<PP_Duty>, (e.NewValue as OrganizationTemplate).DutyList);
        //}

        private void MenuItemCreateSubOrg_Click(object sender, RoutedEventArgs e)
        {
            var templateNode=(sender as MenuItem).DataContext as TreeNode<OrganizationTemplate>;
            if (templateNode != null)
            {
                var parentTemplate = templateNode.Value;
                OrganizationTemplate newTemplate = new OrganizationTemplate { IsNew = true, ParentGuid = parentTemplate.Guid, Name = "新建小组" };
                newTemplate.TemplateGuid = parentTemplate.TemplateGuid;
                newTemplate.Guid = CO_IA.Client.Utility.NewGuid();
                newTemplate.DutyList = new List<string>();
                templateNode.SubTreeNodes.Add(new TreeNode<OrganizationTemplate> { Value = newTemplate });
            }
        }

        private void treeViewOrg_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.treeViewOrg.Focus();
            if (!this.organizationEditControl.NeedSave)
            {
                return;
            }
            //if (!this.isChanged)
            //{
            //    return;
            //}
            var data = (e.OriginalSource as FrameworkElement).DataContext as TreeNode<OrganizationTemplate>;
            if (data.IsSelected)
            {
                return;
            }
            var templateNode = (this.treeViewOrg.SelectedValue as TreeNode<OrganizationTemplate>);
            e.Handled = true;
            var dialogResult = MessageBox.Show("当前数据已经修改,是否保存", "保存提示", MessageBoxButton.YesNoCancel);
            if (dialogResult == MessageBoxResult.Cancel)
            {
                return;
            }
            if (dialogResult == MessageBoxResult.Yes)
            {
                this.organizationEditControl.Save();
            }
            templateNode.IsSelected = false;
            data.IsSelected = true;
        }

        private void gridOrgTemplate_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var orgTemplate=e.NewValue as OrganizationTemplate;
            this.isChanged = false;
            if (orgTemplate != null)
            {
                orgTemplate.PropertyChanged += this.NotifyOrganizationChanged;
                if (orgTemplate.IsNew)
                {
                    this.dataGridMonitorEquipment.ItemsSource = new ObservableCollection<MonitorStationEquInfo>();
                    this.dataGridPersons.ItemsSource = new ObservableCollection<StaffInfo>();
                }
                else
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            var equipments=channel.GetMonitorEquipments(orgTemplate.Guid);
                            this.dataGridMonitorEquipment.ItemsSource = new ObservableCollection<MonitorStationEquInfo>(equipments);
                        });
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                    {
                        var staffs = channel.GetStaffInfos(orgTemplate.Guid);
                        this.dataGridPersons.ItemsSource = new ObservableCollection<StaffInfo>(staffs);
                    });
                }
            }
        }

        private void menuItemDeleteOrg_Click(object sender, RoutedEventArgs e)
        {
            var templateNode = (sender as MenuItem).DataContext as TreeNode<OrganizationTemplate>;
            if (templateNode != null && templateNode.Level>0)
            {
                if (!templateNode.Value.IsNew)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
                        {
                            channel.DeleteOrganizationTemplate(templateNode.Value.Guid);
                        });
                }
                var parentTemplate = templateNode.Parent;
                parentTemplate.SubTreeNodes.Remove(templateNode);
                parentTemplate.IsSelected = true;
            }
        }

        private void treeViewOrg_Selected(object sender, RoutedEventArgs e)
        {
            var orgTemplate=this.treeViewOrg.SelectedValue as TreeNode<OrganizationTemplate>;
            if (orgTemplate!=null && orgTemplate.Level==0)
            {
                this.tabItemStaffSetting.IsSelected=true;
            }
        }

        //private void buttonDeleteVehicle_Click(object sender, RoutedEventArgs e)
        //{
        //    var template = this.gridOrgTemplate.DataContext as OrganizationTemplate;
        //    if (template != null)
        //    {
        //        template.Vehicle = null;
        //    }
        //}

        //private void listBoxDuty_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    OrganizationTemplate dataContext=(sender as ListBox).DataContext as OrganizationTemplate;
        //    if (dataContext != null)
        //    {
        //        dataContext.DutyList = new List<string>((this.listBoxDuty.ItemsSource as List<PP_Duty>).GetCheckedKeys());
        //    }
        //}
    }

    internal class OrganizationEquipmentVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var template=value as TreeNode<OrganizationTemplate>;
            if (template!=null && template.Level==0)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
