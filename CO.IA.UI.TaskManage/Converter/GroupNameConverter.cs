using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CO_IA.Data;

namespace CO.IA.UI.TaskManage.Converter
{
    public class GroupNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string))
            {
                return value;
            }
            PP_OrgInfo getOrginfo = TaskHelper.GetPP_OrgInfo(value.ToString());
            string personDisplay = getOrginfo.NAME;
            return personDisplay;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
