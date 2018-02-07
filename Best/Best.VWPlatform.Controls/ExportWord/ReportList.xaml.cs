using Best.VWPlatform.Controls.Container;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Best.VWPlatform.Controls.ExportWord
{
    /// <summary>
    /// ReportList.xaml 的交互逻辑
    /// </summary>
    public partial class ReportList : UserControl
    {
        #region 字段
        private string _filePath;
        #endregion

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public PopupWindow PopWindow
        {
            get;
            set;
        }
        
        public ReportList()
        {
            InitializeComponent();
        }

        private void OnBtnOpenClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                FilePath = button.Tag.ToString();
                System.Diagnostics.Process.Start(FilePath);
                //OpenWordFile(FilePath);
                if (PopWindow != null && PopWindow.Visibility == System.Windows.Visibility.Visible)
                    PopWindow.Close();
            }
        }

        private void OpenWordFile(string fileName)
        {
            string winwordPath = "";

            // 判断系统中是否已经有 Word 实例在运行。
            Process[] wordProcesses = Process.GetProcessesByName("winword");
            foreach (Process process in wordProcesses)
            {
                Debug.WriteLine(process.MainWindowTitle);
                winwordPath = process.MainModule.FileName;        // 如果有的话获得 Winword.exe 的完全限定名称。
                break;
            }

            Process wordProcess = new Process();

            if (winwordPath.Length > 0)    // 如果有 Word 实例在运行，使用 /w 参数来强制启动新实例，并将文件名作为参数传递。
            {
                wordProcess.StartInfo.FileName = winwordPath;
                wordProcess.StartInfo.UseShellExecute = false;
                wordProcess.StartInfo.Arguments = fileName + " /w";
            }
            else                            // 如果没有 Word 实例在运行，还是
            {
                wordProcess.StartInfo.FileName = fileName;
                wordProcess.StartInfo.UseShellExecute = true;
            }

            wordProcess.Start();
        }
    }
}
