using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO.IA.UI.TaskManage.Converter
{
    /// <summary>  
    /// 0-非监测组，1-监测组
    /// </summary>
    public class IntToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (int)value;
            if (type == 1)
            {
                return @"/CO.IA.UI.TaskManage;component/Images/jcc.png";
            }
            else
            {
                return @"/CO.IA.UI.TaskManage;component/Images/user3.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
