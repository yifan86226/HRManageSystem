using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Best.VWPlatform.Common.Converters
{
    /// <summary>
    /// 信号性质数值转为对应的性质图标
    /// </summary>
    public class IntToSignalImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            int v;
            if (!int.TryParse(value.ToString(), out v))
                return null;
            var type = (SignalDescribe.SignalType)v;
            return SignalDescribe.GetSignalImage(type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
    /// <summary>
    /// 信号性质数值转为对应的性质文本描述
    /// </summary>
    public class IntToSignalToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            int v;
            if (!int.TryParse(value.ToString(), out v))
                return null;
            var type = (SignalDescribe.SignalType)v;
            return SignalDescribe.GetSignalText(type);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
