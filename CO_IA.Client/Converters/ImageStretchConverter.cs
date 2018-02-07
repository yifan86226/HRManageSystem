using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace CO_IA.Client.Converters
{
    public class ImageStretchConverter : System.Windows.Data.IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is ImageSource && values[1] is double && values[2] is double)
            {
                var limitWidth = (double)values[1];
                var limitHeight = (double)values[2];
                var imageSource = values[0] as ImageSource;
                if (limitWidth>0 && limitHeight>0 && imageSource.Width <= limitWidth && imageSource.Height <= limitHeight)
                {
                    return Stretch.None;
                }
            }
            return Stretch.Uniform;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
