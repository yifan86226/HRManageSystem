using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Printing;
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

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// PrintPreviewDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PrintPreviewDialog : Window
    {
        List<byte[]> images = new List<byte[]>();

        public PrintPreviewDialog(WrapPanel panel)
        {
            InitializeComponent();
            ControlToImage(panel);
            this.Loaded += PrintPreviewDialog_Loaded;
        }

        void PrintPreviewDialog_Loaded(object sender, RoutedEventArgs e)
        {
            //Window owner = LayoutHelper.FindParentObject<Window>(this);
            //PrintHelper.ShowPrintPreviewDialog(owner, (IPrintableControl)this.gridControl.View, "Grid Document");
        }


        //控件转换成图片
        private void ControlToImage(WrapPanel _contentPanel)
        {
            foreach (FrameworkElement item in _contentPanel.Children)
            {
                RenderTargetBitmap bitmapRender = new RenderTargetBitmap((int)item.ActualWidth, (int)item.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bitmapRender.Render(item);

                PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(bitmapRender));

                MemoryStream memoryStream = new MemoryStream();
                pngEncoder.Save(memoryStream);

                //var bmp = new BitmapImage();
                //bmp.BeginInit();
                //bmp.StreamSource = new MemoryStream(memoryStream.ToArray());
                //bmp.EndInit();

                byte[] bytearray = null;
                memoryStream.Position = 0;
                using (BinaryReader br = new BinaryReader(memoryStream))
                {
                    bytearray = br.ReadBytes((int)memoryStream.Length);
                }

                images.Add(bytearray);


                memoryStream.Flush();
                memoryStream.Close();
            }

            gridControl.ItemsSource = images;
        }
    }
}
