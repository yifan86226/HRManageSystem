using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO.IA.UI.TaskManage.Converter
{
    public class DistrubLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            //2严重、0一般、1轻微，默认0一般
            switch (value.ToString())
            {
                case "0":
                    return "一般";
                case "1":
                    return "轻微";
                case "2":
                    return "严重";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
