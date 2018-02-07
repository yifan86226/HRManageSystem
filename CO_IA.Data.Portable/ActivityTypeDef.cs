using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityTypeDef
    {
        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Order
        {
            get;
            set;
        }

        public string IconKey
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public const string MajorDisturbance="MajorDisturbance";

        public const string Exam="Exam";

        public const string Conference="Conference";

        public const string RecreationalActivities="RecreationalActivities";

        public const string Drill="Drill";

        public const string Emergency="Emergency";

        public const string Other = "Other";
    }
}
