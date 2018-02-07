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
using System.Windows.Shapes;

namespace CO_IA.UI.MonitorTask.Monitor
{
    /// <summary>
    /// MonitorPlanEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorPlanEditWindow : Window
    {
        public MonitorPlanEditWindow()
        {
            InitializeComponent();
            //DataContextChanged += MonitorPlanEditWindow_DataContextChanged;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            double mHzFreqFrom, mHzFreqTo;
            if (!double.TryParse(this.textBoxFreqFrom.Text, out mHzFreqFrom))
            {
                MessageBox.Show("起始频率必须为数字");
                return;
            }
            if (!double.TryParse(this.textBoxFreqTo.Text, out mHzFreqTo))
            {
                MessageBox.Show("终止频率必须为数字");
                return;
            }

            if (mHzFreqFrom > mHzFreqTo)
            {
                MessageBox.Show("终止频率不应该小于起始频率");
                return;
            }
            var monitorPlan=this.DataContext as MonitorPlanInfo;
            if (monitorPlan != null)
            {
                monitorPlan.MHzFreqFrom = mHzFreqFrom;
                monitorPlan.MHzFreqTo = mHzFreqTo;
                monitorPlan.Comments = this.textBoxComments.Text;
            }
            this.DialogResult = true;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        //private void MonitorPlanEditWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.NewValue is MonitorPlanInfo)
        //    {
        //        var monitorPlan = e.NewValue as MonitorPlanInfo;
        //        this.textBoxFreqFrom.Text = monitorPlan.MHzFreqFrom.ToString();
        //        this.textBoxFreqTo.Text = monitorPlan.MHzFreqTo.ToString();
        //        this.textBoxComments.Text = monitorPlan.Comments;
        //    }
        //}
    }
}
