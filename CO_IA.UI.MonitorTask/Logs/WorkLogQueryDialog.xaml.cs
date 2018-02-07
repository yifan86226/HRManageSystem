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

namespace CO_IA.UI.MonitorTask.Logs
{
    /// <summary>
    /// WorkLogQueryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WorkLogQueryDialog : Window, INotifyPropertyChanged
    {
        private WorkLogQueryCondition defaultcondition;
        public event Action<WorkLogQueryCondition> OnQueryEvent;

        public WorkLogQueryDialog(WorkLogQueryCondition queryCondition)
        {
            InitializeComponent();
            defaultcondition = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<WorkLogQueryCondition>(queryCondition);
            this.DataContext = defaultcondition;
        }

        private void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (OnQueryEvent != null)
            {
                OnQueryEvent(this.DataContext as WorkLogQueryCondition);
            }
            this.Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new WorkLogQueryCondition() { ActivityGuid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid };
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
