using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Utility
{
    public static class WMonitorUtile
    {
        private static DateTime _startDt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        /// <summary>
        /// 获取灰色着色器
        /// </summary>
        /// <returns></returns>
        //public static GrayShaderEffect GetGrayShaderEffect()
        //{
        //    var pixelShader = new PixelShader
        //    {
        //        UriSource = new Uri(ThemeManage.GetUri("Gray.ps"), UriKind.RelativeOrAbsolute)
        //    };

        //    var grayShaderEffect = new GrayShaderEffect(pixelShader)
        //    {
        //        Factor = 0
        //    };
        //    return grayShaderEffect;
        //}

        /// <summary>
        /// 获取 0001 年至 1970 年以来经过的毫秒数
        /// </summary>
        public static double DateTime1970Milliseconds
        {
            get
            {
                //System.Diagnostics.Debug.WriteLine("test");
                var ts = new TimeSpan(_startDt.Ticks);
                return ts.TotalMilliseconds;
            }
        }
        /// <summary>
        /// 逻辑视图范围的值转换为屏幕范围内的像素值
        /// </summary>
        /// <param name="pLogicalXorY">X | Y</param>
        /// <param name="pXYwmax">windows窗口坐标最大值</param>
        /// <param name="pXYwmin">windows窗口坐标最小值</param>
        /// <param name="pXYvmax">实际可视窗口最大值</param>
        /// <param name="pXYvmin">实际可视窗口最小值</param>
        /// <param name="pReversal">取反值 </param>
        /// <returns>像素坐标 x | y</returns>
        public static long ViewToScreen(double pLogicalXorY, double pXYwmax, double pXYwmin, double pXYvmax, double pXYvmin, bool pReversal = false)
        {
            return (long)ViewToScreenDouble(pLogicalXorY, pXYwmax, pXYwmin, pXYvmax, pXYvmin, pReversal);
        }

        public static double ViewToScreenDouble(double pLogicalXorY, double pXYwmax, double pXYwmin, double pXYvmax, double pXYvmin, bool pReversal = false)
        {
            double logicalXorY = pLogicalXorY;
            double xywmax = pXYwmax;
            double xywmin = pXYwmin;
            double xyvmax = pXYvmax;
            double xyvmin = pXYvmin;
            double d = 0;
            if (xyvmin < 0)
            {
                d = Math.Abs(xyvmin);
            }
            else if (xyvmin > 0)
            {
                d = -xyvmin;
            }
            xyvmin = 0;
            xyvmax = xyvmax + d;
            logicalXorY = logicalXorY + d;
            if (pReversal)
            {
                logicalXorY = xyvmax - logicalXorY;
            }

            return ((xywmax - xywmin) / (xyvmax - xyvmin)) * logicalXorY +
                   (xyvmin - ((xywmax - xywmin) / (xyvmax - xyvmin)) * xywmin);
        }
        /// <summary>
        /// 屏幕范围内像素值转换为逻辑视图的像素值
        /// </summary>
        /// <param name="pLogicalXorY">X | Y</param>
        /// <param name="pXYvmax"></param>
        /// <param name="pXYvmin"></param>
        /// <param name="pXYwmax"></param>
        /// <param name="pXYwmin"></param>
        /// <param name="pReversal"> </param>
        /// <returns></returns>
        public static long ScreenToView(double pLogicalXorY, double pXYvmax, double pXYvmin, double pXYwmax, double pXYwmin, bool pReversal = false)
        {
            return ViewToScreen(pLogicalXorY, pXYvmax, pXYvmin, pXYwmax, pXYwmin, pReversal);
        }

        public static double ScreenToViewDouble(double pLogicalXorY, double pXYvmax, double pXYvmin, double pXYwmax, double pXYwmin, bool pReversal = false)
        {
            return ViewToScreenDouble(pLogicalXorY, pXYvmax, pXYvmin, pXYwmax, pXYwmin, pReversal);
        }
        /// <summary>
        /// 获取平均值
        /// </summary>
        /// <param name="pMaxValue">最大值</param>
        /// <param name="pMinValue">最小值</param>
        /// <returns>平均值</returns>
        public static double GetAverageValue(double pMaxValue, double pMinValue)
        {
            return (pMaxValue + pMinValue) / 2;
        }
        /// <summary>
        /// 整数转小数
        /// </summary>
        /// <param name="pValue">整数值</param>
        /// <param name="pDecimal">小数位数</param>
        /// <returns></returns>
        public static double IntToDouble(Int32 pValue, uint pDecimal)
        {
            return pValue / Math.Pow(10, pDecimal);
        }

        /// <summary>
        /// 频率值转换
        /// </summary>
        /// <param name="pSrcUnit">源单位</param>
        /// <param name="pTargetUnit">目标单位</param>
        /// <param name="pSrcValue">源频率值</param>
        /// <returns>格式化后目标频率值</returns>
        //public static string ConvertFreqValue(FreqUnit pSrcUnit, FreqUnit pTargetUnit, double pSrcValue)
        //{
        //    var v = ConvertFreqValue(pSrcUnit.ToString(), pTargetUnit.ToString(), pSrcValue);
        //    string s = string.Empty;
        //begin:
        //    switch (pTargetUnit)
        //    {
        //        case FreqUnit.GHz:
        //            {
        //                s = "{0:N9}{1}";
        //            }
        //            break;
        //        case FreqUnit.MHz:
        //            {
        //                if (v > 1000)
        //                {
        //                    var srcUnit = pTargetUnit;
        //                    pTargetUnit = FreqUnit.GHz;
        //                    v = ConvertFreqValue(srcUnit.ToString(), pTargetUnit.ToString(), v);
        //                    goto begin;
        //                }
        //                s = "{0:N6}{1}";
        //            }
        //            break;
        //        case FreqUnit.kHz:
        //            {
        //                if (v > 1000)
        //                {
        //                    var srcUnit = pTargetUnit;
        //                    pTargetUnit = FreqUnit.MHz;
        //                    v = ConvertFreqValue(srcUnit.ToString(), pTargetUnit.ToString(), v);
        //                    goto begin;
        //                }
        //                s = "{0:N3}{1}";
        //            }
        //            break;
        //        case FreqUnit.Hz:
        //            {
        //                if (v > 1000)
        //                {
        //                    var srcUnit = pTargetUnit;
        //                    pTargetUnit = FreqUnit.kHz;
        //                    v = ConvertFreqValue(srcUnit.ToString(), pTargetUnit.ToString(), v);
        //                    goto begin;
        //                }
        //                s = "{0}{1}";
        //            }
        //            break;
        //    }
        //    return string.Format(s, v, pTargetUnit.ToString());
        //}
        /// <summary>
        /// 频率值转换
        /// </summary>
        /// <param name="pSrcUnit">源单位</param>
        /// <param name="pTargetUnit">目标单位</param>
        /// <param name="pSrcValue">源频率值</param>
        /// <returns>目标频率值</returns>
        public static double ConvertFreqValue(string pSrcUnit, string pTargetUnit, double pSrcValue)
        {
            double targetValue = 0;
            switch (pSrcUnit.ToLower())
            {
                case "ghz":
                    targetValue = ConvertFreqValueFromGhz(pTargetUnit, pSrcValue);
                    break;
                case "mhz":
                    targetValue = ConvertFreqValueFromMhz(pTargetUnit, pSrcValue);
                    break;
                case "khz":
                    targetValue = ConvertFreqValueFromKhz(pTargetUnit, pSrcValue);
                    break;
                case "hz":
                    targetValue = ConvertFreqValueFromHz(pTargetUnit, pSrcValue);
                    break;
            }
            return targetValue;
        }
        /// <summary>
        /// 转换自定义的日期时间字符串到实际DateTime类型
        /// </summary>
        /// <param name="pDateTime">自定义的日期时间字符串，如:20130704001624，表示年月日时分秒</param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertToDateTime1(string pDateTime)
        {
            DateTime dt;
            try
            {
                int year = int.Parse(pDateTime.Substring(0, 4));
                int month = int.Parse(pDateTime.Substring(4, 2));
                int day = int.Parse(pDateTime.Substring(6, 2));
                int hour = int.Parse(pDateTime.Substring(8, 2));
                int minute = int.Parse(pDateTime.Substring(10, 2));
                int second = int.Parse(pDateTime.Substring(12, 2));
                dt = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
            }
            catch (Exception)
            {
                throw;
            }
            return dt;
        }

        private static double ConvertFreqValueFromGhz(string pTargetUnit, double pSrcValue)
        {
            double targetValue = 0;
            switch (pTargetUnit.ToLower())
            {
                case "ghz":
                    targetValue = pSrcValue;
                    break;
                case "mhz":
                    targetValue = pSrcValue * 1000;
                    break;
                case "khz":
                    targetValue = pSrcValue * 1000000;
                    break;
                case "hz":
                    targetValue = pSrcValue * 1000000000;
                    break;
            }
            return targetValue;
        }
        private static double ConvertFreqValueFromMhz(string pTargetUnit, double pSrcValue)
        {
            double targetValue = 0;
            switch (pTargetUnit.ToLower())
            {
                case "ghz":
                    targetValue = Utile.MathNoRound(pSrcValue / 1000, 9);
                    break;
                case "mhz":
                    targetValue = pSrcValue;
                    break;
                case "khz":
                    targetValue = pSrcValue * 1000;
                    break;
                case "hz":
                    targetValue = pSrcValue * 1000000;
                    break;
            }
            return targetValue;
        }
        private static double ConvertFreqValueFromKhz(string pTargetUnit, double pSrcValue)
        {
            double targetValue = 0;
            switch (pTargetUnit.ToLower())
            {
                case "ghz":
                    targetValue = Utile.MathNoRound(pSrcValue / 1000000, 9);
                    break;
                case "mhz":
                    targetValue = Utile.MathNoRound(pSrcValue / 1000, 6);
                    break;
                case "khz":
                    targetValue = pSrcValue;
                    break;
                case "hz":
                    targetValue = pSrcValue * 1000;
                    break;
            }
            return targetValue;
        }
        private static double ConvertFreqValueFromHz(string pTargetUnit, double pSrcValue)
        {
            double targetValue = 0;
            switch (pTargetUnit.ToLower())
            {
                case "ghz":
                    targetValue = Utile.MathNoRound(pSrcValue / 1000000000, 9);
                    break;
                case "mhz":
                    targetValue = Utile.MathNoRound(pSrcValue / 1000000, 6);
                    break;
                case "khz":
                    targetValue = Utile.MathNoRound(pSrcValue / 1000, 3);
                    break;
                case "hz":
                    targetValue = pSrcValue;
                    break;
            }
            return targetValue;
        }
    }
}
