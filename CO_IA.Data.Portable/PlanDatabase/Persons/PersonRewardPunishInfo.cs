using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.PlanDatabase
{
  public  class PersonRewardPunishInfo : INotifyPropertyChanged
    {
        public PersonRewardPunishInfo()
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






        private string guid;

        /// <summary>
        /// 人员ID
        /// </summary>
        public string NAMEID
        {
            get { return guid; }
            set { guid = value; NotifyPropertyChanged("NAMEID"); }
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
        /// 出生年月
        /// </summary>
        private double fraction=0;

        public double FRACTION
        {
            get { return fraction; }
            set { fraction = value; NotifyPropertyChanged("FRACTION"); }
        }

        private string rptype;
        /// <summary>
        /// 0 奖励  1惩罚
        /// </summary>
        public string RPTYPE
        {
            get { return rptype; }
            set { rptype = value; NotifyPropertyChanged("RPTYPE"); }
        }



        private string rptime;
        /// <summary>
        /// 奖惩时间
        /// </summary>
        public string RPTIME
        {
            get { return rptime; }
            set { rptime = value; NotifyPropertyChanged("RPTIME"); }
        }



        private string rpreportor;
        /// <summary>
        /// 申请人
        /// </summary>
        public string RPREPORTOR
        {
            get { return rpreportor; }
            set { rpreportor = value; NotifyPropertyChanged("RPREPORTOR"); }
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





        private string bz;
        /// <summary>
        /// BZ
        /// </summary>
        public string BZ
        {
            get { return bz; }
            set { bz = value; NotifyPropertyChanged("BZ"); }
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
