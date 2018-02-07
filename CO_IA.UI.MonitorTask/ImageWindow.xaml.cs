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
using System.Windows.Xps.Packaging;

namespace CO_IA.UI.MonitorTask
{
    /// <summary>
    /// XpsDocumentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow()
        {
            InitializeComponent();
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
            this.Left = 0;//设置位置
            this.Top = 0;
            Rect rect = SystemParameters.WorkArea;//获取工作区大小
            this.Width = rect.Width;
            this.Height = rect.Height;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public void LoadImage(BitmapImage image)
        {
            this.image.Source = image;//设置图像Source
        }
    }
}
