using System.Windows.Threading;
using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Helper;
using Best.VWPlatform.Common.Interfaces;
using Best.VWPlatform.Common.Rmtp.Commands;
using Best.VWPlatform.Common.Rmtp.DataFrames;
using Best.VWPlatform.Common.Rmtp.MeasureHandler;
using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 射频全景测量任务处理类
    /// </summary>
    public sealed class WidebandFreqMeasureHandler : IReceiveDataFrame, IRmtpNotification, IDisposable, ITimer, IMonitorMeasureHandler
    {
        private WbfqParameter _parameter;
        private WbfqexDescribeHeader _header;
        private readonly TimeCalculateHelper _timeHelper = new TimeCalculateHelper();
        private int _pricode = 1;
        private int _channelId = 0;

        private readonly SynchronizationContext _uiContext;

        private readonly CacheSignalStatistics _cSignalStatisticsItems = new CacheSignalStatistics();

        private CacheSpectrumData _cSpectrumDataItems = new CacheSpectrumData();

        /// <summary>
        /// 记录返回的执行结果，1表示预执行指令成功发送参数指令，2表示参数指令成功开始接收数据,3表示停止指令成功，清空数据，断开连接
        /// </summary>
        private int _executeresultNum = 0;

        public WidebandFreqMeasureHandler()
        {
            _timeHelper.PerSecondCalculated += OnPerSecondFrameCount;

            GlobalTimer.RegisterRespondObject(this, 0);

            _uiContext = SynchronizationContext.Current;
        }

        #region private functions

        public void ClearSignalStatisticsList()
        {
            _cSignalStatisticsItems.Clear();
        }

        public void OnSignalStatisticsCompleted(SignalStatisticsItem pSignalItem)
        {
            if (pSignalItem == null)
            {
                return;
            }

            Action<SignalStatisticsItem> handler = SignalStatisticsCompleted;
            if (handler != null) handler(pSignalItem);
        }

        public void OnBackgroundCompleted(WbfqexDataFrame pDataFrame)
        {
            if (pDataFrame == null)
            {
                return;
            }

            Action<WbfqexDataFrame> handler = BackgroundCompleted;
            if (handler != null) handler(pDataFrame);
        }

        public void OnNoiseCompleted(WbfqexDataFrame pDataFrame)
        {
            if (pDataFrame == null)
            {
                return;
            }

            Action<WbfqexDataFrame> handler = NoiseCompleted;
            if (handler != null) handler(pDataFrame);
        }

        public void OnSpectrumDataCompleted(WbfqexDataFrame pData)
        {
            if (pData != null)
            {
                Action<WbfqexDataFrame> handler = SpectrumDataCompleted;
                if (handler != null) handler(pData);               
            }
        }

        #endregion

        #region IReceiveDataFrame
        public void Receive(RmtpDataFrame pDataFrame)
        {
            switch (pDataFrame.Header.DataType)
            {
                case RmtpDataTypes.执行结果信息:
                    _uiContext.Send(ReceiveExecuteResult, pDataFrame);
                    break;
                case RmtpDataTypes.设备参数信息:
                    break;
                case RmtpDataTypes.数据描述头:
                    _uiContext.Send(ReceiveHeader, pDataFrame);    
                    break;
                case RmtpDataTypes.业务数据:
                    ReceiveData(pDataFrame);
                    break;
            }
        }

        private void ReceiveExecuteResult(object pObj)
        {
            try
            {
                RmtpDataFrame pDataFrame = pObj as RmtpDataFrame;
                var args = new MeasureExecuteResultEventArgs();
                var result = RmtpDataFrame.GetDataFrame<RmtpExecuteResult>(pDataFrame);
                if (result.Result != ExecuteResult.SUCCESSED)
                {
                    if (result.ErrorCode != ExecuteResultErrorCode.已停止)
                    {
                        args.Error = result.ErrorMessage;
                        OnReceiveExecuteResult(args);
                    }

                    throw new Exception();
                }

                _executeresultNum++;
                //通道指令，发送测量序号
                if (_executeresultNum == 1)
                {
                    RmtpManager.Rmtp.ChannelCommand(_channelId, _pricode);
                }
                //下参数指令
                else if (_executeresultNum == 2)
                {
                    RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.PARAM, new object[] { _parameter.ToList() });//参数指令
                }
                //开始测量
                else if (_executeresultNum == 3)
                {
                    OnStartMeasureCompleted();
                }
                //停止测量
                else
                {
                    RmtpManager.Rmtp.Close();//关闭socket连接
                    _executeresultNum = 0;
                    OnStopMeasureCompleted();
                }
            }
            catch (Exception)
            {
                RmtpManager.Rmtp.Close();//关闭socket连接
                _executeresultNum = 0;
            }
        }

        private void ReceiveHeader(object pObj)
        {
            RmtpDataFrame pDataFrame = pObj as RmtpDataFrame;
            if (_header == null)
            {
                _header = RmtpDataFrame.GetDataFrame<WbfqexDescribeHeader>(pDataFrame);
                if (HeaderCompleted != null)
                {
                    HeaderCompleted(_header);
                }
            }
        }

        private CacheSpectrumData _backgrounDataItems = new CacheSpectrumData();
        private CacheSpectrumData _noiseDataItems = new CacheSpectrumData();

        private void ReceiveData(RmtpDataFrame pDataFrame)
        {
            if (_header == null)
                return;

            var wfdataFrame = RmtpDataFrame.GetDataFrame<WbfqexDataFrame>(pDataFrame, (short)(_header.NeXkind - 1));

            _timeHelper.PerSecondInvokeCount();

            switch (wfdataFrame.频谱数据类型)
            {
                case MeasureDataType.Background:
                    _backgrounDataItems.EnqueueItem(wfdataFrame);
                    break;
                case MeasureDataType.Noise:
                    _noiseDataItems.EnqueueItem(wfdataFrame);
                    break;
                case MeasureDataType.RealTime:

                    if (wfdataFrame.频谱点数 > 0)
                    {
                        _cSpectrumDataItems.EnqueueItem(wfdataFrame);
                    }
                    else if (wfdataFrame.信号数量 > 0)
                    {
                        foreach (var item in wfdataFrame.SignalStatisticsItems)
                        {
                            _cSignalStatisticsItems.EnqueueItem(item);
                        }
                    }

                    break;
            }
        }

        #endregion

        #region IMonitorMeasureHandler

        public void Initialize()
        {
            RmtpManager.Rmtp.RegisterDataFrameHandler(this);
        }
        /// <summary>
        /// 开始测量,初始化参数及优先级和通道号
        /// </summary>
        /// <param name="pParameter">object[] 参数，[0] - WbfqParameter；[1] - pricode优先级；[2] - channelid通道号</param>
        public void Start(params object[] pParameter)
        {
            //建立socket连接
            RmtpManager.Rmtp.ConnectRmtpService(RmtpCommand.WBQEX);

            _parameter = pParameter[0] as WbfqParameter;

            _pricode = Convert.ToInt32(pParameter[1]);
            _channelId = Convert.ToInt32(pParameter[2]);
        }

        public void Stop()
        {
            if (RmtpManager.Rmtp.IsConnected)
            {
                RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.RSTOP);//停止指令
            }

            _timeHelper.StopTimeCalculateHelper();
            
            _header = null;
            _cSpectrumDataItems.Clear();
            _backgrounDataItems.Clear();
            _noiseDataItems.Clear();
            _cSignalStatisticsItems.Clear();
        }

        private void OnReceiveExecuteResult(MeasureExecuteResultEventArgs pArgs)
        {
            Action<MeasureExecuteResultEventArgs> handler = ExecuteResultCompleted;
            if (handler != null) handler(pArgs);
        }

        public event Action StartMeasureCompleted;
        public void OnStartMeasureCompleted()
        {
            Action handler = StartMeasureCompleted;
            if (handler != null) handler();
        }

        public event Action StopMeasureCompleted;
        public void OnStopMeasureCompleted()
        {
            Action handler = StopMeasureCompleted;
            if (handler != null) handler();
        }

        private void OnPerSecondFrameCount(double pCount)
        {
            Action<double> handler = PerSecondFrameCount;
            if (handler != null) handler(pCount);
        }

        public event Action<MeasureExecuteResultEventArgs> ExecuteResultCompleted;

        public event Action<object> HeaderCompleted;

        public event Action<object> DataCompleted;

        public event Action<double> PerSecondFrameCount;

        #endregion

        #region IRmtpNotification
        public void OnCacheRefreshCompleted()
        {

        }

        #endregion

        #region publics

        public event Action<SignalStatisticsItem> SignalStatisticsCompleted;

        public event Action<WbfqexDataFrame> BackgroundCompleted;

        public event Action<WbfqexDataFrame> NoiseCompleted;

        public event Action<WbfqexDataFrame> SpectrumDataCompleted;

        #endregion


        public void Tick(TimeSpan pTime)
        {
            OnBackgroundCompleted(_backgrounDataItems.DequeueItem());
            OnNoiseCompleted(_noiseDataItems.DequeueItem());
            OnSpectrumDataCompleted(_cSpectrumDataItems.DequeueItem());
            OnSignalStatisticsCompleted(_cSignalStatisticsItems.DequeueItem());
        }

        #region IDisposable

        private bool _disposed;
        ~WidebandFreqMeasureHandler()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool pDisposing)
        {
            if (!_disposed)
            {
                if (pDisposing)
                {
                    GlobalTimer.UnRegister(this);
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
/////////////////////////////////////

public class CacheSpectrumData
{
    private ConcurrentQueue<WbfqexDataFrame> _queFrames = new ConcurrentQueue<WbfqexDataFrame>();

    private volatile int _cc;

    public void EnqueueItem(WbfqexDataFrame pItem)
    {
        if (pItem.频率序号 == 0)
        {
            _cc++;
        }

        _queFrames.Enqueue(pItem);
    }

    public WbfqexDataFrame DequeueItem()
    {
        if (_queFrames.IsEmpty)
        {
            return null;
        }

        WbfqexDataFrame re = null;
        _queFrames.TryDequeue(out re);

        if (re != null && re.频率序号 == 0)
        {
            _cc--;

            while (_cc > 0)
            {
                _queFrames.TryDequeue(out re);
                if (re != null && re.频率序号 == 0)
                {
                    _cc--;
                }
            }
        }

        return re;
    }

    public void Clear()
    {
        _queFrames = new ConcurrentQueue<WbfqexDataFrame>();
        _cc = 0;
    }
}

public class CacheSignalStatistics
{
    private ConcurrentDictionary<int, SignalStatisticsItem> _DicSignalStatisticsItems = new ConcurrentDictionary<int, SignalStatisticsItem>();

    public void EnqueueItem(SignalStatisticsItem pItem)
    {
        _DicSignalStatisticsItems[pItem.SignalId] = pItem;
    }

    private int _dicIndex;

    public SignalStatisticsItem DequeueItem()
    {
        if (_DicSignalStatisticsItems.Count == 0)
        {
            return null;
        }

        if (_dicIndex >= _DicSignalStatisticsItems.Count)
        {
            _dicIndex = 0;
        }

        SignalStatisticsItem re = _DicSignalStatisticsItems.ElementAt(_dicIndex).Value;
        _dicIndex++;

        return re;
    }

    public void Clear()
    {
        _DicSignalStatisticsItems.Clear();
    }
}