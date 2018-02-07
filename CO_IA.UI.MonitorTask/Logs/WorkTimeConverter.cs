using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.MonitorTask.Logs
{
    public class WorkTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                WorkLog log = value as WorkLog;
                return string.Format("{0} - {1}",
                    log.WorkDateFrom.ToString("yyyy年MM月dd HH:mm"),
                    log.WorkDateTo.ToString("yyyy年MM月dd HH:mm"));
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
