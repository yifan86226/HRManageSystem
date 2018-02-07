#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案人员实体
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO_IA.Data
{
    public class PP_PersonLeaveInfo : AT_BC.Data.CheckableData<string>
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChange(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}


        private bool isChecked = false;
        /// <summary>
        /// 是否被选则
        /// </summary>
        public bool ISCHECKED
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChanged("ISCHECKED"); }
        }


        private string guid;

        public string GUID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChanged("GUID"); }
        }

        private string org_guid;

        public string ORG_GUID
        {
            get { return org_guid; }
            set { org_guid = value; NotifyPropertyChanged("ORG_GUID"); }
        }


        private string dept_ID;

        public string DEPT_ID
        {
            get { return dept_ID; }
            set { dept_ID = value; NotifyPropertyChanged("DEPT_ID"); }
        }


        private string districtCode;

        public string DISTRICT_CODE
        {
            get { return districtCode; }
            set { districtCode = value; NotifyPropertyChanged("DISTRICT_CODE"); }
        }




        private string name;

        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("NAME"); }
        }


        private string sex;

        public string SEX
        {
            get { return sex; }
            set { sex = value; NotifyPropertyChanged("SEX"); }
        }


        private string unit;

        public string UNIT
        {
            get { return unit; }
            set { unit = value; NotifyPropertyChanged("UNIT"); }
        }

        private string dept;

        public string DEPT
        {
            get { return dept; }
            set { dept = value; NotifyPropertyChanged("DEPT"); }
        }


        private string duty;

        public string DUTY
        {
            get { return duty; }
            set { duty = value; NotifyPropertyChanged("ADD_TYPE"); }
        }



        private string phone;

        public string PHONE
        {
            get { return phone; }
            set { phone = value; NotifyPropertyChanged("PHONE"); }
        }

        private string task;

        public string TASK
        {
            get { return task; }
            set { task = value; NotifyPropertyChanged("ADD_TYPE"); }
        }

        private byte[] photo;

        public byte[] PHOTO
        {
            get { return photo; }
            set { photo = value; NotifyPropertyChanged("PHOTO"); }
        }


        private int person_type;

        public int PERSON_TYPE
        {
            get { return person_type; }
            set { person_type = value; NotifyPropertyChanged("ADD_TYPE"); }
        }


        private int add_type;

        public int ADD_TYPE
        {
            get { return add_type; }
            set { add_type = value; NotifyPropertyChanged("ADD_TYPE"); }

        }

        private string leaveday;

        /// <summary>
        /// 请假日期
        /// </summary>
        public string LEAVE_DAY
        {
            get { return leaveday; }
            set { leaveday = value; NotifyPropertyChanged("LEAVE_DAY"); }
        }


        private string leave_type;

        /// <summary>
        /// 请假类别
        /// </summary>
        public string LEAVE_TYPE
        {
            get { return leave_type; }
            set { leave_type = value; NotifyPropertyChanged("LEAVE_TYPE"); }
        }
    }
}
