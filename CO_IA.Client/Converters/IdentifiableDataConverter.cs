using AT_BC.Data;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client.Converters
{
    public class IdentifiableDataConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string && parameter is System.Collections.IList)
            {
                string code = value.ToString();
                var list=parameter as System.Collections.IList;
                foreach (object obj in list)
                {
                    var data=obj as IIdentifiableData;
                    if (data != null && data.Guid == code)
                    {
                        return data.Value;
                    }
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
