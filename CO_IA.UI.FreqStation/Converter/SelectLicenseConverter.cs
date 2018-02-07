using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace CO_IA.UI.FreqStation.Converter
{
    public class SelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new SolidColorBrush(Colors.White);
            else
            {
                bool bvalue;
                if (bool.TryParse(value.ToString(), out  bvalue))
                {
                    if (bvalue) //选中
                    {
                        return new SolidColorBrush(Colors.LightBlue);
                    }
                    else
                    {
                        return new SolidColorBrush(Color.FromArgb(1, 1, 1, 1));
                    }
                }
                return new SolidColorBrush(Color.FromArgb(1, 1, 1, 1));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
