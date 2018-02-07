using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.FreqStation.Converter
{
    public class CheckStateConverter : IValueConverter
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
                            return "/CO_IA.UI.FreqStation;component/Images/UnCheck.png";
                        case Data.CheckStateEnum.Qualified:
                            return "/CO_IA.UI.FreqStation;component/Images/Qualified.png";
                        case Data.CheckStateEnum.UnQualified:
                            return "/CO_IA.UI.FreqStation;component/Images/UnQualified.png";
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
