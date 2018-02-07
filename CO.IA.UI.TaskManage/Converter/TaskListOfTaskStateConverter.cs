using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CO_IA.Data.TaskManage;

namespace CO.IA.UI.TaskManage.Converter
{
    public class TaskListOfTaskStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            switch (value.ToString())
            {
                case "0":
                    return "进行中";
                case "1":
                    return "已完成";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "进行中":
                    return 0;
                case "已完成":
                    return 1;
                default:
                    return null;
            }
        }
    }
}
