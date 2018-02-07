using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.UI.Collection
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(int))
            {
                int temp = 0;
                if (Int32.TryParse(value.ToString(), out temp))
                {
                    return temp;
                }
                else
                    return 0;

            }
            else if (targetType == typeof(double))
            {

                double temp = 0.0;
                if (double.TryParse(value.ToString(), out temp))
                {
                    return temp;
                }
                else
                    return 0.0;
            }
            return 0;
        }
    }

    public class VissbilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //var sender = value as DevExpress.Xpf.Grid.GridControl;
            //if (sender == null)
            //    return Visibility.Collapsed;

            var list = (ObservableCollection<Data.Collection.AnalysisResult>)value;
            if (list != null && list.Count > 0)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
