#region 文件描述
/**********************************************************************************
 * 创建人：Xiaguohui
 * 摘  要：日程详细
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
    public class ScheduleDetail : INotifyPropertyChanged
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

        private string schedule_guid;
        public string SCHEDULE_GUID
        {
            get { return schedule_guid; }
            set { schedule_guid = value; NotifyPropertyChange("SCHEDULE_GUID"); }
        }

        private string content;

        public string CONTENT
        {
            get { return content; }
            set { content = value; NotifyPropertyChange("CONTENT"); }
        }

        private string areas;
        /// <summary>
        /// 显示用
        /// </summary>
        public string AREAS
        {
            get { return areas; }
            set { areas = value; NotifyPropertyChange("AREAS"); }
        }


        private string groups;
        /// <summary>
        /// 显示用
        /// </summary>
        public string GROUPS
        {
            get { return groups; }
            set { groups = value; NotifyPropertyChange("GROUPS"); }
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
        private string timedesc;
        public string TIMEDESC
        {
            get { return timedesc; }
            set { timedesc = value; NotifyPropertyChange("TIMEDESC"); }
        }

        //private int timetype;
        //public int TIMETYPE
        //{
        //    get { return timetype; }
        //    set { timetype = value; NotifyPropertyChange("TIMETYPE"); }
        //}
       

        public ScheduleOrg[] ScheduleOrgs { get; set; }
    }
}
