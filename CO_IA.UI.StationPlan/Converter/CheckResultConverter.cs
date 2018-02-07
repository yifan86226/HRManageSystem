using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.StationPlan.Converter
{
    public class CheckResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                CO_IA.Data.CheckStateEnum state;
                if (Enum.TryParse(value.ToString(), out state))
                {
                    switch (state)
                    {
                        case Data.CheckStateEnum.UnCheck:
                            return "未检测";
                        case Data.CheckStateEnum.Qualified:
                            return "通过检测";
                        case Data.CheckStateEnum.UnQualified:
                            return "未通过检测";
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
