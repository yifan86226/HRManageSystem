using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Best.VWPlatform.Common.Converters
{
    /// <summary>
    /// 频率值转换
    /// <remarks>
    /// 参数类型
    /// Convert
    ///     GHz - Hz 转 GHz
    ///     MHZ - Hz 转 MHz
    ///     KHZ - Hz 转 kHz
    ///     HZ
    /// ConvertBack
    ///     GHz - GHz 转 Hz
    ///     MHZ - MHz 转 Hz
    ///     KHZ - kHz 转 Hz
    ///     HZ
    /// </remarks>
    /// </summary>
    public class FreqValueConverter : IValueConverter
    {
        private string _converterUnit;

        public FreqValueConverter()
        {

        }

        public FreqValueConverter(string pUnit)
        {
            _converterUnit = pUnit;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object v = value;
            string t = parameter as string;
            if (t != null)
            {
                _converterUnit = t;
            }
            if (!string.IsNullOrWhiteSpace(_converterUnit))
            {
                switch (_converterUnit.ToUpper())
                {
                    case "GHZ":
                        v = WMonitorUtile.ConvertFreqValue("hz", "ghz", System.Convert.ToDouble(value));
                        break;
                    case "MHZ":
                        v = WMonitorUtile.ConvertFreqValue("hz", "mhz", System.Convert.ToDouble(value));
                        break;
                    case "KHZ":
                        v = WMonitorUtile.ConvertFreqValue("hz", "khz", System.Convert.ToDouble(value));
                        break;
                    case "HZ":
                        v = Utile.MathNoRound(System.Convert.ToDouble(value), 0);
                        break;
                }
            }
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object v = value;
            string t = parameter as string;
            if (t != null)
            {
                _converterUnit = t;
            }
            if (!string.IsNullOrWhiteSpace(_converterUnit))
            {
                switch (_converterUnit.ToUpper())
                {
                    case "GHZ":
                        v = WMonitorUtile.ConvertFreqValue("ghz", "hz", System.Convert.ToDouble(value));
                        break;
                    case "MHZ":
                        v = WMonitorUtile.ConvertFreqValue("mhz", "hz", System.Convert.ToDouble(value));
                        break;
                    case "KHZ":
                        v = WMonitorUtile.ConvertFreqValue("khz", "hz", System.Convert.ToDouble(value));
                        break;
                    case "HZ":
                        v = value;
                        break;
                }
            }
            return v;
        }

    }
}
