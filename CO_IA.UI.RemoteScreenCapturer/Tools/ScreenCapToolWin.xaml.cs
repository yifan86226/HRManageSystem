using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CO_IA.UI.RemoteScreenCapturer.CapService;

namespace CO_IA.UI.RemoteScreenCapturer.Tools
{
    
    /// <summary>
    /// ScreenCapToolWin.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenCapToolWin : Window
    {
        /// <summary>
        /// 抓取服务
        /// </summary>
        public CaptureService capService { get; set; }
        private ObservableCollection<Tuple<string, string>> _ips;
        private CaptureBorder cborder;

        Action<int> CallBackState;

        public ScreenCapToolWin(Action<int> callBackState)
        {
            InitializeComponent();

            this.Loaded += ScreenCapToolWin_Loaded;

            //btnStart.IsEnabled = false;
            CallBackState = callBackState;
            Height = 70;
            _ips = GetAllIPv4Addresses();
            lstBoxIP.DataContext = _ips;
            lstBoxIP.SelectedIndex = 0;
        }

        void ScreenCapToolWin_Loaded(object sender, RoutedEventArgs e)
        {
            //初始默认全屏
            int w, h; 
            var primaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();
            if (primaryScreen != null)
            {
                w = primaryScreen.WorkingArea.Width ;
                h = primaryScreen.WorkingArea.Height ;
            }
            else
            {
                Rect rect = SystemParameters.WorkArea;//获取工作区大小
                w = (int)rect.Width ;
                h = (int)rect.Height;
            }

            Rectangle rects = new Rectangle(0, 0, w, h);
            capService.Bounds = rects;
        }


        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (CallBackState != null)
                CallBackState(-1);
            this.Close();       
        }

        private void buttonSetCapZone_Click(object sender, RoutedEventArgs e)
        {
            if (capService.Bounds.Width == 0)
            {
                Rectangle rect = new Rectangle(300, 300, 550, 350);
                capService.Bounds = rect;
            }

            imgCapZone.Source = getImg("cut02.png");
            CapSeleZone csz = new CapSeleZone();
            csz.Left = capService.Bounds.Left;
            csz.Top = capService.Bounds.Top;
            csz.Width = capService.Bounds.Width;
            csz.Height = capService.Bounds.Height;
            csz.getPosition += GetPosition;
            csz.ShowDialog();

            //btnStart.IsEnabled = true;
        }

        private void GetPosition(System.Windows.Point p, double width, double height)
        {
            //MessageBox.Show(width.ToString());
            imgCapZone.Source = getImg("cut01.png");
            Rectangle rect = new Rectangle((int)p.X, (int)p.Y, (int)width, (int)height);
            capService.Bounds = rect;
        }

