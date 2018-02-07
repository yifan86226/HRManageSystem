using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class EquipmentClass : AT_BC.Data.IdentifiableData<string>
    {
        public string Comments
        {
            get;
            set;
        }
    }
}
