#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：int到图片转换类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    /// <summary>  
    /// 传入职责 duty 05为监测组
    /// </summary>
    public class DutyToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (string)value;
            if(type.IndexOf("05")>=0)
            {
                return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/jcc.png";
            }
            else
            {
                return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
