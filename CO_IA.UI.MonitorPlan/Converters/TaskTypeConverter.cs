using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace CO_IA.UI.MonitorPlan.Converters
{
    public class TaskTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }
            else
            {
                if (value.ToString() == "SignCollect")
                {
                    return "信号采集";
                }
                else
                {
                    if (value.ToString() == "Rehearse")
                    {
                        return "预演";
                    }
                    else
                    {
                        if (value.ToString() == "SiteSafeguard")
                        {
                            return "现场保障";
                        }
                        else
                        {
                            return value;
                        }
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
