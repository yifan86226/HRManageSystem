using CO_IA.Client;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.ActivityManage
{
    public class GetPlaceImageConverter : System.Windows.Data.IValueConverter
    {
        private static System.Windows.Media.Imaging.BitmapImage defaultImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(@"/CO_IA.UI.ActivityManage;component/Images/defaultPlace.png", UriKind.RelativeOrAbsolute));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var bytes=value as byte[];
            if (bytes != null)
            {
                try
                {
                    var imageSource = ClientHelper.LoadImageFromBytes(bytes);
                    if (imageSource != null)
                    {
                        return imageSource;
                    }
                }
                catch
                {
                }
            }
            return defaultImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
