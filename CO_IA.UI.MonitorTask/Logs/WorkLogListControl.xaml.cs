using AT_BC.Common;
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

namespace CO_IA.UI.MonitorTask.Logs
{
    /// <summary>
    /// WorkLogListControl.xaml 的交互逻辑
    /// </summary>
    public partial class WorkLogListControl : UserControl
    {
        CheckBox checkBoxAll;
        public event Action<WorkLog> OnMouseDoubleClick;

        public WorkLog[] WorkLogItemSource
        {
            get { return worklogdatagrid.ItemsSource as WorkLog[]; }
        }

        public WorkLog SelectedWorkLog
        {
            get { return worklogdatagrid.SelectedItem as WorkLog; }
        }

        public WorkLogListControl()
        {
            InitializeComponent();
        }

        private void chkAll_Loaded(object sender, RoutedEventArgs e)
        {
            checkBoxAll = sender as CheckBox;
        }

        private void worklogdatagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OnMouseDoubleClick != null)
            {
                OnMouseDoubleClick(SelectedWorkLog);
            }
        }

        private void CheckableDataCheckedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CheckableCheckBoxHelper.Checked(sender, e, this.checkBoxAll);
        }

        public void UnCheckedAll()
        {
            checkBoxAll.IsChecked = false;
        }
    }
}
