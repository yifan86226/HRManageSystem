#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：int 到 visibility转换类
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
    /// int转换为Visibility状态，2-Visible 其他-Collapsed
    /// 0-组织机构，1-部门，2-用户
    /// </summary>
    class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (int)value;
            return type == 2 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
