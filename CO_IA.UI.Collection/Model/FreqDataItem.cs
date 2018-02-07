using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection
{
    /// <summary>
    /// 频率数据项  对应一次回调函数
    /// </summary>
    public partial class FreqDataTemplate
    {
        //System.Collections.Concurrent.ConcurrentQueue<T>
        public AgilentDll.Sensor.SegmentData segData;
        //public IntPtr data;
        public float[] volList;

    }

    /// <summary>
    /// 一个完整的频段数据
    /// </summary>
    public class FreqFrameItem
    {
        public FreqFrameItem(int _FreqMeasureId, TimeSpan _FreqFrameTs)
        {
            FreqMeasureId = _FreqMeasureId;
            FreqFrameTs = _FreqFrameTs;
        }
        /// <summary>
        /// 测量的编号
        /// </summary>
        public int FreqMeasureId;
        public List<FreqDataTemplate> FreqDataItemList = new List<FreqDataTemplate>();
        /// <summary>
        /// 与上一帧数据的时间间隔
        /// </summary>
        public TimeSpan FreqFrameTs;

    }
}
