using CO_IA.Client;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.ActivityManage
{
    public class GetImagesConverter : System.Windows.Data.IValueConverter
    {
        private static System.Windows.Media.Imaging.BitmapImage defaultImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(@"/CO_IA.UI.ActivityManage;component/Images/defaultPlace.png", UriKind.RelativeOrAbsolute));
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ActivityPlaceLocation)
            {
                var placeInfo1 = value as ActivityPlaceLocation;
                if (placeInfo1 != null)
                {
                    try
                    {
                        var imageSource = ClientHelper.LoadImageFromBytes(placeInfo1.activityPlaceLocationImage[0].Image);
                        if (imageSource != null)
                        {
                            return imageSource;
                        }
                    }
                    catch
                    {
                    }
                }
               
            }
            var placeInfo = value as ActivityPlaceLocationImage;
            if (placeInfo != null)
            {
                try
                {
                    var imageSource = ClientHelper.LoadImageFromBytes(placeInfo.Image);
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
