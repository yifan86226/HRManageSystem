#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：客户端辅助类,提供通用的客户端辅助操作
 * 日 期 ：2016-08-12
 ***************************************************************#@#***************************************************************/
#endregion

using Microsoft.Windows.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace CO_IA.Client
{
    /// <summary>
    /// 客户端辅助类,提供通用的客户端辅助操作
    /// </summary>
    public static class ClientHelper
    {
        /// <summary>
        /// 从参数指定字节数组加载图片
        /// </summary>
        /// <param name="bytes">要从中加载图片的自己数组</param>
        /// <returns>从参数指定字节数组中加载的图片</returns>
        public static System.Windows.Media.ImageSource LoadImageFromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            return bitmapImage;
        }
        /// <summary>
        /// 资源图片转byte数组
        /// </summary>
        /// <param name="path">/CO_IA.UI.ActivityManage;component/Images/add.png</param>
        /// <returns></returns>
        public static byte[] ResourceImageToBytes(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var str = Application.GetResourceStream(new Uri(path, UriKind.RelativeOrAbsolute));
            System.Drawing.Bitmap srcBitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(str.Stream);
            MemoryStream ms = new MemoryStream();
            srcBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            if (ms.Length > 0)
                return ms.GetBuffer();
            return null;
        }

        public static void Busy(this UIElement uiElement,string busyInfo)
        {
            var busyIndicator=AT_BC.Common.VisualTreeHelperExtension.GetParentObject<BusyIndicator>(uiElement);
            if (busyIndicator != null)
            {
                busyIndicator.BusyContent = busyInfo;
                busyIndicator.IsBusy = true;
            }
        }

        public static void Idle(this UIElement uiElement)
        {
            var busyIndicator = AT_BC.Common.VisualTreeHelperExtension.GetParentObject<BusyIndicator>(uiElement);
            if (busyIndicator != null)
            {
                busyIndicator.IsBusy = false;
            }
        }
    }
}
