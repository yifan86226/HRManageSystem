#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：xiaguohui
 * 摘 要 ：基本类型方法扩展
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.UI.MAP
{
    public static class ExtMethod
    {
        public static int? TryToInt(this string p_str)
        {
            int resu;
            bool isSucc = int.TryParse(p_str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return null;
            }
        }

        public static double? TryToDouble(this string p_str)
        {
            double resu;
            bool isSucc = double.TryParse(p_str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return null;
            }
        }

        public static double TryToDoubleZero(this string p_str)
        {
            double resu;
            bool isSucc = double.TryParse(p_str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return 0;
            }
        }

        public static int? TryToInt(this object p_obj)
        {
            if (p_obj == null)
            {
                return null;
            }
            string str = p_obj.ToString();
            int resu;
            bool isSucc = int.TryParse(str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return null;
            }
        }

        public static int TryToIntZero(this object p_obj)
        {
            return p_obj.TryToInt() ?? 0;
        }

        public static double? TryToDouble(this object p_obj)
        {
            if (p_obj == null)
            {
                return null;
            }
            string str = p_obj.ToString();
            double resu;
            bool isSucc = double.TryParse(str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return null;
            }
        }

        public static double TryToDoubleZero(this object p_obj)
        {
            return p_obj.TryToDouble() ?? 0;
        }

        public static DateTime? TryToDateTime(this object p_obj)
        {
            if (p_obj == null)
            {
                return null;
            }
            string str = p_obj.ToString();
            DateTime resu;
            bool isSucc = DateTime.TryParse(str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return null;
            }
        }

        public static DateTime TryToDateTimeDef(this object p_obj)
        {
            return p_obj.TryToDateTime() ?? Convert.ToDateTime("1900-01-01");
        }


        public static object IntDBNull(this object p_obj)
        {
            if (p_obj == null)
            {
                return DBNull.Value;
            }
            string str = p_obj.ToString();
            int resu;
            bool isSucc = int.TryParse(str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public static object DoubleDBNull(this object p_obj)
        {
            if (p_obj == null)
            {
                return DBNull.Value;
            }
            string str = p_obj.ToString();
            double resu;
            bool isSucc = double.TryParse(str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public static object DateTimeDBNull(this object p_obj)
        {
            if (p_obj == null)
            {
                return DBNull.Value;
            }
            string str = p_obj.ToString();
            DateTime resu;
            bool isSucc = DateTime.TryParse(str, out resu);
            if (isSucc)
            {
                return resu;
            }
            else
            {
                return DBNull.Value;
            }
        }

        /// <summary>
        /// 23:59:59
        /// </summary>
        /// <param name="p_dt"></param>
        /// <returns></returns>
        public static DateTime ToMaxTime(this DateTime p_dt)
        {
            string str = p_dt.ToString("yyyy-MM-dd 23:59:59");
            return Convert.ToDateTime(str);
        }
        /// <summary>
        /// 00:00:01
        /// </summary>
        /// <param name="p_dt"></param>
        /// <returns></returns>
        public static DateTime ToMinTime(this DateTime p_dt)
        {
            string str = p_dt.ToString("yyyy-MM-dd 00:00:01");
            return Convert.ToDateTime(str);
        }

        /// <summary>
        /// 23:59:59
        /// </summary>
        /// <param name="p_dt"></param>
        /// <returns></returns>
        public static DateTime? ToMaxTime(this DateTime? p_dt)
        {
            if (p_dt == null)
                return null;
            string str = ((DateTime)p_dt).ToString("yyyy-MM-dd 23:59:59");
            return Convert.ToDateTime(str);
        }
        /// <summary>
        /// 00:00:01
        /// </summary>
        /// <param name="p_dt"></param>
        /// <returns></returns>
        public static DateTime? ToMinTime(this DateTime? p_dt)
        {
            if (p_dt == null)
                return null;
            string str = ((DateTime)p_dt).ToString("yyyy-MM-dd 00:00:01");
            return Convert.ToDateTime(str);
        }
    }
}
