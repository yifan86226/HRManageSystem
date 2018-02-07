using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;

namespace CO.IA.UI.TaskManage.Rules
{
    /// <summary>
    /// LicenseSend.xaml 的交互逻辑
    /// </summary>
    public partial class LicenseSend : UserControl
    {
        public LicenseSend()
        {
            InitializeComponent();
          
        }

        /// <summary>
        /// 执照上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.CheckFileExists = true;//检查文件是否存在
            dlg.Multiselect = false;//是否允许多选，false表示单选
            dlg.CheckPathExists = true;
            //dlg.Filter = "文本文件 (*.xls,*.xlsx)|*.doc;*.docx|" +
            //             "All files (*.*)|*.*";
            dlg.Filter = "All files (*.*)|*.*";//文件过滤器
            if ((bool)dlg.ShowDialog())
            {
                string filePath = dlg.FileName;
              
            }
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DownloadFile_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void DgQuestion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            PrintPreviewWindow previewWnd = new PrintPreviewWindow();
            previewWnd.ShowInTaskbar = false;
            previewWnd.ShowDialog();
        }

        private void btnPrintDlg_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pdlg = new PrintDialog();
            if (pdlg.ShowDialog() == true)
            {
                
            }
        }
        private Timer m_timerToEnableButton;
        private delegate void EnableButtonMethod();
        private void EnableButton()
        {
            btnPrintDirect.IsEnabled = true;
        }
        private void btnPrintDirect_Click(object sender, RoutedEventArgs e)
        {
            btnPrintDirect.IsEnabled = false;
            PrintDialog pdlg = new PrintDialog();

            m_timerToEnableButton = new Timer(TestTimerCallback, null, 3000, Timeout.Infinite);
        }
        public void TestTimerCallback(Object state)
        {
            m_timerToEnableButton.Dispose();
            Dispatcher.BeginInvoke(new EnableButtonMethod(EnableButton));
        }
        private void cBox_All_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
