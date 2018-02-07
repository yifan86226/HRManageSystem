using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityOrganization : Organization 
    {
        private string activityGuid;

        public string ActivityGuid
        {
            get
            {
                return this.activityGuid;
            }
            set
            {
                if (value != this.activityGuid)
                {
                    this.activityGuid = value;
                    this.NotifyPropertyChanged("ActivityGuid");
                }
            }
        }

        public void CopyFrom(Organization organization)
        {
            this.Guid = organization.Guid;
            this.Name = organization.Name;
            this.ShortName = organization.ShortName;
            this.Address = organization.Address;
            this.SecurityClass = organization.SecurityClass;
            this.Contact = organization.Contact;
            this.Phone = organization.Phone;
            this.DataSate = organization.DataSate;
        }
    }
}
