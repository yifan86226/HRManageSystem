using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data.Template
{
    public class ActivityTemplate : AT_BC.Data.NotifyPropertyChangedObject
    {
                /// <summary>
        /// 默认构造函数,默认活动类别被定义为其他
        /// </summary>
        public ActivityTemplate()
        {
        }

        private string name;
        /// <summary>
        /// 获取或设置活动名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// 获取或设置活动标识(32位)
        /// </summary>
        public string Guid
        {
            get;
            set;
        }

        private string shortHand;
        /// <summary>
        /// 获取或设置活动简称
        /// </summary>
        public string ShortHand
        {
            get
            {
                return this.shortHand;
            }
            set
            {
                if (this.shortHand != value)
                {
                    this.shortHand = value;
                    this.NotifyPropertyChanged("ShortHand");
                }
            }
        }

        private string description;

        /// <summary>
        /// 获取或设置活动描述信息
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.NotifyPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// 获取或设置活动创建者
        /// </summary>
        public string Creator
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置活动创建日期
        /// </summary>
        public DateTime CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置活动类别
        /// </summary>
        public string ActivityType
        {
            get;
            set;
        }

        private string areaCode;
        public string AreaCode
        {
            get
            {
                return this.areaCode;
            }
            set
            {
                if (this.areaCode != value)
                {
                    this.areaCode = value;
                    this.NotifyPropertyChanged("AreaCode");
                }
            }
        }

        private bool isDefault;
        public bool IsDefault
        {
            get
            {
                return this.isDefault;
            }
            set
            {
                if (this.isDefault != value)
                {
                    this.isDefault = value;
                    this.NotifyPropertyChanged("IsDefault");
                }
            }
        }

        private bool isNew;
        public bool IsNew
        {
            get
            {
                return this.isNew;
            }
            set
            {
                if (value != this.isNew)
                {
                    this.isNew = value;
                    this.NotifyPropertyChanged("IsNew");
                }
            }
        }
    }
}
