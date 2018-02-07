using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace CO_IA.UI.StationPlan.Converter
{
    public class FreqUseConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.White);
            }
            else
            {
                Usage use;
                if (Enum.TryParse(value.ToString(), out use))
                {
                    switch (use)
                    {
                        case Usage.None:
                        case Usage.Other:
                            return new SolidColorBrush(Colors.White);
                        case Usage.Lawful:
                            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF89F589"));
                        case Usage.UnLawful:
                            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF97272"));
                        case Usage.Applied:
                            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF5F58F"));
                        default:
                            return new SolidColorBrush(Colors.White);
                    }
                }
                return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
