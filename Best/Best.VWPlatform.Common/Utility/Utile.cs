using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Threading;
using System.Threading;

namespace Best.VWPlatform.Common.Utility
{
    public static class Utile
    {
        private static readonly Dictionary<Type, int> MTypeLength = InitializeTypes();
        private static string _mHostUrl = "";
        private static Action<string> _exceptionHandler;
        private static Dictionary<string, object> _globalVariables;
        private static Dictionary<Type, int> InitializeTypes()
        {
            Dictionary<Type, int> typeLength = new Dictionary<Type, int>();
            Int16 i16 = 0;
            typeLength[typeof(Int16)] = BitConverter.GetBytes(i16).Length;
            Int32 i32 = 0;
            typeLength[typeof(Int32)] = BitConverter.GetBytes(i32).Length;
            Int64 i64 = 0;
            typeLength[typeof(Int64)] = BitConverter.GetBytes(i64).Length;
            UInt16 ui16 = 0;
            typeLength[typeof(UInt16)] = BitConverter.GetBytes(ui16).Length;
            UInt32 ui32 = 0;
            typeLength[typeof(UInt32)] = BitConverter.GetBytes(ui32).Length;
            UInt64 ui64 = 0;
            typeLength[typeof(UInt64)] = BitConverter.GetBytes(ui64).Length;
            Boolean bl = true;
            typeLength[typeof(Boolean)] = BitConverter.GetBytes(bl).Length;
            Double db = 0;
            typeLength[typeof(Double)] = BitConverter.GetBytes(db).Length;
            typeLength[typeof(byte)] = 1;
            float f = 0;
            typeLength[typeof(float)] = BitConverter.GetBytes(f).Length;
            char c = 'a';
            typeLength[typeof(char)] = BitConverter.GetBytes(c).Length;
            long l = 0;
            typeLength[typeof(long)] = BitConverter.GetBytes(l).Length;
            sbyte sb = 0;
            typeLength[typeof(sbyte)] = BitConverter.GetBytes(sb).Length;
            short st = 0;
            typeLength[typeof(short)] = BitConverter.GetBytes(st).Length;
            uint ut = 0;
            typeLength[typeof(uint)] = BitConverter.GetBytes(ut).Length;
            ulong ul = 0;
            typeLength[typeof(ulong)] = BitConverter.GetBytes(ul).Length;
            ushort us = 0;
            typeLength[typeof(ushort)] = BitConverter.GetBytes(us).Length;
            return typeLength;
        }

