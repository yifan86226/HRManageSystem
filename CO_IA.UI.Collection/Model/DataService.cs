using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.UI.Collection.Model
{
    public class DataService : IDataService
    {
        //CwSerialPort serPort = CwSerialPort.GetCwSerialPort();
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service
            //string title = serPort.iniReader.ReadString("SoftName");
            //var item = new DataItem(title);
            //item.ChestBitmapTitle = serPort.iniReader.ReadString("ChestBitmapButtonTitle");
            //item.MultiPersonTrainingTitle = serPort.iniReader.ReadString("MultiPersionButtonTitle");
            //callback(item, null);
        }
    }
}
