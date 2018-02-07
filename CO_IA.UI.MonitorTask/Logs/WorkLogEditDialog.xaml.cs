using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CO_IA.UI.MonitorTask
{
    /// <summary>
    /// WorkLogEditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class WorkLogEditDialog : Window
    {
        public event Action UpdateWorkLog;

        private WorkLog CurrentWorkLog;
        private WorkLog DefaultWorkLog;
        public WorkLogEditDialog()
        {
            InitializeComponent();
            this.DataContextChanged += WorkLogEditDialog_DataContextChanged;
            this.worklogeditcontrol.OpenWorkLogStuff += worklogeditcontrol_OpenWorkLogStuff;
        }

        private void WorkLogEditDialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CurrentWorkLog = this.DataContext as WorkLog;
            this.DefaultWorkLog = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<WorkLog>(this.CurrentWorkLog);
            worklogeditcontrol.DataContext = DefaultWorkLog;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (Validate(this.DefaultWorkLog))
            {
                try
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(worklog =>
                    {
                        worklog.SaveWorkLog(this.DefaultWorkLog);
                    });

                    this.Close();
                    if (UpdateWorkLog != null)
                    {
                        UpdateWorkLog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetExceptionMessage(), "保存失败");
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void worklogeditcontrol_OpenWorkLogStuff(object arg1, ExecutedRoutedEventArgs arg2)
        {
            WorkLogStuffFileHelper.OpenFile(arg1, arg2, this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            //删除WorkLogStuffs文件夹下所有文件
            string basePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorkLogStuffs");
            if (System.IO.Directory.Exists(basePath))
            {
                DirectoryInfo dicinfo = new DirectoryInfo(basePath);
                FileInfo[] files = dicinfo.GetFiles();
                foreach (FileInfo file in files)
                {
                    file.Delete();
                }
            }
        }

        private bool Validate(WorkLog log)
        {
            bool success = true;
            StringBuilder errormsg = new StringBuilder();
            if (string.IsNullOrWhiteSpace(log.Title))
            {
                errormsg.AppendLine("标题不能为空");
                success = false;
            }
            if (log.WorkDateFrom == null)
            {
                errormsg.AppendLine("工作开始时间不能为空");
                success = false;
            }
            if (log.WorkDateTo == null)
            {
                errormsg.AppendLine("工作结束时间不能为空");
                success = false;
            }
            if (log.WorkDateFrom != null && log.WorkDateTo != null)
            {
                if (log.WorkDateTo.CompareTo(log.WorkDateFrom) < 0)
                {
                    errormsg.AppendLine("开始时间不能大于结束时间");
                    success = false;
                }
            }
            if (string.IsNullOrEmpty(log.Worker))
            {
                errormsg.AppendLine("工作人员不能为空");
                success = false;
            }
            if (string.IsNullOrWhiteSpace(log.Submitter))
            {
                errormsg.AppendLine("提交人不能为空");
                success = false;
            }
            if (log.SubmitTime == null)
            {
                errormsg.AppendLine("提交时间不能为空");
                success = false;
            }
            if (string.IsNullOrWhiteSpace(log.Description))
            {
                errormsg.AppendLine("内容描述不能为空");
                success = false;
            }
            if (!success)
            {
                MessageBox.Show(errormsg.ToString(), "提示");
            }
            return success;
        }
    }
}
