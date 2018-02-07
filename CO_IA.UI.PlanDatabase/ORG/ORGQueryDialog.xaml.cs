using CO_IA.Data;
using System;
using System.Collections.Generic;
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

namespace CO_IA.UI.PlanDatabase.ORG
{
    /// <summary>
    /// ORGQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ORGQueryDialog : Window
    {
        public OrgQueryCondition CurrentCondition
        {
            get;
            set;
        }

        public event Action<OrgQueryCondition> OnQueryEvent;

        public ORGQueryDialog(OrgQueryCondition condition)
        {
            InitializeComponent();
            CurrentCondition = condition;
            cbtype.ItemsSource = CO_IA.Client.Utility.GetSecurityClasses();
            InitCondition(CurrentCondition);
        }

        private void InitCondition(OrgQueryCondition queryCondition)
        {
            cbtype.UnselectAllItems();
            this.txtName.Text = queryCondition.Name;
            this.txtAddress.Text = queryCondition.Address;

            if (queryCondition.SecurityClasses != null && queryCondition.SecurityClasses.Count > 0)
            {
                List<string> classes = queryCondition.SecurityClasses;
                foreach (SecurityClass type in cbtype.ItemsSource as SecurityClass[])
                {
                    if (classes.Contains(type.Guid))
                    {
                        cbtype.SelectedItems.Add(type);
                    }
                }
            }
        }

        private OrgQueryCondition GetCondition()
        {
            OrgQueryCondition condition = new OrgQueryCondition();
            condition.ActivityGuid = CurrentCondition.ActivityGuid;
            condition.Name = this.txtName.Text;
            condition.Address = this.txtAddress.Text;
            if (cbtype.SelectedItems.Count > 0)
            {
                List<string> classes = cbtype.SelectedItems.Select(r => ((SecurityClass)r).Guid).ToList();
                condition.SecurityClasses = classes;
            }
            return condition;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (OnQueryEvent != null)
            {
                CurrentCondition = GetCondition();
                OnQueryEvent(CurrentCondition);
            }
            this.Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            CurrentCondition = new OrgQueryCondition { ActivityGuid = CurrentCondition.ActivityGuid };
            InitCondition(CurrentCondition);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