        #region 属性
        /// <summary>
        /// 系统全局变量集
        /// </summary>
        public static Dictionary<string, object> GlobalVariables
        {
            get { return _globalVariables ?? (_globalVariables = new Dictionary<string, object>()); }
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 获取主机Url地址
        /// </summary>
        /// <returns>主机地址URI</returns>
        public static string GetHostUrl()
        {
            if (string.IsNullOrEmpty(_mHostUrl))
            {
                //string regex = "(?<host>http[s]{0,1}://[a-zA-Z0-9.]{1,}[:0-9]*/.*)/ClientBin/.*.xap";
                //XGZ: GetHostUrl 正则表达式，有斜杠时无法正确匹配？
                const string regex = "(?<host>http[s]{0,1}://[a-zA-Z0-9.]{1,}[:0-9]*.*)/ClientBin/.*.xap";
                const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
                Regex reg = new Regex(regex, options);
                Match mat = reg.Match("");
                if (mat.Success)
                    _mHostUrl = mat.Groups["host"].Value;
                else
                    throw new Exception("未能找到主机地址。");
            }
            return _mHostUrl;
        }

        /// <summary>
        /// 获取主机Ip地址
        /// </summary>
        /// <returns>Ip地址</returns>
        public static string GetHostIp()
        {
            string url = GetHostUrl();
            const string regex = "http[s]{0,1}://(?<host>[a-zA-Z0-9.]{1,})[:0-9]*.*";
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            Regex reg = new Regex(regex, options);
            Match mat = reg.Match(url);
            string ip = string.Empty;
            if (mat.Success)
                ip = mat.Groups["host"].Value;
            return ip;
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="pObject">对象</param>
        /// <returns>序列化结果</returns>
        public static string JsonSerialize(object pObject)
        {
            return JsonConvert.SerializeObject(pObject);
        }
        /// <summary>
        /// 反序列化为CLR对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="pJsonValue">json字符串表达式</param>
        /// <returns>返回反序列化后的对象</returns>
        public static T Deserialize<T>(string pJsonValue)
        {
            return JsonConvert.DeserializeObject<T>(pJsonValue);
        }

        /// <summary>
        /// 获取托管类型大小（以字节为单位）
        /// </summary>
        /// <param name="pType">元数据类型</param>
        /// <returns>大小</returns>
        public static int SizeOf(Type pType)
        {
            if (!MTypeLength.ContainsKey(pType))
                return 0;
            return MTypeLength[pType];
        }
        /// <summary>
        /// 获取托管类型大小（以字节为单位）
        /// </summary>
        /// <param name="pValue">元数据</param>
        /// <returns>大小</returns>
        public static int SizeOf(object pValue)
        {
            if (pValue == null)
                return 0;
            return SizeOf(pValue.GetType());
        }

        /// <summary>
        /// 双精度数转换，忽略自动四舍五入
        /// </summary>
        /// <param name="pValue">待转换的双精度值</param>
        /// <param name="pDigits">小数位数</param>
        /// <returns>返回双精度值</returns>
        public static double MathNoRound(double pValue, uint pDigits)
        {
            double d = Math.Pow(10, pDigits);
            if (pDigits == 0)
                return pValue > 0 ? Math.Floor(pValue) : Math.Ceiling(pValue);
            return pValue > 0 ? Math.Floor(pValue * d) / d : Math.Ceiling(pValue * d) / d;
        }
        /// <summary>
        /// 双精度数转换，忽略自动四舍五入，小数位数不足pDigits个数，补零
        /// </summary>
        /// <param name="pValue">待转换的双精度值</param>
        /// <param name="pDigits">小数位数</param>
        /// <returns>返回字符串值</returns>
        public static string MathNoRoundFill(double pValue, uint pDigits)
        {
            double v = MathNoRound(pValue, pDigits);
            string strValue = v.ToString(CultureInfo.InvariantCulture);
            string[] arrayValue = strValue.Split('.');
            int strLen = arrayValue[1].Length;
            if (strLen == pDigits)
                return strValue;
            int fillLen = (int)(pDigits - strLen);
            return strValue.PadRight(strValue.Length + fillLen, '0');
        }
        /// <summary>
        /// 获取数组中从索引处开始一定范围内的最大值
        /// </summary>
        /// <typeparam name="T">数值类型</typeparam>
        /// <param name="pValArray">数组</param>
        /// <param name="pIndex">开始索引</param>
        /// <param name="pLength">判断范围</param>
        /// <returns></returns>
        public static T GetMaxValue<T>(T[] pValArray, int pIndex, int pLength)
            where T : struct
        {
            T max = default(T);
            if (pValArray == null || pValArray.Length < pIndex)
                return max;
            max = pValArray[pIndex];
            double tm = double.Parse(max.ToString());

            for (int i = 0; i < pLength; i++)
            {
                if (pIndex + i >= pValArray.Length)
                    break;
                T tempv = pValArray[pIndex + i];
                double dtempv = double.Parse(tempv.ToString());
                if (dtempv > tm)
                {
                    max = tempv;
                    tm = dtempv;
                }
            }
            return max;
        }
        /// <summary>
        /// 获取数组中从索引处开始一定范围内的最小值
        /// </summary>
        /// <typeparam name="T">数值类型</typeparam>
        /// <param name="pValArray">数组</param>
        /// <param name="pIndex">开始索引</param>
        /// <param name="pLength">判断范围</param>
        /// <returns></returns>
        public static T GetMinValue<T>(T[] pValArray, int pIndex, int pLength)
            where T : struct
        {
            T min = default(T);
            if (pValArray == null || pValArray.Length < pIndex)
                return min;
            min = pValArray[pIndex];
            double tm = double.Parse(min.ToString());

            for (int i = 0; i < pLength; i++)
            {
                if (pIndex + i >= pValArray.Length)
                    break;
                T tempv = pValArray[pIndex + i];
                double dtempv = double.Parse(tempv.ToString());
                if (dtempv < tm)
                {
                    min = tempv;
                    tm = dtempv;
                }
            }
            return min;
        }

        public static string SecondToMinute(int secend)
        {
            int m = secend / 60;
            int s = secend % 60;
            string m1 = string.Format("{0:00}", m);
            string s1 = string.Format("{0:00}", s);
            return string.Format("{0}:{1}", m1, s1);
        }
        public static void RegisterExceptionHandler(Action<string> pExceptionHandler)
        {
            _exceptionHandler = pExceptionHandler;
        }

        /// <summary>
        /// 获取元素父对象
        /// </summary>
        /// <typeparam name="T">父对象类型</typeparam>
        /// <param name="pElement">元素</param>
        /// <returns>查找到的父对象</returns>
        public static T GetParent<T>(FrameworkElement pElement)
        {
            T v = default(T);
            var element = pElement.Parent as FrameworkElement;
            while (element != null && !(element is T))
            {
                element = element.Parent as FrameworkElement;
            }
            object obj = element;
            if (obj != null && obj is T)
                return (T)obj;
            return v;
        }
        /// <summary>
        /// 获取元素父对象总数
        /// </summary>
        /// <param name="pDependencyObject">元素</param>
        /// <returns></returns>
        public static int GetParentCount(DependencyObject pDependencyObject)
        {
            int count = 0;
            DependencyObject parent = pDependencyObject;
            do
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent != null)
                {
                    count++;
                }
            } while (parent != null);
            return count;
        }
        #endregion
    }


