using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CO_IA.Client;
using CO_IA.Data.Collection;
using CO_IA.UI.Collection;
using CO_IA.UI.Collection.DbEntity;
using I_CO_IA.Collection;
using CO_IA_Data;

namespace CO_IA.MonitoringCollecting.Views
{
    /// <summary>
    /// DataUpLoadView.xaml 的交互逻辑
    /// </summary>
    public partial class DataUpLoadView : Window
    {
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        public DataUpLoadView()
        {
            InitializeComponent();
        }
        private void BeginUpLoad()
        {
            _progressBar.Maximum = 2;
            _progressBar.Value = 0;
           
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(_progressBar.SetValue);
            OperateLog log = new OperateLog();
            log.Guid =CO_IA.Client.Utility.NewGuid();
            log.Operater = RiasPortal.Current.UserSetting.UserName;
            log.OperateDate = DateTime.Now;
            log.OperateType = OperateTypeEnum.UpLoad;
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        if (_analysisUpLoad.IsChecked == true)
                        {
                            _upLoadInfo.Text = "正在上传分析结果数据";
                            UpLoadAnalysisResult();
                            log.OperateTables += "ACTIVITY_ANALYSIS_RESULT,";
                        }
                    }
                    else if (i == 1)
                    {
                        if (_emitUpLoad.IsChecked == true)
                        {
                            _upLoadInfo.Text = "正在上传发射信息数据";
                            UpLoadEmitInfo();
                            log.OperateTables += "ACTIVITY_EMIT_INFO,";
                        }
                    }
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { System.Windows.Controls.ProgressBar.ValueProperty, Convert.ToDouble(i + 1) });
                }
                SQLiteDataService.Transaction = SQLiteConnect.GetSQLiteTransaction(SystemLoginService.CurrentActivity.Name);
                SQLiteDataService.SaveOperaterLog(log);
                SQLiteDataService.Transaction.Commit();
                _upLoadInfo.Text = "上传成功";
            }
            catch (Exception ex)
            {
                _upLoadInfo.Text = "上传失败";
                //MessageBox.Show("失败原因：\r\n" + ex.Message);
            }
        }
        private void UpLoadBtn_Click(object sender, RoutedEventArgs e)
        {
            BeginUpLoad();
        }

        /// <summary>
        /// 上传分析结果
        /// </summary>
        private void UpLoadAnalysisResult()
        {
            List<AnalysisResult> analysisList = SQLiteDataService.QueryAnalysisResult(SystemLoginService.CurrentActivityPlace.Guid);
            SaveAnalysisToOracle(analysisList);
        }

        /// <summary>
        /// 上传发射信息
        /// </summary>
        private void UpLoadEmitInfo()
        {
            List<StationEmitInfo> emitInfoList = SQLiteDataService.QueryEmitInfo(SystemLoginService.CurrentActivity.Guid,SystemLoginService.CurrentActivityPlace.Guid);
            SaveEmitInfoToOracle(emitInfoList);
        }

        private bool SaveEmitInfoToOracle(List<StationEmitInfo> emitInfoList)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, bool>(channel =>
            {
                return channel.UpLoadEmitInfo(emitInfoList);
            });
        }

        private bool SaveAnalysisToOracle(List<AnalysisResult> analysisList)
        {
            return PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA_Collection, bool>(channel =>
            {
                return channel.UpLoadAnalysisResult(analysisList);
            });
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
