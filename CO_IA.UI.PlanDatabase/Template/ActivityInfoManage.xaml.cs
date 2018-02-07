using CO_IA.Data.Template;
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

namespace CO_IA.UI.PlanDatabase.Template
{
    /// <summary>
    /// ActivityInfoManage.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityInfoManage : UserControl
    {
        public ActivityInfoManage()
        {
            InitializeComponent();
            this.DataContextChanged += ActivityInfoManage_DataContextChanged;
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.comboBoxEditArea.ItemsSource = CO_IA.Client.Utility.GetProvinceAreaCode();
            }
        }

        private void ActivityInfoManage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ActivityTemplate activity = e.NewValue as ActivityTemplate;
            activity.PropertyChanged += (obj, arg) =>
                {
                    this.buttonSaveActivity.IsEnabled = true;
                };
            if (activity != null)
            {
                (sender as ActivityInfoManage).textBlockActivityType.Text = CO_IA.Client.Utility.GetActivityTypeName(activity.ActivityType);
            }
            this.buttonSaveActivity.IsEnabled = activity.IsNew;
        }


        private void buttonSaveActivity_Click(object sender, RoutedEventArgs e)
        {
            var activity=this.DataContext as ActivityTemplate;
            if (string.IsNullOrWhiteSpace(activity.Name))
            {
                MessageBox.Show("活动名称不能为空");
                return;
            }
            if (activity.Name.Length > 100)
            {
                MessageBox.Show("活动名称过长(最多100个字)");
                return;
            }
            if (string.IsNullOrWhiteSpace(activity.ShortHand))
            {
                MessageBox.Show("活动简称不能为空");
                return;
            }
            if (activity.ShortHand.Length > 10)
            {
                MessageBox.Show("活动简称过长(最多10个字)");
                return;
            }
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(
                channel =>
                {
                    channel.SaveActivity(activity);
                });
            this.buttonSaveActivity.IsEnabled = false;
            activity.IsNew = false;
        }
    }
}
