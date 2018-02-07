using CO_IA.Types;
using AT_BC.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.UI.Screen.Converter
{
    public class ActivityStageTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var stage = (ActivityStage)value;
            return stage.GetDisplayNameFromEnumDisplayNameAttribute().ToString() + "阶段";                        
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
