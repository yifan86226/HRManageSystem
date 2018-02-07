using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.Statistic.Converter
{
    [ValueConversion(typeof(string), typeof(string))]
    public class DataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                StatisticDataSource data = value as StatisticDataSource;
                double count = data[parameter.ToString()];
                return count;
            }
            return 0;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
