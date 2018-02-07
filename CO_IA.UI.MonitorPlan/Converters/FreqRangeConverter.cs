using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using AT_BC.Data;
using CO_IA.Data;
using CO_IA.Data.Monitor;

namespace CO_IA.UI.MonitorPlan.Converters
{
    public class FreqRangeConverter : IMultiValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (value != null)
        //    {
        //        Range<double> freqValue = value as Range<double>;
        //        return (freqValue.Little / 1000000).ToString() + "-" + (freqValue.Great / 1000000).ToString();
        //    }
        //    return null;
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2 || !(values[0] is List<FreqRange>))
            {
                return null;
            }
            var list = values[0] as List<FreqRange>;
            if (list.Count == 0)
            {
                return null;
            }
            var rstMsg = string.Empty;
            if (parameter != null)
            {
                rstMsg = parameter.ToString();
            }
            foreach (FreqRange freq in list)
            {
                rstMsg += freq.FreqFrom + "MHz ~ " + freq.FreqTo + "MHz ";
            }
            return rstMsg;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FreqRangeListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is List<FreqRange>))
            {
                return null;
            }
            var list = value as List<FreqRange>;
            if (list.Count == 0)
            {
                return null;
            }
            var rstMsg = string.Empty;
            if (parameter != null)
            {
                rstMsg = parameter.ToString();
            }
            foreach (FreqRange freq in list)
            {
                rstMsg += "["+freq.FreqFrom + "MHz ~ " + freq.FreqTo + "MHz ]";
            }
          
            return rstMsg;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class FreqRangeFreqConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                //SendParameter send = value as SendParameter;
                SendParameter send = new SendParameter();
                send.FreqStart = ((AT_BC.Data.Range<double>)(value)).Little;
                send.FreqEnd = ((AT_BC.Data.Range<double>)(value)).Great;
                return (send.FreqStart / 1000000).ToString() + "-" + (send.FreqEnd / 1000000).ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
