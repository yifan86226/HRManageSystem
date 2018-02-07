using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CO_IA.Client
{
    public static class ActivityIconDictionary
    {
        private static Dictionary<string, ImageSource> iconDictionary = new Dictionary<string, ImageSource>();
        static ActivityIconDictionary()
        {
            
        }

        public static void RegisterIcon(string activityType, ImageSource icon)
        {
            iconDictionary[activityType] = icon;
        }

        public static ImageSource GetIcon(string activityType)
        {
            ImageSource icon;
            iconDictionary.TryGetValue(activityType, out icon);
            return icon;
        }
    }

}
