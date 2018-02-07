using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CO_IA.UI.Collection.Model;

namespace CO_IA.UI.Collection.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data
            var item = new DataItem("Welcome to MVVM Light [design]");
            item.MultiPersonTrainingTitle = "多人[design]";
            item.ChestBitmapTitle = "单人[design]";
            callback(item, null);
        }
    }
}
