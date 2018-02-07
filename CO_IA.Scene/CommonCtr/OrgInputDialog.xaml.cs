using CO_IA.Data;
using CO_IA.Types;
using I_CO_IA.FreqStation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace CO_IA.Scene.CommonCtr
{
    /// <summary>
    /// OrgInputDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OrgInputDialog : Window
    {
        private ActivityOrganization orgdatacontent;
        private SecurityClass[] securityclasses;
        private ActivityOrganization _currentOrg;
        public event Action<ActivityOrganization> AfterSave;
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
        public OrgInputDialog()
        {
            InitializeComponent();
            if (this.orgdatacontent == null)
            {
                this.orgdatacontent = CreateOrganization();
            }
            InitSecurityClass();
            _currentOrg = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ActivityOrganization>(orgdatacontent);
            this.DataContext = this;
        }
        public OrgInputDialog(ActivityOrganization p_orgdatacontent)
            : this()
        {
            this.orgdatacontent = p_orgdatacontent;
        }
        private ActivityOrganization CreateOrganization()
        {
            ActivityOrganization neworg = new ActivityOrganization();
            neworg.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
            neworg.Guid = System.Guid.NewGuid().ToString();
            neworg.DataSate = DataStateEnum.Added;
            neworg.Name = "新建单位";
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
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveORGInfos();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            ORGDataContent = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ActivityOrganization>(_currentOrg);
            this.DataContext = null;
            this.DataContext = this;
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
                        condition.ActivityGuid = SystemLoginService.CurrentActivity.Guid;
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
                                this.Close();
                                if (AfterSave != null)
                                {
                                    AfterSave(this.ORGDataContent);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.GetExceptionMessage());
                            }
                        }
                        else
                        {
                            MessageBox.Show("单位名称已经存在,请重新输入");
                        }
                    });
                }
            }
        }
        private void InitSecurityClass()
        {
            securityclasses = CO_IA.Client.Utility.GetSecurityClasses();
            combClass.ItemsSource = securityclasses;
            if (securityclasses == null || securityclasses.Length == 0)
            {
                MessageBox.Show("请现在基础数据设置中增加保障级别");
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
