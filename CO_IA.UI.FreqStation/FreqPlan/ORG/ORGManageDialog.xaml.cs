﻿using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using I_CO_IA.FreqStation;
using PT_BS_Service.Client.Framework;
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
using System.Windows.Shapes;

namespace CO_IA.UI.FreqStation.FreqPlan
{
    /// <summary>
    /// ORGManageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ORGManageDialog : Window, INotifyPropertyChanged
    {
        private bool IsPropertyChanged = false; //单位信息是否有更改
        private bool IsORGItemSourceChanged = false; //单位数据源是否有更改
        SecurityClass[] securityclasses;
        private CheckBox chkAll;
        private ActivityOrganization orgdatacontent;
        public event Action RefreshORGItemSource; //刷新数据源
        Activity activity = CO_IA.Client.RiasPortal.ModuleContainer.Activity;

        public ActivityOrganization ORGDataContent
        {
            get
            {
                return orgdatacontent;
            }
            set
            {
                orgdatacontent = value;
                NotifyPropertyChange("ORGDataContent");
            }
        }

        public ActivityOrganization SelectedORG
        {
            get;
            set;
        }

        private ObservableCollection<ActivityOrganization> orgitemsource;
        public ObservableCollection<ActivityOrganization> ORGItemSource
        {
            get
            {

                return orgitemsource;
            }
            set
            {
                orgitemsource = value;
                NotifyPropertyChange("ORGItemSource");
            }
        }

        public ORGManageDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            InitSecurityClass();
            GetORGInfos();
            this.Closing += ORGManageDialog_Closing;
        }


