using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CO_IA.Client.Converters
{
    public class BytesToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is byte[])
            {
                var imageSource = ClientHelper.LoadImageFromBytes(value as byte[]);
                if (imageSource != null)
                {
                    return imageSource;
                }
            }
            if (parameter is string)
            {
                System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(parameter as string));
                return bitmapImage;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage newBitmapImage = (BitmapImage)value;
            byte[] bytearray = null;

            try
            {
                Stream smarket = newBitmapImage.StreamSource;

                if (smarket != null && smarket.Length > 0)
                {
                    //很重要，因为position经常位于stream的末尾，导致下面读取到的长度为0。
                    smarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(smarket))
                    {
                        bytearray = br.ReadBytes((int)smarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling
            }

            return bytearray;
        }
    }
}
