using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.PersonSchedule
{
    public class ToLongDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt;
            if (value == null || !DateTime.TryParse(value.ToString(), out dt))
                return null;

            return dt.ToString("yyyy-MM-dd hh:mm");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return value;
            DateTime dt = DateTime.Now;

            if (DateTime.TryParse(value.ToString(), out dt))
                return dt;
            return dt;
        }
    }
    public class ToShortLongDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt;
            if (value == null || !DateTime.TryParse(value.ToString(), out dt))
                return null;

            return dt.ToString("yyyy-MM-dd");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return value;
            DateTime dt = DateTime.Now;

            if (DateTime.TryParse(value.ToString(), out dt))
                return dt;
            return dt;
        }
    }
    public class TimeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int type = (int)value;
            if (type == 0)
                return "多天时间段";
            if (type == 1)
                return "单天时间段";
            return "多天时间段";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string v = (string)value;
            if (v == "单天时间段")
                return "1";
            else
                return "0";
        }
    }
    public class DoubleDateTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt1, dt2;
            if (values[0] == null || !DateTime.TryParse(values[0].ToString(), out dt1))
                return "";
            if (values[1] == null || !DateTime.TryParse(values[1].ToString(), out dt2))
                return "";
            string s1 = dt1.ToString("yyyy-MM-dd");
            string s2 = dt2.ToString("yyyy-MM-dd");
            if (s1 == s2)
                return s1;
            return s1 + " 至 " + s2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
