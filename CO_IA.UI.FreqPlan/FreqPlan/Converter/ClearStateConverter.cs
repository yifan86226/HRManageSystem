using AT_BC.Data;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.FreqPlan.FreqPlan.Converter
{
    public class ClearStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                switch ((string)value)
                {

                    case "0":
                        return "未作处理";
 
                    case "1":
                        return "清理成功";
                    case "2":
                        return "清理失败";
                    default :
                        return value;
                
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value != null)
                {
                    switch ((string)value)
                    {
                        case "未作处理":
                            return "0";

                        case "清理成功":
                            return "1";
                        case "清理失败":
                            return "2";
                        default:
                            return value;
                    }
                }
              
            }
            catch
            { }

            return null;
        }
    }
}
