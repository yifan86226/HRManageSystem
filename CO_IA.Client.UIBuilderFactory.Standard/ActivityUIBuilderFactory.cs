using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace CO_IA.Client.UIBuilderFactory.Standard
{
    public class ActivityUIBuilderFactory : IActivityUIBuilderFactory
    {
        public virtual IActivityUIBuilder GetUIBuilder(string activityType)
        {
            if (activityType == ActivityTypeDef.MajorDisturbance)
            {
                return new MajorDisturbanceUIBuilder();
            }
            if (activityType == ActivityTypeDef.Exam)
            {
                return new ExamUIBuilder();
            }
            if (activityType == ActivityTypeDef.Emergency)
            {
                return new EmergencyUIBuilder();
            }
            return new StandardActivityUIBuilder();
        }

        public virtual System.Windows.Media.ImageSource GetImageSource(string activityType)
        {
            if (activityType == ActivityTypeDef.MajorDisturbance)
            {
                return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/MajorDisturbance.png", UriKind.RelativeOrAbsolute));
            }
            if (activityType == ActivityTypeDef.Exam)
            {
                return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/Exam.png", UriKind.RelativeOrAbsolute));
            }
            if (activityType == ActivityTypeDef.Emergency)
            {
                return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/Emergency.png", UriKind.RelativeOrAbsolute));
            }
            if (activityType == ActivityTypeDef.Conference)
            {
                return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/Conference.png", UriKind.RelativeOrAbsolute));
            }
            if (ActivityTypeDef.Drill == activityType)
            {
                return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/Drill.png", UriKind.RelativeOrAbsolute));
            }
            if (ActivityTypeDef.RecreationalActivities == activityType)
            {
                return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/RecreationalActivities.png", UriKind.RelativeOrAbsolute));
            }
            if (ActivityTypeDef.Other == activityType)
            {
                return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/OtherActivity.png", UriKind.RelativeOrAbsolute));
            }
            return new BitmapImage(new Uri("/CO_IA.Client.UIBuilderFactory.Standard;component/Images/defaultActivity.png", UriKind.RelativeOrAbsolute));
        }

        public IActivityDutyCodeQuerier DutyCodeQuerier
        {
            get
            {
                return ActivityDutyCodeQuerier.CodeQuerier;
            }
        }

        private class ActivityDutyCodeQuerier : IActivityDutyCodeQuerier
        {
            public static readonly IActivityDutyCodeQuerier CodeQuerier = new ActivityDutyCodeQuerier();
            public string Lead
            {
                get 
                {
                    return "01";
                }
            }

            public string Command
            {
                get 
                {
                    return "02";
                }
            }

            public string Coordinate
            {
                get
                {
                    return "03";
                }
            }

            public string FreqStation
            {
                get
                {
                    return "04";
                }
            }

            public string FreqMonitor
            {
                get
                {
                    return "05";
                }
            }

            public string EquipmentInspection
            {
                get
                {
                    return "06";
                }
            }

            public string Administrator
            {
                get
                {
                    return "07";
                }
            }

            public string EnsureSupplies
            {
                get
                {
                    return "08";
                }
            }

            public string Propaganda
            {
                get
                {
                    return "09";
                }
            }
        }

    }
}
