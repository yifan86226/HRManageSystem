using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CO_IA.Client.Converters
{
    public class ImageSourceStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                double limit;
                double.TryParse(parameter.ToString(), out limit);
                if (value is ImageSource && limit>0)
                {
                    var imageSource = value as ImageSource;
                    if (imageSource.Width < limit && imageSource.Height < limit)
                    {
                        return Stretch.None;
                    }
                }
            }
            return Stretch.Uniform;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
