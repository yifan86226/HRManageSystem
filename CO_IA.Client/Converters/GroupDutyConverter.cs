using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CO_IA.Client.Converters
{
    
    /// <summary>  
    /// 传入职责 duty 05为监测组
    /// </summary>
    public class GroupDutyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var orginfo = (PP_OrgInfo)value;
            if(orginfo==null)
                return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user.png"; //jcc_offline
            if (orginfo.DUTY.IndexOf("01") >= 0)
            {
                return "pack://application:,,,/CO_IA.Themes;component/Images/Area/zhzx.png";
            }
            if (orginfo.DUTY.IndexOf("05") >= 0)
            {
                if (orginfo.OnLine == null)
                {
                    return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/jcc.png";
                }
                if (orginfo.OnLine.Value)
                    return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/jcc_online.png";
                else
                    return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/jcc_offline.png";
            }
            else
            {
                if (orginfo.OnLine == null)
                    return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user.png";
                if (orginfo.OnLine.Value)
                    return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user_online.png";
                else
                    return @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user_offline.png";
            }
            
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>  
    /// 传入职责 duty 05为监测组
    /// </summary>
    public class GroupStateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string url = "pack://application:,,,/CO_IA.Themes;component/Images/Group/user.png";
            if (values[0] is string)
            {
                
                string duty = (string)values[0];
                bool? online = (bool?)values[1];
                if (duty.IndexOf("01") >= 0)
                {
                    url = "pack://application:,,,/CO_IA.Themes;component/Images/Area/zhzx.png";
                }
                else
                if (duty.IndexOf("05") >= 0)
                {
                    if (online == null)
                    {
                        url = @"pack://application:,,,/CO_IA.Themes;component/Images/Group/jcc.png";
                    }
                    else
                    if (online.Value)
                        url = @"pack://application:,,,/CO_IA.Themes;component/Images/Group/jcc_online.png";
                    else
                        url = @"pack://application:,,,/CO_IA.Themes;component/Images/Group/jcc_offline.png";
                }
                else
                {
                    if (online == null)
                        url = @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user.png";
                    else
                    if (online.Value)
                        url = @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user_online.png";
                    else
                        url = @"pack://application:,,,/CO_IA.Themes;component/Images/Group/user_offline.png";
                }
            }
            if (url == "")
                return null;
            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(url as string));
            return bitmapImage;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
