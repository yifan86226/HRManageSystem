#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：活动阶段到布尔值转换器定义,如果活动为进行状态返回true,否则为未知状态返回false
 * 日 期 ：2016-08-11
 ***************************************************************#@#***************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Client.Converters
{
    /// <summary>
    /// 活动阶段到布尔值转换器定义,如果活动为进行状态返回true,否则为未知状态返回false
    /// </summary>
    public class ActivityStageToBoolConverter : System.Windows.Data.IValueConverter
    {
        /// <summary>
        /// 转换方法
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is CO_IA.Types.ActivityStage)
            {
                if ((CO_IA.Types.ActivityStage)value== Types.ActivityStage.None)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
