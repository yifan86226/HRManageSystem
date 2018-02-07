using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CO_IA.UI.RemoteScreenCapturer.CapService
{
    public class CaptureService
    {
        #region 对外公开属性
        /// <summary>
        /// 是否正在抓屏
        /// </summary>
        public bool IsCapture { get; set; }
        /// <summary>
        /// 抓屏边界设置
        /// </summary>
        public Rectangle Bounds { get; set; }
        /// <summary>
        /// 抓屏服务访问地址
        /// </summary>
        public string ServiceUrl { get; set; }
        /// <summary>
        /// 抓屏服务图片访问地址
        /// </summary>
        public string ServiceImgUrl { get; set; }
        /// <summary>
        /// 服务器使用的IP
        /// </summary>
        public string IPaddress { get; set; }
        /// <summary>
        /// 服务器使用的端口号
        /// </summary>
        public string ListenPort { get; set; }
        /// <summary>
        /// 抓屏间隔
        /// </summary>
        public double CaptureInterval { get; set; }
        #endregion

        #region 服务器自用属性
       
        private bool isTakingScreenshots;
        private bool isPrivateTask;
        private bool isPreview;
        private bool isMouseCapture;

        private object locker = new object();
        private ReaderWriterLock rwl = new ReaderWriterLock();
        private MemoryStream img;        
        HttpListener serv;
        #endregion

        public CaptureService()
        {            
            serv = new HttpListener();
            serv.IgnoreWriteExceptions = true; 
            img = new MemoryStream();
            IsCapture = false;
            isPrivateTask = false;
            isPreview = false;
            isMouseCapture = false;
            isTakingScreenshots = true;
        }


        public void Start()
        {
            IsCapture = true;

            Task ctask = Task.Factory.StartNew(() => CaptureScreenEvery((int)CaptureInterval));

            Task stask = Task.Factory.StartNew( () =>
                  {
                      //while (IsCapture)
                      //{
                      //    System.Windows.MessageBox.Show("1");
                      //    Thread.Sleep(2000);
                      //}

                      StartServer();
                  });
              
        }

        public void Stop()
        {
            IsCapture = false;
        }

       
        /// <summary>
        /// 需要后台线程执行
        /// </summary>
        /// <returns></returns>
        private bool StartServer()
        {
            //serv = serv??new HttpListener();
            var selectedIP = IPaddress;// _ips.ElementAt(comboIPs.SelectedIndex).Item2;

            var url = string.Format("http://{0}:{1}", selectedIP, ListenPort);
            
            serv.Prefixes.Clear();
           // serv.Prefixes.Add("http://localhost:" + numPort.Value.ToString() + "/"); 测试用
            serv.Prefixes.Add("http://*:" + ListenPort + "/"); // Uncomment this to Allow Public IP Over Internet. [Commented for Security Reasons.]
            serv.Prefixes.Add(url + "/");
            serv.Start();
            Log("Server Started Successfuly!");
            Log("Private Network URL : " + url);
            Log("Localhost URL : " + "http://localhost:" + ListenPort + "/");
            while (IsCapture)
            {
                var ctx = serv.GetContext();
                //Screenshot();
                var resPath = ctx.Request.Url.LocalPath;
                if (resPath == "/") // Route The Root Dir to the Index Page
                    resPath += "index.html";
                var page = AppDomain.CurrentDomain.BaseDirectory + "WebServer" + resPath;
                bool fileExist;
                lock (locker)
                    fileExist = File.Exists(page);
                if (!fileExist)
                {
                    var errorPage = Encoding.UTF8.GetBytes("<h1 style=\"color:red\">404错误！, 未找到文件 </h1><hr><a href=\".\\\">返回</a>");
                    ctx.Response.ContentType = "text/html";
                    ctx.Response.StatusCode = 404;
                    try
                    {
                        ctx.Response.OutputStream.Write(errorPage, 0, errorPage.Length);
                    }
                    catch (Exception ex)
                    {


                    }
                    ctx.Response.Close();
                    continue;
                }

                #region 带用户名密码判断，暂时不用
                //if (isPrivateTask)
                //{
                //    if (!ctx.Request.Headers.AllKeys.Contains("Authorization"))
                //    {
                //        ctx.Response.StatusCode = 401;
                //        ctx.Response.AddHeader("WWW-Authenticate", "Basic realm=Screen Task Authentication : ");
                //        ctx.Response.Close();
                //        continue;
                //    }
                //    else
                //    {
                //        var auth1 = ctx.Request.Headers["Authorization"];
                //        auth1 = auth1.Remove(0, 6); // Remove "Basic " From The Header Value
                //        auth1 = Encoding.UTF8.GetString(Convert.FromBase64String(auth1));
                //        var auth2 = string.Format("{0}:{1}", txtUser.Text, txtPassword.Text);
                //        if (auth1 != auth2)
                //        {
                //            // MessageBox.Show(auth1+"\r\n"+auth2);
                //            Log(string.Format("Bad Login from {0} using {1}", ctx.Request.RemoteEndPoint.Address.ToString(), auth1));
                //            var errorPage = Encoding.UTF8.GetBytes("<h1 style=\"color:red\">Not Authorized !!! </h1><hr><a href=\"./\">Back to Home</a>");
                //            ctx.Response.ContentType = "text/html";
                //            ctx.Response.StatusCode = 401;
                //            ctx.Response.AddHeader("WWW-Authenticate", "Basic realm=Screen Task Authentication : ");
                //            try
                //            {
                //                await ctx.Response.OutputStream.WriteAsync(errorPage, 0, errorPage.Length);
                //            }
                //            catch (Exception ex)
                //            {


                //            }
                //            ctx.Response.Close();
                //            continue;
                //        }

                //    }
                //}

                #endregion

                #region 读取硬盘数据发送客户端
                
                byte[] filedata;
                
                // 需要读取一次文件
                rwl.AcquireReaderLock(Timeout.Infinite);
                filedata = File.ReadAllBytes(page);
                rwl.ReleaseReaderLock();

                var fileinfo = new FileInfo(page);
                if (fileinfo.Extension == ".css") // important for IE -> Content-Type must be defiend for CSS files unless will ignored !!!
                    ctx.Response.ContentType = "text/css";
                else if (fileinfo.Extension == ".html" || fileinfo.Extension == ".htm")
                    ctx.Response.ContentType = "text/html"; // Important For Chrome Otherwise will display the HTML as plain text.



                ctx.Response.StatusCode = 200;
                try
                {
                     ctx.Response.OutputStream.Write(filedata, 0, filedata.Length);
                }
                catch (Exception ex)
                {


                }

                ctx.Response.Close();

                #endregion

               
            }
            return true;
        }
        private  bool CaptureScreenEvery(int msec)
        {
            while (IsCapture)
            {
                if (isTakingScreenshots)
                {
                    TakeScreenshot(isMouseCapture);
                    msec = (int)CaptureInterval;
                    //等待一定秒数定时执行
                    //Task.Delay(msec);
                    Thread.Sleep(msec);
                }


            }
            return true;
        }
        private void TakeScreenshot(bool captureMouse)
        {
            if (captureMouse)
            {
                var bmp = ScreenCapturePInvoke.CaptureFullScreen(true);
                rwl.AcquireWriterLock(Timeout.Infinite);
                bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "WebServer" + "/ScreenTask.jpg", ImageFormat.Jpeg);
                rwl.ReleaseWriterLock();
                //预览截屏
                //if (isPreview)
                //{
                //    img = new MemoryStream();
                //    bmp.Save(img, ImageFormat.Jpeg);
                //    imgPreview.Image = new Bitmap(img);
                //}
                return;
            }
            //Rectangle bounds = Screen.GetBounds(System.Drawing.Point.Empty);
            Rectangle bounds = Bounds; //只截取指定范围
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(bounds.X,bounds.Y,0,0,bounds.Size);
                }
                rwl.AcquireWriterLock(Timeout.Infinite);
                bitmap.Save(AppDomain.CurrentDomain.BaseDirectory + "WebServer" + "/ScreenTask.jpg", ImageFormat.Jpeg);
                rwl.ReleaseWriterLock();

                //预览截屏
                //if (isPreview)
                //{
                //    img = new MemoryStream();
                //    bitmap.Save(img, ImageFormat.Jpeg);
                //    imgPreview.Image = new Bitmap(img);
                //}


            }
        }

        private void Log(string text)
        {
           // txtLog.Text += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : " + text + "\r\n";

        }
    }
}
