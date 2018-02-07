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

namespace CO_IA.Data.PersonPlan
{
    public class PP_PersonInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private bool isChecked = false;
        /// <summary>
        /// 是否被选则
        /// </summary>
        public bool ISCHECKED
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChange("ISCHECKED"); }
        }


        private string guid;

        public string GUID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChange("GUID"); }
        }

        private string org_guid;

        public string ORG_GUID
        {
            get { return org_guid; }
            set { org_guid = value; NotifyPropertyChange("ORG_GUID"); }
        }

        private string name;

        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChange("NAME"); }
        }


        private string unit;

        public string UNIT
        {
            get { return unit; }
            set { unit = value; NotifyPropertyChange("UNIT"); }
        }



        private string duty;

        public string DUTY
        {
            get { return duty; }
            set { duty = value; NotifyPropertyChange("ADD_TYPE"); }
        }



        private string phone;

        public string PHONE
        {
            get { return phone; }
            set { phone = value; NotifyPropertyChange("PHONE"); }
        }

        private string task;

        public string TASK
        {
            get { return task; }
            set { task = value; NotifyPropertyChange("ADD_TYPE"); }
        }

        private byte[] photo;

        public byte[] PHOTO
        {
            get { return photo; }
            set { photo = value; NotifyPropertyChange("PHOTO"); }
        }


        private int person_type;

        public int PERSON_TYPE
        {
            get { return person_type; }
            set { person_type = value; NotifyPropertyChange("ADD_TYPE"); }
        }


        private int add_type;

        public int ADD_TYPE
        {
            get { return add_type; }
            set { add_type = value; NotifyPropertyChange("ADD_TYPE"); }

        }
    }
}
