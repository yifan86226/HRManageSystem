using CO_IA.Data;
using System;
using System.Windows.Data;

namespace CO_IA.UI.FreqStation.Converter
{
    public class AssignFreqConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
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
