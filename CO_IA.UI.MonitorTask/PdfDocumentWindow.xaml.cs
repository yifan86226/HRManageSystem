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
using System.Windows.Shapes;

namespace CO_IA.UI.MonitorTask
{
    /// <summary>
    /// PdfDocumentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PdfDocumentWindow : Window
    {
        PdfReader pdfReader = new PdfReader();
        public PdfDocumentWindow()
        {
            InitializeComponent();
            this.winFormHost.Child = pdfReader;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            this.AdjustWindowArea();
        }

        public override void EndInit()
        {
            base.EndInit();
            this.AdjustWindowArea();
        }

        private void AdjustWindowArea()
        {
            Rect rect = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rect.Width;
            this.Height = rect.Height;
        }

        public void OpenPdfFile(string filePath)
        {

            pdfReader.OpenFile(filePath);
            //this.winFormHost.Child = pdfReader;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
