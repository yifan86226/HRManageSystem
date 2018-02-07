using CO_IA.Client;
using CO_IA.Data;
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

namespace CO_IA.RIAS
{
    /// <summary>
    /// ActivityTemplateSelectControl.xaml 的交互逻辑
    /// </summary>
    public partial class ActivityTemplateSelectWindow : Window
    {
        public ActivityTemplateSelectWindow()
        {
            InitializeComponent();
            this.DataContextChanged += ActivityTemplateSelectWindow_DataContextChanged;
        }

        private void ActivityTemplateSelectWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //var activityType = e.NewValue as ActivityTypeDef;
            //if (activityType != null)
            //{
            //    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PlanDatabase.I_CO_IA_Template>(
            //        channel =>
            //        {
            //            var result = channel.GetActivitiesByTypeCode(activityType.ID);
            //            this.dataGridTemplate.ItemsSource = result;
            //            //if (result != null && result.Length > 0)
            //            //{
            //            //    this.dataGridTemplate.ItemsSource = new ObservableCollection<ActivityTemplate>(result);
            //            //}
            //            //else
            //            //{
            //            //    this.dataGridTemplate.ItemsSource = new ObservableCollection<ActivityTemplate>();
            //            //}
            //        });
            //}
        }
        public void dataGridTemplate_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
            //    if (dgr != null)
            //    {
            //        ActivityTemplate activity = dgr.DataContext as ActivityTemplate;
            //        if (activity != null)
            //        {
            //            this.OpenTemplateManageModule(activity);
            //        }
            //    }
            //}
        }

        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {
            if (this.GetSelectedActivity() == null)
            {
                MessageBox.Show("请先选择活动模版!");
                return;
            }
            this.DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public ActivityTemplate GetSelectedActivity()
        {
            return this.dataGridTemplate.SelectedValue as ActivityTemplate;
        }
    }
}
