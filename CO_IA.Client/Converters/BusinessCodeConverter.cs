using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    public class BusinessCodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                //BusinessType type = Utility.BusinessTypes.FirstOrDefault(r => r.Guid == value.ToString());
                //if (type == null)
                //{
                //    return type;
                //}
                //return type.Name;

                return value;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
