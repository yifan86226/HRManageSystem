#region 文件描述
/***************************************************************#@#***************************************************************
 * 创建人：Lizk
 * 摘 要 ：活动定义,提供简单的活动信息
 * 日 期 ：2016-08-09
 ***************************************************************#@#***************************************************************/
#endregion

using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    /// <summary>
    /// 活动定义
    /// </summary>
    public class Activity : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// 默认构造函数,默认活动类别被定义为其他
        /// </summary>
        public Activity()
        {
            //this.ActivityType = ActivityType.Other;
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
                this.name = value;
                this.NotifyPropertyChanged("Name");
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

        /// <summary>
        /// 获取或设置活动简称
        /// </summary>
        public string ShortHand
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置活动开始日期
        /// </summary>
        public DateTime DateFrom
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置活动结束日期
        /// </summary>
        public DateTime DateTo
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置活动图标
        /// </summary>
        public byte[] Icon
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置活动组织单位
        /// </summary>
        public string Organizer
        {
            get;
            set;
        }

        private ActivityStage activityStage;

        /// <summary>
        /// 获取或或设置活动阶段
        /// </summary>
        public ActivityStage ActivityStage
        {
            get
            {
                return this.activityStage;
            }
            set
            {
                this.activityStage = value;
                this.NotifyPropertyChanged("ActivityStage");
            }
        }

        /// <summary>
        /// 获取或设置活动描述信息
        /// </summary>
        public string Description
        {
            get;
            set;
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

        private void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propName));
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }

    public class ActivityExt : Activity
    {
        public List<ActivityPlace> PlaceList
        {
            get;
            set;
        }
    }
}
