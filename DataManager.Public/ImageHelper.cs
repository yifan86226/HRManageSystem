using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
//using static System.Net.Mime.MediaTypeNames;

namespace DataManager.Public
{
    public class ImageHelper
    {

        //将图片控制在宽度为1130像素

        public static Bitmap PercentImage(Image srcImage)

        {

            int newW = srcImage.Width < 1130 ? srcImage.Width : 1130;

            int newH = int.Parse(Math.Round(srcImage.Height * (double)newW / srcImage.Width).ToString());

            try

            {

                Bitmap b = new Bitmap(newW, newH);

                Graphics g = Graphics.FromImage(b);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;

                g.DrawImage(srcImage, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, srcImage.Width, srcImage.Height), GraphicsUnit.Pixel);

                g.Dispose();

                return b;

            }

            catch (Exception)

            {

                return null;

            }

        }

        //将图片按百分比压缩，flag取值1到100，越小压缩比越大

        public static Stream YaSuo(Image iSource, int flag)

        {

            Stream memstream = new MemoryStream();

            ImageFormat tFormat = iSource.RawFormat;

            EncoderParameters ep = new EncoderParameters();

            long[] qy = new long[1];

            qy[0] = flag;

            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);

            ep.Param[0] = eParam;

            try

            {

                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageDecoders();

                ImageCodecInfo jpegICIinfo = null;

                for (int x = 0; x < arrayICI.Length; x++)

                {

                    if (arrayICI[x].FormatDescription.Equals("JPEG"))

                    {

                        jpegICIinfo = arrayICI[x];

                        break;

                    }

                }

                if (jpegICIinfo != null)
                {
                    iSource.Save(memstream, jpegICIinfo, ep);
                }

                else
                {
                    iSource.Save(memstream, tFormat);
                }
                return memstream;

            }

            catch

            {

                return memstream;

            }

            iSource.Dispose();

        }


        /// <summary>
        /// 获取md5校验码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(retVal);
        }


        /// <summary>
        /// 获取md5校验码
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromByte(byte[] filebyte)
        {
        
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(filebyte);
    ;
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(retVal);
        }



        /// <summary>
        /// 对图片进行压缩
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static byte[] YaSuo(byte[] photo)
        {

            Image image = BytesToImage(photo);

            Bitmap bitmap = PercentImage(image);


            return BitmapToBytes(bitmap);

        }



        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Equals(ImageFormat.Jpeg))
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Equals(ImageFormat.Png))
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Equals(ImageFormat.Bmp))
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Equals(ImageFormat.Gif))
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Equals(ImageFormat.Icon))
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                byte[] buffer = new byte[ms.Length];
                //Image.Save()会改变MemoryStream的Position，需要重新Seek到Begin
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }



        //byte[] 转换 Bitmap
        public static Bitmap BytesToBitmap(byte[] Bytes)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(Bytes);
                return new Bitmap((Image)new Bitmap(stream));
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            finally
            {
                stream.Close();
            }
        }

        //Bitmap转byte[]  
        public static byte[] BitmapToBytes(Bitmap Bitmap)
        {
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                Bitmap.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = new Byte[ms.Length];
                byteImage = ms.ToArray();
                return byteImage;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            finally
            {
                ms.Close();
            }
        }
    


    /// <summary>
    /// Convert Byte[] to a picture and Store it in file
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static string CreateImageFromBytes(string fileName, byte[] buffer)
        {
            string file = fileName;
            Image image = BytesToImage(buffer);
            ImageFormat format = image.RawFormat;
            if (format.Equals(ImageFormat.Jpeg))
            {
                file += ".jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                file += ".png";
            }
            else if (format.Equals(ImageFormat.Bmp))
            {
                file += ".bmp";
            }
            else if (format.Equals(ImageFormat.Gif))
            {
                file += ".gif";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                file += ".icon";
            }
            System.IO.FileInfo info = new System.IO.FileInfo(file);
            System.IO.Directory.CreateDirectory(info.Directory.FullName);
            File.WriteAllBytes(file, buffer);
            return file;
        }
    }
}
