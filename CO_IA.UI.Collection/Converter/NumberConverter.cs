using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// 转换器，是否需要清理
    /// </summary>
    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double vl = 0;
            if (double.TryParse(value.ToString(), out vl))
            {
                return vl / 2;
            }
            return vl;          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
