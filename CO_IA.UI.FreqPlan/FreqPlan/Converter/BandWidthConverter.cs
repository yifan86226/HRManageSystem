using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.FreqPlan.FreqPlan.Converter
{
    public class BandWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                string resultstr = string.Empty;
                string bandWidth = value as string;
                if (bandWidth.Contains("/"))
                { 
                    string[] bands = bandWidth.Split('/');
                    if (bands.Length>0)
                    {
                        for (int i = 0;i<bands.Length;i++)
                        {
                            double resultVal;
                            if (double.TryParse(bands[i], out resultVal))
                                resultstr +=(resultVal / 1000).ToString()+"/";
                        }
                        resultstr = resultstr.TrimEnd('/');
                    }
                }
                else
                {
                    double resultVal;
                    if (double.TryParse(bandWidth, out resultVal))
                        resultstr = (resultVal / 1000).ToString();
                }
                return resultstr;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
