using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CO_IA.UI.Screen.Dialog
{
    /// <summary>
    /// ProgramInfo.xaml 的交互逻辑
    /// </summary>
    public partial class ProgramInfo : Window
    {
        ExtentProgramData Data;
        ExtentProgramData pargramData;
        public ProgramInfo(ExtentProgramData pargramdata)
        {
            InitializeComponent();
            Data = pargramdata;
            pargramData = AT_BC.Data.Helpers.DataContractSerializeHelper.Clone<ExtentProgramData>(pargramdata); ;
            this.DataContext = pargramData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pargramData.Name))
            {
                MessageBox.Show("名称不能为空！");
                return;
            }
                if (string.IsNullOrEmpty(pargramData.Path))
            { 
                MessageBox.Show("程序路径不能为空！");
                return;
            }
            Data.Name = pargramData.Name;
            Data.Path = pargramData.Path;
            Data.IconURL = CopyImage(pargramData.IconURL);
            this.DialogResult = true;
        }
        private string CopyImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return filePath;
            string newFilePath = "";
            if (File.Exists(filePath))
            {
                string extName=filePath.Substring(filePath.LastIndexOf('.')+1,filePath.Length-filePath.LastIndexOf('.')-1);
                string AppPath = AppDomain.CurrentDomain.BaseDirectory+ "EPImage";
                if (!Directory.Exists(AppPath))
                {
                    Directory.CreateDirectory(AppPath);
                }
                newFilePath = AppPath+"\\img"+DateTime.Now.ToString("yyyyMMddhhmmss")+"."+extName;
                File.Copy(filePath, newFilePath);
            }
            return newFilePath;
        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
                ofd.Filter = "exe|*.exe";//过滤器
                if (ofd.ShowDialog() == true)
                {
                    string fileName = ofd.FileName;
                    pargramData.Path = fileName;
                }
            }
            catch
            {
            }
        }

        private void ImageChange_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
                ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp|gif|*.gif";//过滤器
                if (ofd.ShowDialog() == true)
                {
                    string fileName = ofd.FileName;                    
                    pargramData.IconURL = fileName;
                }
            }
            catch
            {
                MessageBox.Show("更换图片失败！");
            }
        }
       
    }

    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string url = value as string;
                if (!String.IsNullOrEmpty(url))
                {
                    if (url.StartsWith("/"))
                    {
                        return "/CO_IA.UI.Screen;component/Images/defaultprogram.png";
                    }
                    System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(value as string));
                    return bitmapImage;
                }
                else
                    return "/CO_IA.UI.Screen;component/Images/defaultprogram.png";
            }
            if(value==null)
            {
                return "/CO_IA.UI.Screen;component/Images/defaultprogram.png";
            }
            return null;
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

                    using (BinaryReader br = new BinaryReader(smarket))
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
