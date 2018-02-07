using CO_IA.Data;
using CO_IA.UI.MonitorTask.Logs;
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

namespace CO_IA.UI.MonitorTask
{
    /// <summary>
    /// WorkLogModule.xaml 的交互逻辑
    /// </summary>
    public partial class WorkLogModule : UserControl
    {
        string activityguid = CO_IA.Client.RiasPortal.ModuleContainer.Activity.Guid;
        WorkLogQueryCondition queryCondition = new WorkLogQueryCondition();
        public WorkLogModule()
        {
            InitializeComponent();
            workloglistcontrol.OnMouseDoubleClick += workloglistcontrol_OnMouseDoubleClick;
            queryCondition.ActivityGuid = activityguid;
            QueryWorkLog(queryCondition);
        }

        private void QueryWorkLog(WorkLogQueryCondition condition)
        {
            PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(worklog =>
            {
                WorkLog[] logs = worklog.GetWorkLogsByCondition(condition);
                workloglistcontrol.DataContext = logs;
            });
        }

        private void workloglistcontrol_OnMouseDoubleClick(WorkLog obj)
        {
            WorkLogEditDialog logedit = new WorkLogEditDialog();
            logedit.DataContext = obj;
            logedit.UpdateWorkLog += () => { QueryWorkLog(queryCondition); };
            logedit.ShowDialog();
        }

        private void btnLogAdd_Click(object sender, RoutedEventArgs e)
        {
            WorkLogEditDialog logadd = new WorkLogEditDialog();
            logadd.DataContext = new WorkLog()
            {
                ActivityGuid = activityguid,
                Submitter = CO_IA.Client.RiasPortal.Current.UserSetting.UserName,
                Worker = CO_IA.Client.RiasPortal.Current.UserSetting.UserName,
                Title = DateTime.Today.Date.ToString("D") + "工作日志"

            };
            logadd.UpdateWorkLog += () => { QueryWorkLog(queryCondition); };
            logadd.ShowDialog();
        }

        private void btnLogDel_Click(object sender, RoutedEventArgs e)
        {
            if (workloglistcontrol.WorkLogItemSource != null)
            {
                WorkLog[] selects = workloglistcontrol.WorkLogItemSource.Where(r => r.IsChecked == true).ToArray();

                if (selects.Length == 0)
                {
                    MessageBox.Show("请勾选要删除的日志");
                }
                else
                {
                    List<string> guids = selects.Select(r => r.Key).ToList();
                    try
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke
                            <I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(worklog =>
                            {
                                worklog.DeleteWorkLog(guids);
                            });
                        QueryWorkLog(queryCondition);
                        workloglistcontrol.UnCheckedAll();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.GetExceptionMessage(), "删除失败");
                    }
                }
            }
        }

        private void btnLogQuery_Click(object sender, RoutedEventArgs e)
        {
            WorkLogQueryDialog query = new WorkLogQueryDialog(queryCondition);
            query.OnQueryEvent += (condition) =>
            {
                queryCondition = condition;
                QueryWorkLog(queryCondition);
            };
            query.ShowDialog();
        }
    }
}
