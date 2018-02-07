using System;
using System.Collections.Generic;
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
using CO_IA.Data;
using I_CO_IA.ActivitySummarize;
using System.IO;
using Microsoft.Win32;
using System.Globalization;
using System.Threading;

namespace CO_IA.UI.ActivitySummarize.ActivityDetail
{
    /// <summary>
    /// MonitorEquipInput.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorEquipInput : Window
    {
        private GuaranteeProcess gp = new GuaranteeProcess();
        public event Action RefreshListEvent;
        public MonitorEquipInput(GuaranteeProcess _gp)
        {
            InitializeComponent();

            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.gp = _gp;
            this.DataContext = this.gp;


            if (gp.PHOTO != null)
            {
                MemoryStream stream = new MemoryStream(gp.PHOTO);
                BitmapImage bmp = new BitmapImage();
                bmp.BeginInit();//初始化
                bmp.StreamSource = stream;//设置源
                bmp.EndInit();//初始化结束
                imagePhone.Source = bmp;//设置图像Source
            }
        }

        private bool Validated()
        {
            bool flag = true;
            StringBuilder strmsg = new StringBuilder();
            if (string.IsNullOrEmpty(txtName.Text))
            {
                strmsg.Append("请填写名称！");
                flag = false;
            }
            if (!string.IsNullOrEmpty(txtName.Text) && txtName.Text.Length > 100)
            {
                strmsg.Append("名称不能超过100个字！");
                flag = false;
            }
            if (!string.IsNullOrEmpty(txtTask.Text) && txtTask.Text.Length > 1000)
            {
                strmsg.Append("描述不能超过1000个字！");
                flag = false;
            }
            if (gp.PHOTO == null)
            {
                strmsg.Append("请上传图片！");
                flag = false;
            }
            if (!string.IsNullOrEmpty(strmsg.ToString()))
            {
                MessageBox.Show(strmsg.ToString(), "提示", MessageBoxButton.OK);
            }
            return flag;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Validated())
            {
                PT_BS_Service.Client.Framework.BeOperationInvoker.Invoke<I_CO_IA.ActivitySummarize.I_CO_IA_ActivitySummarize>(
                    channel =>
                    {
                        channel.SaveGuaranteeProcess(gp);
                        MessageBox.Show("保存成功", "提示", MessageBoxButton.OK);
                        if (RefreshListEvent != null)
                        {
                            RefreshListEvent();
                        }
                        this.Close();
                    });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        DrawingVisual _visual;
        private void imagePhone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();//打开选择文件窗口
            ofd.Filter = "jpg|*.jpg|png|*.png|bmp|*.bmp|gif|*.gif|mp4|*.mp4";//过滤器
            if (ofd.ShowDialog() == true)
            {
                //if (string.IsNullOrEmpty(txtName.Text))
                //{
                txtName.Text = ofd.SafeFileName;
                gp.NAME = ofd.SafeFileName;
                //}
                string fileName = ofd.FileName;//获得文件的完整路径
                if (IsVoide(ofd.SafeFileName))
                {
                    gp.VIDEO = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中

                    _visual = new DrawingVisual();
                    DrawingContext dc = _visual.RenderOpen();
                    MediaPlayer player = new MediaPlayer();
                    player.Open(new Uri(fileName));
                    player.Position = TimeSpan.FromSeconds(1);
                    player.ScrubbingEnabled = true;
                    player.Pause();
                    dc.DrawVideo(player, new Rect(0, 0, 300, 300));
                    dc.Close();

                    Thread.Sleep(1000);

                    string iamgeFolder = System.Windows.Forms.Application.StartupPath + "\\tempImages\\";
                    if (!Directory.Exists(iamgeFolder))
                    {
                        Directory.CreateDirectory(iamgeFolder);
                    }

                    string path = iamgeFolder + ofd.SafeFileName.Substring(0, ofd.SafeFileName.LastIndexOf('.') - 1) + ".png";
                    
                    FileStream stream = File.Open(path, FileMode.Create);
                    RenderTargetBitmap bmp = new RenderTargetBitmap(300,
                                      300, 300, 300, PixelFormats.Pbgra32);
                    bmp.Render(_visual);
                    PngBitmapEncoder coder = new PngBitmapEncoder();
                    coder.Interlace = PngInterlaceOption.Off;
                    coder.Frames.Add(BitmapFrame.Create(bmp));
                    coder.Save(stream);
                    stream.Close();

                    gp.PHOTO = File.ReadAllBytes(path);//把图像的二进制数据存储到emp的Photo属性中
                    imagePhone.Source = new BitmapImage(new Uri(path));//将图片显示到Image控件上

                    //FileStream stream = File.Open("temp.png", FileMode.Create);
                    //RenderTargetBitmap bmp = new RenderTargetBitmap(96,96, 96, 96, PixelFormats.Pbgra32);
                    //bmp.Render(me);
                    //PngBitmapEncoder coder = new PngBitmapEncoder();
                    //coder.Interlace = PngInterlaceOption.Off;
                    //coder.Frames.Add(BitmapFrame.Create(bmp));
                    //coder.Save(stream);
                    //stream.Close();



                    //string path = System.Windows.Forms.Application.StartupPath + "\\tempImages\\" + ofd.SafeFileName.Substring(0,ofd.SafeFileName.LastIndexOf('.')-1) + ".png";

                    //FileStream stream = File.Open(path, FileMode.Create);
                    //RenderTargetBitmap bmp = new RenderTargetBitmap(300,
                    //     300, 100, 100, PixelFormats.Pbgra32);
                    //bmp.Render(me);
                    //PngBitmapEncoder coder = new PngBitmapEncoder();
                    //coder.Interlace = PngInterlaceOption.Off;
                    //coder.Frames.Add(BitmapFrame.Create(bmp));
                    //coder.Save(stream);
                    //stream.Close();

                    //gp.PHOTO = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                }
                else
                {
                    gp.PHOTO = File.ReadAllBytes(fileName);//把图像的二进制数据存储到emp的Photo属性中
                    imagePhone.Source = new BitmapImage(new Uri(fileName));//将图片显示到Image控件上
                }
            }
        }

        /// <summary>
        /// 判断文件是否是视频
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns> 
        public bool IsVoide(string fileName)
        {
            string strFilter = ".mp4|.flv|.rmvb|.avi|";
            char[] separtor = { '|' };
            string[] tempFileds = strFilter.Split(separtor);
            foreach (string str in tempFileds)
            {
                if (str.ToUpper() == fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")).ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
