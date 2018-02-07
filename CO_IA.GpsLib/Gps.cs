using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CO_IA.GpsLib
{
    public class Gps
    {
        #region Struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Decoder_Gps_Data  //unsafe
        {
            public double BTC_Time;
            public char LocationState;
            public double Latitude;
            public double Longitude;
            public char LatitudeHemisphere;
            public char LongitudeHemisphere;
            public double Height;
            public double Speed;
            public int UsingStartNum;
        };
        #endregion


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool Gps_CALLBACK(ref Decoder_Gps_Data GpsDataHeader);


        private const String DLL_NAME_Win32 = @"Gps.dll";


        [DllImport(DLL_NAME_Win32, EntryPoint = "Connect", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static bool Connect(string port, int Baudrate, Gps_CALLBACK callBack);


        [DllImport(DLL_NAME_Win32, EntryPoint = "Close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static bool Close();

        public static void CloseGps()
        {
            if (dataInvokers != null)
            {
                foreach (var invoker in dataInvokers)
                {
                    invoker.Stop();
                }
            }
            Close();
        }

        [DllImport(DLL_NAME_Win32, EntryPoint = "CheckGPSHotSwap", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static int CheckGPSHotSwap();

        public static bool IsOpenGps = false;

        private static GpsLib.Gps.Gps_CALLBACK _Gps_Proc;

        private static bool CALLBACK(ref GpsLib.Gps.Decoder_Gps_Data GpsDataHeader)
        {
            foreach (var dataInvoker in dataInvokers)
            {
                dataInvoker.Dispose(ref GpsDataHeader);
            }
            return true;
        }

        private static IGpsDataInvoker[] dataInvokers;

        public static void Connect(string port, int Baudrate, params IGpsDataInvoker[] dataInvokers)
        {
            try
            {
                Gps.dataInvokers = dataInvokers;
                if (dataInvokers == null)
                {
                    Gps._Gps_Proc = null;
                }
                else
                {
                    foreach (var invoker in Gps.dataInvokers)
                    {
                        invoker.Start();
                    }
                    Gps._Gps_Proc = Gps.CALLBACK;
                }
                Gps.IsOpenGps = Gps.Connect(port, Baudrate, Gps._Gps_Proc);
            }
            catch
            {
                if (Gps.dataInvokers != null)
                {
                    foreach (var invoker in Gps.dataInvokers)
                    {
                        invoker.Stop();
                    }
                }
                throw;
            }
        }
    }
}
