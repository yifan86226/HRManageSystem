using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Best.VWPlatform.Common.Converters
{
    public class PolarizationModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "垂直";

            switch (value.ToString())
            {
                case "horizontal":
                    return "水平";
                case "vertical":
                    return "垂直";
                case "circle":
                    return "圆极化";
                default:
                    return "垂直";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
