//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Drawing;

//namespace CO_IA.Client
//{
//    public static class ImageUtility
//    {
//        /// <summary>
//        /// 压缩图片，固定高度，宽度按照比例计算
//        /// </summary>
//        /// <param name="strOldPic"></param>
//        /// <param name="intHeight"></param>
//        /// <returns></returns>
//        public static System.Drawing.Bitmap SmallPic(string strOldPic, int intHeight)
//        {
//            int intWidth = 0;
//            System.Drawing.Bitmap objPic, objNewPic;
//            try
//            {
//                objPic = new System.Drawing.Bitmap(strOldPic);
//                intWidth = Convert.ToInt32(((double)intHeight / objPic.Size.Height) * objPic.Size.Width);
//                if (objPic.Size.Width < intWidth || objPic.Size.Height < intHeight)
//                {
//                    return null;
//                }
//                objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);

//            }
//            catch (Exception exp)
//            { throw exp; }
//            return objNewPic;
//        }
//        /// <summary>
//        /// 压缩图片
//        /// </summary>
//        /// <param name="strOldPic"></param>
//        /// <param name="intWidth"></param>
//        /// <param name="intHeight"></param>
//        /// <returns></returns>
//        public static System.Drawing.Bitmap SmallPic(string strOldPic, int intWidth, int intHeight)
//        {
//            System.Drawing.Bitmap objPic, objNewPic;
//            try
//            {
//                objPic = new System.Drawing.Bitmap(strOldPic);
//                int oldWidth = objPic.Size.Width;
//                int oldHeight = objPic.Size.Height;
//                if (oldWidth < intWidth && oldHeight < intHeight)
//                {
//                    return null;
//                }
//                double widthProportion = (double)oldWidth / intWidth;
//                double heightProportion = (double)oldHeight / intHeight;
                
//                //宽度缩小的比例大，以宽为基准
//                if (widthProportion >= heightProportion)
//                {
//                    intHeight = Convert.ToInt32(((double)intWidth / oldWidth) * oldHeight);
//                }
//                else
//                {
//                    intWidth = Convert.ToInt32(((double)intHeight / oldHeight) * oldWidth);
//                }
//                objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);

//            }
//            catch (Exception exp)
//            { throw exp; }
//            return objNewPic;
//        }
//        /// <summary>
//        /// 压缩图片，按比例压缩
//        /// </summary>
//        /// <param name="strOldPic">图片路径</param>
//        /// <param name="size1">宽或高</param>
//        /// <param name="type">0：size1是宽；1：size1是高</param>
//        /// <returns></returns>
//        public static System.Drawing.Bitmap SmallPic(string strOldPic, int size1, string type)
//        {
//            int size2 = 0;
//            System.Drawing.Bitmap objPic, objNewPic;
//            try
//            {
//                objPic = new System.Drawing.Bitmap(strOldPic);
//                if (type == "0")//此时size2代表高
//                {
//                    size2 = Convert.ToInt32(((double)size1 / objPic.Size.Width) * objPic.Size.Height);
//                    objNewPic = new System.Drawing.Bitmap(objPic, size1, size2);
//                }
//                else//type:1,代表size1是高，size2是宽
//                {
//                    size2 = Convert.ToInt32(((double)size1 / objPic.Size.Height) * objPic.Size.Width);
//                    objNewPic = new System.Drawing.Bitmap(objPic, size2, size1);
//                }
//            }
//            catch (Exception exp)
//            { throw exp; }
//            return objNewPic;
//        }
//    }
//}
