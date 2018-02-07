using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO.IA.UI.TaskManage.Converter
{
   public class TaskTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            switch (value.ToString())
            {
                case "0":
                    return "一般任务";
                case "1":
                    return "监测任务";
                case "2":
                    return "干扰任务";
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

