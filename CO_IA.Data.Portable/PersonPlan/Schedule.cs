#region 文件描述
/**********************************************************************************
 * 创建人：Xiaguohui
 * 摘  要：日程
 * 日  期：2017-05-22
 * ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class Schedule: INotifyPropertyChanged
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

        private string activity_guid;

        public string ACTIVITY_GUID
        {
            get { return activity_guid; }
            set { activity_guid = value; NotifyPropertyChange("ACTIVITY_GUID"); }
        }

        private string name;

        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChange("NAME"); }
        }

        private DateTime starttime;

        public DateTime STARTTIME
        {
            get { return starttime; }
            set { starttime = value; NotifyPropertyChange("STARTTIME"); }
        }


        private DateTime stoptime;

        public DateTime STOPTIME
        {
            get { return stoptime; }
            set { stoptime = value; NotifyPropertyChange("STOPTIME"); }
        }

        private string memo;
        public string MEMO 
        {
            get { return memo; }
            set { memo = value; NotifyPropertyChange("MEMO"); }
        }

        public ScheduleDetail[] ScheduleDetailInfos{get;set;}
    }
}
