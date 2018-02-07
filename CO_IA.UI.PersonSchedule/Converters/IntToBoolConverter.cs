#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：int到bool转换类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.UI.PersonSchedule
{
    /// <summary>  
    /// 0-非监测组，1-监测组
    /// </summary>
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (int)value;
            return type == 0 ? false: true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (bool)value;
            return type == false ? 0 : 1;
        }
    }
}
