using AT_BC.Data;
using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using CO_IA.UI.MonitorTask.Task;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CO_IA.UI.Screen.Task
{
    /// <summary>
    /// TaskAllList.xaml 的交互逻辑
    /// </summary>
    public partial class TaskAllList : Window
    {
        public List<PP_OrgInfo> OrgList
        {
            get;
            set;
        }
        private string[] OrgGuid;
        public TaskAllList(string[] orgGuid)
        {
            InitializeComponent();
            OrgGuid = orgGuid;
            this.Loaded += TaskManageModule_Loaded;
        }

        private void TaskManageModule_Loaded(object sender, RoutedEventArgs e)
        {

            var existTasks = PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, CO_IA.Data.Task[]>(channel =>
            {
                return channel.GetTasksByActivityGuid(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });
            var tasks = existTasks.Where(item =>
                {
                    if (item.FormState != FormState.Check)
                        return false;
                    if (item.Executors != null && item.Executors.Length > 0)
                    {
                        foreach (var o in item.Executors)
                        {
                            foreach (string s in OrgGuid)
                            {
                                if (o.Executor == s)
                                    return true;
                            }
                        }
                    }
                    
                    return false;
                });
            this.dataGridTask.ItemsSource = new ObservableCollection<CO_IA.Data.Task>(tasks);
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.PersonSchedule.I_CO_IA_PersonSchedule>(channel =>
            {
                this.OrgList = channel.GetPP_OrgInfos(CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid);
            });

            //this.gridTask.Children.Add(new Task.TaskEditControl());
        }
        private void dataGridTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void dataGridTask_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow dgr = DataGridRow.GetRowContainingElement(e.OriginalSource as FrameworkElement);
                if (dgr != null)
                {
                    CO_IA.Data.Task task = dgr.DataContext as CO_IA.Data.Task;
                    if (task != null)
                    {
                        Window wnd;
                        if (task.FormState == AT_BC.Data.FormState.Check)
                        {
                            wnd = new TaskExecutedWindow();
                        }
                        else
                        {
                            wnd = new TaskEditWindow();
                        }
                        wnd.DataContext = task;
                        wnd.ShowDialog(this);
                    }
                }
            }
        }

        private void dataGridTask_LayoutUpdated(object sender, EventArgs e)
        {
            this.dataGridTask.RowHeight = double.NaN;
        }      

       
    }
    public class OrgNameMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length > 1)
            {
                if (values[1] is IList<PP_OrgInfo>)
                {
                    IList<PP_OrgInfo> list = values[1] as IList<PP_OrgInfo>;
                    string code = values[0] as string;
                    foreach (var org in list)
                    {
                        if (org.GUID == code)
                        {
                            return org.NAME;
                        }
                    }
                    return code;
                }
            }
            return values;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }   

    public class ExecutorListSelector : DataTemplateSelector
    {
        private DataTemplate submittedTemplate = null;

        public DataTemplate SubmittedTemplate
        {
            get { return submittedTemplate; }
            set { submittedTemplate = value; }
        }

        private DataTemplate unSubmittedTemplate = null;
        public DataTemplate UnSubmittedTemplate
        {
            get { return unSubmittedTemplate; }
            set { unSubmittedTemplate = value; }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is CO_IA.Data.Task)
            {
                if ((item as CO_IA.Data.Task).FormState == AT_BC.Data.FormState.Check)
                {
                    return this.SubmittedTemplate;
                }
                else
                {
                    return this.UnSubmittedTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }

    }
}
