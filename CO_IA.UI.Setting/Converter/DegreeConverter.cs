using System;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.UI.Setting.Converter
{
    public class DegreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && parameter != null)
            {
                if ((bool)value)
                {
                    return parameter.ToString() == "TRUE" ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return parameter.ToString() == "TRUE" ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
   
}
