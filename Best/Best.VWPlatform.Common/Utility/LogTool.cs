using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Utility
{
    public class LogTool
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Debug
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="pMessage"></param>
        public static void Debug(object pMessage)
        {
            _log.Debug(pMessage);
        }

        public static void Debug(object pMessage, Exception pExp)
        {
            _log.Debug(pMessage, pExp);
        }

        public static void DebugFormat(string pFormat, params object[] pArgs)
        {
            _log.DebugFormat(pFormat, pArgs);
        }

        public static void DebugFormat(IFormatProvider pProvider, string pFormat, params object[] pArgs)
        {
            _log.DebugFormat(pProvider, pFormat, pArgs);
        }

        #endregion

        #region Error
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="pMessage"></param>
        public static void Error(object pMessage)
        {
            _log.Error(pMessage);
        }

        public static void Error(object pMessage, Exception pException)
        {
            _log.Error(pMessage, pException);
        }

        public static void ErrorFormat(string pFormat, params object[] pArgs)
        {
            _log.ErrorFormat(pFormat, pArgs);
        }

        public static void ErrorFormat(IFormatProvider pProvider, string pFormat, params object[] pArgs)
        {
            _log.ErrorFormat(pProvider, pFormat, pArgs);
        }

        #endregion

        #region Fatal
        /// <summary>
        /// 致命的,毁灭性的
        /// </summary>
        /// <param name="pMessage"></param>
        public static void Fatal(object pMessage)
        {
            _log.Fatal(pMessage);
        }

        public static void Fatal(object pMessage, Exception pException)
        {
            _log.Fatal(pMessage, pException);
        }

        public static void FatalFormat(string pFormat, params object[] pArgs)
        {
            _log.FatalFormat(pFormat, pArgs);
        }

        public static void FatalFormat(IFormatProvider pProvider, string pFormat, params object[] pArgs)
        {
            _log.FatalFormat(pProvider, pFormat, pArgs);
        }

        #endregion

        #region Info
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="pMessage"></param>
        public static void Info(object pMessage)
        {
            _log.Info(pMessage);
        }

        public static void Info(object pMessage, Exception pException)
        {
            _log.Info(pMessage, pException);
        }

        public static void InfoFormat(string pFormat, params object[] pArgs)
        {
            _log.InfoFormat(pFormat, pArgs);
        }

        public static void InfoFormat(IFormatProvider pProvider, string pFormat, params object[] pArgs)
        {
            _log.InfoFormat(pProvider, pFormat, pArgs);
        }

        #endregion

        #region Warn
        /// <summary>
        /// 警告,注意,通知
        /// </summary>
        /// <param name="pMessage"></param>
        public static void Warn(object pMessage)
        {
            _log.Warn(pMessage);
        }

        public static void Warn(object pMessage, Exception pException)
        {
            _log.Warn(pMessage, pException);
        }

        public static void WarnFormat(string pFormat, params object[] pArgs)
        {
            _log.WarnFormat(pFormat, pArgs);
        }

        public static void WarnFormat(IFormatProvider pProvider, string pFormat, params object[] pArgs)
        {
            _log.WarnFormat(pProvider, pFormat, pArgs);
        }

        #endregion
    }
}
