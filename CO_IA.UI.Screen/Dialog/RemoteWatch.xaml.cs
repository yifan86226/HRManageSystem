using CO_IA.Client;
using CO_IA.Client.Orgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
    /// RemoteWatch.xaml 的交互逻辑
    /// </summary>
    public partial class RemoteWatch : Window
    {
        private bool begin = false;
        private string imageUrl = "";
        private System.Windows.Forms.Timer timer = new  System.Windows.Forms.Timer();
        public OrgToMapStyle org;
        public RemoteWatch(string ip)
        {
            InitializeComponent();
            imageUrl = string.IsNullOrEmpty(ip)?"":"http://" + ip + ":7070/ScreenTask.jpg";
            this.Loaded += RemoteWatch_Loaded;
            timer.Tick +=timer_Tick;
            timer.Interval = 500;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            
            timer.Enabled = false;
            if (org == null || org.OrgInfo.OnLine == false)
            {
                MessageBox.Show("当前现场端不在线,远程监控关闭!");
                timer.Stop();
                this.Close();
                begin = false;
                return;
            }
            try
            {
                new Thread(new ThreadStart(() =>
                {
                    byte[] b = GetImage();                    
                    
                    img.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var imageSource = ClientHelper.LoadImageFromBytes(b as byte[]);
                        this.img.Source = imageSource;
                        timer.Enabled = true;
                    }));

                })).Start();

            }
            catch
            {
                timer.Enabled = true;
            }
            //this.img.Source = new BitmapImage(new Uri(imageUrl));  
            
        }
       
        void RemoteWatch_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                begin = true;
                timer.Start();
            }
        }

        private void cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int interval = 500;
            if(cmb.SelectedIndex>=0)
            {
                ComboBoxItem item = cmb.Items[cmb.SelectedIndex] as ComboBoxItem;

                int.TryParse(item.Content.ToString(),out interval);
            }
            if (interval == 0)
                interval = 500;
            if(begin)
                timer.Stop();
            timer.Interval = interval;
            if (begin)
                timer.Start();

        }

        private Byte[] GetImage()
        {
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(imageUrl); 
                System.Net.WebResponse response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Byte[] buffer = new Byte[1024];
                        int current = 0;
                        do
                        {
                            ms.Write(buffer, 0, current);
                        } while ((current = stream.Read(buffer, 0, buffer.Length)) != 0);
  
                        return ms.ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }  
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Enabled = false;
            timer.Tick -= timer_Tick;
            timer.Stop();
            timer = null;
        }
        //private  void DownloadImage()
        //{
        //    var request = WebRequest.Create("http://localhost:8000/www/test.jpg");
        //    using (var response = await request.GetResponseAsync())
        //    using (var destStream = new MemoryStream())
        //    {
        //        var responseStream = response.GetResponseStream();
        //        var downloadTask = responseStream.CopyToAsync(destStream);
        //        RefreshUI(downloadTask, destStream);
        //        await downloadTask;
        //    }
        //}

        //async void RefreshUI(Task downloadTask, MemoryStream stream)
        //{
        //    await Task.WhenAny(downloadTask, Task.Delay(1000));            //每隔一秒刷新一次

        //    var data = stream.ToArray();
        //    var tmpStream = new MemoryStream(data);        //TODO 当图片的头没有下载到时，这儿可能抛异常
        //    var bmp = new BitmapImage();

        //    bmp.BeginInit();
        //    bmp.StreamSource = tmpStream;
        //    bmp.EndInit();

        //    img.Source = bmp;        //刷新图片

        //    if (!downloadTask.IsCompleted)
        //        RefreshUI(downloadTask, stream, contentLength);
        //}
    }
}
