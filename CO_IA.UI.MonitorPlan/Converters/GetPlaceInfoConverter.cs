using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CO_IA.Data;
using CO_IA.Data.MonitorPlan;

namespace CO_IA.UI.MonitorPlan.Converters
{
    public class GetPlaceInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            //GroupAndLocation group = value as GroupAndLocation;
            ActivityPlaceInfo getOrgPlaceinfo = PrototypeDatas.GetPlaceInfo(value.ToString());
            return getOrgPlaceinfo.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
