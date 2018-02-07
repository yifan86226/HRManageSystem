#region 文件描述
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：人员预案组织结构实体
 * 日  期：2016-08-17
 * ********************************************************************************/
#endregion


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class PP_OrgInfo : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 子集合
        /// </summary>
        public List<PP_OrgInfo> Children { get; set; }
        public PP_OrgInfo Parent;
        public PP_OrgInfo()
        {
            Children = new List<PP_OrgInfo>();
        }

        private string guid;

        /// <summary>
        /// 组织结构ID
        /// </summary>
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
        /// <summary>
        /// 组织名称
        /// </summary>
        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChange("NAME"); }
        }

        private string parent_guid="";

        /// <summary>
        /// 父辈ID
        /// </summary>
        public string PARENT_GUID
        {
            get { return parent_guid; }
            set { parent_guid = value; NotifyPropertyChange("PARENT_GUID"); }
        }

        private string duty ="";

        /// <summary>
        /// 职责
        /// </summary>
        public string DUTY
        {
            get { return duty; }
            set { duty = value; NotifyPropertyChange("DUTY"); }
        }
         
        public void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool? online = null;
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool? OnLine
        {
            get { return online; }
            set { online = value; NotifyPropertyChange("OnLine"); }
        }
        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value; NotifyPropertyChange("IsChecked");                 

            }
        }
    }
}
