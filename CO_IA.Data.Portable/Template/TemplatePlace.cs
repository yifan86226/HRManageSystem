using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Template
{
    public class TemplatePlace : CheckableData<string>
    {
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public string TemplateGuid
        {
            get;
            set;
        }
    }
}
