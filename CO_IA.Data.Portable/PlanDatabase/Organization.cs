using AT_BC.Data;
using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class Organization : IdentifiableData<string>
    {
        public Organization()
        {
        }

        private bool ischecked;
        public bool IsChecked
        {
            get
            { 
                return ischecked; 
            }
            set
            {
                ischecked = value;
                this.NotifyPropertyChanged("IsChecked");
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
                shortName = value;
                this.NotifyPropertyChanged("ShortName");
            }
        }

        private string address;

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                NotifyPropertyChanged("Address");
            }
        }

        private SecurityClass securityClass;
        /// <summary>
        /// 保障类别
        /// </summary>
        public SecurityClass SecurityClass
        {
            get
            {
                return securityClass;
            }
            set
            {
                securityClass = value;
            }
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
                contact = value;
                NotifyPropertyChanged("Contact");
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
                phone = value;
                NotifyPropertyChanged("Phone");
            }
        }

        /// <summary>
        /// 数据状态
        /// </summary>
        public DataStateEnum DataSate
        {
            get;
            set;
        }
    }
}
