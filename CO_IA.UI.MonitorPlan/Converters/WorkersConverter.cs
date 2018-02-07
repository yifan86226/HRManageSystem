using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace CO_IA.UI.MonitorPlan.Converters
{
    public class WorkersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is List<string>))
            {
                return value;
            }
            else
            {
                var list = value as List<string>;
                if (list.Count == 0)
                {
                    return null;
                }
                var rstMsg = string.Empty;
                if (parameter != null)
                {
                    rstMsg = parameter.ToString();
                }
                foreach (string person in list)
                {
                    rstMsg += person + ", ";
                }
                return rstMsg;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
