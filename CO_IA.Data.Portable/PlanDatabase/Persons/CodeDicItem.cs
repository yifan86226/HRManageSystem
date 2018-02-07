using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.PlanDatabase
{
 public   class CodeDicItem : INotifyPropertyChanged
    {



        private string code;
        /// <summary>
        /// 编码
        /// </summary>
        public string CODE
        {
            get { return code; }
            set { code = value; NotifyPropertyChanged("CODE"); }
        }





        private string name;
        /// <summary>
        /// 内容
        /// </summary>
        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("NAME"); }
        }



        private string type;
        /// <summary>
        /// 类型
        /// </summary>
        public string TYPE
        {
            get { return type; }
            set { type = value; NotifyPropertyChanged("TYPE"); }
        }






        private string dsc;
        /// <summary>
        /// 描述
        /// </summary>
        public string DSC
        {
            get { return dsc; }
            set { dsc = value; NotifyPropertyChanged("DSC"); }
        }




        private string a1;
        /// <summary>
        /// A1
        /// </summary>
        public string A1
        {
            get { return a1; }
            set { a1 = value; NotifyPropertyChanged("A1"); }
        }

        private string a2;
        /// <summary>
        /// A2
        /// </summary>
        public string A2
        {
            get { return a2; }
            set { a2 = value; NotifyPropertyChanged("A2"); }
        }



        private string a3;
        /// <summary>
        /// A3
        /// </summary>
        public string A3
        {
            get { return a3; }
            set { a3 = value; NotifyPropertyChanged("A3"); }
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
