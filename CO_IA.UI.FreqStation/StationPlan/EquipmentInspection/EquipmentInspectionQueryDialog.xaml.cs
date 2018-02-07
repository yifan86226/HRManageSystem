using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// EquipmentInspectionQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquipmentInspectionQueryDialog : Window, INotifyPropertyChanged
    {
        private EquInspectionQueryCondition querycondition;
        public EquInspectionQueryCondition QueryCondition
        {
            get
            {
                return querycondition;
            }
            set
            {
                querycondition = value;
                NotifyPropertyChanged("QueryCondition");
            }
        }

        public event Action<EquInspectionQueryCondition> OnQueryEvent;

        public EquipmentInspectionQueryDialog(EquInspectionQueryCondition condition)
        {
            InitializeComponent();
            QueryCondition = condition;
            this.DataContext = QueryCondition;
            InidData();
            InitCondition(QueryCondition);
        }

        private void InidData()
        {
            Dictionary<CheckStateEnum, string> checkstatesource = new Dictionary<CheckStateEnum, string>();
            checkstatesource.Add(CheckStateEnum.UnCheck, "未检测");
            checkstatesource.Add(CheckStateEnum.Qualified, "检测通过");
            checkstatesource.Add(CheckStateEnum.UnQualified, "检测未通过");
            lstboxCheckState.ItemsSource = checkstatesource;

            Dictionary<SendLicenseEnum, string> sendlicensesource = new Dictionary<SendLicenseEnum, string>();
            sendlicensesource.Add(SendLicenseEnum.SendLicense, "已发放");
            sendlicensesource.Add(SendLicenseEnum.UnSendLicense, "未发放");
            lstboxSendlicense.ItemsSource = sendlicensesource;
        }
        /// <summary>
        /// 初始化查询条件
        /// </summary>
        private void InitCondition(EquInspectionQueryCondition queryCondition)
        {
            lstboxCheckState.UnSelectAll();
            if (queryCondition.CheckState.Count > 0)
            {
                List<CheckStateEnum> checkstate = queryCondition.CheckState;
                foreach (KeyValuePair<CheckStateEnum, string> item in lstboxCheckState.ItemsSource as Dictionary<CheckStateEnum, string>)
                {
                    if (checkstate.Contains(item.Key))
                    {
                        lstboxCheckState.SelectedItems.Add(item);
                    }
                }
            }

            lstboxSendlicense.UnSelectAll();
            if (queryCondition.SendLicense.Count > 0)
            {
                List<SendLicenseEnum> sendlicense = queryCondition.SendLicense;
                foreach (KeyValuePair<SendLicenseEnum, string> item in lstboxSendlicense.ItemsSource as Dictionary<SendLicenseEnum, string>)
                {
                    if (sendlicense.Contains(item.Key))
                    {
                        lstboxSendlicense.SelectedItems.Add(item);
                    }
                }
            }
        }

        private EquInspectionQueryCondition GetEquInspectionQueryCondition()
        {
            EquInspectionQueryCondition condition = this.DataContext as EquInspectionQueryCondition;

            List<CheckStateEnum> checkstates = new List<CheckStateEnum>();
            foreach (KeyValuePair<CheckStateEnum, string> checkstate in this.lstboxCheckState.SelectedItems)
            {
                checkstates.Add(checkstate.Key);
            }
            condition.CheckState = checkstates;

            List<SendLicenseEnum> sendlicenses = new List<SendLicenseEnum>();
            foreach (KeyValuePair<SendLicenseEnum, string> sendlicense in this.lstboxSendlicense.SelectedItems)
            {
                sendlicenses.Add(sendlicense.Key);
            }
            condition.SendLicense = sendlicenses;
            return condition;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new EquInspectionQueryCondition();
            lstboxCheckState.UnSelectAll();
            lstboxSendlicense.UnSelectAll();
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (OnQueryEvent != null)
            {
                EquInspectionQueryCondition condition = GetEquInspectionQueryCondition();
                OnQueryEvent(condition);
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

    }
}