    public class UIHelper
    {
        //刷新界面
        private static DispatcherOperationCallback
             exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        public static void DoEvents()
        {
            DispatcherFrame nestedFrame = new DispatcherFrame();
            DispatcherOperation exitOperation =
                Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                exitFrameCallback, nestedFrame);
            Dispatcher.PushFrame(nestedFrame);

            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }
        private static object ExitFrame(object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;
            frame.Continue = false;
            return null;
        }
    }

    public class ServiceHelper
    {
        private static ServiceController _sc = null;
        public static void RestartService(string pName)
        {
            try
            {
                _sc = new ServiceController(pName);
                if (_sc.Status == ServiceControllerStatus.Running)
                {
                    _sc.Stop();
                    _sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                _sc.Start();
                _sc.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (Exception)
            {

            }
            finally
            {
                Thread.Sleep(1000);
                _sc.Close();
                _sc = null;
            }
        }
    }

    //public static class SystemFunctionExtends
    //{
    //    public static T ToValue<T>(this Array pSender, int pIndex)
    //    {
    //        T v = default(T);
    //        if (pIndex < 0 || pIndex >= pSender.Length)
    //            return v;
    //        object o = pSender.GetValue(pIndex);
    //        return o.ToValue<T>();
    //    }

    //    public static T ToValue<T>(this object pSender)
    //    {
    //        try
    //        {
    //            if (pSender.GetType() == typeof(JObject)
    //                || pSender.GetType() == typeof(JArray)
    //                || pSender is string)
    //                return JsonConvert.DeserializeObject<T>(pSender.ToString());
    //            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(pSender));
    //        }
    //        catch (Exception ex)
    //        {
    //            return default(T);
    //        }
    //    }

    //    public static bool IsNull(this object pSender)
    //    {
    //        return pSender == null || string.IsNullOrEmpty(pSender.ToString());
    //    }

    //    public static T ToValue<T>(this List<object> pSender, int pIndex)
    //    {
    //        T v = default(T);
    //        if (pIndex < 0 || pIndex >= pSender.Count)
    //            return v;
    //        object o = pSender[pIndex];
    //        return o.ToValue<T>();
    //    }

    //    #region Newtonsoft.Json
    //    /// <summary>
    //    /// 获取JObject内的属性值
    //    /// </summary>
    //    /// <typeparam name="T">转换类型</typeparam>
    //    /// <param name="pSender">JObject</param>
    //    /// <param name="pPropertyName">属性名称</param>
    //    /// <returns></returns>
    //    public static T GetPropertyValue<T>(this JObject pSender, string pPropertyName)
    //    {
    //        T v = default(T);
    //        JValue jv = pSender[pPropertyName] as JValue;
    //        if (jv != null)
    //            v = jv.ToValue<T>();
    //        return v;
    //    }
    //    #endregion
    //}
}
