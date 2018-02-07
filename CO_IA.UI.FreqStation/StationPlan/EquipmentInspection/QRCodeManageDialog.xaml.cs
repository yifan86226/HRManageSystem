using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using System.Drawing.Imaging;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// QRCodeManageDialog.xaml 的交互逻辑
    /// </summary>
    public partial class QRCodeManageDialog : Window
    {
        private bool isnew;

        public event Action<List<string>, byte[]> UpdateQRCodeSource;

        public List<string> SelectFields
        {
            get;
            set;
        }

        private byte[] QRImageSource
        {
            get;
            set;
        }

        public QRCodeManageDialog(List<string> selectfields, byte[] imagesource)
        {
            InitializeComponent();
            SelectFields = selectfields;
            QRImageSource = imagesource;
            lstFields.ItemsSource = FreqStationHelper.Fields;
            InitQRCode();
        }

        private void InitQRCode()
        {
            if (SelectFields != null && SelectFields.Count > 0)
            {
                foreach (string item in lstFields.ItemsSource as List<string>)
                {
                    string field = item.Replace(" ", "");
                    if (SelectFields.Contains(field))
                    {
                        lstFields.SelectedItems.Add(item);
                    }
                }

                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                imgQRcode.Source = (ImageSource)imageSourceConverter.ConvertFrom(QRImageSource);
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            isnew = true;
            if (lstFields.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择要生成二维码的字段");
            }
            else
            {
                SelectFields = new List<string>();
                StringBuilder qrcontent = new StringBuilder();
                foreach (object item in lstFields.SelectedItems)
                {
                    string filed = item.ToString().Replace(" ", "");
                    qrcontent.AppendLine(filed);
                    SelectFields.Add(filed);
                }

                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(qrcontent.ToString(), out qrCode);

                GraphicsRenderer render = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), System.Drawing.Brushes.Black, System.Drawing.Brushes.White);
                MemoryStream memoryStream = new MemoryStream();
                render.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

                byte[] bytearray = null;
                memoryStream.Position = 0;
                using (BinaryReader br = new BinaryReader(memoryStream))
                {
                    bytearray = br.ReadBytes((int)memoryStream.Length);
                }

                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                imgQRcode.Source = (ImageSource)imageSourceConverter.ConvertFrom(bytearray);

                QRImageSource = bytearray;
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateQRCodeSource != null)
            {
                UpdateQRCodeSource(SelectFields, QRImageSource);
            }
            if (QRImageSource != null)
            {
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //if (isnew)
            //{
            //    if (UpdateQRCodeSource != null)
            //    {
            //        UpdateQRCodeSource(SelectFields, QRImageSource);
            //    }
            //}
        }
    }
}
