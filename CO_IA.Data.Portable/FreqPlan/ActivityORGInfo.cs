using AT_BC.Data;
/**********************************************************************************
 * 创建人：Liuenliang
 * 摘  要：组织信息类
 * 日  期：2016-08-17
 * ********************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityORGInfo : INotifyPropertyChanged
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

        private string activity_guid = string.Empty;

        /// <summary>
        /// 活动GUID
        /// </summary>
        public string Activity_Guid
        {
         

            get
            {
                return activity_guid;
            }

            set
            {
                activity_guid = value; NotifyPropertyChange("Activity_Guid");
            }
        }



        public ActivityORGInfo()
        {
            if (string.IsNullOrEmpty(Guid))
            {
                Guid = System.Guid.NewGuid().ToString();
            }
            Securityclass = new IdentifiableData<string>();
        }

        private string guid;
        /// <summary>
        /// Guid
        /// </summary>
     
        public string Guid
        {
            get
            {
                return guid;
            }

            set
            {
                guid = value; NotifyPropertyChange("Guid");
            }
        }


        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value; NotifyPropertyChange("Name");
            }
        }
        private string shortName;

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName
        {
            get
            {
                return shortName;
            }

            set
            {
                shortName = value; NotifyPropertyChange("ShortName");
            }
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        private IdentifiableData<string> securityclass = new IdentifiableData<string>();
        /// <summary>
        /// 保障级别
        /// </summary>
        public IdentifiableData<string> Securityclass
        {
            get { return securityclass; }
            set { securityclass = value; }
        }

        private string contact;
        /// <summary>
        /// 单位联系人
        /// </summary>
        public string Contact
        {
            get
            {
                return contact;
            }

            set
            {
                contact = value; NotifyPropertyChange("Contact");
            }
        }
        private string phone;

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Phone
        {
            get
            {
                return phone;
            }

            set
            {
                phone = value; NotifyPropertyChange("Phone");
            }
        }





        ///// <summary>
        ///// 活动联系人
        ///// </summary>
        //public string ActionManager { get; set; }

        ///// <summary>
        ///// 活动人联系电话
        ///// </summary>
        //public string ActionTelNo { get; set; }


        //public string activity_guid;
    }
}
