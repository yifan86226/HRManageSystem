using CO_IA.Data.ActivitySummarize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.ActivitySummarize
{
    public class FileSizeConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SummarizeDoc summarizeDoc = value as SummarizeDoc;
            double size = Math.Round(double.Parse(summarizeDoc.FILESIZE) / 1024.0, 1);
            return size;

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
