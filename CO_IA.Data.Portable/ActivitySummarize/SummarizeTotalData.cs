using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.ActivitySummarize
{
    public class SummarizeTotalData : INotifyPropertyChanged
    {
        private bool isChecked;
        /// <summary>
        ///  选择
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged("IsChecked");
            }
        }

        private string guid;
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return guid;
            }
            set
            {
                guid = value;
                this.NotifyPropertyChanged("GUID");
            }
        }
        private string activity_guid;
        /// <summary>
        /// 活动GUID
        /// </summary>
        public string ACTIVITY_GUID
        {
            get
            {
                return activity_guid;
            }
            set
            {
                activity_guid = value;
                this.NotifyPropertyChanged("ACTIVITY_GUID");
            }
        }
        private string key;
        /// <summary>
        /// 关键字
        /// </summary>
        public string KEY
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                this.NotifyPropertyChanged("KEY");
            }
        }
        private int summarizevalue;
        /// <summary>
        /// 系统统计值
        /// </summary>
        public int SUMMARIZEVALUE
        {
            get
            {
                return summarizevalue;
            }
            set
            {
                summarizevalue = value;
                this.NotifyPropertyChanged("SUMMARIZEVALUE");
            }
        }
        private int updatevalue;
        /// <summary>
        /// 用户修改值
        /// </summary>
        public int UPDATEVALUE
        {
            get
            {
                return updatevalue;
            }
            set
            {
                updatevalue = value;
                this.NotifyPropertyChanged("UPDATEVALUE");
            }
        }

        private string description;
        public string DESCRIPTION
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                this.NotifyPropertyChanged("DESCRIPTION");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 通知属性变更方法
        /// </summary>
        /// <param name="propertyName">发生变更的属性名称</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
