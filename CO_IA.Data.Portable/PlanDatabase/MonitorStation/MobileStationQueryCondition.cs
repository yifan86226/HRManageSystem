using CO_IA.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class MobileStationQueryCondition : INotifyPropertyChanged
    {
        private string name;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get{return name;}
            set { name = value; }
        }

        private List<string> types;
        /// <summary>
        /// 类别
        /// </summary>
        public List<string> Types
        {
            get { return types; }
            set { types = value; }
        }

        private List<string> areacodes;
        /// <summary>
        /// 地区
        /// </summary>
        public List<string> AreaCodes
        {
            get { return areacodes; }
            set { areacodes = value; }
        }

        private string address;
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set { address = value; }
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
