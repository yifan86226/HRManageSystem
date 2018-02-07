//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Data;

//namespace CO_IA.UI.PlanDatabase.Converter
//{
//    public class DateTimeToStringConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            if (value != null)
//            {

//                DateTime dt = (DateTime)value;

//                string str = dt.ToString("yyyy-MM-dd") + " " + dt.Hour + ":" + dt.Minute;
//                return str;
//            }
//            return value;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
