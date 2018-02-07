using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class OrgQueryCondition : INotifyPropertyChanged
    {
        private string activityguid;

        /// <summary>
        /// 活动Guid
        /// </summary>
        public string ActivityGuid
        {
            get;
            set;
        }

        private string name;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 单位保障类别
        /// </summary>
        public List<string> SecurityClasses
        {
            get;
            set;
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get;
            set;
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
