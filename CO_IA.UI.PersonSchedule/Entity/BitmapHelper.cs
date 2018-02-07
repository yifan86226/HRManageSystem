#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：图片展示辅助类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CO_IA.UI.PersonSchedule
{
    public static class BitmapHelper
    {
        public static byte[] GetPictureData(string pImagePath)
        {
            if (!File.Exists(pImagePath))
                return null;
            byte[] imgBytes = null;
            //根据图片文件的路径使用文件流打开，并保存为byte[]   
            using (var fs = new FileStream(pImagePath, FileMode.Open))
            {
                imgBytes = new byte[fs.Length];
                fs.Read(imgBytes, 0, imgBytes.Length);
                fs.Close();
            }
            return imgBytes;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }

        public static byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] byteArray = null;

            try
            {
                Stream sMarket = bmp.StreamSource;

                if (sMarket != null && sMarket.Length > 0)
                {
                    sMarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(sMarket))
                    {
                        byteArray = br.ReadBytes((int)sMarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling   
            }
            return byteArray;
        }

        public static BitmapSource MakeNavButtonGroupPicture(
            BitmapSource pBmp1,
            BitmapSource pBmp2,
            BitmapSource pBmp3,
            BitmapSource pBmp4,
            int pixelWidth = 40,
            int pixelHeight = 40)
        {
            if (pBmp1 == null && pBmp2 == null && pBmp3 == null && pBmp4 == null)
                return null;
            const double dpi = 96;
            var composeImage = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Default);
            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            if (pBmp1 != null)
            {
                drawingContext.DrawImage(ScaleImage(pBmp1, pixelWidth, pixelHeight), new Rect(2, 2, pixelWidth / 2 - 2, pixelHeight / 2 - 2));
            }
            if (pBmp2 != null)
            {
                drawingContext.DrawImage(ScaleImage(pBmp2, pixelWidth, pixelHeight), new Rect(pixelWidth / 2 + 1, 2, pixelWidth / 2 - 3, pixelHeight / 2 - 3));
            }
            if (pBmp3 != null)
            {
                drawingContext.DrawImage(ScaleImage(pBmp3, pixelWidth, pixelHeight), new Rect(2, pixelHeight / 2 + 1, pixelWidth / 2 - 2, pixelHeight / 2 - 2));
            }
            if (pBmp4 != null)
            {
                drawingContext.DrawImage(ScaleImage(pBmp4, pixelWidth, pixelHeight), new Rect(pixelWidth / 2 + 1, pixelHeight / 2 + 1, pixelWidth / 2 - 3, pixelHeight / 2 - 3));
            }
            drawingContext.Close();
            composeImage.Render(drawingVisual);
            return composeImage;
        }

        private static BitmapSource ScaleImage(BitmapSource pSourceImage, int pixelWidth, int pixelHeight)
        {
            var tb = new TransformedBitmap();
            tb.BeginInit();
            tb.Source = pSourceImage;
            tb.Transform = new ScaleTransform((double)pixelWidth / 100, (double)pixelHeight / 100);
            tb.EndInit();
            return tb;
        }
    }
}
