using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.PlanDatabase
{
    public class ExamCondition : INotifyPropertyChanged
    {
        public ExamCondition()
        {
            AreaCodes = new List<string>();
        }
        public List<string> AreaCodes { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _Guid;
        public string Guid
        {
            get { return _Guid; }
            set
            {
                _Guid = value;
                NotifyPropertyChanged("Guid");
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
