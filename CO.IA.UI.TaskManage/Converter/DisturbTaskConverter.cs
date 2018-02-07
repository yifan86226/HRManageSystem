using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO.IA.UI.TaskManage.Converter
{
    public class DisturbTaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            //0语音、1数据、2噪音、3其他
            switch (value.ToString())
            {
                case "0":
                    return "语音";
                case "1":
                    return "数据";
                case "2":
                    return "噪音";
                case "3":
                    return "其他";
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
