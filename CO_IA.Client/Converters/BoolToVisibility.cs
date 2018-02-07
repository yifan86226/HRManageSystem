#region 文件描述
/**********************************************************************************
 * 创建人：Niext
 * 摘  要：Bool类型转成Visibility
 * 日  期：2016-09-2
 * ********************************************************************************/
#endregion
using System;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            bool bresult = true;
            if (bool.TryParse(value.ToString(), out bresult))
            {
                if (bresult)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
