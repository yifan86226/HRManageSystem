#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：行政区域名称转换器,支持省市级名称转换
 * 日 期 ：2016-08-16
 ***************************************************************#@#***************************************************************/
#endregion
using PT.Profile.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    /// <summary>
    /// 行政区域名称转换器,支持省市级名称转换
    /// </summary>
    public class DistrictNameConverter : IValueConverter
    {
        /// <summary>
        /// 将省市编码转换为对应的名称
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string key = value as string;
                DistrictInfo result;
                if (Utility.DistrictMapping.TryGetValue(key, out result))
                {
                    return result.Name;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
