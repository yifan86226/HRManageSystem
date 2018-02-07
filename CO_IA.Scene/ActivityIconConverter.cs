using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Scene
{
    public class ActivityIconConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Activity)
            {
                Activity activity = value as Activity;
                System.Windows.Media.ImageSource source = CO_IA.Client.ClientHelper.LoadImageFromBytes(activity.Icon);
                if (source==null)
                {
                    source = CO_IA.Themes.ActivityIcon.GetActivityIcon(string.Format("ActivityType.{0}", activity.ActivityType));
                }
                return source;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
