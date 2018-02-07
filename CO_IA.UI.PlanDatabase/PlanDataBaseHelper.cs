using CO_IA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.PlanDatabase
{
    public class PlanDataBaseHelper
    {
        //isDigit是否是数字 
        public static bool IsNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        public static T Clone<T>(T obj)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
            serializer.WriteObject(ms, obj);
            ms.Flush();
            ms = new System.IO.MemoryStream(ms.ToArray());
            return (T)serializer.ReadObject(ms);
        }

        public static string TableNameConvert(string tablename)
        {
            tablename = tablename.TrimStart('\'').TrimEnd('\'');
            if (tablename.Contains('$'))
            {
                tablename = tablename.Replace('$', ' ').Trim();
            }
            if (tablename.Contains('|'))
            {
                tablename = tablename.Replace('|', '/');
            }
            if (tablename.Contains('#'))
            {
                tablename = tablename.Replace('#', '.').Trim();
            }
            return tablename;
        }

        public static string GetBusinessCode(string tablename)
        {
            BusinessType type = CO_IA.Client.Utility.BusinessTypes.FirstOrDefault(r => r.Value == tablename);
            if (type == null)
            {
                return null;
            }
            else
            {
                return type.Guid;
            }
        }
    }
}
