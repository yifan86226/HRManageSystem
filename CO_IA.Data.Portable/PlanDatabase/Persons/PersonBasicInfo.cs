using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CO_IA.Data.PlanDatabase
{
  public  class PersonBasicInfo : INotifyPropertyChanged
    {
        public PersonBasicInfo()
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



        private string guid;

        /// <summary>
        /// 唯一标识
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


        private string sex;
        /// <summary>
        /// 性别
        /// </summary>
        public string SEX
        {
            get { return sex; }
            set { sex = value; NotifyPropertyChanged("SEX"); }
        }

        /// <summary>
        /// 出生年月
        /// </summary>
        private string birthdate;

        public string BIRTHDATE
        {
            get { return birthdate; }
            set { birthdate = value; NotifyPropertyChanged("BIRTHDATE"); }
        }

        private string nation;
        /// <summary>
        /// 民族
        /// </summary>
        public string NATION
        {
            get { return nation; }
            set { nation = value; NotifyPropertyChanged("NATION"); }
        }



        private string enlistmentdate;
        /// <summary>
        /// 入伍年月
        /// </summary>
        public string ENLISTMENTDATE
        {
            get { return enlistmentdate; }
            set { enlistmentdate = value; NotifyPropertyChanged("ENLISTMENTDATE"); }
        }



        private string militaryrank;
        /// <summary>
        /// 军衔
        /// </summary>
        public string MILITARYRANK
        {
            get { return militaryrank; }
            set { militaryrank = value; NotifyPropertyChanged("MILITARYRANK"); }
        }





        private string originplace;
        /// <summary>
        /// 籍贯
        /// </summary>
        public string ORIGINPLACE
        {
            get { return originplace; }
            set { originplace = value; NotifyPropertyChanged("ORIGINPLACE"); }
        }



        private string armyseat;
        /// <summary>
        /// 入伍所在地
        /// </summary>
        public string ARMYSEAT
        {
            get { return armyseat; }
            set { armyseat = value; NotifyPropertyChanged("ARMYSEAT"); }
        }


        private string major;
        /// <summary>
        /// 专业
        /// </summary>
        public string MAJOR
        {
            get { return major; }
            set { major = value; NotifyPropertyChanged("MAJOR"); }
        }


          private string education;
        /// <summary>
        /// 文化程度
        /// </summary>
        public string EDUCATION
        {
            get { return education; }
            set { education = value; NotifyPropertyChanged("EDUCATION"); }
        }


        private string political;
        /// <summary>
        /// 政治面貌
        /// </summary>
        public string POLITICAL
        {
            get { return political; }
            set { political = value; NotifyPropertyChanged("POLITICAL"); }
        }


        private string partytime;
        /// <summary>
        /// 党团时间
        /// </summary>
        public string PARTYTIME
        {
            get { return partytime; }
            set { partytime = value; NotifyPropertyChanged("PARTYTIME"); }
        }



        private string hjqk;
        /// <summary>
        /// 户籍情况
        /// </summary>
        public string HJQK
        {
            get { return hjqk; }
            set { hjqk = value; NotifyPropertyChanged("HJQK"); }
        }




        private string bloodtype;
        /// <summary>
        /// 血型
        /// </summary>
        public string BLOODTYPE
        {
            get { return bloodtype; }
            set { bloodtype = value; NotifyPropertyChanged("BLOODTYPE"); }
        }






        private string idcard;
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCARD
        {
            get { return idcard; }
            set { idcard = value; NotifyPropertyChanged("IDCARD"); }
        }





        private string hobby;
        /// <summary>
        /// 兴趣爱好
        /// </summary>
        public string HOBBY
        {
            get { return hobby; }
            set { hobby = value; NotifyPropertyChanged("HOBBY"); }
        }



        private string charactertype;
        /// <summary>
        /// 性格类型
        /// </summary>
        public string CHARACTERTYPE
        {
            get { return charactertype; }
            set { charactertype = value; NotifyPropertyChanged("CHARACTERTYPE"); }
        }


        private string qqid;
        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQID
        {
            get { return qqid; }
            set { qqid = value; NotifyPropertyChanged("QQID"); }
        }


        private string homeaddress;
        /// <summary>
        /// 家庭住址
        /// </summary>
        public string HOMEADDRESS
        {
            get { return homeaddress; }
            set { homeaddress = value; NotifyPropertyChanged("HOMEADDRESS"); }
        }

        private string phone;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PHONE
        {
            get { return phone; }
            set { phone = value; NotifyPropertyChanged("PHONE"); }
        }



        private string spousename;
        /// <summary>
        /// 配偶名称
        /// </summary>
        public string SPOUSENAME
        {
            get { return spousename; }
            set { spousename = value; NotifyPropertyChanged("SPOUSENAME"); }
        }



        private string spousemarriagetime;
        /// <summary>
        /// 结婚时间
        /// </summary>
        public string SPOUSEMARRIAGETIME
        {
            get { return spousemarriagetime; }
            set { spousemarriagetime = value; NotifyPropertyChanged("SPOUSEMARRIAGETIME"); }
        }




        private string spousesunit;
        /// <summary>
        /// 配偶工作单位
        /// </summary>
        public string  SPOUSESUNIT
        {
            get { return spousesunit; }
            set { spousesunit = value; NotifyPropertyChanged("SPOUSESUNIT"); }
        }




        private string spouseshomeaddress;
        /// <summary>
        /// 配偶家庭住址
        /// </summary>
        public string SPOUSESHOMEADDRESS
        {
            get { return spouseshomeaddress; }
            set { spouseshomeaddress = value; NotifyPropertyChanged("SPOUSESHOMEADDRESS"); }
        }


        private string spousesphone;
        /// <summary>
        /// 配偶联系电话
        /// </summary>
        public string SPOUSESPHONE
        {
            get { return spousesphone; }
            set { spousesphone = value; NotifyPropertyChanged("SPOUSESPHONE"); }
        }



        private string childrenname;
        /// <summary>
        /// 子女姓名
        /// </summary>
        public string CHILDRENNAME
        {
            get { return childrenname; }
            set { childrenname = value; NotifyPropertyChanged("CHILDRENNAME"); }
        }


        private string childrensex;
        /// <summary>
        /// 子女性别
        /// </summary>
        public string CHILDRENSEX
        {
            get { return childrensex; }
            set { childrensex = value; NotifyPropertyChanged("CHILDRENSEX"); }
        }


        private string childrenbirth;
        /// <summary>
        /// 出生年月
        /// </summary>
        public string CHILDRENBIRTH
        {
            get { return childrenbirth; }
            set { childrenbirth = value; NotifyPropertyChanged("CHILDRENBIRTH"); }
        }


        private string enlistingresume;
        /// <summary>
        /// 入伍后简历
        /// </summary>
        public string ENLISTINGRESUME  
        {
            get { return enlistingresume; }
            set { enlistingresume = value; NotifyPropertyChanged("ENLISTINGRESUME"); }
        }



        private string trainingsituation;
        /// <summary>
        /// 培训情况
        /// </summary>
        public string TRAININGSITUATION
        {
            get { return trainingsituation; }
            set { trainingsituation = value; NotifyPropertyChanged("TRAININGSITUATION"); }
        }



        private string rewardspunishments;
        /// <summary>
        /// 奖惩情况
        /// </summary>
        public string REWARDSPUNISHMENTS
        {
            get { return rewardspunishments; }
            set { rewardspunishments = value; NotifyPropertyChanged("REWARDSPUNISHMENTS"); }
        }




        private string familymember;
        /// <summary>
        /// 家庭主要成员
        /// </summary>
        public string FAMILYMEMBER
        {
            get { return familymember; }
            set { familymember = value; NotifyPropertyChanged("FAMILYMEMBER"); }
        }

        private byte[] photo;
        /// <summary>
        /// 照片
        /// </summary>
        public byte[] PHOTO
        {
            get { return photo; }
            set { photo = value; NotifyPropertyChanged("PHOTO"); }
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




        private string greatevent;
        /// <summary>
        /// GREATEVENT
        /// </summary>
        public string GREATEVENT
        {
            get { return greatevent; }
            set { greatevent = value; NotifyPropertyChanged("GREATEVENT"); }
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
