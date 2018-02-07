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
    public class PP_DepartmentInfo : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 子集合
        /// </summary>
        public List<PP_DepartmentInfo> Children { get; set; }
        public PP_DepartmentInfo()
        {
            Children = new List<PP_DepartmentInfo>();
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
            set { displayName = value; NotifyPropertyChange("DISPLAYNAME"); }
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


        private string deptFax = "";

        /// <summary>
        /// 机构传真 VARCHAR2(200) 
        /// </summary>
        public string DeptFax
        {
            get { return deptFax; }
            set { deptFax = value; NotifyPropertyChange("DeptFax"); }
        }

        private string deptPhone = "";

        /// <summary>
        /// 机构电话 VARCHAR2(200) 
        /// </summary>
        public string DeptPhone
        {
            get { return deptPhone; }
            set { deptPhone = value; NotifyPropertyChange("DeptPhone"); }
        }

        //public string DeptId { get; set; }
        //public string DeptName { get; set; }

        private int displaySequence = 0;

        /// <summary>
        /// 显示顺序,用于在组织机构树中排序 
        /// </summary>
        public int DisplaySequence
        {
            get { return displaySequence; }
            set { displaySequence = value; NotifyPropertyChange("DisplaySequence"); }
        }

      

        private string districtCode = "";

        /// <summary>
        /// 机构代码/地区代码 VARCHAR2(32) 
        /// </summary>
        public string DistrictCode
        {
            get { return districtCode; }
            set { districtCode = value; NotifyPropertyChange("DistrictCode"); }
        }

        private bool isOrganization = false;


        /// <summary>
        /// 是否是无委组织机构或部门
        /// </summary>
        public bool IsOrganization
        {
            get { return isOrganization; }
            set { isOrganization = value; NotifyPropertyChange("IsOrganization"); }
        }



 
        public void NotifyPropertyChange(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //根据PropertyChanged事件的委托类，实现PropertyChanged事件： 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        } 

      
    }
}
