using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.Scene.Converters
{
    public class FreqChangeReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                CO_IA.Data.CheckStateEnum state;
                if (Enum.TryParse(value.ToString(), out state))
                {
                    if (state != Data.CheckStateEnum.UnCheck)
                    {
                        return true;
                    }
                }
            }
             return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
