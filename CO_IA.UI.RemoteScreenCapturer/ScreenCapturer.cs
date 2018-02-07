/**
 *远程桌面内容抓取 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CO_IA.UI.RemoteScreenCapturer.CapService;
using CO_IA.UI.RemoteScreenCapturer.Tools;

namespace CO_IA.UI.RemoteScreenCapturer
{
    public sealed class ScreenCapturer
    {
        private static ScreenCapturer _instance;

        // Lock synchronization object
        private static readonly object _syncLock = new object();
        //private static System.Threading.Mutex mutex;

        private static CaptureService capService;
        private static ScreenCapToolWin toolWindow;

        Action<int> CallBackState;
        private ScreenCapturer()
        {
            capService = new CaptureService();
           
        }

        /// <summary>
        /// 单件模式
        /// </summary>
        /// <returns></returns>
     
        public static ScreenCapturer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ScreenCapturer();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取是否处于抓屏状态
        /// </summary>
        /// <returns></returns>
        public bool IsCapture
        {
            get
            {

                return capService.IsCapture;
            }
        }


        /// <summary>
        /// 获取截屏刷新时间
        /// </summary>
        /// <returns></returns>
        public int CaptureInteval
        {
            get
            {

                return (int) capService.CaptureInterval;
            }
        }

        /// <summary>
        /// 获取服务器访问的URL地址
        /// </summary>
        /// <returns></returns>
        public string ServerUrl
        {
            get
            {

                return capService.ServiceUrl;
            }
        }

        /// <summary>
        /// 获取服务器图片访问的URL地址
        /// </summary>
        /// <returns></returns>
        public string ImageUrl
        {
            get
            {

                return capService.ServiceImgUrl;
            }
        }


        /// <summary>
        /// 获取抓屏的大小
        /// </summary>
        /// <returns></returns>
        public Size CaptureSize
        {
            get
            {
                Size sz = new Size();
                sz.Width = capService.Bounds.Width;
                sz.Height = capService.Bounds.Height;
                return sz;
            }
        }

        /// <summary>
        /// 显示工具窗口
        /// </summary>
        public  void ShowToolWindow(Action<int> CallBackState)
        {
            toolWindow = new ScreenCapToolWin(CallBackState);
            toolWindow.capService = capService;
            //toolWindow.Hide();
            toolWindow.Show();
            toolWindow.Activate();

        }


        /// <summary>
        /// 关闭服务器
        /// </summary>
        public void ShutDownServer()
        {
            if(toolWindow!=null)
            {
                toolWindow.Close();
            }

        }




        /// <summary>
        /// 隐藏工具窗口
        /// </summary>        
        //public  void HideToolWindow()
        //{
        //   toolWindow.Hide();
        //}

        /// <summary>
        /// 释放资源
        /// </summary>
        private void Release()
        {
            //toolWindow.Close();
        }
    }
}
