using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using CO_IA.Data;
using CO_IA.Data.Monitor;

namespace CO_IA.UI.MonitorPlan.Converters
{
    public class WorkerGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value as string))
            {
                return value;
            }
            PP_OrgInfo getOrginfo = PrototypeDatas.GetPP_OrgInfo(value.ToString());
            List<PP_PersonInfo> personList = PrototypeDatas.GetPersonList(value.ToString());
            string personDisplay = string.Empty;
            personDisplay = getOrginfo.NAME;
            if (personList.Count > 0)
            {
                personDisplay += "{";
                foreach (PP_PersonInfo personInfo in personList)
                {
                    personDisplay += personInfo.NAME + ",";
                }
                personDisplay = personDisplay.Substring(0, personDisplay.Length - 1);
                personDisplay += "}";
            }
            return personDisplay;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
