using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public interface IGpsOrbitReceiver
    {
        void Receive(CO_IA.Data.Gps.GpsOrbit gpsOrbit);

        /// <summary>
        /// 要在其上执行接收消息的同步上下文,通常该对象未消息接收器实现对象的创建者(UI)同步上下文
        /// </summary>
        System.Threading.SynchronizationContext SyncContext
        {
            get;
        }
    }
}
