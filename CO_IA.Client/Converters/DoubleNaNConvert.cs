using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    public class DoubleNaNConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (Double.IsNaN(double.Parse(value.ToString())))
                {
                    return null;
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    return null;
                }

                double result;

                if (double.TryParse(value.ToString(), out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            return value;
        }
    }
}
