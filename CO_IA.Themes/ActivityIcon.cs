using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace CO_IA.Themes
{
    public static class ActivityIcon
    {
        public const string ConferenceKey="ActivityType.Conference";
        public const string DrillKey="ActivityType.Drill";
        public const string EmergencyKey="ActivityType.Emergency";
        public const string ExamKey="ActivityType.Exam";
        public const string MajorDisturbanceKey="ActivityType.MajorDisturbance";
        public const string RecreationalActivitiesKey="ActivityType.RecreationalActivities";
        public const string OtherKey="ActivityType.Other";
        public const string DefaultKey="defaultActivity";

        public static ImageSource Conference
        {
            get
            {
                return resourceDic[ConferenceKey] as ImageSource;
            }
        }

        public static ImageSource Drill
        {
            get
            {
                return resourceDic[DrillKey] as ImageSource;
            }
        }

        public static ImageSource Emergency
        {
            get
            {
                return resourceDic[EmergencyKey] as ImageSource;
            }
        }

        public static ImageSource Exam
        {
            get
            {
                return resourceDic[ExamKey] as ImageSource;
            }
        }

        public static ImageSource MajorDisturbance
        {
            get
            {
                return resourceDic[MajorDisturbanceKey] as ImageSource;
            }
        }
        public static ImageSource RecreationalActivities
        {
            get
            {
                return resourceDic[RecreationalActivitiesKey] as ImageSource;
            }
        }
        public static ImageSource Other
        {
            get
            {
                return resourceDic[OtherKey] as ImageSource;
            }
        }
        public static ImageSource Default
        {
            get
            {
                return resourceDic[DefaultKey] as ImageSource;
            }
        }

        public static System.Windows.ResourceDictionary resourceDic=new System.Windows.ResourceDictionary();
        static ActivityIcon()
        {
            resourceDic.Source = new Uri(@"\CO_IA.Themes;component\Style.xaml", UriKind.RelativeOrAbsolute);
        }

        public static ImageSource GetActivityIcon(string activityType)
        {
            foreach (object key in resourceDic.Keys)
            {
                if (activityType.Equals(key))
                {
                    ImageSource imageSource = resourceDic[key] as ImageSource;
                    if (imageSource != null)
                    {
                        return imageSource;
                    }
                }
            }
            return Default;
        }
    }
}
