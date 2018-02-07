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

namespace CO_IA.Data.PersonPlan
{
    public class PP_OrgInfo : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 子集合
        /// </summary>
        public List<PP_OrgInfo> Children { get; set; }
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



        //private bool isMonitorGroup=false;
        ///// <summary>
        ///// 是否是监测组
        ///// </summary>
        //public bool ISMONITORGROUP
        //{
        //    get { return isMonitorGroup; }
        //    set { isMonitorGroup = value; NotifyPropertyChange("ISMONITORGROUP"); }
        //}



        private string displayName;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DISPLAYNAME
        {
            get { return displayName; }
            set { displayName = value; NotifyPropertyChange("GUID"); }
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

        private int org_type=0;
        /// <summary>
        /// 组织类型：0.非监测组；1. 监测组
        /// </summary>
        public int ORG_TYPE
        {
            get { return org_type; }
            set { org_type = value; NotifyPropertyChange("ORG_TYPE"); }
        }


        private int add_type=1;

        /// <summary>
        /// 添加方式：1.导入；2.新加
        /// </summary>
        public int ADD_TYPE
        {
            get { return add_type; }
            set { add_type = value; NotifyPropertyChange("ADD_TYPE"); }
        }


 
        private void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        } 

      
    }
}
