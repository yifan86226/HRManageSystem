using CO_IA.Client;
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

namespace CO_IA.UI.StationPlan
{
    /// <summary>
    /// EquCheckQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EquCheckQueryDialog : Window
    {

        public event Action<EquipmentCheckQueryCondition> OnQueryEvent;

        public EquipmentCheckQueryCondition QueryCondition
        {
            get { return (EquipmentCheckQueryCondition)GetValue(QueryConditionProperty); }
            set { SetValue(QueryConditionProperty, value); }
        }

        public static readonly DependencyProperty QueryConditionProperty =
            DependencyProperty.Register("QueryCondition", typeof(EquipmentCheckQueryCondition), typeof(EquCheckQueryDialog), new PropertyMetadata(null));

        public EquCheckQueryDialog(EquipmentCheckQueryCondition condition)
        {
            InitializeComponent();
            QueryCondition = condition;
            //comboxORG.ItemsSource = Utility.GetORGInfos();
            this.DataContext=QueryCondition;
            SetQueryCondition();
        }

        /// <summary>
        /// 设置查询条件
        /// </summary>
        private void SetQueryCondition()
        {
            //未检测
            if ((QueryCondition.CheckState & CheckStateEnum.UnCheck) == CheckStateEnum.UnCheck)
            {
                chkuncheced.IsChecked = true;
            }

            //检测通过
            if ((QueryCondition.CheckState & CheckStateEnum.Qualified) == CheckStateEnum.Qualified)
            {
                chkqualified.IsChecked = true;
            }

            //检测未通过
            if ((QueryCondition.CheckState & CheckStateEnum.UnQualified) == CheckStateEnum.UnQualified)
            {
                chkunqualified.IsChecked = true;
            }

            //发放许可证
            if ((QueryCondition.SendLicense & SendLicenseEnum.SendLicense) == SendLicenseEnum.SendLicense)
            {
                chksendlicense.IsChecked = true;
            }

            //未发放许可证
            if ((QueryCondition.SendLicense & SendLicenseEnum.UnSendLicense) == SendLicenseEnum.UnSendLicense)
            {
                chkunsendlicense.IsChecked = true;
            }
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        private EquipmentCheckQueryCondition GetQueryCondition()
        {
            EquipmentCheckQueryCondition condition = new EquipmentCheckQueryCondition();
            if (chkuncheced.IsChecked.Value)
            {
                condition.CheckState = condition.CheckState | CheckStateEnum.UnCheck;
            }
            if (chkqualified.IsChecked.Value)
            {
                condition.CheckState = condition.CheckState | CheckStateEnum.Qualified;
            }
            else if (chkuncheced.IsChecked.Value)
            {
                condition.CheckState = condition.CheckState | CheckStateEnum.UnQualified;
            }

            //许可证发放
            if (chksendlicense.IsChecked.Value)
            {
                condition.SendLicense = condition.SendLicense | SendLicenseEnum.SendLicense;
            }

            if (chkunsendlicense.IsChecked.Value)
            {
                condition.SendLicense = condition.SendLicense | SendLicenseEnum.UnSendLicense;
            }

            return condition;
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            EquipmentCheckQueryCondition querycondition = GetQueryCondition();
            QueryCondition.CheckState = querycondition.CheckState;
            QueryCondition.SendLicense = querycondition.SendLicense;
            if (OnQueryEvent != null)
            {
                OnQueryEvent(QueryCondition);
            }
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
