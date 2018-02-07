using CO_IA.Data;
using GS_MapBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.ActivityManage
{
    public class LocationLGConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ActivityPlaceLocation activityPlaceLocation = value as ActivityPlaceLocation;
            if (activityPlaceLocation != null)
            {
                ClientUtile client = ClientUtile.Create();
                double[] a = client.DecimalDegreeToDegree(activityPlaceLocation.LocationLG);
                string str = "";
                if (a.Length > 0)
                {
                    str = a[0] + "°" + a[1] + "′" + a[2] + "″";
                }
                return str;
            }
            else
            {
                return "0";
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
