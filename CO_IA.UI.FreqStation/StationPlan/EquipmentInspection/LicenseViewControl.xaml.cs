using CO_IA.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace CO_IA.UI.FreqStation.StationPlan
{
    /// <summary>
    /// LicenseViewDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LicenseViewControl : UserControl
    {
        public LicenseViewControl()
        {
            InitializeComponent();
        }

        public void InitQRCodeProperty(double left, double top, double height, double width)
        {
            Canvas.SetTop(_borderqriamge, top);
            Canvas.SetLeft(_borderqriamge, left);
            this._borderqriamge.Height = height;
            this._borderqriamge.Width = width;
        }


        public void AddLicenseItem(LicenseItem licenseItem)
        {
            LicenseItemControl licenseitem = new LicenseItemControl();
            licenseitem.IsSelect = false;
            licenseitem.PropertyName = licenseItem.PropertyName;
            licenseitem.PropertyValue = licenseItem.PropertyValue;
            Canvas.SetLeft(licenseitem, licenseItem.Left);
            Canvas.SetTop(licenseitem, licenseItem.Top);
            _canvasPanel.Children.Add(licenseitem);
        }

        public void CreateQRCode(byte[] qrbytes)
        {
            ImageSourceConverter converter = new ImageSourceConverter();
            _qrCodeImage.Source = (ImageSource)converter.ConvertFrom(qrbytes);
        }
    }
}