        private void buttonConfig_Click(object sender, RoutedEventArgs e)
        {
            if (this.Height == 134)
            {
                Height = 70;
            }
            else
            {
                Height = 134;
            }
        }

        

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (capService.IsCapture) //停止服务器
            {
                imgStartServer.Source = getImg("play.png");
                btnCapSele.IsEnabled = true;
                btnConfig.IsEnabled = true;
                capService.IsCapture = false;

                if (cborder != null) cborder.Close();
                capService.Stop();
                if (CallBackState != null)
                    CallBackState(-2);//停止服务
                
            }
            else //启动服务器
            {
                //判断是否进行配置了抓取区域，没有则弹出需要设置
                if(capService.Bounds.Width==0)
                {
                    MessageBox.Show("请先设置抓取区域，再重新启动服务！");
                    buttonSetCapZone_Click(null, null);
                    return;
                }

                imgStartServer.Source = getImg("stop.png");
                btnCapSele.IsEnabled = false;
                btnConfig.IsEnabled = false;
                capService.IsCapture = true;
                //获取其他参数


                //显示抓取边框
                cborder = new CaptureBorder();
                cborder.Left = capService.Bounds.Left - 4;
                cborder.Top = capService.Bounds.Top - 4;
                cborder.Width = capService.Bounds.Width + 8;
                cborder.Height = capService.Bounds.Height + 8;
                cborder.Show();

                setConfig();
                capService.Start();
                if (CallBackState != null)
                    CallBackState(0);
            }
        }

        private void setConfig()
        {
            capService.IPaddress = _ips.ElementAt(lstBoxIP.SelectedIndex).Item2;
            capService.ListenPort = txtPort.Text.Trim();
            capService.CaptureInterval = int.Parse(txtInterval.Text.Trim());
            capService.ServiceUrl = "http://" + capService.IPaddress + ":" + capService.ListenPort + "/";
            capService.ServiceImgUrl = "http://" + capService.IPaddress + ":" + capService.ListenPort + "/ScreenTask.jpg";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                    DragMove();               
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
                        
        }

        #region 鼠标操作和部分工具函数
        /// <summary>
        /// 保证窗口不跑出屏幕外面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //POINT p = new POINT();
            //Point pp = Mouse.GetPosition(e.Source as FrameworkElement);//WPF方法
            //GetCursorPos(out p);
            double mouseX = Left;
            double mouseY = Top;
            double x = SystemParameters.WorkArea.Width;//得到屏幕工作区域宽度
            double y = SystemParameters.WorkArea.Height;//得到屏幕工作区域高度
            bool isOverBorder = false;
            if (mouseX < 0)
            {
                mouseX = 0;
                isOverBorder = true;
            }
            if (mouseX + 240 > x)
            {
                mouseX = x - 240;
                isOverBorder = true;
            }
            if (mouseY < 0)
            {
                mouseY = 0;
                isOverBorder = true;
            }
            if (mouseY + 70 > y)
            {
                mouseY = y - 70;
                isOverBorder = true;
            }
            if (isOverBorder)
            {
                Left = mouseX;
                Top = mouseY;
            }
        }
        /// <summary>   
        /// 设置鼠标的坐标   
        /// </summary>   
        /// <param name="x">横坐标</param>   
        /// <param name="y">纵坐标</param>   
        [DllImport("User32")]
        public extern static void SetCursorPos(int x, int y);
        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        /// <summary>   
        /// 获取鼠标的坐标   
        /// </summary>   
        /// <param name="lpPoint">传址参数，坐标point类型</param>   
        /// <returns>获取成功返回真</returns>   
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
        private BitmapImage getImg(string imgname)
        {
            string realPath = string.Format("/CO_IA.UI.RemoteScreenCapturer;component/Tools/images/" + imgname);

			Uri pathUri = new Uri(realPath,UriKind.RelativeOrAbsolute);

			System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();

			bi.BeginInit();

			bi.UriSource = pathUri;

			bi.EndInit();

			return bi;
        }

        #endregion


        #region 获取IP地址等信息
        private string GetIPv4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
        private ObservableCollection<Tuple<string, string>> GetAllIPv4Addresses()
        {
            ObservableCollection<Tuple<string, string>> ipList = new ObservableCollection<Tuple<string, string>>();
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {

                foreach (var ua in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(Tuple.Create(ni.Name, ua.Address.ToString()));
                    }
                }
            }
            return ipList;
        }
        //private Task AddFirewallRule(int port)
        //{
        //    return Task.Run(() =>
        //    {

        //        string cmd = RunCMD("netsh advfirewall firewall show rule \"Screen Task\"");
        //        if (cmd.StartsWith("\r\nNo rules match the specified criteria."))
        //        {
        //            cmd = RunCMD("netsh advfirewall firewall add rule name=\"Screen Task\" dir=in action=allow remoteip=localsubnet protocol=tcp localport=" + port);
        //            if (cmd.Contains("Ok."))
        //            {
        //                Log("Screen Task Rule added to your firewall");
        //            }
        //        }
        //        else
        //        {
        //            cmd = RunCMD("netsh advfirewall firewall delete rule name=\"Screen Task\"");
        //            cmd = RunCMD("netsh advfirewall firewall add rule name=\"Screen Task\" dir=in action=allow remoteip=localsubnet protocol=tcp localport=" + port);
        //            if (cmd.Contains("Ok."))
        //            {
        //                Log("Screen Task Rule updated to your firewall");
        //            }
        //        }
        //    });

        //}
        private string RunCMD(string cmd)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = "/C " + cmd;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            string res = proc.StandardOutput.ReadToEnd();
            proc.StandardOutput.Close();

            proc.Close();
            return res;
        }
        private void Log(string text)
        {
           // txtLog.Text += DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " : " + text + "\r\n";
        }

        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]+");// Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        #endregion

        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox) sender;
            int iInteval=0;
            if (int.TryParse(tb.Text, out iInteval))
            {
                if (iInteval > 10000) tb.Text = "9999";
                if (iInteval < 1000) tb.Text = "1000";
            }
        }

        private void TextBox2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int iInteval = 0;
            if (int.TryParse(tb.Text, out iInteval))
            {
                if (iInteval > 2000) tb.Text = "2000";
                if (iInteval < 100) tb.Text = "100";
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (capService.IsCapture) //停止服务器
            {
                imgStartServer.Source = getImg("play.png");
                btnCapSele.IsEnabled = true;
                btnConfig.IsEnabled = true;
                capService.IsCapture = false;

                if (cborder != null) cborder.Close();
                capService.Stop();
                if (CallBackState != null)
                    CallBackState(-2);//停止服务

            }

            if (cborder != null) cborder.Close();
        }
        public override void EndInit()
        {
            base.EndInit();
            double w, h;
            var primaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();
            if (primaryScreen != null)
            {
                w = primaryScreen.WorkingArea.Width;
                h = primaryScreen.WorkingArea.Height;
            }
            else
            {
                Rect rect = SystemParameters.WorkArea;//获取工作区大小
                w = rect.Width;
                h = rect.Height;
            }
            this.Left = w-260;//设置位置
            this.Top = h - 120;
        }
    }
}
