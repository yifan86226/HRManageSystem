﻿#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：byte到image转化类
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CO_IA.UI.PersonSchedule
{
   public class ByteToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || ((byte[])value) == null || ((byte[])value).Length==0)
            {
                if (parameter != null)
                {
                    return @"/CO_IA.UI.PersonSchedule;component/Images/morenvehicle.png";
                }
                return @"/CO_IA.UI.PersonSchedule;component/Images/morentouxiang5.png";
            }
            else
            {

                byte[] imageData = (byte[])value;
                System.IO.MemoryStream ms = new System.IO.MemoryStream(imageData);//imageData是从数据库中读取出来的字节数组

                ms.Seek(0, System.IO.SeekOrigin.Begin);

                BitmapImage newBitmapImage = new BitmapImage();

                newBitmapImage.BeginInit();

                newBitmapImage.StreamSource = ms;

                newBitmapImage.EndInit();

                return newBitmapImage;
            }
           
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage newBitmapImage = (BitmapImage)value;
            byte[] bytearray = null;

            try
            {
                Stream smarket = newBitmapImage.StreamSource;

                if (smarket != null && smarket.Length > 0)
                {
                    //很重要，因为position经常位于stream的末尾，导致下面读取到的长度为0。
                    smarket.Position = 0;

                    using (BinaryReader  br = new BinaryReader(smarket))
                    {
                        bytearray = br.ReadBytes((int)smarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling
            }

            return bytearray;
        }
    }
}
