using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class StatisticDataSource : LandscapeExpandable<string, double>
    {
        public string Group
        {
            get;
            set;
        }

        public string GroupGuid
        {
            get;
            set;
        }
    }
}
