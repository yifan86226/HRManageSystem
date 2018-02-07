using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Data.Template;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace CO_IA.UI.PlanDatabase.Template
{
    /// <summary>
    /// OrganizationEditControl.xaml 的交互逻辑
    /// </summary>
    public partial class OrganizationEditControl : UserControl
    {
        private OrganizationTemplate editOrganization;

        public Func<List<OrganizationTemplate>> GetExistOrganizationTemplateList;
        public OrganizationEditControl()
        {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.combArea.ItemsSource = CO_IA.Client.Utility.GetProvinceAreaCode();
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.Setting.I_CO_IA_Setting>(channel =>
                {
                    //更新当前节点
                    this.listBoxDuty.ItemsSource = channel.Get_DutyList();
                });
            }

            this.DataContextChanged += OrganizationEditControl_DataContextChanged;
        }

        private void OrganizationEditControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is OrganizationTemplate)
            {
                editOrganization = e.NewValue as OrganizationTemplate;
                this.textBoxName.Text = editOrganization.Name;
                (this.listBoxDuty.ItemsSource as IList<PP_Duty>).CheckItems(editOrganization.DutyList);
                this.gridVehicle.DataContext = editOrganization.Vehicle;
            }
        }

        private void imageVehicle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<OrganizationTemplate> existOrganizationTemplateList = null;
            if (this.GetExistOrganizationTemplateList != null)
            {
                existOrganizationTemplateList = this.GetExistOrganizationTemplateList();
            }
            List<string> existVehicleGuid = new List<string>();
            if (existOrganizationTemplateList != null)
            {
                foreach (var orgTemplate in existOrganizationTemplateList)
                {
                    if (orgTemplate == this.editOrganization)
                    {
                        if (this.gridVehicle.DataContext is VehicleInfo)
                        {
                            existVehicleGuid.Add((this.gridVehicle.DataContext as VehicleInfo).GUID);
                        }
                    }
                    else if (orgTemplate.Vehicle!=null)
                    {
                        existVehicleGuid.Add(orgTemplate.Vehicle.GUID);
                    }
                }
            }
            //var treeNodeList = this.treeViewOrg.ItemsSource as IList<TreeNode<OrganizationTemplate>>;
            //foreach (TreeNode<OrganizationTemplate> treeNode in treeNodeList)
            //{
            //    foreach (var template in treeNode.GetNodeDataList())
            //    {
            //        if (template.Vehicle != null)
            //        {
            //            existVehicleGuid.Add(template.Vehicle.GUID);
            //        }
            //    }
            //}
            VehicleSettingWindow settingWindow = new VehicleSettingWindow(existVehicleGuid);
            settingWindow.ShowDialog(this);
            if (settingWindow.DialogResult == true)
            {
                this.gridVehicle.DataContext = settingWindow.GetSelectedVehicle();
            }
        }

        public bool NeedSave
        {
            get
            {
                if (editOrganization == null)
                {
                    return false;
                }
                if (!editOrganization.IsNew)
                {
                    if (this.editOrganization.Name != this.textBoxName.Text)
                    {
                        return true;
                    }
                    if (this.editOrganization.Vehicle != (this.gridVehicle.DataContext as VehicleInfo))
                    {
                        return true;
                    }
                    IList<string> dutyList = new List<string>((this.listBoxDuty.ItemsSource as List<PP_Duty>).GetCheckedKeys());
                    if (dutyList.Count != this.editOrganization.DutyList.Count)
                    {
                        return true;
                    }
                    foreach (var duty in dutyList)
                    {
                        if (!this.editOrganization.DutyList.Contains(duty))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void Save()
        {
            var duties = (this.listBoxDuty.ItemsSource as List<PP_Duty>).GetCheckedKeys();
            if (duties == null || duties.Length == 0)
            {
                throw new Exception("工作组职责不能为空!");
            }
            OrganizationTemplate orgTemplate = new OrganizationTemplate();
            orgTemplate.Guid = this.editOrganization.Guid;
            orgTemplate.ParentGuid = this.editOrganization.ParentGuid;
            orgTemplate.Name = this.textBoxName.Text;
            orgTemplate.TemplateGuid = this.editOrganization.TemplateGuid;
            orgTemplate.Vehicle = this.gridVehicle.DataContext as VehicleInfo;
            orgTemplate.DutyList = new List<string>((this.listBoxDuty.ItemsSource as List<PP_Duty>).GetCheckedKeys());
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(channel =>
            {
                channel.SaveOrganizationTempate(orgTemplate);
            });
            this.editOrganization.IsNew = false;
            this.editOrganization.Name = orgTemplate.Name;
            this.editOrganization.Vehicle = orgTemplate.Vehicle;
            this.editOrganization.DutyList = orgTemplate.DutyList;
        }

        private void buttonDeleteVehicle_Click(object sender, RoutedEventArgs e)
        {
            this.gridVehicle.DataContext = null;
        }
    }

    internal class VehicleImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is VehicleInfo)
            {
                byte[] bytes = (value as VehicleInfo).Picture;
                if (bytes != null && bytes.Length > 0)
                {
                    var imageSource = ClientHelper.LoadImageFromBytes(bytes);
                    if (imageSource != null)
                    {
                        return imageSource;
                    }
                }
            }
            if (parameter is string)
            {
                System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(parameter as string, UriKind.RelativeOrAbsolute));
                return bitmapImage;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class NullObjectToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
