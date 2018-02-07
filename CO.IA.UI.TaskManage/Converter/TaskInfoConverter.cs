using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using CO_IA.Data.TaskManage;

namespace CO.IA.UI.TaskManage.Converter
{
    public class TaskInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            string urgencyContent = "";
            string getchildid = value.ToString();
            TaskInfo[] source = TaskHelper.GetTaskInfo(getchildid);
            if (source.Count() > 0)
            {
               string  urgency = source.FirstOrDefault().URGENCY.ToString();
                switch (urgency)
                {
                    case "0":
                        urgencyContent = "一般";;
                        break;
                    case "1":
                        urgencyContent = "紧急";;
                        break;
                }
            }
            DisturbTaskInfo[] disSource = TaskHelper.GetDisturbTaskInfo(getchildid);
            if (disSource.Count() > 0)
            {
                string urgency = disSource.FirstOrDefault().DISTRUBLEVEL.ToString();
                switch (urgency)
                {
                    case "0":
                        urgencyContent = "一般"; ;
                        break;
                    case "1":
                        urgencyContent = "轻微"; ;
                        break;
                    case "2":
                        urgencyContent = "严重"; 
                        break;
                }
            }
            return urgencyContent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
