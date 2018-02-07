using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataManager.Public
{
  public  static class ConstantEntity
    {

        public static string DBName = "RisdbDatabase.accdb";

        private static string dBConnectionString = "Provider=Microsoft.ACE.OleDB.12.0;data source=RisdbDatabase.accdb";

        public static string DBConnectionString
        {
            get
            {
                return dBConnectionString;
            }

            set
            {
                dBConnectionString = value;
            }
        }
    }
}
