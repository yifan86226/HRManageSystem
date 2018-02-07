using Best.VWPlatform.Controls.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Controls.Common
{
    /// <summary>
    /// 谱图数据缓存管理
    /// </summary>
    internal class SpectrumDataCacheManage : IDisposable
    {
        private readonly MemoryStream _stream = new MemoryStream();
        private int _freqPointCount;
        private int _capacity;
        private short _defaultValue;
        private readonly int _dataStructSize;
        private readonly byte[] _buffer;
        private readonly object _lockObj = new object();
        private double _initializersStartFreq;
        private double _defaultStep;
        private double _defaultStartFreq;
        public SpectrumDataCacheManage()
        {
            _dataStructSize = sizeof(double) + sizeof(short);
            _buffer = new byte[_dataStructSize];
        }

        /// <summary>
        /// 初始化缓存，根据起始频率和步长初始化缓存
        /// </summary>
        /// <param name="pStartFreq">整帧起始频率</param>
        /// <param name="pStep">默认频点之间的步长</param>
        /// <param name="pFreqPointCount">容量初始大小</param>
        /// <param name="pDefaultValue">初始幅度值，默认0</param>
        public void InitializersCache(double pStartFreq, double pStep, int pFreqPointCount, short pDefaultValue = 0)
        {
            lock (_lockObj)
            {
                Clear();
                if (pFreqPointCount == 0 || pStartFreq.Equals(0) || pStep.Equals(0))
                    return;
                _initializersStartFreq = pStartFreq;
                _defaultStep = pStep;
                _freqPointCount = pFreqPointCount;
                _capacity = pFreqPointCount * _dataStructSize;
                _stream.SetLength(_capacity);
                _stream.Seek(0, SeekOrigin.Begin);
                double freq = _defaultStartFreq = pStartFreq;
                _defaultValue = pDefaultValue;
                byte[] dbuvBytes = BitConverter.GetBytes(_defaultValue);
                for (int i = 0; i < pFreqPointCount; i++)
                {
                    _stream.Write(BitConverter.GetBytes(freq), 0, sizeof(double));
                    _stream.Write(dbuvBytes, 0, sizeof(short));
                    freq += pStep;
                }
            }
        }



        public void InitializersCache1(double pStartFreq, double pStep, int pFreqPointCount, short pDefaultValue = 0)
        {
            Clear();
            if (pFreqPointCount == 0 || pStartFreq.Equals(0) || pStep.Equals(0))
                return;
            _initializersStartFreq = pStartFreq;
            _defaultStep = pStep;

        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void Clear()
        {
            _freqPointCount = 0;
            _capacity = 0;
            _stream.SetLength(0);
        }
        /// <summary>
        /// 获取频率值所在缓存中的索引位置
        /// </summary>
        /// <param name="pFreqVal">频率值</param>
        /// <returns>总频点中的索引，未找到返回 -1</returns>
        public long GetIndex(double pFreqVal)
        {
            return GetRoughlyIndex(pFreqVal);
        }
        /// <summary>
        /// 获取频率值所在缓存中的索引位置，如果没有相同的pFreqVal位置的索引，就取小于pFreqVal位置的索引
        /// </summary>
        /// <param name="pFreqVal">频率值</param>
        /// <returns>总频点中的索引，未找到返回 -1</returns>
        public long GetRoughlyIndex(double pFreqVal)
        {
            long high;
            long roughlyIndex;
            if (FoundIndex(pFreqVal, out high, out roughlyIndex))
                return roughlyIndex;
            return high;
        }

        private bool FoundIndex(double pFreqVal, out long high, out long roughlyIndex)
        {
            if (_capacity == 0)
            {
                roughlyIndex = 0;
                high = 0;
                return true;
            }
            long low = 0;
            high = _stream.Length / _dataStructSize - 1;
            while (low <= high)
            {
                long mid = (low + high) / 2;
                _stream.Seek(mid * _dataStructSize, SeekOrigin.Begin);
                _stream.Read(_buffer, 0, _dataStructSize);
                double freqVal = BitConverter.ToDouble(_buffer, 0);
                if (freqVal.Equals(pFreqVal))
                {
                    roughlyIndex = mid;
                    return true;
                }
                if (freqVal > pFreqVal)
                    high = mid - 1;
                else
                    low = mid + 1;
            }
            if (high < 0)
            {
                roughlyIndex = 0;
                return true;
            }

            //if (low > high)
            //{
            //    roughlyIndex = high;
            //    return true;
            //}

            roughlyIndex = 0;
            return false;
        }
        /// <summary>
        /// 获取缓存中的幅度值
        /// </summary>
        /// <param name="pFreqIndex">频率值在频点中的索引</param>
        /// <returns></returns>
        public short GetDbuv(long pFreqIndex)
        {
            if (_capacity == 0 || pFreqIndex == -1 || pFreqIndex >= _freqPointCount)
                return 0;
            //超出流范围
            long offset = pFreqIndex * _dataStructSize;
            if (_stream.Length == 0 || offset >= _stream.Length)
                return _defaultValue;
            _stream.Seek(offset, SeekOrigin.Begin);
            _stream.Read(_buffer, 0, _dataStructSize);
            return BitConverter.ToInt16(_buffer, 8);
        }
        /// <summary>
        /// 获取缓存中的频率值
        /// </summary>
        /// <param name="pFreqIndex">频率值在频点中的索引</param>
        /// <returns>频率值</returns>
        public double GetFreq(long pFreqIndex)
        {
            if (_capacity == 0 || pFreqIndex == -1 || pFreqIndex >= _freqPointCount)
                return 0;
            //超出流范围
            long offset = pFreqIndex * _dataStructSize;
            if (_stream.Length == 0 || offset >= _stream.Length)
                return 0;
            _stream.Seek(offset, SeekOrigin.Begin);
            _stream.Read(_buffer, 0, _dataStructSize);
            return BitConverter.ToDouble(_buffer, 0);
        }

        /// <summary>
        /// 获取缓存中的谱图点数据
        /// </summary>
        /// <param name="pFreqIndex">频率值在频点中的索引</param>
        /// <returns>谱图点数据</returns>
        public SpectrumPointData? GetSpectrumPointData(long pFreqIndex)
        {
            if (_capacity == 0)
                return null;

            if (pFreqIndex < 0)
            {
                _stream.Seek(0, SeekOrigin.Begin);
            }
            else if (pFreqIndex >= _freqPointCount)
            {
                long offset = _freqPointCount - 1;
                if (offset <= 0)
                {
                    _stream.Seek(0, SeekOrigin.Begin);
                }
                else
                {
                    offset = offset * _dataStructSize;
                    _stream.Seek(offset, SeekOrigin.Begin);
                }
            }
            else
            {
                long offset = pFreqIndex * _dataStructSize;
                if (offset > _stream.Length)
                    return null;
                _stream.Seek(offset, SeekOrigin.Begin);
            }

            _stream.Read(_buffer, 0, _dataStructSize);

            double freqV = BitConverter.ToDouble(_buffer, 0);
            short dbuv = BitConverter.ToInt16(_buffer, 8);

            return new SpectrumPointData { Freq = freqV, Dbuv = dbuv };
        }

        /// <summary>
        /// 获取频率范围内最大谱图点数据
        /// </summary>
        /// <param name="pStartFreq">起始频率</param>
        /// <param name="pStopFreq">终止频率</param>
        /// <returns>谱图点数据</returns>
        public SpectrumPointData? GetMaxSpectrumPointData(double pStartFreq, double pStopFreq)
        {
            long sIndex = GetIndex(pStartFreq) + 1;
            long eIndex = GetIndex(pStopFreq) - 1;
            long count = eIndex - sIndex;
            if (count <= 0)
                return null;
            var sp = new SpectrumPointData();
            lock (_lockObj)
            {
                double maxDubv = -500;
                for (long i = sIndex; i < eIndex; i++)
                {
                    long offset = i * _dataStructSize;
                    if (offset > _stream.Length)
                        return null;
                    _stream.Seek(offset, SeekOrigin.Begin);
                    _stream.Read(_buffer, 0, _dataStructSize);
                    double freqV = BitConverter.ToDouble(_buffer, 0);
                    short dbuv = BitConverter.ToInt16(_buffer, 8);
                    if (dbuv > maxDubv)
                    {
                        maxDubv = dbuv;
                        sp.Dbuv = dbuv;
                        sp.Freq = freqV;
                    }
                }
            }
            return sp;
        }
        /// <summary>
        /// 获取频率范围内最小谱图点数据
        /// </summary>
        /// <param name="pStartFreq">起始频率</param>
        /// <param name="pStopFreq">终止频率</param>
        /// <returns>谱图点数据</returns>
        public SpectrumPointData? GetMinSpectrumPointData(double pStartFreq, double pStopFreq)
        {
            long sIndex = GetIndex(pStartFreq) + 1;
            long eIndex = GetIndex(pStopFreq) - 1;
            long count = eIndex - sIndex;
            if (count <= 0)
                return null;
            var sp = new SpectrumPointData();
            lock (_lockObj)
            {
                double minDubv = 0;
                for (long i = sIndex; i < eIndex; i++)
                {
                    long offset = i * _dataStructSize;
                    if (offset > _stream.Length)
                        return null;
                    _stream.Seek(offset, SeekOrigin.Begin);
                    _stream.Read(_buffer, 0, _dataStructSize);
                    double freqV = BitConverter.ToDouble(_buffer, 0);
                    short dbuv = BitConverter.ToInt16(_buffer, 8);
                    if (dbuv < minDubv)
                    {
                        minDubv = dbuv;
                        sp.Dbuv = dbuv;
                        sp.Freq = freqV;
                    }
                }
            }
            return sp;
        }
        /// <summary>
        /// 刷新谱数据到缓存，针对分段数据帧处理方法
        /// </summary>
        /// <param name="pStartFreq">整帧起始频率</param>
        /// <param name="pStep">默认频点之间的步长</param>
        /// <param name="pSecStartFreq">分段谱数据的起始频率，为 0 时使用 pStartFreq作为起始频率</param>
        /// <param name="pSecStep">分段谱数据的步长，为 0 时使用 pStep 作为频点之间的步长</param>
        /// <param name="pDbuvData">幅度值</param>
        public void UpdateSpectrumData(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, short[] pDbuvData)
        {
            lock (_lockObj)
            {
                if (_capacity == 0 || pStartFreq.Equals(0) || pStep.Equals(0) || pDbuvData == null
                    || (pSecStartFreq.Equals(0) && pSecStep.Equals(0) && pDbuvData.Length > _freqPointCount))
                    return;
                double startFreq = pStartFreq;
                double step = pStep;

                if (!pSecStartFreq.Equals(0))
                    startFreq = pSecStartFreq;
                if (!pSecStep.Equals(0))
                    step = pSecStep;

                //计算谱数据在缓存中的写入位置
                long index = GetIndex(startFreq);
                if (index != -1)
                {
                    _stream.Seek(index * _dataStructSize, SeekOrigin.Begin);
                }
                else
                {
                    if (!pStartFreq.Equals(pSecStartFreq))
                    {
                        //分段谱数据的起始频率不是缓存起点
                        index = GetRoughlyIndex(pSecStartFreq);
                        _stream.Seek(index * _dataStructSize, SeekOrigin.Begin);
                    }
                    else
                    {
                        index = 0;
                        _stream.Seek(index, SeekOrigin.Begin);
                    }
                }

                foreach (short t in pDbuvData)
                {
                    _stream.Write(BitConverter.GetBytes(startFreq), 0, sizeof(double));
                    _stream.Write(BitConverter.GetBytes(t), 0, sizeof(short));
                    OnUpdateSpectrumPointData(index++, new SpectrumPointData { Freq = startFreq, Dbuv = t });
                    //频率点加步进等于下一个频率点
                    startFreq += step;
                }
            }
        }

        /// <summary>
        /// 刷新谱数据到缓存，针对分段数据帧处理方法
        /// </summary>
        /// <param name="pStartFreq">整帧起始频率</param>
        /// <param name="pStep">默认频点之间的步长</param>
        /// <param name="pSecStartFreq">分段谱数据的起始频率，为 0 时使用 pStartFreq作为起始频率</param>
        /// <param name="pSecStep">分段谱数据的步长，为 0 时使用 pStep 作为频点之间的步长</param>
        /// <param name="pNumber">频率序号</param>
        /// <param name="pDbuvData">幅度值</param>
        public void UpdateSpectrumData(double pStartFreq, double pStep, double pSecStartFreq, double pSecStep, uint pNumber, short[] pDbuvData)
        {
            lock (_lockObj)
            {
                if (_capacity == 0 || pStartFreq.Equals(0) || pStep.Equals(0) || pDbuvData == null
                    || (pSecStartFreq.Equals(0) && pSecStep.Equals(0) && pDbuvData.Length > _freqPointCount))
                    return;

                double startFreq = pStartFreq;
                double step = pStep;
                if (!pSecStartFreq.Equals(0))
                    startFreq = pSecStartFreq;
                if (!pSecStep.Equals(0))
                    step = pSecStep;

                //计算谱数据在缓存中的写入位置
                long index = pNumber;// GetIndex(startFreq);
                if (index != -1)
                {
                    _stream.Seek(index * _dataStructSize, SeekOrigin.Begin);
                }
                else
                {
                    _stream.Seek(index * _dataStructSize, SeekOrigin.Begin);
                }

                foreach (short t in pDbuvData)
                {
                    _stream.Write(BitConverter.GetBytes(startFreq), 0, sizeof(double));
                    _stream.Write(BitConverter.GetBytes(t), 0, sizeof(short));
                    OnUpdateSpectrumPointData(index++, new SpectrumPointData { Freq = startFreq, Dbuv = t });
                    startFreq += step;
                }
            }
        }
        /// <summary>
        /// 刷新谱数据到缓存
        /// </summary>
        /// <param name="pDbuvData">幅度值</param>
        public void UpdateSpectrumData(short[] pDbuvData)
        {
            UpdateSpectrumData(_initializersStartFreq, _defaultStep, 0, 0, pDbuvData);
        }
        /// <summary>
        /// 获取指定频率范围内的幅度值数组
        /// </summary>
        /// <param name="pStartFreq">起始频率</param>
        /// <param name="pStopFreq">终止频率</param>
        /// <returns>幅度值数组</returns>
        public short[] GetDbuvData(double pStartFreq, double pStopFreq)
        {
            if (pStartFreq >= pStopFreq)
                return null;
            long l = GetRoughlyIndex(pStartFreq);
            long r = GetRoughlyIndex(pStopFreq);
            if (l >= r)
                return null;
            long count = r - l;
            var buffer = new List<short>((int)count);
            for (int i = 0; i < count; i++)
            {
                _stream.Seek(l * _dataStructSize, SeekOrigin.Begin);
                _stream.Read(_buffer, 0, _dataStructSize);
                buffer.Add(BitConverter.ToInt16(_buffer, sizeof(double)));
                l += 1;
            }
            return buffer.ToArray();
        }
        /// <summary>
        /// 获取指定频率范围内的频点数量
        /// </summary>
        /// <param name="pStartFreq">起始频率</param>
        /// <param name="pStopFreq">终止频率</param>
        /// <returns>频点数量</returns>
        public long GetFreqCount(double pStartFreq, double pStopFreq)
        {
            long l = GetRoughlyIndex(pStartFreq);
            long r = GetRoughlyIndex(pStopFreq);
            if (l >= r)
                return 0;
            return r - l;
        }

        internal void InitializersCache(MemoryStream pStream)
        {
            Debug.Assert(pStream != null);
            _stream.Seek(0, SeekOrigin.Begin);
            pStream.Seek(0, SeekOrigin.Begin);
            pStream.WriteTo(_stream);
        }
        #region Propertys
        /// <summary>
        /// 获取频率总点数
        /// </summary>
        public int FreqPointCount
        {
            get { return _freqPointCount; }
        }

        internal double DefaultStartFreq
        {
            get { return _defaultStartFreq; }
        }

        internal double DefaultStep
        {
            get { return _defaultStep; }
        }

        internal MemoryStream Stream
        {
            get { return _stream; }
        }
        #endregion

        #region events
        /// <summary>
        /// 刷新谱图点数据事件
        /// </summary>
        /// <remarks>
        /// long - 频率点索引
        /// SpectrumPointData - 谱图点数据
        /// </remarks>
        public event Action<long, SpectrumPointData> UpdateSpectrumPointData;

        public void OnUpdateSpectrumPointData(long pFreqIndex, SpectrumPointData pPointData)
        {
            Action<long, SpectrumPointData> handler = UpdateSpectrumPointData;
            if (handler != null) handler(pFreqIndex, pPointData);
        }
        #endregion

        #region IDisposable
        private bool _disposed;
        ~SpectrumDataCacheManage()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool mDisposing)
        {
            if (!_disposed)
            {
                if (mDisposing)
                {
                    if (_stream != null)
                    {
                        _stream.Close();
                        _stream.Dispose();
                    }
                }
                _disposed = true;
            }
        }
        #endregion
    }
    /// <summary>
    /// 谱图点数据
    /// </summary>
    public struct SpectrumPointData
    {
        /// <summary>
        /// 频率值
        /// </summary>
        public double Freq { get; internal set; }
        /// <summary>
        /// 幅度值、幅度中值
        /// </summary>
        public short Dbuv { get; internal set; }

        //多频段
        /// <summary>
        /// 频点在整个谱图中的点索引
        /// </summary>
        public int Index { get; internal set; }
        /// <summary>
        /// 频段内索引
        /// </summary>
        public int SegmentIndex { get; internal set; }
        /// <summary>
        /// 频段内频点总数
        /// </summary>
        public int SegmentCount { get; internal set; }
        /// <summary>
        /// 幅度最大值
        /// </summary>
        public short? MaxDbuv { get; internal set; }
        /// <summary>
        /// 噪声
        /// </summary>
        public short? Noise { get; internal set; }
        /// <summary>
        /// 与该频点相关的信号集合
        /// </summary>
        public List<SpectrumSignalData> SignalDatas { get; set; }

        public void InitSignals()
        {
            SignalDatas = new List<SpectrumSignalData>();
        }
    }
}
