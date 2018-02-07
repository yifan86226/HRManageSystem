using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Threading;
using Best.VWPlatform.Common.Helper;
using Best.VWPlatform.Common.Rmtp.Commands;
using Best.VWPlatform.Common.Rmtp.DataFrames;
using Best.VWPlatform.Common.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Best.VWPlatform.Common.Core;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    /// <summary>
    /// 中频全景测量任务处理类
    /// </summary>
    public class IntermediateFreqMeasureHandler : ReceiveRmtpDataFrameBase, IMonitorMeasureHandler
    {
        private IfqParameter _parameter;
        private IffqexDescribeHeader _header;
        private readonly TimeCalculateHelper _timeHelper = new TimeCalculateHelper();
        private DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Send);
        private RmtpDataFrame _data = null;
        private int _pricode = 1;
        private int _channelId;

        /// <summary>
        /// 记录返回的执行结果，1表示预执行指令成功发送参数指令，2表示参数指令成功开始接收数据,3表示停止指令成功，清空数据，断开连接
        /// </summary>
        private int _executeresultNum = 0;

        private readonly SynchronizationContext _uiContext;
        public IntermediateFreqMeasureHandler()
        {
            _uiContext = SynchronizationContext.Current;
            _timeHelper.PerSecondCalculated += OnPerSecondFrameCount;
            _timer.Tick += _timer_Tick;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (_data != null)
            {
                var dataFrame = RmtpDataFrame.GetDataFrame<IffqexDataFrame>(_data, _header);
                OnDataCompleted(dataFrame);   
            }
        }

        #region privates
        private void ReceiveExecuteResult(object pDataFrame)
        {
            try
            {
                var args = new MeasureExecuteResultEventArgs();
                var result = RmtpDataFrame.GetDataFrame<RmtpExecuteResult>((RmtpDataFrame)pDataFrame);
                if (result.Result != ExecuteResult.SUCCESSED)
                {
                    if (result.ErrorCode != ExecuteResultErrorCode.已停止)
                    {
                        args.Error = result.ErrorMessage;
                        OnExecuteResultCompleted(args);
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
                else if (_executeresultNum == 4)
                {
                    OnStopMeasureCompleted();

                    RmtpManager.Rmtp.Close();//关闭socket连接
                    _executeresultNum = 0;
                }
            }
            catch(Exception ex)
            {
                RmtpManager.Rmtp.Close();//关闭socket连接
                _executeresultNum = 0;
            }
        }

        private void ReceiveHeader(object pDataFrame)
        {
            if (_header == null)
            {
                var args = new IffqMeasureHeaderEventArgs();
                _header = args.Header = RmtpDataFrame.GetDataFrame<IffqexDescribeHeader>((RmtpDataFrame)pDataFrame);
                OnHeaderCompleted(args);

                _timer.Start();
            }  
        }

        private void ReceiveData(RmtpDataFrame pDataFrame)
        {
            if (_header == null)
                return;

            _timeHelper.PerSecondInvokeCount();
            _data = pDataFrame;
        }

        private void ReceiveAudio(RmtpDataFrame pDataFrame)
        {
            var tmp = new byte[pDataFrame.Data.Length - 19];
            Array.Copy(pDataFrame.Data, 18, tmp, 0, pDataFrame.Data.Length - 19);
            OnReceiveAudioData(tmp);
        }

        #endregion

        public event Action<byte[]> ReceiveAudioData;

        public void OnReceiveAudioData(byte[] obj)
        {
            Action<byte[]> handler = ReceiveAudioData;
            if (handler != null) handler(obj);
        }

        #region IReceiveDataFrame
        protected override void Received(RmtpDataFrame pDataFrame)
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
                case RmtpDataTypes.音频数据:
                    ReceiveAudio(pDataFrame);
                    break;
            }
        }
        #endregion


        #region IMonitorMeasureHandler

        /// <summary>
        /// 开始测量
        /// </summary>
        /// <param name="pParameter">object[] 参数，[0] - WbfqParameter；[1] - pricode优先级；[2] - channelid通道号</param>
        public void Start(params object[] pParameter)
        {
            //建立socket连接
            RmtpManager.Rmtp.ConnectRmtpService(RmtpCommand.IFQEX);  

            _parameter = pParameter[0] as IfqParameter;
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
            _timer.Stop();

            _header = null;
            _data = null;
        }

        public void PlayAudio(bool pStop)
        {
            RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.AUDIO, pStop ? "ON" : "OFF");
        }

        public event Action<MeasureExecuteResultEventArgs> ExecuteResultCompleted;

        public void OnExecuteResultCompleted(MeasureExecuteResultEventArgs obj)
        {
            Action<MeasureExecuteResultEventArgs> handler = ExecuteResultCompleted;
            if (handler != null) handler(obj);
        }

        public event Action<object> HeaderCompleted;

        public void OnHeaderCompleted(object obj)
        {
            Action<object> handler = HeaderCompleted;
            if (handler != null) handler(obj);
        }

        public event Action<object> DataCompleted;

        public void OnDataCompleted(object obj)
        {
            Action<object> handler = DataCompleted;
            if (handler != null) handler(obj);
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

        public event Action<double> PerSecondFrameCount;

        public void OnPerSecondFrameCount(double obj)
        {
            Action<double> handler = PerSecondFrameCount;
            if (handler != null) handler(obj);

            if (!obj.Equals(0))
            {
                double d = 1000 / obj;
                _timer.Interval = TimeSpan.FromMilliseconds(d);
            }
        }

        #endregion

        public void Initialize()
        {
            RmtpManager.Rmtp.RegisterDataFrameHandler(this);
        }
    }
}