        private void row_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPropertyChanged)
            {
                MessageBoxResult result = MessageBoxResult.Yes;

                if (SelectedORG.DataSate == DataStateEnum.Added)
                {
                    result = MessageBox.Show(string.Format("是否保存添加的单位{0}", SelectedORG.Name), "提示", MessageBoxButton.YesNo);
                }
                else
                {
                    result = MessageBox.Show(string.Format("{0}信息已经改变,是否保存修改", SelectedORG.Name), "提示", MessageBoxButton.YesNo);
                }

                if (result == MessageBoxResult.Yes)
                {
                    SaveORGInfos();
                    GetORGInfos(SelectedORG);
                    DetailLostFocus();
                }
                else
                {
                    //新添加的单位
                    if (SelectedORG.DataSate == DataStateEnum.Added)
                    {
                        ORGItemSource.Remove(SelectedORG);
                        if (ORGItemSource == null || ORGItemSource.Count == 0)
                        {
                            bordermask.Visibility = Visibility.Visible;
                        }
                    }

                    IsPropertyChanged = false;
                    DataGridRow row = sender as DataGridRow;
                    ActivityOrganization neworg = row.Item as ActivityOrganization;
                    orgdatagrid.SelectedItem = neworg;
                    InitORGDataContent(neworg);
                    DetailLostFocus();
                }
            }
        }

        /// <summary>
        /// 详细信息失去焦点
        /// </summary>
        private void DetailLostFocus()
        {
            txtFocus.Focus();
            if (SelectedORG != null)
            {
                this.combClass.SelectedItem = SelectedORG.SecurityClass;
            }
            else
            {
                this.combClass.SelectedItem = securityclasses[0];
            }
        }

        private void ORGDataContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsPropertyChanged = true;
        }

        private void combClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = ((sender as ComboBox).SelectedItem as SecurityClass).Name;
            if (name != SelectedORG.SecurityClass.Name)
            {
                IsPropertyChanged = true;
            }
        }

        private void companydatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedORG = orgdatagrid.SelectedItem as ActivityOrganization;
            InitORGDataContent(SelectedORG);
        }

        private void ORGManageDialog_Closing(object sender, CancelEventArgs e)
        {
            if (IsPropertyChanged) //属性改变
            {
                MessageBoxResult result =
                  MessageBox.Show(string.Format("是否对单位'{0}进行保存'", SelectedORG.Name), "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                    SaveORGInfos();
                }
            }
            if (IsORGItemSourceChanged) //数据源改变
            {
                if (RefreshORGItemSource != null)
                {
                    RefreshORGItemSource();//刷新数据源
                }
            }
            base.OnClosed(e);
        }


        private void ORGManageControl_Closed(object sender, EventArgs e)
        {
            if (IsORGItemSourceChanged)
            {
                if (RefreshORGItemSource != null)
                {
                    RefreshORGItemSource();//刷新数据源
                }
            }
        }

        private void InitORGDataContent(ActivityOrganization org)
        {
            if (org != null)
            {
                ORGDataContent = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ActivityOrganization>(org);
                ORGDataContent.PropertyChanged -= ORGDataContent_PropertyChanged;
                ORGDataContent.PropertyChanged += ORGDataContent_PropertyChanged;
            }
        }

        private void InitSecurityClass()
        {
            securityclasses = CO_IA.Client.Utility.GetSecurityClasses();
            combClass.ItemsSource = securityclasses;
            //if (securityclasses == null || securityclasses.Length == 0)
            //{
            //    MessageBox.Show("请现在基础数据设置中增加保障级别");
            //}
        }

        private void GetORGInfos(Organization selectORG = null)
        {
            ActivityOrganization[] source = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation, ActivityOrganization[]>(channel =>
            {
                return channel.GetActivityOrgs(new OrgQueryCondition() { ActivityGuid = activity.Guid });
            });

            ORGItemSource = new ObservableCollection<ActivityOrganization>(source);

            if (orgitemsource == null || orgitemsource.Count == 0)
            {
                bordermask.Visibility = Visibility.Visible;
            }
            else
            {
                bordermask.Visibility = Visibility.Collapsed;
            }

            if (selectORG == null)
            {
                orgdatagrid.SelectedIndex = 0;
            }
            else
            {
                Organization org = ORGItemSource.FirstOrDefault(r => r.Guid == selectORG.Guid);
                if (org != null)
                {
                    this.orgdatagrid.SelectedItem = org;
                }
                else
                {
                    this.orgdatagrid.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// 保存单位信息
        /// </summary>
        private void SaveORGInfos()
        {
            if (this.ORGDataContent != null)
            {
                if (ValidORG(this.ORGDataContent))
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(org =>
                    {
                        string orgguid = ORGDataContent.Guid;

                        #region 验证单位名称
                        OrgQueryCondition condition = new OrgQueryCondition();
                        condition.ActivityGuid = RiasPortal.ModuleContainer.Activity.Guid;
                        condition.Name = ORGDataContent.Name;
                        ActivityOrganization[] orgs = org.GetActivityOrgs(condition);
                        ActivityOrganization sameorg = orgs.FirstOrDefault(r => r.Name == ORGDataContent.Name && r.Guid != ORGDataContent.Guid);
                        #endregion

                        if (sameorg == null)
                        {
                            try
                            {
                                org.SaveActivityOrg(this.ORGDataContent);
                                MessageBox.Show("保存成功");
                                GetORGInfos(SelectedORG);
                                chkAll.IsChecked = false;
                                IsORGItemSourceChanged = true;
                                IsPropertyChanged = false;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.GetExceptionMessage());
                            }
                        }
                        else
                        {
                            MessageBox.Show("单位名称已经存在,请重新输入");
                            this.txtName.Focus();
                            this.ORGDataContent.Name = SelectedORG.Name;
                        }
                    });
                }
            }
        }

        private bool ValidORG(ActivityOrganization orginfo)
        {
            bool issuccess = true;
            StringBuilder strmsg = new StringBuilder();

            if (string.IsNullOrEmpty(orginfo.Name))
            {
                issuccess = false;
                strmsg.Append("单位名称不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.ShortName))
            {
                issuccess = false;
                strmsg.Append("单位简称不能为空! \r");
            }
            if (orginfo.SecurityClass == null)
            {
                issuccess = false;
                strmsg.Append("保障类别不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.Address))
            {
                issuccess = false;
                strmsg.Append("单位地址不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.Contact))
            {
                issuccess = false;
                strmsg.Append("联系人不能为空! \r");
            }
            if (string.IsNullOrEmpty(orginfo.Phone))
            {
                issuccess = false;
                strmsg.Append("联系电话不能为空! \r");
            }
            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return issuccess;
        }

        string defname = "新建单位";
        private ActivityOrganization CreateOrganization()
        {
            ActivityOrganization neworg = new ActivityOrganization();
            neworg.ActivityGuid = activity.Guid;
            neworg.Guid = System.Guid.NewGuid().ToString();
            neworg.DataSate = DataStateEnum.Added;
            neworg.Name = defname;
            if (securityclasses != null && securityclasses.Length > 0)
            {
                neworg.SecurityClass = securityclasses[0];
            }
            else
            {
                neworg.SecurityClass = new SecurityClass();
            }
            return neworg;
        }

        /// <summary>
        /// 添加单位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ActivityOrganization org = CreateOrganization();
            this.orgdatagrid.SelectedItem = org;
            this.ORGItemSource.Add(org);
            bordermask.Visibility = Visibility.Collapsed;
            this.IsPropertyChanged = true;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveORGInfos();
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ORGDataContent = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ActivityOrganization>(SelectedORG);
            this.IsPropertyChanged = false;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<ActivityOrganization> orgs = ORGItemSource.Where(r => r.IsChecked == true).ToList();
            if (orgs.Count == 0)
            {
                MessageBox.Show("请先勾线要删除的单位");
                return;
            }
            else
            {
                MessageBoxResult msresult = MessageBox.Show("确认要删除选择的设备?", "提示", MessageBoxButton.YesNo);
                if (msresult == MessageBoxResult.Yes)
                {
                    List<string> guids = orgs.Select(r => r.Guid).ToList();

                    BeOperationInvoker.Invoke<I_CO_IA_FreqStation>(org =>
                    {
                        List<string> resultguids = org.CountActivityEqusForOrg(guids, RiasPortal.ModuleContainer.Activity.Guid);
                        StringBuilder errormsg = new StringBuilder();

                        if (resultguids != null && resultguids.Count > 0)
                        {
                            foreach (string orgguid in guids)
                            {
                                ActivityOrganization orginfo = orgs.FirstOrDefault(r => r.Guid == orgguid);
                                errormsg.AppendFormat("{0},", orginfo.Name);
                            }
                            string message = errormsg.ToString().TrimEnd(',');
                            MessageBox.Show(string.Format("当前活动下,{0}已经包含设备", message), "提示");
                        }
                        else
                        {
                            try
                            {
                                org.DeleteActivityOrg(guids, RiasPortal.ModuleContainer.Activity.Guid);
                                GetORGInfos();
                                chkAll.IsChecked = false;
                                IsORGItemSourceChanged = true;
                                MessageBox.Show("删除成功");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.GetExceptionMessage(), "删除失败");
                            }
                        }
                    });
                }
            }
        }

        #region 全选

        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            if (ORGItemSource != null)
            {
                CheckBox chk = sender as CheckBox;
                bool ischecked = chk.IsChecked.Value;

                foreach (ActivityOrganization item in ORGItemSource)
                {
                    item.IsChecked = ischecked;
                }
            }
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            chkAll = sender as CheckBox;
            if (this.ORGItemSource != null)
            {
                chkAll.IsChecked = ORGItemSource.Any(item => item.IsChecked);
            }
        }

        private void chkCell_Click(object sender, RoutedEventArgs e)
        {
            if (ORGItemSource != null)
            {
                int checkcount = ORGItemSource.Count(r => r.IsChecked == true);
                if (checkcount == ORGItemSource.Count)
                {
                    chkAll.IsChecked = true;
                }
                else if (checkcount == 0)
                {
                    chkAll.IsChecked = false;
                }
                else
                {
                    chkAll.IsChecked = null;
                }
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}