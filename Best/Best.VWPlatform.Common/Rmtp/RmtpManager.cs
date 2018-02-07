using Best.VWPlatform.Common.Core;
using Best.VWPlatform.Common.Interfaces;
using Best.VWPlatform.Common.Net;
using Best.VWPlatform.Common.Rmtp.Commands;
using Best.VWPlatform.Common.Rmtp.DataFrames;
using Best.VWPlatform.Common.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Rmtp
{
    /// <summary>
    /// Rmtp协议管理
    /// </summary>
    public class RmtpManager : IDisposable
    {
        private SocketController _rmtpControler;
        private RmtpDataFrameCache _rmtpDataFrameCache;
        private readonly Dictionary<RmtpCommand, IRmtpCommandHandler> _executeRmtpFun = new Dictionary<RmtpCommand, IRmtpCommandHandler>();
        private readonly Dictionary<int, MonitorMeasureState> _deviceMeasureStateCaches = new Dictionary<int, MonitorMeasureState>();//用于设置测量序号等信息,目前没有使用

        private  IReceiveDataFrame _rmtpDataFrameHandler;

        public static readonly RmtpManager Rmtp = new RmtpManager();
        private RmtpManager()
        {
            _executeRmtpFun[RmtpCommand.PARAM] = new RmtpCmdParam();
            _executeRmtpFun[RmtpCommand.RSTOP] = new RmtpCmdStop();
            _executeRmtpFun[RmtpCommand.AUDIO] = new RmtpCmdAudio();
            _executeRmtpFun[RmtpCommand.VERIF] = new RmtpCmdVerify();
            _executeRmtpFun[RmtpCommand.CHANNEL] = new RmtpCmdChannel();
            _executeRmtpFun[RmtpCommand.WBQEX] = new RmtpCmdWbfqex();
            _executeRmtpFun[RmtpCommand.IFQEX] = new RmtpCmdIffqex();
            _executeRmtpFun[RmtpCommand.FSCAN] = new RmtpCmdFscan();
            _executeRmtpFun[RmtpCommand.MSCAN] = new RmtpCmdMscan();
        }

        #region 属性
        public bool IsConnected
        {
            get { return _rmtpControler != null && _rmtpControler.IsConnected; }
        }
        #endregion

        #region 内部方法

        private void OnError(string pMsg)
        {
            if (Error != null)
                Error(pMsg);
        }

        private void OnReceiveDataFrame(RmtpDataFrame pDataFrame)
        {
            if (!IsConnected)
                return;

            _rmtpDataFrameHandler.Receive(pDataFrame);
        }

        private void OnCacheClearCompleted()
        {
            if (CacheClearCompleted != null)
            {
                CacheClearCompleted();
            }
        }

        private void OnRmtpError(string pErrorMsg)
        {
            OnError(pErrorMsg);
            AssistTool.WriteOutput(string.Format("Rmtp服务连接发生异常。{0}", pErrorMsg));
            Close();
        }

        private void OnRmtpConnected(bool pIsConnected)
        {
            if (pIsConnected)
            {
                AssistTool.WriteOutput("Rmtp服务连接成功");
            }
            if (Connected != null)
                Connected(pIsConnected);
        }

        /// <summary>
        /// 获取RmtpDataFrame的唯一ID
        /// </summary>
        /// <param name="pDataFrame">RmtpDataFrame</param>
        /// <returns></returns>
        internal static int GetRmtpDataFrameGuid(RmtpDataFrame pDataFrame)
        {
            return pDataFrame.Header.ChannelId;
        }

        /// <summary>
        /// 发送测量序号
        /// </summary>
        public void ChannelCommand(int channelid, int pricode)
        {
            var cmdMessageFrame = new CommandMessageFrame(RmtpCommand.CHANNEL);
            var serialNumber = GenerateMeasureSerialNumber(cmdMessageFrame, true, channelid, pricode);
            var channelList = new List<Tuple<string, string>>
                {
                    new Tuple<string,string>("MSN", serialNumber.ToString()),
                };
            ExecuteCommand(RmtpCommand.CHANNEL, new object[] { channelList });
        }

        /// <summary>
        /// 生成测量序号
        /// </summary>
        /// <param name="pMessageFrame"></param>
        /// <param name="pIsNewMeasureNumber"></param>
        /// <param name="pChannelId"> </param>
        /// <param name="pPricode"> </param>
        private int GenerateMeasureSerialNumber(CommandMessageFrame pMessageFrame, bool pIsNewMeasureNumber, int pChannelId, int pPricode)
        {
            pMessageFrame.Header.Tag = -1;

            string v = string.Format("{0:d2}{1:d2}{2:d2}{3:d2}{4:d2}", DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                         DateTime.Now.Minute, DateTime.Now.Second);
            int serialNumber = int.Parse(v);
            var key = pChannelId;
            if (pIsNewMeasureNumber)
            {
                pMessageFrame.Header.Tag = serialNumber;//生成测量序号
                _deviceMeasureStateCaches[key] = new MonitorMeasureState
                {
                    MeasureSerialNumber = serialNumber,
                    Channelid = pChannelId,
                    Pricode = pPricode
                };   
            }
            else
            {
                //使用已缓存某设备的测量序号
                if (_deviceMeasureStateCaches.ContainsKey(key))
                {
                    var state = _deviceMeasureStateCaches[key];
                    pMessageFrame.Header.Tag = serialNumber = state.MeasureSerialNumber;
                }
                else
                {
                    pMessageFrame.Header.Tag = -1;
                    serialNumber = -1;
                }
            }
            return serialNumber;
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 连接Rmtp服务
        /// </summary>
        /// <param name="pIp">Rmtp目标IP</param>
        /// <param name="pPort">端口号</param>
        public void ConnectRmtpService(RmtpCommand pCmd)
        {
            string pIp = ConfigurationManager.AppSettings["IP"];
            int pPort = int.Parse(ConfigurationManager.AppSettings["Port"]);

            if (_rmtpControler != null && _rmtpControler.IsConnected)
            {
                return;                
            }

            if (_rmtpDataFrameCache != null)
            {
                _rmtpDataFrameCache.ReceiveDataFrame -= OnReceiveDataFrame;
                _rmtpDataFrameCache = null;
            }

            _rmtpDataFrameCache = new RmtpDataFrameCache(pCmd);
            _rmtpDataFrameCache.ReceiveDataFrame += OnReceiveDataFrame;

            if (_rmtpControler != null)
            {
                _rmtpControler.Close();
                _rmtpControler.SocketConnected -= OnRmtpConnected;
                _rmtpControler.SocketError -= OnRmtpError;
                _rmtpControler = null;
            }

            _rmtpControler = new SocketController(pIp, pPort, _rmtpDataFrameCache);
            _rmtpControler.SocketConnected += OnRmtpConnected;
            _rmtpControler.SocketError += OnRmtpError;

            _rmtpControler.Connect();
        }

        /// <summary>
        /// 关闭Rmtp通讯
        /// </summary>
        public void Close()
        {
            if (_rmtpControler == null)
                return;

            _rmtpDataFrameCache.StopDequeueFromCache();
            OnCacheClearCompleted();
            _rmtpControler.Close();
        }

        /// <summary>
        /// 执行RMTP监测相关指令（监测指令和非监测指令）
        /// </summary>
        /// <param name="pCommand">命令类型</param>
        /// <param name="pParameter">命令携带参数</param>
        /// <param name="pPricode">是正在进行的控制的优先级代码，默认值：1</param>
        /// <param name="pChannelid">执行任务的设备通道号，默认值：0</param>
        /// <param name="pIsNewMeasureNumber">true - 生成新测量序号，false - 保持原测量序号</param>
        public void ExecuteCommand(RmtpCommand pCommand,
            object pParameter = null,
            int pPricode = 1,
            int pChannelid = 0,
            bool pIsNewMeasureNumber = false)
        {
            if (_rmtpControler == null || !_rmtpControler.IsConnected)
            {
                AssistTool.WriteOutput(string.Format("Rmtp服务连接已断开，指令“{0}”无效", pCommand));
                return;
            }

            if (!_executeRmtpFun.ContainsKey(pCommand))
                return;

            var cmdMessageFrame = new CommandMessageFrame(pCommand);
            _executeRmtpFun[pCommand].CommandHandler(cmdMessageFrame, new[] { pParameter, pPricode, pChannelid });
            cmdMessageFrame.Header.ChannelId = pChannelid;

            _rmtpControler.Send(cmdMessageFrame);
        }

        /// <summary>
        /// 注册数据帧处理对象
        /// </summary>
        /// <param name="key">对应宽扫还是中频</param>
        /// <param name="pReceiveHandler">处理对象接口</param>
        public void RegisterDataFrameHandler(IReceiveDataFrame pReceiveHandler)
        {
            _rmtpDataFrameHandler = pReceiveHandler;
        }

        #endregion

        #region 事件
        /// <summary>
        /// 连接Rmtp服务事件
        /// </summary>
        public event Action<bool> Connected;
        /// <summary>
        /// Rmtp通讯时错误事件
        /// </summary>
        public event Action<string> Error;

        public event Action CacheClearCompleted;
        #endregion

        #region IDisposable
        private bool _mDisposed;
        ~RmtpManager()
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
            if (!_mDisposed)
            {
                if (mDisposing)
                {
                    Close();
                }
                _mDisposed = true;
            }
        }
        #endregion
    }
}
