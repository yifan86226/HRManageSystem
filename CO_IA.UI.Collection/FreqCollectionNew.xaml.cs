using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using GalaSoft.MvvmLight.Threading;
using AgilentDll;
using GalaSoft.MvvmLight.Messaging;
using CO_IA.UI.Collection.MessageEntity;
using CO_IA.UI.Collection.ViewModel;

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// FreqCollection.xaml 的交互逻辑
    /// </summary>
    public partial class FreqCollectionNew : UserControl
    {
        public FreqCollectionNew()
        {
            InitializeComponent();
            DispatcherHelper.Initialize();
            this.Initialized += new EventHandler(WinFreqDataCollect_SourceInitialized);
            //Messenger.Default.Register<PageGoMessage>(this, (action) => ReceiveMessage(action));
            //Messenger.Default.Register<GenericMessage<string>>(this, GenericMessageAction);
        } 

        private void GenericMessageAction(GenericMessage<string> msg)
        {
            if (msg.Content.ToLower().IndexOf("showmainwindow") > -1)
            {
                this.Visibility = Visibility.Visible;
            }
            else if (msg.Content.IndexOf("ErrorSerialPort") > -1)
            {
                MessageBox.Show("串口打开失败，请检查串口设置！");
            }
            else if (msg.Content.IndexOf("ErrorConnectReciver") > -1)
            {
                MessageBox.Show("接收机连接失败，请检查接收机设备！");
            }
            else if (msg.Content.IndexOf("Error") > -1)
            {
            }
        }

        private void WinFreqDataCollect_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(this)).Handle;
            HwndSource.FromHwnd(hwnd).AddHook(new HwndSourceHook(WndProc));
        }

        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                switch (wParam.ToInt32())
                {
                    case DBT_DEVICEARRIVAL:
                        {
                            System.Console.WriteLine("添加了usb设备！！");
                            break;
                        }
                    case DBT_DEVICEREMOVECOMPLETE:
                        {
                            System.Console.WriteLine("移除了usb设备！");
                            //if (GpsLib.Gps.CheckGPSHotSwap() == 1)
                            //{
                            //    GpsLib.Gps.IsOpenGps = false;
                            //}
                            //else if (AgilentDll.Decoder_CDMA2G.CheckCDMAHotSwap() == 1)
                            //{
                            //    AgilentDll.Decoder_CDMA2G.IsOpen = false;
                            //}
                            break;
                        }

                    default:
                        break;
                }
            }
            return IntPtr.Zero;
        }

        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (IsCorrenctIP(text_IP.Text))
            {
                if (!AgilentDll.Sensor.Connect(text_IP.Text))
                {
                    Sensor.IsUseSensor = false;
                    text_status.Text = "连接状态：设备未连接";
                    text_status.Foreground = Brushes.Red;
                    return;
                }
                else
                {
                    Sensor.IsUseSensor = true;
                    text_status.Text = "连接状态：设备准备就绪Sensor";
                    img_status.Source = new BitmapImage(new Uri("/CO_IA.UI.Collection;component/Images/Connect.png", 0));
                    text_status.Foreground = Brushes.Green;
                    btn_Connect.IsEnabled = false;
                    //btn_collect.IsEnabled = true;
                    btn_disConnect.IsEnabled = true;
                }
            }
            else
            {
                MessageBox.Show("请重新填写IP地址");
            }
        }

        private void btn_disConnect_Click(object sender, RoutedEventArgs e)
        {
            Sensor.Close();
            Sensor.IsUseSensor = false;

            btn_Connect.IsEnabled = true;
            btn_disConnect.IsEnabled = false;
            text_status.Text = "连接状态：设备未连接";
            text_status.Foreground = Brushes.Red;
            //btn_collect.IsEnabled = false;
            img_status.Source = new BitmapImage(new Uri("/CO_IA.UI.Collection;component/Images/disconnect.png", 0));
        }

        private void btn_collect_Click(object sender, RoutedEventArgs e)
        {

            //btn_collect.IsEnabled = false;
            //btn_disCollect.IsEnabled = true;
            btn_disConnect.IsEnabled = false;
            //TextEable = true;
        }

        private void btn_disCollect_Click(object sender, RoutedEventArgs e)
        {
            //btn_collect.IsEnabled = true;
            //btn_disCollect.IsEnabled = false;
            btn_disConnect.IsEnabled = true;
            //TextEable = false;
        }
        public bool IsCorrenctIP(string ip)
        {
            string pattrn = @"(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])";
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, pattrn))
            {
                return true;
            }
            else
            {
                return
            false;

            }
        }

        private void SpinEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (spin_start.Value > 0 && spin_stop.Value > spin_start.Value)
            {
                //btn_collect.IsEnabled = true;
            }
            else
            {
                //btn_collect.IsEnabled = false;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
        }
                
        //private void onRadioA(object sender, RoutedEventArgs e)
        //{
        //    if (spA != null)
        //    {
        //        spA.Visibility = System.Windows.Visibility.Visible;
        //        spB.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //}
        
        //private void onRadioB(object sender, RoutedEventArgs e)
        //{
        //    if (spA != null)
        //    {
        //        spA.Visibility = System.Windows.Visibility.Collapsed;
        //        spB.Visibility = System.Windows.Visibility.Visible;
        //    }
        //}
    }
}
