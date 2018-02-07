using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace CO_IA.Client.Converters
{

    public class ActivityTypeIDToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource imageSource = null;
            if (value != null)
            {
                imageSource = ActivityIconDictionary.GetIcon(value.ToString());
            }
            if (imageSource == null)
            {

            }
            return imageSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
