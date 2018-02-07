#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：int到图片转换类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using CO_IA.Client;
using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CO_IA.UI.FreqPlan
{
    /// <summary>  
    /// 取得相关字典项目类别
    /// </summary>
    public class SecurityCodeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (string)value;

            foreach (SecurityClass sc in Utility.GetSecurityClasses())
            {

                if (sc.Guid == type)
                {
                    return sc.Name;
                    
                }
            }

            return value;
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (string)value;

            foreach (SecurityClass sc in Utility.GetSecurityClasses())
            {

                if (sc.Name == type)
                {
                    return sc.Guid;
                    
                }
            }

            return value;
        }
    }
}
