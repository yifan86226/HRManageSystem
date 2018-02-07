using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.PlanDatabase
{
  public  class PersonOutInfo : INotifyPropertyChanged
    {
        public PersonOutInfo()
        {
            if (string.IsNullOrEmpty(NAMEID))
            {
                NAMEID = System.Guid.NewGuid().ToString();
            }
        }

        private bool isChecked;
        /// <summary>
        ///  选择
        /// </summary>
        public bool ISCHECKED
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                this.isChecked = value;
                this.NotifyPropertyChanged("ISCHECKED");
            }
        }



        private string id;

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("ID"); }
        }






        private string guid;

        /// <summary>
        /// 人员ID
        /// </summary>
        public string NAMEID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChanged("NAMEID"); }
        }



        private PersonBasicInfo nameidcodeitem;
        /// <summary>
        /// 人名类型
        /// </summary>
        public PersonBasicInfo NAMEIDCODEITEM
        {
            get { return nameidcodeitem; }
            set { nameidcodeitem = value; NotifyPropertyChanged("NAMEIDCODEITEM"); }
        }





        private CodeDicItem incidentcodeitem;
        /// <summary>
        /// 类型
        /// </summary>
        public CodeDicItem INCIDENTCODEITEM
        {
            get { return incidentcodeitem; }
            set { incidentcodeitem = value; NotifyPropertyChanged("INCIDENTCODEITEM"); }
        }





        private string name;
        /// <summary>
        /// 姓名
        /// </summary>
        public string NAME
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("NAME"); }
        }


        private string incident;
        /// <summary>
        /// 事由
        /// </summary>
        public string INCIDENT
        {
            get { return incident; }
            set { incident = value; NotifyPropertyChanged("INCIDENT"); }
        }

        /// <summary>
        /// 外出时间
        /// </summary>
        private string outtime;

        public string OUTTIME
        {
            get { return outtime; }
            set { outtime = value; NotifyPropertyChanged("OUTTIME"); }
        }

        private string backtime;
        /// <summary>
        /// 归队时间
        /// </summary>
        public string BACKTIME
        {
            get { return backtime; }
            set { backtime = value; NotifyPropertyChanged("BACKTIME"); }
        }



        private string bz;
        /// <summary>
        /// 备注
        /// </summary>
        public string BZ
        {
            get { return bz; }
            set { bz = value; NotifyPropertyChanged("BZ"); }
        }



        private string operatoraa;
        /// <summary>
        /// 操作者
        /// </summary>
        public string OPERATOR
        {
            get { return operatoraa; }
            set { operatoraa = value; NotifyPropertyChanged("OPERATOR"); }
        }





        private DateTime operatortime;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OPERATORTIME
        {
            get { return operatortime; }
            set { operatortime = value; NotifyPropertyChanged("OPERATORTIME"); }
        }



        private string operatorid;
        /// <summary>
        /// 操作人ID
        /// </summary>
        public string OPERATORID
        {
            get { return operatorid; }
            set { operatorid = value; NotifyPropertyChanged("OPERATORID"); }
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




        private string outtimehour="8";
        /// <summary>
        /// 外出小时
        /// </summary>
        public string OUTTIMEHOUR
        {
            get { return outtimehour; }
            set { outtimehour = value; NotifyPropertyChanged("OUTTIMEHOUR"); }
        }






        private string outtimeminute;
        /// <summary>
        /// 外出分钟
        /// </summary>
        public string OUTTIMEMINUTE
        {
            get { return outtimeminute; }
            set { outtimeminute = value; NotifyPropertyChanged("OUTTIMEMINUTE"); }
        }





        private string backtimehour="13";
        /// <summary>
        /// 归队小时
        /// </summary>
        public string BACKTIMEHOUR
        {
            get { return backtimehour; }
            set { backtimehour = value; NotifyPropertyChanged("BACKTIMEHOUR"); }
        }




        private string backtimeminute;
        /// <summary>
        /// 归队分钟
        /// </summary>
        public string BACKTIMEMINUTE
        {
            get { return backtimeminute; }
            set { backtimeminute = value; NotifyPropertyChanged("BACKTIMEMINUTE"); }
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
