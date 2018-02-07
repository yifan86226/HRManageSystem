using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp
{
    public class RmtpDataFrameCacheValue
    {
        public RmtpDataFrameCacheValue()
        {
            ExecuteResult = new ConcurrentQueue<RmtpDataFrame>();
            Audio = new ConcurrentQueue<RmtpDataFrame>();
            Other = new ConcurrentQueue<RmtpDataFrame>();
        }


        public ConcurrentQueue<RmtpDataFrame> ExecuteResult { set; get; }

        public ConcurrentQueue<RmtpDataFrame> Audio { set; get; }

        public ConcurrentQueue<RmtpDataFrame> Other { set; get; }

        public void Clear()
        {
            ExecuteResult = new ConcurrentQueue<RmtpDataFrame>();
            Audio = new ConcurrentQueue<RmtpDataFrame>();
            Other = new ConcurrentQueue<RmtpDataFrame>();
        }

        private RmtpDataFrame DequeueData(ConcurrentQueue<RmtpDataFrame> pQueue)
        {
            RmtpDataFrame re = null;
            if (pQueue.TryDequeue(out re))
            {
                return re;
            }

            return re;
        }

        private int i = 0;
        public RmtpDataFrame DequeueFrameData()
        {
            i++;
            if (i == 1)
            {
                return DequeueData(ExecuteResult);
            }
            else if (i == 2)
            {
                return DequeueData(Audio);
            }
            else
            {
                i = 0;
                return DequeueData(Other);
            }
        }
    }
}
