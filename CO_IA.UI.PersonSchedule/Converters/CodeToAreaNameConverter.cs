#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：code 到 地区名称转换类
 * 日  期：2016-11-09
 * ********************************************************************************/
#endregion
using PT.Profile.Business;
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
    class CodeToAreaNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (string)value;
            if (type != null)
            {

                DistrictInfo rootDistrict = PT.Profile.Business.DistrictHelper.DistrictInfos.FindItemByCode(type);
                //NameValueDict nvd = setting.pdsArea.FindDictByName(textBox_DeptCode.Text, false);
                if (rootDistrict != null)
                {
                    return rootDistrict.Name + "无委";
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
