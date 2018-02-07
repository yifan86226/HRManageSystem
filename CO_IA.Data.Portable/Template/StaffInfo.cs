using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Template
{
    public class StaffInfo : AT_BC.Data.CheckableData<string>
    {
        private string name;

        public string Name
        {
            get 
            { 
                return name;
            }
            set 
            { 
                name = value; 
                NotifyPropertyChanged("Name");
            }
        }

        private string orgGuid;

        public string OrgGuid
        {
            get 
            {
                return orgGuid; 
            }
            set
            { 
                orgGuid = value;
                NotifyPropertyChanged("OrgGuid"); 
            }
        }


        private string dept;

        public string Dept
        {
            get { return dept; }
            set { dept = value; NotifyPropertyChanged("Dept"); }
        }


        private string areaCode;

        public string AreaCode
        {
            get { return areaCode; }
            set { areaCode = value; NotifyPropertyChanged("AreaCode"); }
        }

        private string sex;

        public string Sex
        {
            get { return sex; }
            set { sex = value; NotifyPropertyChanged("Sex"); }
        }


        private string unit;

        public string Unit
        {
            get { return unit; }
            set { unit = value; NotifyPropertyChanged("Unit"); }
        }

        private string duty;

        public string Duty
        {
            get { return duty; }
            set { duty = value; NotifyPropertyChanged("Duty"); }
        }

        private string phone;

        public string Phone
        {
            get { return phone; }
            set { phone = value; NotifyPropertyChanged("Phone"); }
        }

        private byte[] photo;

        public byte[] Photo
        {
            get { return photo; }
            set { photo = value; NotifyPropertyChanged("Photo"); }
        }
    }
}
