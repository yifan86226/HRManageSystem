using Best.VWPlatform.Controls.Container;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace Best.VWPlatform.Controls.ExportWord
{
    public partial class ExportReport : INotifyPropertyChanged
    {
        #region 字段
        private PopupWindow _popupWindow;
        private ReportList _reportList;
        private List<ReportFile> _reportFiles;
        private bool _canExport;
        private bool _canCheck;
        #endregion

        #region 属性
        public bool CanExport
        {
            get { return _canExport; }
            set
            {
                _canExport = value;
                NotifyPropertyChanged("CanExport");
            }
        }

        public bool CanCheck
        {
            get { return _canCheck; }
            set
            {
                _canCheck = value;
                NotifyPropertyChanged("CanCheck");
            }
        }
        #endregion

        #region 事件
        public event Action Export;
        #endregion

        #region 构造函数
        public ExportReport()
        {
            InitializeComponent();
            DataContext = this;
            _reportList = new ReportList();
        }
        #endregion

        #region 公共方法

        public void AddReport(string pFile)
        {
            if (_reportFiles == null)
            {
                _reportFiles = new List<ReportFile>();
            }

            var pFileName = Path.GetFileNameWithoutExtension(pFile);
            _reportFiles.Add(new ReportFile { FileName = pFileName, FilePath = pFile });

            if (_reportFiles.Count > 0 && CanCheck == false)
            {
                CanCheck = true;
            }
        }

        public void Clear()
        {
            _popupWindow = null;
            //_reportList = null;
            //_reportFiles = null;
            //CanExport = false;
            //CanCheck = false;
        }
        #endregion

        #region 私有方法
        private void OnBtnExportClick(object sender, RoutedEventArgs e)
        {
            if (Export != null)
                Export();
        }

        private void OnBtnCheckClick(object sender, RoutedEventArgs e)
        {
            OpenReportList();
        }

        private void OpenReportList()
        {
            if (_popupWindow != null && _popupWindow.IsVisible)    
                return;

            _reportList.xReportList.ItemsSource = null;
            _reportList.xReportList.ItemsSource = _reportFiles;
            var rootVisual = Application.Current.MainWindow as FrameworkElement;
            _popupWindow = new PopupWindow("报表列表", string.Empty, _reportList);
            if (rootVisual != null)
                _popupWindow.Show(rootVisual.ActualWidth / 2 + 200, (rootVisual.ActualHeight - 200) / 2);
            _reportList.PopWindow = _popupWindow;
        }
        #endregion

        #region 变更通知
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class ReportFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
