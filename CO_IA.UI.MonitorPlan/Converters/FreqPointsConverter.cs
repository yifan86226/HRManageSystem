using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace CO_IA.UI.MonitorPlan.Converters
{
    public class FreqPointsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2 || !(values[0] is List<double>))
            {
                return null;
            }
            var list = values[0] as List<double>;
            if (list.Count == 0)
            {
                return null;
            }
            var rstMsg = string.Empty;
            if (parameter != null)
            {
                rstMsg = parameter.ToString();
            }
            foreach (double freq in list)
            {
                rstMsg += freq + "MHz,";
            }
            if (values[1].ToString() == "SignCollect")
            {
                rstMsg = "单频点监测" + rstMsg;
            }
            else
            {
                if (values[1].ToString() == "Rehearse")
                {
                    rstMsg = "预演监测频点" + rstMsg;
                }
                else
                {
                    if (values[1].ToString() == "SiteSafeguard")
                    {
                        rstMsg = "保障频点" + rstMsg;
                    }
                }
            }
            return rstMsg;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
