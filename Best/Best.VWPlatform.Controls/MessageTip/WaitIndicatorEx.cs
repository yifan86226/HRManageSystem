using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Best.VWPlatform.Common.Utility;
using System.Windows;

namespace Best.VWPlatform.Controls.MessageTip
{
    /// <summary>
    /// 等待信息提示
    /// </summary>
    public static class WaitIndicatorEx
    {
        private static Popup _topPopup;
        private static BackgroundPanelEx _topBackgound;
        private static DispatcherTimer _timer;
        private static string _waitSecondMessage;
        private static uint _waitSecond;
        private static Action _timeOverCallback;
        private static readonly object LockObj = new object();
        private static readonly List<object> WaitMessageObjects = new List<object>();

        /// <summary>
        /// 显示等待信息
        /// </summary>
        /// <param name="pWaitMessage">等待消息</param>
        /// <param name="pOwner">表示拥有此窗体的所有者</param>
        /// <param name="pCanCancel">是否可以手动取消是否继续等待，默认 false </param>
        public static IWaitIndicatorEx WaitMessage(string pWaitMessage, UserControl pOwner, bool pCanCancel = false)
        {
            if (pOwner == null || pOwner.Parent == null)
            {
                return null;
            }

            var waitIndicator = Utile.GetParent<IWaitIndicatorEx>(pOwner);
            if (waitIndicator == null)
                return null;
            waitIndicator.WaitMessage = pWaitMessage;
            waitIndicator.CanCancelWaitMessage = pCanCancel;
            if (string.IsNullOrWhiteSpace(pWaitMessage) && WaitMessageObjects.Contains(pOwner))
            {
                WaitMessageObjects.Remove(pOwner);
            }
            return waitIndicator;
        }

        /// <summary>
        /// 显示等待信息
        /// </summary>
        /// <param name="pWaitMessage">等待时显示的消息</param>
        /// <param name="pOwner">表示拥有此窗体的所有者</param>
        /// <param name="pSecond">多长时间后关闭消息窗（单位：秒），在时间范围内再次执行该方法，将重新计算时间。最小值 3 秒</param>
        /// <param name="pCallback">给定时间结束后回调</param>
        public static void WaitMessage(string pWaitMessage, UserControl pOwner, uint pSecond, Action pCallback)
        {
            if (pSecond < 3 || pOwner == null)
                return;
            if (!WaitMessageObjects.Contains(pOwner))
                WaitMessageObjects.Add(pOwner);
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, (int)pSecond);
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                timer = null;
                if (WaitMessageObjects.Contains(pOwner))
                {
                    WaitMessage(null, pOwner);
                    if (pCallback != null)
                        pCallback();
                }
            };
            timer.Start();
            WaitMessage(pWaitMessage, pOwner);
        }

        /// <summary>
        /// 显示等待信息（全局，影响整个窗体）
        /// </summary>
        /// <param name="pWaitMessage"></param>
        public static void WaitMessage(string pWaitMessage)
        {
            if (_topPopup == null)
            {
                _topPopup = new Popup();
                _topBackgound = new BackgroundPanelEx();
            }

            if (string.IsNullOrWhiteSpace(pWaitMessage))
            {
                _topPopup.Child = null;
                _topPopup.IsOpen = false;
            }
            else
            {
                _topBackgound.WaitMessage = pWaitMessage;
                _topPopup.Child = _topBackgound;
                _topPopup.Placement = PlacementMode.Center;
                _topPopup.PlacementTarget = Application.Current.MainWindow;
                _topPopup.IsOpen = true;
            }
        }

        /// <summary>
        /// 显示等待信息（全局，影响整个窗体）
        /// </summary>
        /// <param name="pWaitMessage">等待时显示的消息</param>
        /// <param name="pSecond">多长时间后关闭消息窗（单位：秒），在时间范围内再次执行该方法，将重新计算时间</param>
        /// <param name="pTimeOverCallback">时间到了之后，回调方法</param>
        public static void WaitMessage(string pWaitMessage, uint pSecond, Action pTimeOverCallback = null)
        {
            if (string.IsNullOrWhiteSpace(pWaitMessage))
                return;
            lock (LockObj)
            {
                _waitSecondMessage = pWaitMessage;
                _waitSecond = pSecond;
                _timeOverCallback = pTimeOverCallback;
            }
            if (_timer == null)
            {
                lock (LockObj)
                {
                    if (_timer == null)
                    {
                        _timer = new DispatcherTimer();
                        _timer.Interval = new TimeSpan(0, 0, 0, 1);
                        _timer.Tick += OnTimerTick;
                        _timer.Start();
                    }
                }
            }
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            if (_waitSecond != 0)
                _waitSecond--;
            WaitMessage(string.Format("{0}{1}", _waitSecondMessage, _waitSecond));
            if (_waitSecond <= 0 && _timer != null)
            {
                _timer.Stop();
                _timer = null;
                if (_timeOverCallback != null)
                    _timeOverCallback();
            }
        }
    }
}
