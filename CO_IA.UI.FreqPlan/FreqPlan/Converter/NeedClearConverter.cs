using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.FreqPlan.FreqPlan.Converter
{
    public class NeedClearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                NeedClearEunm clearenum;
                if (Enum.TryParse(value.ToString(), out clearenum))
                {
                    return clearenum == NeedClearEunm.NeedClear ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = true;
            if (bool.TryParse(value.ToString(), out result))
            {
                if (result)
                {
                    return NeedClearEunm.NeedClear;
                }
                else
                {
                    return NeedClearEunm.NotNeedClear;
                }
            }
            return NeedClearEunm.NeedClear;
        }
    }
}
