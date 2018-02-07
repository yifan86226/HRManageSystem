using CO_IA.Client;
using CO_IA.Data;
using CO_IA.Types;
using CO_IA.UI.MAP;
using AT_BC.Types;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AT_BC.Data;
using AT_BC.Common;

namespace CO_IA.UI.MonitorTask.Logs
{
    /// <summary>
    /// TaskEditControl.xaml 的交互逻辑
    /// </summary>
    public partial class WorkLogEditControl : AT_BC.Client.Extensions.EditableUserControl
    {
        private WorkLog CurrentWorkLog;
        public event Action<object, ExecutedRoutedEventArgs> OpenWorkLogStuff;

        private ObservableCollection<WorkLogStuff> WorkLogStuffSource
        {
            get { return this.listBoxLogStuff.ItemsSource as ObservableCollection<WorkLogStuff>; }
        }

        public WorkLogEditControl()
        {
            InitializeComponent();
            this.DataContextChanged += WorkLogEditControl_DataContextChanged;
        }

        private void WorkLogEditControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CurrentWorkLog = this.DataContext as WorkLog;
            ObservableCollection<WorkLogStuff> stuffsource = new ObservableCollection<WorkLogStuff>(GetWorkLogStuffs(CurrentWorkLog.Key));
            this.listBoxLogStuff.ItemsSource = stuffsource;
        }

        private void buttonWorkLogAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentWorkLog != null)
            {
                byte[] bytes = null;
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    Stream fs = null;
                    try
                    {
                        fs = dialog.OpenFile();
                        bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, bytes.Length);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                    if (bytes != null)
                    {
                        WorkLogStuff stuff = new WorkLogStuff();
                        stuff.Key = Utility.NewGuid();
                        stuff.IsLoaded = true;
                        stuff.Content = bytes;
                        stuff.ContentLength = bytes.Length;
                        stuff.Name = dialog.SafeFileName;
                        stuff.WorkLogGuid = CurrentWorkLog.Key;
                        stuff.SubmitUser = RiasPortal.Current.UserSetting.UserName;
                        CurrentWorkLog.SubmitTime = DateTime.Now;
                        stuff.DataState = DataStateEnum.Added;
                        CurrentWorkLog.StuffList.Add(stuff);
                        //PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        //{
                        //    channel.SaveWorkLogStuff(stuff);
                        //});
                        WorkLogStuffSource.Add(stuff);
                    }
                }
            }
        }

        private void checkBoxAudio_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                var logStuff = checkBox.DataContext as WorkLogStuff;
                if (logStuff == null)
                { 
                    return;
                }
                if (checkBox.IsChecked == true)
                {
                    string displayFile = WorkLogStuffFileHelper.GetDisplayFile(logStuff);
                    if (displayFile.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) || displayFile.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                    {
                        CO_IA.Client.AudioPlayer.Play(checkBox,new Uri(displayFile, UriKind.RelativeOrAbsolute), () =>{ checkBox.IsChecked = false; });
                    }
                }
                else
                {
                    CO_IA.Client.AudioPlayer.Stop();
                }
            }
        }

        //private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    CO_IA.Client.AudioPlayer.Stop();
        //}

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonWorkLogDelete_Click(object sender, RoutedEventArgs e)
        {
            var hyperlink = sender as Hyperlink;
            if (hyperlink != null)
            {
                WorkLogStuff logStuff = hyperlink.DataContext as WorkLogStuff;
                if (logStuff != null)
                {
                    if (MessageBox.Show("确实要删除选中的资料吗? 删除后将不能取消", "删除提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                        {
                            channel.DeleteWorkLogStuff(logStuff.Key);
                        });
                        WorkLogStuffSource.Remove(logStuff);
                    }
                }
            }
        }

        private WorkLogStuff[] GetWorkLogStuffs(string worklogguid)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask, WorkLogStuff[]>(worklog =>
             {
                 return worklog.GetWorkLogStuffs(worklogguid);
             });
        }

        private void FileDescriptionSaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is WorkLogStuff)
            {
                WorkLogStuff logStuff = e.Parameter as WorkLogStuff;
                if (logStuff != null)
                {
                    PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.MonitorTask.I_CO_IA_MonitorTask>(channel =>
                    {
                        var result = channel.GetWorkLogStuffByID(logStuff.Key);
                        if (result != null && result.Length > 0)
                        {
                            logStuff.Content = result;
                        }
                    });
                }
                FileDescriptionHelper.SaveAs(sender, e);
            }
        }

        private void FileDescriptionOpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (OpenWorkLogStuff != null)
            {
                OpenWorkLogStuff(sender, e);
            }
        }


    }
}
