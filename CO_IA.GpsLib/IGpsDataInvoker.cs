using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.GpsLib
{
    public interface IGpsDataInvoker
    {
        void Dispose(ref GpsLib.Gps.Decoder_Gps_Data gpsDataHeader);

        void Start();

        void Stop();
    }
}
