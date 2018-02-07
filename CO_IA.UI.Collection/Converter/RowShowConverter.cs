using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.UI.Collection.Converter
{
    /// <summary>
    /// 行是否显示
    /// </summary>
    public class RowShowConverter : IValueConverter
    {
        public static string ShowType = "ALL";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ShowType.Equals("ALL"))
            {
                return Visibility.Visible;
            }

            if (value == null)
            {
                return Visibility.Collapsed;
            }

            var ar = (Data.Collection.AnalysisResult)value;
            return ShowType.Contains(ar.FreqType.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
