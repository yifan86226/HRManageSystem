using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Net
{
    public class TCPServiceController
    {
        private int _port;
        public TCPServiceController()
        {

        }

        public string RecieveData(String pServer, String pMsg)
        {
            string reData = string.Empty;
            try
            {
                TcpClient client = new TcpClient(pServer, _port);
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(pMsg);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                StreamReader sr = new StreamReader(stream);
                reData = sr.ReadToEnd();

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {

            }
            catch (SocketException e)
            {

            }

            return reData;
        }
    }
}
