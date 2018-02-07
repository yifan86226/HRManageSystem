using SensorXeGps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.GpsLib
{
    public class GpsSensor
    {
        private static SensorXeGpsHelper gpshelper;
        private static void connect(string ip,int port)
        {
            gpshelper = new SensorXeGpsHelper(ip, port);
            gpshelper.ReciveGpsData += gpshelper_ReciveGpsData;
            gpshelper.ServerConnected += gpshelper_ServerConnected;
            gpshelper.ServerDisconnected += gpshelper_ServerDisconnected;
            gpshelper.ServerExceptionOccurred += gpshelper_ServerExceptionOccurred;
            gpshelper.Connect();
        }

        static void gpshelper_ServerExceptionOccurred(object sender, Gimela.Net.Sockets.TcpServerExceptionOccurredEventArgs e)
        {
            IsOpenGps = false;
        }

        static void gpshelper_ServerDisconnected(object sender, Gimela.Net.Sockets.TcpServerDisconnectedEventArgs e)
        {
            IsOpenGps = false;
        }

        static void gpshelper_ServerConnected(object sender, Gimela.Net.Sockets.TcpServerConnectedEventArgs e)
        {
            IsOpenGps = true;
        }

        private static void gpshelper_ReciveGpsData(object sender, XeGps e)
        {
            CO_IA.GpsLib.Gps.Decoder_Gps_Data gpsdata = CreateGpsOrbit(e.lat, e.lon );
            foreach (var dataInvoker in dataInvokers)
            {
                dataInvoker.Dispose(ref gpsdata);
            }            
        }
        
        public static void CloseGps()
        {
            if (!IsOpenGps)
                return;
            IsOpenGps = false;
            if (dataInvokers != null)
            {
                foreach (var invoker in dataInvokers)
                {
                    invoker.Stop();
                }
            }
            if (gpshelper != null)
            {

                gpshelper.Close();
                gpshelper.ReciveGpsData -= gpshelper_ReciveGpsData;
                gpshelper.ServerConnected -= gpshelper_ServerConnected;
                gpshelper.ServerDisconnected -= gpshelper_ServerDisconnected;
                gpshelper.ServerExceptionOccurred -= gpshelper_ServerExceptionOccurred;
            }
            gpshelper = null;
        }

        public static bool IsOpenGps = false;

        private static IGpsDataInvoker[] dataInvokers;
        public static void Connect(string ip, int port, params IGpsDataInvoker[] dataInvokers)
        {
            try
            {
                GpsSensor.dataInvokers = dataInvokers;
                if (dataInvokers == null)
                {

                }
                else
                {
                    foreach (var invoker in GpsSensor.dataInvokers)
                    {
                        invoker.Start();
                    }
                }
                GpsSensor.connect(ip, port);
            }
            catch
            {
                if (GpsSensor.dataInvokers != null)
                {
                    foreach (var invoker in GpsSensor.dataInvokers)
                    {
                        invoker.Stop();
                    }
                }
                throw;
            }
        }
        private static CO_IA.GpsLib.Gps.Decoder_Gps_Data CreateGpsOrbit(double longitude, double latitude)
        {
            CO_IA.GpsLib.Gps.Decoder_Gps_Data data = new CO_IA.GpsLib.Gps.Decoder_Gps_Data();
            data.Height = 100;
            data.Latitude = latitude;
            data.Longitude = longitude;
            data.LongitudeHemisphere = 'E';
            data.LatitudeHemisphere = 'N';
            data.LocationState = 'A';
            data.Speed = 100;
            data.UsingStartNum = 10;
            data.BTC_Time= double.Parse(DateTime.Now.ToString("HHmmss.fff"));
            return data;

        }

    }
}
