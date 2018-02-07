using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Best.VWPlatform.Common.Helper
{
    public class TimeCalculateHelper
    {
        private volatile int _invokeCount;
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Send);

        public TimeCalculateHelper()
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 1);
            _timer.Tick += new EventHandler(OnTimerTick);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (PerSecondCalculated != null)
                PerSecondCalculated(_invokeCount);

            _invokeCount = 0;
        }

        public void PerSecondInvokeCount()
        {
            if (!_timer.IsEnabled)
                _timer.Start();
                
            _invokeCount++;
        }

        public void StopTimeCalculateHelper()
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                _invokeCount = 0;
            }             
        }

        public event Action<double> PerSecondCalculated;
    }
}
