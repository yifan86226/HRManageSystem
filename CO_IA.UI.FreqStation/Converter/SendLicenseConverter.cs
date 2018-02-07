using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.FreqStation.Converter
{
    public class SendLicenseConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                CO_IA.Data.SendLicenseEnum sendlicense;
                if (Enum.TryParse(value.ToString(), out sendlicense))
                {
                    switch (sendlicense)
                    {
                        case Data.SendLicenseEnum.SendLicense:
                            return "/CO_IA.UI.FreqStation;component/Images/SendLicense.png";
                        case Data.SendLicenseEnum.UnSendLicense:
                            return "/CO_IA.UI.FreqStation;component/Images/UnSendLicense.png";
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
