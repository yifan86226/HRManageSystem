using Best.VWPlatform.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Net
{
    public class SocketController : IDisposable
    {
        #region 私有成员
        private SocketAsyncEventArgs _socketAsyncArgs;
        private readonly SynchronizationContext _uiContext;
        private Socket _socket;
        private readonly ISocketReceive _socketReceiveHandler;
        private readonly string _serverIp;
        private readonly int _serverPort;
        private static readonly object LockObj = new object();
        private const int BufferSize = 65535;
        #endregion

        #region 构造函数
        public SocketController(string pServerIp, int pServerPort, ISocketReceive pSocketReceiveHandler)
        {
            _serverIp = pServerIp;
            _serverPort = pServerPort;
            _uiContext = SynchronizationContext.Current;
            _socketReceiveHandler = pSocketReceiveHandler;
        }
        #endregion

        #region 属性
        /// <summary>
        /// Socket是否连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _socket != null && _socket.Connected;
            }
        }
        #endregion

        #region 私有

        private void OnSocketConnected(object pIsConnected)
        {
            if (SocketConnected != null)
                SocketConnected((bool)pIsConnected);
        }

        private void OnSocketError(object pError)
        {
            if (SocketError != null)
                SocketError((string)pError);
        }

        /// <summary>
        /// socket连接完成时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSocketConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            var response = new byte[BufferSize];
            try
            {
                if (e.ConnectByNameError == null)
                {
                    e.SetBuffer(response, 0, response.Length);
                    e.Completed -= OnSocketConnectCompleted;
                    e.Completed += OnSocketReceiveCompleted;
                    _socket.ReceiveAsync(e);
                }
                else
                {
                    string ip = ((System.Net.DnsEndPoint) (e.RemoteEndPoint)).Host;
                    int port = ((System.Net.DnsEndPoint) (e.RemoteEndPoint)).Port;
                    _uiContext.Post(OnSocketError, string.Format("{0}:{1}", e.SocketError, e.ConnectByNameError.Message));
                }

                if (_socket != null)
                    _uiContext.Post(OnSocketConnected, _socket.Connected);
            }
            catch (SocketException ex)
            {
                Close();
                _uiContext.Post(OnSocketError, ex.Message);
            }
        }

        /// <summary>
        /// Socket数据接收完成时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSocketReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                byte[] buffer = null;
                if (e.SocketError != System.Net.Sockets.SocketError.Success)
                {
                    Close();
                    _uiContext.Post(OnSocketError, SocketErrorMessage.GetMessage(e.SocketError));
                }
                else if (e.BytesTransferred > 0)
                {
                    buffer = new byte[e.BytesTransferred];
                    Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesTransferred);
                }
                
                _socketReceiveHandler.Receive(buffer);

                if (_socket != null && _socket.Connected)
                {
                    _socket.ReceiveAsync(e);
                }
            }
            catch (Exception ex)
            {
                Close();
                _uiContext.Post(OnSocketError, ex.Message);
            }
        }

        private void OnSendCompleted(object pSender, SocketAsyncEventArgs pArgs)
        {
            if (pArgs.SocketError != System.Net.Sockets.SocketError.Success)
            {
                Close();
                _uiContext.Post(OnSocketError, SocketErrorMessage.GetMessage(pArgs.SocketError));
            }
        }
        #endregion

        #region 公有
        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            lock (LockObj)
            {
                try
                {
                    if (_socketAsyncArgs == null)
                    {
                        _socketAsyncArgs = new SocketAsyncEventArgs { RemoteEndPoint = new DnsEndPoint(_serverIp, _serverPort) };
                        _socketAsyncArgs.Completed += OnSocketConnectCompleted;
                    }
                    if (_socket == null)
                    {
                        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                    if (!_socket.Connected)
                    {
                        _socket.ConnectAsync(_socketAsyncArgs);
                    }
                }
                catch (Exception ex)
                {
                    if (_socket != null)
                        _socket.Dispose();
                    OnSocketError(ex.Message);
                }
            }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            lock (LockObj)
            {
                if (_socketAsyncArgs != null)
                {
                    _socketAsyncArgs.Dispose();
                    _socketAsyncArgs = null;
                }
                if (_socket != null)
                {
                    if (_socket.Connected)
                        _socket.Close();
                    _socket.Dispose();
                    _socket = null;
                }
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="pArgs"></param>
        public void Send(ISocketSend pArgs)
        {
            if (pArgs == null || _socket == null || !_socket.Connected)
            {
                OnSocketError("未连接主机。");
                return;
            }
            using (var myMsg = new SocketAsyncEventArgs())
            {
                myMsg.RemoteEndPoint = _socket.RemoteEndPoint;
                myMsg.Completed += OnSendCompleted;
                byte[] buffer = pArgs.ToBytes();
                myMsg.SetBuffer(buffer, 0, buffer.Length);
                _socket.SendAsync(myMsg);
            }
        }

        #endregion

        #region 事件
        public event Action<bool> SocketConnected;
        public event Action<string> SocketError;
        #endregion

        #region IDisposable
        private bool _mDisposed;
        ~SocketController()
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
                    if (_socket != null)
                        _socket.Dispose();
                    if (_socketAsyncArgs != null)
                        _socketAsyncArgs.Dispose();
                }
                _mDisposed = true;
            }
        }
        #endregion
    }
}
