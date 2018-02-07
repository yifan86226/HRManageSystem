using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Best.WMonitor.Common;
using System.Windows.Data;
using System.Windows;
using System.Globalization;
using System.Windows.Media;
using CO_IA.Data.Collection;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace CO_IA.UI.Collection.Converter
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
                        v = MathNoRound(System.Convert.ToDouble(value), 0);
                        break;
                }
            }
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double v;
            if (!double.TryParse(value.ToString(), out v))
                return null;

            if (parameter != null)
                _converterUnit = parameter.ToString();

            if (!string.IsNullOrWhiteSpace(_converterUnit))
            {
                switch (_converterUnit.ToUpper())
                {
                    case "GHZ":
                        v = WMonitorUtile.ConvertFreqValue("ghz", "hz", v);
                        break;
                    case "MHZ":
                        v = WMonitorUtile.ConvertFreqValue("mhz", "hz", v);
                        break;
                    case "KHZ":
                        v = WMonitorUtile.ConvertFreqValue("khz", "hz", v);
                        break;
                    case "HZ":
                        break;
                }
            }
            return v;
        }
        /// <summary>
        /// 双精度数转换，忽略自动四舍五入
        /// </summary>
        /// <param name="pValue">待转换的双精度值</param>
        /// <param name="pDigits">小数位数</param>
        /// <returns>返回双精度值</returns>
        public double MathNoRound(double pValue, uint pDigits)
        {
            double d = Math.Pow(10, pDigits);
            if (pDigits == 0)
                return pValue > 0 ? Math.Floor(pValue) : Math.Ceiling(pValue);
            return pValue > 0 ? Math.Floor(pValue * d) / d : Math.Ceiling(pValue * d) / d;
        }
    }

    public class IsCheckedToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        #endregion
    }

    [ValueConversion(typeof(AnalysisResult), typeof(Brush))]
    public class BackGroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AnalysisResult analysisResult = value as AnalysisResult;
            if (analysisResult == null)
                return Brushes.Transparent;
            if (SignalTypeEnum.空闲 == analysisResult.FreqType)
                return Brushes.Transparent;
            if (SignalTypeEnum.已占 == analysisResult.FreqType)
                return Brushes.IndianRed;
            else if (SignalTypeEnum.清理 == analysisResult.FreqType)
                return Brushes.Orange;
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        //public override object ProvideValue(IServiceProvider serviceProvider)
        //{
        //    return this;
        //}
    }

    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            AnalysisResult analysisResult = value as AnalysisResult;

            if (analysisResult.IsSend == true)
            {
                BitmapImage bi = new BitmapImage(new Uri("Images/right_24x24.png", UriKind.Relative));
                return bi;
            }
            else
            {
                BitmapImage bi = new BitmapImage(new Uri("Images/wrong_24x24.png", UriKind.Relative));
                return bi;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        //public override object ProvideValue(IServiceProvider serviceProvider)
        //{
        //    return this;
        //}
    }
}
