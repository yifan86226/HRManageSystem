using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Core
{
    /// <summary>
    /// 全局时间控制，整个系统中只有一个
    /// </summary>
    public static class GlobalTimer
    {
        private static readonly object LockObj = new object();
        private static readonly List<object[]> TimerList = new List<object[]>();
        private static object[][] _tempTimerArray;
        private const int DefaultMillsec = 9;
        private static SynchronizationContext _uiContext;
        private static bool _isChanged;

        static GlobalTimer()
        {
            ThreadPool.QueueUserWorkItem(OnRendering);
        }

        private static void OnRendering(object pState)
        {
            while (true)
            {
                if (TimerList.Count == 0)
                {
                    Thread.Sleep(DefaultMillsec);
                    continue;
                }
                lock (LockObj)
                {
                    if (_tempTimerArray == null || _tempTimerArray.Length != TimerList.Count || _isChanged)
                    {
                        _tempTimerArray = TimerList.ToArray();
                        _isChanged = false;
                    }
                }
                foreach (object[] t in _tempTimerArray)
                {
                    var timer = t[0] as ITimer;
                    var secSpan = (TimeSpan)t[1];
                    var oldTime = (DateTime)t[2];
                    var isBackground = (bool)t[3];
                    TimeSpan ts = DateTime.Now.Subtract(oldTime);
                    if (ts < secSpan)
                        continue;
                    t[2] = DateTime.Now;
                    if (timer == null)
                        continue;
                    if (isBackground)
                        timer.Tick(DateTime.Now.TimeOfDay);
                    else
                        _uiContext.Send(OnSendOrPostCallback, new object[] { timer, DateTime.Now.TimeOfDay });
                }
                Thread.Sleep(DefaultMillsec);
            }
        }

        private static void OnSendOrPostCallback(object state)
        {
            var os = (object[])state;
            var timer = os[0] as ITimer;
            if (timer == null)
                return;
            var timeSpan = (TimeSpan)os[1];
            timer.Tick(timeSpan);
        }
        /// <summary>
        /// 初始化全局时间对象
        /// </summary>
        /// <param name="pCurrentSynchronizationContext">UI主线程同步上下文</param>
        public static void Initialize(SynchronizationContext pCurrentSynchronizationContext)
        {
            if (pCurrentSynchronizationContext == null)
                throw new Exception("GlobalTimer.Initialize 初始化参数不可以为空");
            _uiContext = pCurrentSynchronizationContext;
        }
        /// <summary>
        /// 注册响应时间对象
        /// </summary>
        /// <param name="pRespondObject">响应对象</param>
        /// <param name="pMilliseconds">多少毫秒触发一次</param>
        /// <param name="pIsBackground">true - 非UI线程，false - UI线程</param>
        public static void RegisterRespondObject(ITimer pRespondObject, int pMilliseconds = 500, bool pIsBackground = false)
        {
            if (_uiContext == null)
                throw new Exception("GlobalTimer.Initialize 初始化参数不可以为空");
            if (pRespondObject == null)
                return;
            lock (LockObj)
            {
                TimerList.Add(new object[] { pRespondObject, new TimeSpan(0, 0, 0, 0, pMilliseconds), DateTime.Now, pIsBackground });
                _isChanged = true;
            }
        }
        /// <summary>
        /// 注销响应时间对象
        /// </summary>
        /// <param name="pRespondObject"></param>
        public static void UnRegister(ITimer pRespondObject)
        {
            lock (LockObj)
            {
                if (TimerList.Count == 0)
                    return;
                var foundItem = from i in TimerList where i[0].Equals(pRespondObject) select i;
                if (foundItem.Any())
                {
                    TimerList.Remove(foundItem.First());
                    _isChanged = true;
                }
            }
        }
    }
    /// <summary>
    /// 全局时间控制接口
    /// </summary>
    public interface ITimer
    {
        /// <summary>
        /// 超过计时器间隔时触发
        /// </summary>
        /// <param name="pTime">自系统启动以来触发的时间</param>
        void Tick(TimeSpan pTime);
    }
}
