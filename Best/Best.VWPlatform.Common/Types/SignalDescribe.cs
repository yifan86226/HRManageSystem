using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Best.VWPlatform.Common.Types
{
    /// <summary>
    /// 信号描述相关类
    /// </summary>
    /// <remarks>
    /// 包括信号性质定义、获取信号图标方法
    /// </remarks>
    public sealed class SignalDescribe
    {
        private SignalDescribe() { }
        public enum SignalType
        {
            /// <summary>
            /// 新信号
            /// </summary>
            New = 0,
            /// <summary>
            /// 已知信号，台站数据库（A库）
            /// </summary>
            Known1,
            /// <summary>
            /// 已知信号，台站数据库（B库）
            /// </summary>
            Known2,
            /// <summary>
            /// 已知信号, 未入库
            /// </summary>
            Known3,
            /// <summary>
            /// 未知信号/不明信号
            /// </summary>
            Unknown = 4,
            /// <summary>
            /// 非法信号
            /// </summary>
            Illegal,
            /// <summary>
            /// 违规信号
            /// </summary>
            Violations,
            /// <summary>
            /// 虚假信号
            /// </summary>
            Sham,
            /// <summary>
            /// 其他信号
            /// </summary>
            Other
        }
        private static readonly Dictionary<SignalType, ImageSource> SignalImageCache = new Dictionary<SignalType, ImageSource>();
        /// <summary>
        /// 获取信号类型图标
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static ImageSource GetSignalImage(SignalType pType)
        {
            ImageSource signalImage = null;
            if (SignalImageCache.ContainsKey(pType))
            {
                return SignalImageCache[pType];
            }
            switch (pType)
            {
                case SignalType.New:
                case SignalType.Known3:
                    signalImage = new BitmapImage(new Uri("/Best.VWPlatform.Resources;component/Images/SignalNature/signal_new.png", UriKind.RelativeOrAbsolute));
                    break;
                case SignalType.Known1:
                case SignalType.Known2:
                    signalImage = new BitmapImage(new Uri("/Best.VWPlatform.Resources;component/Images/SignalNature/signal_legal.png", UriKind.RelativeOrAbsolute));
                    break;
                case SignalType.Unknown:
                case SignalType.Other:
                    signalImage = new BitmapImage(new Uri("/Best.VWPlatform.Resources;component/Images/SignalNature/signal_unknown.png", UriKind.RelativeOrAbsolute));
                    break;
                case SignalType.Illegal:
                case SignalType.Violations:
                case SignalType.Sham:
                    signalImage = new BitmapImage(new Uri("/Best.VWPlatform.Resources;component/Images/SignalNature/signal_illegality.png", UriKind.RelativeOrAbsolute));
                    break;
            }
            SignalImageCache[pType] = signalImage;
            return signalImage;
        }
        /// <summary>
        /// 获取信号文本描述
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static string GetSignalText(SignalType pType)
        {
            switch (pType)
            {
                case SignalType.New:
                case SignalType.Known3:
                    return "新信号";
                case SignalType.Known1:
                case SignalType.Known2:
                    return "合法信号";
                case SignalType.Unknown:
                case SignalType.Other:
                    return "未知信号";
                case SignalType.Illegal:
                case SignalType.Violations:
                case SignalType.Sham:
                    return "非法信号";
            }
            return string.Empty;
        }
    }
}
