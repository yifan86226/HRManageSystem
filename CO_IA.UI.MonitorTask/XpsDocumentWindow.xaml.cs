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
using System.Windows.Xps.Packaging;

namespace CO_IA.UI.MonitorTask
{
    /// <summary>
    /// XpsDocumentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class XpsDocumentWindow : Window
    {
        public XpsDocumentWindow()
        {
            InitializeComponent();
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
            this.Loaded += XpsDocumentWindow_Loaded;
        }

        private void XpsDocumentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var findToolbar = this.documentViewer.Template.FindName("PART_FindToolBarHost", this.documentViewer);
            if (findToolbar is UIElement)
            {
                (findToolbar as UIElement).Visibility = System.Windows.Visibility.Collapsed;
            }
            var toolBars = AT_BC.Common.VisualTreeHelperExtension.GetChildObjects<ToolBar>(this.documentViewer);
            if (toolBars != null)
            {
                foreach (var toolBar in toolBars)
                {
                    var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
                    if (overflowGrid != null)
                    {
                        overflowGrid.Visibility = Visibility.Collapsed;
                    }
                    var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
                    if (mainPanelBorder != null)
                    {
                        mainPanelBorder.Margin = new Thickness(0);
                    }
                }
            }
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

        public void OpenXpsDocument(string filePath)
        {
            using (XpsDocument xpsDoc = new XpsDocument(filePath, System.IO.FileAccess.Read))
            {
                this.documentViewer.Document = xpsDoc.GetFixedDocumentSequence();
            }
        }
    }
}
