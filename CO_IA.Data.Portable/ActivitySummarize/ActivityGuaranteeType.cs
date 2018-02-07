using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data
{
    public class ActivityGuaranteeType : INotifyPropertyChanged
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
        /// <summary>
        /// 子集合
        /// </summary>
        public List<ActivityGuaranteeType> Children { get; set; }
        public ActivityGuaranteeType()
        {
            Children = new List<ActivityGuaranteeType>();
        }
        private string guid;
        public string GUID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChange("GUID"); }
        }

        private string name;
        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChange("NAME"); }
        }
        private string parentGuid;
        public string PARENTGUID
        {
            get { return parentGuid; }
            set { parentGuid = value; NotifyPropertyChange("PARENTGUID"); }
        }

        private string activityGuid;
        public string ACTIVITYGUID
        {
            get { return activityGuid; }
            set { activityGuid = value; NotifyPropertyChange("ACTIVITYGUID"); }
        }
    }
}
