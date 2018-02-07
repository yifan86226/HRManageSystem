using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Best.VWPlatform.Common.Utility;
using Best.VWPlatform.Common.Core;

namespace Best.VWPlatform.Common.Net
{

    public partial class UDPServiceController
    {
        
        private int _localPort;
        private static bool _isReceiving = true;

        public event Action<string> DataReceived;
        public UDPServiceController()
        {
            _localPort = int.Parse(ConfigurationManager.AppSettings["GPSPort"]);
        }

        public void StopRecieveData()
        {
            _isReceiving = false;
        }

        public void ReceiveData()
        {
            _isReceiving = true;
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress[] addressList = host.AddressList;
                IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, _localPort);

                Socket clientSocket = new Socket(hostEndPoint.AddressFamily,
                                   SocketType.Dgram, ProtocolType.Udp);

                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint senderRemote = (EndPoint)sender;

                clientSocket.Bind(hostEndPoint);
                while (_isReceiving)
                {
                    Byte[] msg = new byte[256];
                    clientSocket.ReceiveFrom(msg, ref senderRemote);
                    string strRec = Encoding.ASCII.GetString(msg);
                    if (DataReceived != null)
                    {
                        DataReceived(strRec);
                    }
                }

                clientSocket.Close();
            }
            catch (Exception ex)
            {
                LogTool.Error(ex.Message);
            }
        }
    }

    public partial class UDPServiceController
    {
        private SocketAsyncEventArgs _socketAsyncArgs = null;
        private readonly System.Threading.SynchronizationContext _uiContext = null;
        private Socket _socket = null;
        private static readonly object LockObj = new object();
        private const int BufferSize = 65535;
        private readonly ISocketReceive _socketReceiveHandler;

        public UDPServiceController(ISocketReceive pSocketReceiveHandler)
        {
            _uiContext = SynchronizationContext.Current;
            _socketReceiveHandler = pSocketReceiveHandler;
        }

        public void StartReceiveData()
        {
            try
            {
                if (_socketAsyncArgs == null)
                {
                    _socketAsyncArgs = new SocketAsyncEventArgs();
                    _socketAsyncArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receiveBuffer = new byte[256];
                    _socketAsyncArgs.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
                    _socketAsyncArgs.Completed += OnUDPReceiveCompleted;
                }

                if (_socket == null)
                {
                    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress[] addressList = host.AddressList;
                    IPEndPoint hostEndPoint = new IPEndPoint(addressList[1], 5898);
                   
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    _socket.Bind(hostEndPoint);

                    _socket.ReceiveFromAsync(_socketAsyncArgs);
                }
            }
            catch (Exception ex)
            {
                if (_socket != null)
                {
                    _socket.Dispose();
                }

                OnSocketError(ex.Message);
            }
        }

        private void OnUDPReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                string str = Encoding.ASCII.GetString(e.Buffer, e.Offset, e.BytesTransferred);
            }  
        }

        private void OnSocketError(string p)
        {

        }
    }

    /*
     * UDPClient 
     * 
    public class UdpState
    {
        public IPEndPoint EndPoint { set; get; }
        public UdpClient Client { set; get; }
    }

    public class UDPServiceController
    {
        private int _localPort; 
        private static bool _isReceived = true;
        private static bool _isReceiving = true;

        public event Action<string> DataReceived;
        public UDPServiceController()
        {
            _localPort = 5898; //todo
        }

        public void StopRecieveData()
        {
            _isReceiving = false;
        }

        public void ReceiveData()
        {
            UdpClient udpClient = null;
            try
            {
                udpClient = new UdpClient(_localPort);
                IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                UdpState s = new UdpState();
                s.Client = udpClient;
                s.EndPoint = remoteIPEndPoint;
                while (_isReceiving)
                {
                    if (_isReceived)
                    {
                        _isReceived = false;
                        udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), s);
                    }
                }
            }
            catch (Exception)
            {
                if (udpClient != null)
                {
                    udpClient.Close();                    
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).Client;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).EndPoint;

            Byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            if (DataReceived != null)
            {
                DataReceived(receiveString);
            }

            _isReceived = true;
        }
    }
    */
}
