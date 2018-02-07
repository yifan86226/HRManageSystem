using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
         {
            if (value == null)
            {
                return false;
            }
            else
            {
                bool result;
                if (bool.TryParse(value.ToString(), out result))
                {
                    return !result;
                }
                else
                {
                    return false;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                bool result;
                if (bool.TryParse(value.ToString(), out result))
                {
                    return !result;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
