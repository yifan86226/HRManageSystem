using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Best.VWPlatform.Common.Interfaces;
using Best.VWPlatform.Common.Rmtp.DataFrames;
using Best.VWPlatform.Common.Types;
using System.Diagnostics;
using Best.VWPlatform.Common.Rmtp.Commands;
using Best.VWPlatform.Common.Helper;
using Best.VWPlatform.Common.Core;
using System.Configuration;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    public class FreqRangeMeasureHandler : IReceiveDataFrame, IMonitorMeasureHandler
    {
        //private FScanParameter _parameter;
        private NewFscanParameter _parameter; 
        private Fscan_DescribeHeader _describeHeader;
        private int _channelId;
        private readonly TimeCalculateHelper _timeHelper = new TimeCalculateHelper();

        /// <summary>
        /// 记录返回的执行结果，1表示预执行指令成功发送参数指令，2表示参数指令成功开始接收数据,3表示停止指令成功，清空数据，断开连接
        /// </summary>
        private int _executeresultNum = 0;

        private readonly SynchronizationContext _uiContext;
        public　FreqRangeMeasureHandler()
        {
            _uiContext = SynchronizationContext.Current;

            _timeHelper.PerSecondCalculated += OnPerSecondFrameCount;
        }
        public void Initialize()
        {
            RmtpManager.Rmtp.RegisterDataFrameHandler(this);
        }
        public void Start(params object[] pParameters)
        {
            //建立socket连接
            RmtpManager.Rmtp.ConnectRmtpService(RmtpCommand.FSCAN);

            //_parameter = pParameters[0] as FScanParameter;
            _parameter = pParameters[0] as NewFscanParameter;

            _channelId = Convert.ToInt32(pParameters[2]);
        }

        public void Stop()
        {
            if (RmtpManager.Rmtp.IsConnected)
            {
                RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.RSTOP);//停止指令
            }

            Thread.Sleep(500);

            _timeHelper.StopTimeCalculateHelper();
            _describeHeader = null;
        }

        private void ReceiveDeviceAbility(DeviceAbilityDataFrame pDataFrame)
        {
            var args = new DeviceAbilityEventArgs { DeviceAbility = pDataFrame };
            OnDeviceAbilityCompleted(args);
        }

        private void OnDeviceAbilityCompleted(DeviceAbilityEventArgs obj)
        {
            var handler = DeviceAbilityCompleted;
            if (handler != null) handler(obj);
        }

        private void OnPerSecondFrameCount(double pCount)
        {
            Action<double> handler = PerSecondFrameCount;
            if (handler != null) handler(pCount);
        }

        public event Action<DeviceAbilityEventArgs> DeviceAbilityCompleted;

        public event Action<MeasureExecuteResultEventArgs> ExecuteResultCompleted;

        public event Action<object> HeaderCompleted;

        public event Action<object> DataCompleted;

        public event Action StartMeasureCompleted;

        public event Action StopMeasureCompleted;

        public event Action<double> PerSecondFrameCount;

        public event Action<SignalStatisticsItem> SignalStatisticsCompleted;

        public event Action<Fscan_DataFrame> NoiseCompleted;

        private RmtpDataFrame _data;

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
                    _uiContext.Send(OnReceiveHeader, pDataFrame);
                    break;
                case RmtpDataTypes.业务数据:
                    //ReceiveData(pDataFrame); 
                    if (ConfigurationManager.AppSettings["FscanSignalShold"].Equals("1"))
                        _uiContext.Post(OnNewReceiveData, pDataFrame);
                    else
                        _uiContext.Post(OnReceiveData, pDataFrame);
                   break;
            }
        }
                
        private void ReceiveExecuteResult(object pDataFrame)
        {
            try
            {
                var args = new MeasureExecuteResultEventArgs();
                var result = RmtpDataFrame.GetDataFrame<RmtpExecuteResult>(pDataFrame as RmtpDataFrame);
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
                    RmtpManager.Rmtp.ChannelCommand(_channelId, 1);
                }
                //下参数指令
                else if (_executeresultNum == 2)
                {
                    RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.PARAM, new object[] { _parameter.ToTupleString() });//参数指令
                }
                //开始测量
                else if (_executeresultNum == 3)
                {
                    if (StartMeasureCompleted != null)
                        StartMeasureCompleted();
                }
                //停止测量
                else if (_executeresultNum == 4)
                {
                    RmtpManager.Rmtp.Close();//关闭socket连接
                    _executeresultNum = 0;

                    if (StopMeasureCompleted != null)
                        StopMeasureCompleted();
                }
            }
            catch (Exception)
            {
                RmtpManager.Rmtp.Close();//关闭socket连接
                _executeresultNum = 0;
            }
        }

        private void OnReceiveExecuteResult(MeasureExecuteResultEventArgs pArgs)
        {
            Action<MeasureExecuteResultEventArgs> handler = ExecuteResultCompleted;
            if (handler != null) handler(pArgs);
        }
        private void OnReceiveHeader(object dataFrame)
        {
            if (_describeHeader == null)
            {
                _describeHeader = RmtpDataFrame.GetDataFrame<Fscan_DescribeHeader>(dataFrame as RmtpDataFrame);

                if (HeaderCompleted != null)
                    HeaderCompleted(_describeHeader);   
            }
        }

        private void OnReceiveData(object dataFrame)
        {
            if (_describeHeader == null)
                return;
            var data = RmtpDataFrame.GetDataFrame<Fscan_DataFrame>(dataFrame as RmtpDataFrame, _describeHeader);
            _timeHelper.PerSecondInvokeCount();

            if (DataCompleted != null)
                DataCompleted(data);
        }

        private void OnNewReceiveData(object dataFrame)
        {
            if (_describeHeader == null)
                return;
            var data = RmtpDataFrame.GetDataFrame<Fscan_DataFrame>(dataFrame as RmtpDataFrame, _describeHeader);
            _timeHelper.PerSecondInvokeCount();
            if (data == null)
                return;

            switch (data.SpectrumType)
            {
                case MeasureDataType.Noise:
                    if (NoiseCompleted != null)
                        NoiseCompleted(data);
                    break;
                case MeasureDataType.RealTime:
                    if (data.SignalCount > 0)
                    {
                        foreach (var item in data.SignalStatisticsItems)
                        {
                            if (SignalStatisticsCompleted != null)
                                SignalStatisticsCompleted(item);
                        }
                    }
                    else if(data.ScanCount > 0)
                    {
                        if (DataCompleted != null)
                            DataCompleted(data);
                    }

                    break;
            }
        }
    }
}
