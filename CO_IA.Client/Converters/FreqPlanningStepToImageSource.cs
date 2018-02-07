using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client.Converters
{
    public class FreqPlanningStepToImageSource : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is CO_IA.Types.FreqPlanningStep)
            {
                return System.Windows.Application.Current.TryFindResource(string.Format("FreqPlanningStep.{0}", value));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
