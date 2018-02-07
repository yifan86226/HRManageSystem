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
    public class GuaranteeProcess : INotifyPropertyChanged
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

        private string activity_guid;
        /// <summary>
        /// 活动ID
        /// </summary>
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

        private string type;
        public string TYPE
        {
            get { return type; }
            set { type = value; NotifyPropertyChange("TYPE"); }
        }

        private byte[] video;

        public byte[] VIDEO
        {
            get { return video; }
            set { video = value; NotifyPropertyChange("VIDEO"); }
        }
    
    }
}
