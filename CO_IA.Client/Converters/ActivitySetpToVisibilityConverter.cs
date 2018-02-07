using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    public class ActivitySetpToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is ActivityStep)
            {
                var uiFactory = Utility.GetUIFactory();
                if (uiFactory != null)
                {
                    var uiBuilder = uiFactory.GetUIBuilder(value as string);
                    if (uiBuilder.CanBuildStep((ActivityStep)parameter))
                    {
                        return Visibility.Visible;
                    }
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
