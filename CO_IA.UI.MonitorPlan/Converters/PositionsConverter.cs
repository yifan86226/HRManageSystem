using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CO_IA.Data;

namespace CO_IA.UI.MonitorPlan.Converters
{
    public class PositionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            string displayStr = string.Empty;
            ActivityPlaceLocation getLocationinfo = PrototypeDatas.GetLocationByGuid(value.ToString());
            return getLocationinfo.LocationName;

           
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
