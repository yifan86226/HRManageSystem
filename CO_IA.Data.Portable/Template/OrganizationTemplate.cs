using AT_BC.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Template
{
    public class OrganizationTemplate : IdentifiableData<string>
    {
        private bool isNew;

        public bool IsNew
        {
            get
            {
                return this.isNew;
            }
            set
            {
                if (value != this.isNew)
                {
                    this.isNew = value;
                    this.NotifyPropertyChanged("IsNew");
                }
            }
        }

        private string parentGuid;
        public string ParentGuid
        {
            get
            {
                return this.parentGuid;
            }
            set
            {
                this.parentGuid = value;
            }
        }

        private VehicleInfo vehicle;

        public VehicleInfo Vehicle
        {
            get
            {
                return this.vehicle;
            }
            set
            {
                this.vehicle = value;
                this.NotifyPropertyChanged("Vehicle");
            }
        }

        public string TemplateGuid
        {
            get;
            set;
        }

        private List<string> dutyList;

        public List<string> DutyList
        {
            get
            {
                return this.dutyList;
            }
            set
            {
                this.dutyList = value;
                this.NotifyPropertyChanged("DutyList");
            }
        }
    }
}
