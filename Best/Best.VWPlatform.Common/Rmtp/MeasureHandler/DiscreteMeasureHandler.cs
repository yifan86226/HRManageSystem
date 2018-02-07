using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Best.VWPlatform.Common.Interfaces;
using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Rmtp.Commands;
using Best.VWPlatform.Common.Rmtp.DataFrames;
using Best.VWPlatform.Common.Helper;
using System.Diagnostics;
using System.Threading;

namespace Best.VWPlatform.Common.Rmtp.MeasureHandler
{
    public sealed class DiscreteMeasureHandler:IReceiveDataFrame, IMonitorMeasureHandler
    {
        private MscanParameter _parameter;
        private Mscan_DescribeHeader _describeHeader;
        private int _channelId;
        private readonly TimeCalculateHelper _timeHelper = new TimeCalculateHelper();

        /// <summary>
        /// 记录返回的执行结果，1表示预执行指令成功发送参数指令，2表示参数指令成功开始接收数据,3表示停止指令成功，清空数据，断开连接
        /// </summary>
        private int _executeresultNum = 0;

        private readonly SynchronizationContext _uiContext;

        public DiscreteMeasureHandler()
        {
            _uiContext = SynchronizationContext.Current;
            _timeHelper.PerSecondCalculated += OnPerSecondFrameCount;
        }

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
                    _uiContext.Send(OnReceiveHeader, pDataFrame);
                    break;
                case RmtpDataTypes.业务数据:
                    _uiContext.Post(OnReceiveData, pDataFrame);
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
                    RmtpManager.Rmtp.ChannelCommand(_channelId, 1);
                }
                //下参数指令
                else if (_executeresultNum == 2)
                {
                    RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.PARAM, new object[] {_parameter.ToParameterList()});
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
                    RmtpManager.Rmtp.Close(); //关闭socket连接
                    _executeresultNum = 0;

                    if (StopMeasureCompleted != null)
                        StopMeasureCompleted();
                }
            }
            catch (Exception)
            {
                RmtpManager.Rmtp.Close(); //关闭socket连接
                _executeresultNum = 0;
            }
        }
       
        private void OnReceiveHeader(object dataFrame)
        {
            if (_describeHeader == null)
            {
                _describeHeader = RmtpDataFrame.GetDataFrame<Mscan_DescribeHeader>(dataFrame as RmtpDataFrame);

                if (HeaderCompleted != null)
                    HeaderCompleted(_describeHeader);   
            }
        }

        private void OnReceiveData(object dataFrame)
        {
            if (_describeHeader == null)
                return;
            var data = RmtpDataFrame.GetDataFrame<MScan_DataFrame>(dataFrame as RmtpDataFrame, _describeHeader);
            _timeHelper.PerSecondInvokeCount();

            if (DataCompleted != null)
                DataCompleted(data);
        }

        #endregion

        private void OnReceiveExecuteResult(MeasureExecuteResultEventArgs pArgs)
        {
            Action<MeasureExecuteResultEventArgs> handler = ExecuteResultCompleted;
            if (handler != null) handler(pArgs);
        }

        private void OnPerSecondFrameCount(double pCount)
        {
            Action<double> handler = PerSecondFrameCount;
            if (handler != null) handler(pCount);
        }

        public void Initialize()
        {
            RmtpManager.Rmtp.RegisterDataFrameHandler(this);
        }

        /// <summary>
        /// 开始测量,初始化参数及优先级和通道号
        /// </summary>
        /// <param name="pParameter">object[] 参数，[0] - Parameter；[1] - pricode优先级；[2] - channelid通道号</param>
        public void Start(params object[] pParameters)
        {
            //建立socket连接
            RmtpManager.Rmtp.ConnectRmtpService(RmtpCommand.MSCAN);

            _parameter = pParameters[0] as MscanParameter;

            _channelId = Convert.ToInt32(pParameters[2]);
        }

        public void Stop()
        {
            if (RmtpManager.Rmtp.IsConnected)
            {
                RmtpManager.Rmtp.ExecuteCommand(RmtpCommand.RSTOP);//停止指令
            }

            _timeHelper.StopTimeCalculateHelper();
            _describeHeader = null;
        }

        public event Action StartMeasureCompleted;

        public event Action StopMeasureCompleted;

        public event Action<MeasureExecuteResultEventArgs> ExecuteResultCompleted;

        public event Action<object> HeaderCompleted;

        public event Action<object> DataCompleted;

        public event Action<double> PerSecondFrameCount;
    }
}
