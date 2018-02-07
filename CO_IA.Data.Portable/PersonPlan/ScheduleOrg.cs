using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ScheduleOrg: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        } 

        private string guid;
        public string GUID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChange("GUID"); }
        }

        private string schedule_detail_guid;

        public string SCHEDULE_DETAIL_GUID
        {
            get { return schedule_detail_guid; }
            set { schedule_detail_guid = value; NotifyPropertyChange("SCHEDULE_DETAIL_GUID"); }
        }

        private string area_guid;

        public string AREA_GUID
        {
            get { return area_guid; }
            set { area_guid = value; NotifyPropertyChange("AREA_GUID"); }
        }      

        private string group_guid;
        public string GROUP_GUID 
        {
            get { return group_guid; }
            set { group_guid = value; NotifyPropertyChange("GROUP_GUID"); }
        }

        private double longitude;
        public double LONGITUDE
        {
            get { return longitude; }
            set { longitude = value; NotifyPropertyChange("LONGITUDE"); }
        }

        private double latitude;
        public double LATITUDE
        {
            get { return latitude; }
            set { latitude = value; NotifyPropertyChange("LATITUDE"); }
        }

        public PP_OrgInfo OrgInfo
        {
            get;
            set;
        }
        
    }
}
