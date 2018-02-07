using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace CO_IA.UI.Statistic.Converter
{

    [ValueConversion(typeof(string), typeof(string))]
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {

            string strValue = value.ToString();

            if (strValue == "未分类")
                return "Red";

            return "Black";

        }

        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            return "";
        }
    }
}
