using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.PersonSchedule
{
    class VehicleTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (int)value;
            if (type == 0)
                return 1;
            if (type == 1)
                return 2;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (int)value;
            if (type == 0)
                return -1;
            if (type == 1)
                return 0;
            if (type == 2)
                return 1;
            return -1;
        }
    }
    class VehicleTypeConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (int)value;
            if (type == 0)
                return "非监测车";
            if (type == 1)
                return "监测车";
            return "无";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (int)value;
            if (type == 0)
                return -1;
            if (type == 1)
                return 0;
            if (type == 2)
                return 1;
            return -1;
        }
    }
}
