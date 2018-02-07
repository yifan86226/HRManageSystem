﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO.IA.UI.TaskManage.Converter
{
    public class TaskStateConverter : IValueConverter
    {
       public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
       {
           {
               if (value == null || parameter == null)
                   return false;

               string checkvalue = value.ToString();

               string targetvalue = parameter.ToString();

               bool r = checkvalue.Equals(targetvalue,

               StringComparison.InvariantCultureIgnoreCase);

               return r;

           }

       }
       public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
       {
           if (value == null || parameter == null)
               return null;
           bool usevalue = (bool)value;
           if (usevalue)

               return parameter.ToString();
           return null;

       }
    }
   
}
